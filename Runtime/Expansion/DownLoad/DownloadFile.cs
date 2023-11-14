
using System;
using System.Net;
using System.Threading;
using System.IO;
using Framework;
using System.Text;

public class DownloadFile
{
    /// <summary>
    /// 主线程
    /// </summary>
    protected SynchronizationContext _mainThreadSynContext;

    /// <summary>
    /// 下载网址
    /// </summary>
    public string Url;

    /// <summary>
    /// 下载文件的保存的路径
    /// </summary>
    public string filePath;

    /// <summary>
    /// 速度
    /// </summary>
    public long speed;

    /// <summary>
    /// 限速
    /// </summary>
    public bool isOpenImpose = true;
    //错误 回调
    public  Action<Exception> OnError;
    //下载完成 回调
    public  Action<byte[]> OnComplted;
    //下载中 回调
    public  Action<long, long> OnDownloading;

    //暂停 回调
    public Action  OnPause;

    protected static object errorlock = new object();

    public long getSpeed() {
        return speed;
    }


    public long mCurrLength;
    public long fileLength;

    public float Process
    {
        get
        {
            if (mCurrLength == 0) return 0f;
            else
                return (float)mCurrLength / fileLength;
        }
    }

    /// <summary>
    /// 主要用于关闭线程
    /// </summary>
    protected bool _isDownload = false;
    public bool IsDownloading { get { return _isDownload; } }

    public DownloadFile(string url, string filePath)
    {
        // 主线程赋值
        _mainThreadSynContext = SynchronizationContext.Current;
        // 突破Http协议的并发连接数限制
        System.Net.ServicePointManager.DefaultConnectionLimit = 512;

        Url = url;
        this.filePath = filePath;

        mCurrLength = fileLength = 0;

        //默认加上回调
        OnDownloading += OnDownLoading;
    }

    /// <summary>
    /// 下载通用公共接口
    /// </summary>
    /// <param name="threadCount">下载线程 树 默认1（多的线程目前没支持）</param>
    /// <param name="onTrigger">下载完成回调</param>
    /// <param name="err">下载错误回调</param>
    /// <param name="onDownloading">下载过程回调</param>
    public void Download(Action<byte[]> onTrigger = null,Action<Exception> err=null, Action<long, long> onDownloading = null, int threadCount = 1)
    {
        OnError += (ex) =>
        {
            err?.Invoke(ex);
            Log.E("捕获异常 >>> " + ex.Message);
        };

        if (onTrigger != null) OnComplted += onTrigger;
        if (onDownloading!=null) OnDownloading += onDownloading;
        //在下载
        DownloadToFile(threadCount, OnDownloading, OnComplted);
    }

    protected void OnDownLoading(long curSize, long allSize)
    {
        mCurrLength = curSize;
        fileLength = allSize;
    }


    /// <summary>
    /// 多线程下载文件至内存
    /// </summary>
    /// <param name="threadCount">线程总数</param>
    /// <param name="onDownloading">下载过程回调（已下载文件大小、总文件大小）</param>
    /// <param name="onTrigger">下载完毕回调（下载文件数据）</param>
    protected void DownloadToMemory(int threadCount, Action<long, long> onDownloading = null, Action<byte[]> onTrigger = null)
    {
        _isDownload = true;

        long csize = 0; // 已下载大小
        int ocnt = 0;   // 完成线程数
        byte[] cdata;  // 已下载数据
        // 下载逻辑
        GetFileSizeAsyn((size) =>
        {
            cdata = new byte[size];
            // 单线程下载过程回调函数
            Action<int, int, byte[], byte[]> t_onDownloading = (index, rsize, rbytes, data) =>
            {
                csize += rsize;
                PostMainThreadAction<long, long>(onDownloading, csize, size);
            };
            // 单线程下载完毕回调函数
            Action<int, byte[]> t_onTrigger = (index, data) =>
            {
                long dIndex = (long)Math.Ceiling((double)(size * index / threadCount));
                Array.Copy(data, 0, cdata, dIndex, data.Length);

                ocnt++;
                if (ocnt >= threadCount)
                {
                    PostMainThreadAction<byte[]>(onTrigger, cdata);
                }
            };

            Action<int> backOff = (index) =>
            {


            };

            // 分割文件尺寸，多线程下载
            long[] sizes = SplitFileSize(size, threadCount);
            for (int i = 0; i < sizes.Length; i = i + 2)
            {
                long from = sizes[i];
                long to = sizes[i + 1];
                _threadDownload(i / 2, from, to, t_onDownloading, t_onTrigger, backOff);
            }
        });
    }


    /// <summary>/// 删除临时文件/// </summary>
    public void DeleteTempFile()
    {
        try
        {
            string dirPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            // 查看下载临时文件是否存在
            var fileInfos = new DirectoryInfo(dirPath).GetFiles(fileName + "*.temp");

            if (fileInfos != null)
                foreach (var info in fileInfos)
                {
                    info.Delete();
                }
        }
        catch (Exception)
        {
        }
      
    }

    public static void DeleteTempFile(string filePath)
    {
        try
        {
            string dirPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            // 查看下载临时文件是否存在
            var fileInfos = new DirectoryInfo(dirPath).GetFiles(fileName + "*.temp");

            if (fileInfos != null)
                foreach (var info in fileInfos)
                {
                    info.Delete();
                }
        }
        catch (Exception )
        {

        }
    }


    /// <summary>
    /// 查询文件大小
    /// </summary>
    /// <returns></returns>
    protected long GetFileSize()
    {
        HttpWebRequest request;
        HttpWebResponse response;
        try
        {
            request = (HttpWebRequest)HttpWebRequest.CreateHttp(new Uri(Url));
            request.Method = "HEAD";
            response = (HttpWebResponse)request.GetResponse();
            // 获得文件长度
            long contentLength = response.ContentLength;

            response.Close();
            request.Abort();

            return contentLength;
        }
        catch (Exception ex)
        {
            onError(ex);
            // throw;
            return -1;
        }
    }
    /// <summary>
    /// 异步查询文件大小
    /// </summary>
    /// <param name="onTrigger"></param>
    protected void GetFileSizeAsyn(Action<long> onTrigger = null)
    {
        ThreadStart threadStart = new ThreadStart(() =>
        {
            PostMainThreadAction<long>(onTrigger, GetFileSize());
        });
        Thread thread = new Thread(threadStart);
        thread.Start();
    }

    /// <summary>
    /// 多线程下载文件至本地
    /// </summary>
    /// <param name="threadCount">线程总数</param>
    /// <param name="filePath">保存文件路径</param>
    /// <param name="onDownloading">下载过程回调（已下载文件大小、总文件大小）</param>
    /// <param name="onTrigger">下载完毕回调（下载文件数据）</param>
    protected void DownloadToFile(int threadCount, Action<long, long> onDownloading = null, Action<byte[]> onTrigger = null)
    {
        _isDownload = true;

        long csize = 0; //已下载大小
        int ocnt = 0;   //完成线程数


        // 下载逻辑
        GetFileSizeAsyn((size) =>
        {
            if (size == -1) return;
            // 准备工作
            var tempFilePaths = new string[threadCount];
            var tempFileFileStreams = new FileStream[threadCount];
            var dirPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);

            Log.I("tt == >" + dirPath);

            Log.I("tt == >" + fileName);

            // 下载根目录不存在则创建
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            // 查看下载临时文件是否可以继续断点续传
            var fileInfos = new DirectoryInfo(dirPath).GetFiles(fileName + "*.temp");


            if (fileInfos.Length != threadCount)
            {
                try
                { 
                    // 下载临时文件数量不相同，则清理
                    foreach (var info in fileInfos)
                    {
                        info.Delete();
                    }

                }
                catch (Exception)
                {
                }
               
            }
            // 创建下载临时文件，并创建文件流
            for (int i = 0; i < threadCount; i++)
            {
                tempFilePaths[i] = filePath + i + ".temp";
                if (!File.Exists(tempFilePaths[i]))
                {
                    Log.I("重新创建临时文件");
                    File.Create(tempFilePaths[i]).Dispose();
                }
                tempFileFileStreams[i] = File.OpenWrite(tempFilePaths[i]);
                tempFileFileStreams[i].Seek(tempFileFileStreams[i].Length, System.IO.SeekOrigin.Current);

                csize += tempFileFileStreams[i].Length;
            }
            // 单线程下载过程回调函数
            Action<int, int, byte[], byte[]> t_onDownloading = (index, rsize, rbytes, data) =>
            {
                csize += rsize;
                //Log.E("Write count:" + rsize);
                tempFileFileStreams[index].Write(rbytes, 0, rsize);
                PostMainThreadAction<long, long>(onDownloading, csize, size);
            };
            // 单线程下载完毕回调函数
            Action<int, byte[]> t_onTrigger = (index, data) =>
            {
                // 关闭文件流
                Log.I("关闭数据流");
                tempFileFileStreams[index].Close();
                ocnt++;
            if (ocnt >= threadCount)
            {
                // 将临时文件转为下载文件
                //if (!File.Exists(filePath))
                //{
                //    File.Create(filePath).Dispose();
                //}
                //else
                //{
                //    File.WriteAllBytes(filePath, new byte[] { });
                //}

                Log.I("==== > 11111");

               // byte[] rbytes = new byte[8 * 1024];

                //if (File.Exists(tempFilePaths[0]))
                //{
                //    var tempPath = tempFilePaths[0];

                //    WiteFile(tempPath, filePath);
                //    File.Delete(tempPath);
                //}

                    var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                    fs.Seek(fs.Length, System.IO.SeekOrigin.Current);
                    foreach (var tempPath in tempFilePaths)
                    {
                        var tempData = File.ReadAllBytes(tempPath);
                        fs.Write(tempData, 0, tempData.Length);
                        File.Delete(tempPath);
                    }
                    fs.Close();

                    PostMainThreadAction<byte[]>(onTrigger, new byte[] { });
                }
            };

            Action<int> backOff = (index) =>
            {
                Log.I("关闭数据流2");
                tempFileFileStreams[index].Close();
            };
            // 分割文件尺寸，多线程下载
            long[] sizes = SplitFileSize(size, threadCount);

            for (int i = 0; i < sizes.Length; i = i + 2)
            {
                long from = sizes[i];
                long to = sizes[i + 1];

                // 断点续传
                from += tempFileFileStreams[i / 2].Length;
                if (from >= to)
                {
                    t_onTrigger(i / 2, null);
                    continue;
                }
                _threadDownload(i / 2, from, to, t_onDownloading, t_onTrigger, backOff);
            }
        });
    }


    protected void WiteFile(string filePath1, string filePath2) {
      
        //创建两个字节流
        //读取流，用来读取002的内容
        FileStream readSream = new FileStream(filePath1, FileMode.Open);
        //写入流，用来吧002的内容写入到003中
        FileStream writeStream = new FileStream(filePath2, FileMode.Open);

        byte[] data2 = new byte[1024 * 8];

        while (true)
        {
            //读取数据 这里的读取的文本和Read后的参数无关，在定义流的时候，传入的地址是什么就读取这个地址的内容
            //读取002.txt中的内容，存储到data2中

            int length = readSream.Read(data2, 0, data2.Length);
            if (length == 0)
            {
                Log.I("读取完毕");
                break;
            }
            else
            {//写入数据 这里的读取的文本和Write后的参数无关，在定义流的时候，传入的地址是什么就写入到这个地址
             //写入002.txt中的内容带003中
                writeStream.Write(data2, 0, data2.Length);
            }
        }

        readSream.Close();

        writeStream.Close();
    }


    /// <summary>
    /// 单线程下载
    /// </summary>
    /// <param name="index">线程ID</param>
    /// <param name="from">下载起始位置</param>
    /// <param name="to">下载结束位置</param>
    /// <param name="onDownloading">下载过程回调（线程ID、单次下载数据大小、单次下载数据缓存区、已下载文件数据）</param>
    /// <param name="onTrigger">下载完毕回调（线程ID、下载文件数据）</param>
    protected void _threadDownload(int index,long from, long to, Action<int, int, byte[], byte[]> onDownloading = null, Action<int, byte[]> onTrigger = null ,Action<int> backOff = null)
    {
        
        Thread thread = new Thread(new ThreadStart(() =>
        {
            try
            {
                var request = (HttpWebRequest)HttpWebRequest.Create(new Uri(Url));
                request.AddRange(from, to);

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream ns = response.GetResponseStream();

                byte[] rbytes = new byte[8 * 1024];
                int rSize = 0;
                //MemoryStream ms = new MemoryStream();

                long sum = 0;
                long lastReceiveData = 0;
                long lastReceiveTime = TimeExtension.GetTimeStamp();
                long time = 0;
                long tempSize = 0;
                while (true)
                {
                    if (!_isDownload) {

                        ns.Close();
                        response.Close();
                        request.Abort();
                        backOff?.Invoke(index);
                        return;
                    }
                    rSize = ns.Read(rbytes, 0, rbytes.Length);
                    if (rSize <= 0) break;
                    //WriteFile(filePath, rbytes, rbytes.Length);
                    sum += rSize;
                    if (isOpenImpose) {
                        if (sum - lastReceiveData > 80 * 1024)
                        {//限制200k
                            long receive_interval = TimeExtension.GetTimeStamp() - lastReceiveTime;
                            if (receive_interval < 1000)
                            {
                                try
                                {
                                    Thread.Sleep(((int)(1000 - receive_interval)));
                                }
                                catch (Exception e)
                                {
                                    Log.E(e.Message);
                                }
                            }
                            lastReceiveData = sum;
                            lastReceiveTime = TimeExtension.GetTimeStamp();
                        }
                    }
                    if (TimeExtension.GetTimeStamp() > time +1000) {
                        speed = sum - tempSize;
                        tempSize = sum;
                        time = TimeExtension.GetTimeStamp();
                    }

                    if (onDownloading != null) onDownloading(index, rSize, rbytes, null);
                }

                ns.Close();
                response.Close();
                request.Abort();

                //if (sum == (to - from) +1)
                //{

                //}
                //else
                //{
                //    lock (errorlock)
                //    {
                //        if (_isDownload)
                //        {
                //            backOff?.Invoke(index);
                //            onError(new Exception("文件大小校验不通过"));
                //        }
                //    }
                //}
                if (onTrigger != null) onTrigger(index, null);

            }
            catch (Exception ex)
            {
                backOff?.Invoke(index);
                onError(ex);
            }

        }));
        thread.Start();
    }

    public virtual void Close()
    {
        _isDownload = false;
    }
    public virtual void Resume()
    {
        Download();
        _isDownload = true;
    }

    /// <summary>
    /// 分割文件大小
    /// </summary>
    /// <returns></returns>
    private long[] SplitFileSize(long size, int count)
    {
        long[] result = new long[count * 2];
        for (int i = 0; i < count; i++)
        {
            long from = (long)Math.Ceiling((double)(size * i / count));
            long to = (long)Math.Ceiling((double)(size * (i + 1) / count)) - 1;
            result[i * 2] = from;
            result[i * 2 + 1] = to;
        }

        return result;
    }

    protected void onError(Exception ex)
    {
        Close();
        PostMainThreadAction<Exception>(OnError, ex);
    }

    /// <summary>
    /// 通知主线程回调
    /// </summary>
    protected void PostMainThreadAction(Action action)
    {
        _mainThreadSynContext.Post(new SendOrPostCallback((o) =>
        {
            Action e = (Action)o.GetType().GetProperty("action").GetValue(o);
            if (e != null) e();
        }), new { action = action });
    }
    protected void PostMainThreadAction<T>(Action<T> action, T arg1)
    {
        _mainThreadSynContext.Post(new SendOrPostCallback((o) =>
        {
            Action<T> e = (Action<T>)o.GetType().GetProperty("action").GetValue(o);
            T t1 = (T)o.GetType().GetProperty("arg1").GetValue(o);
            if (e != null) e(t1);
        }), new { action = action, arg1 = arg1 });
    }
    public void PostMainThreadAction<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
    {
        _mainThreadSynContext.Post(new SendOrPostCallback((o) =>
        {
            Action<T1, T2> e = (Action<T1, T2>)o.GetType().GetProperty("action").GetValue(o);
            T1 t1 = (T1)o.GetType().GetProperty("arg1").GetValue(o);
            T2 t2 = (T2)o.GetType().GetProperty("arg2").GetValue(o);
            if (e != null) e(t1, t2);
        }), new { action = action, arg1 = arg1, arg2 = arg2 });
    }
    public void PostMainThreadAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
    {
        _mainThreadSynContext.Post(new SendOrPostCallback((o) =>
        {
            Action<T1, T2, T3> e = (Action<T1, T2, T3>)o.GetType().GetProperty("action").GetValue(o);
            T1 t1 = (T1)o.GetType().GetProperty("arg1").GetValue(o);
            T2 t2 = (T2)o.GetType().GetProperty("arg2").GetValue(o);
            T3 t3 = (T3)o.GetType().GetProperty("arg3").GetValue(o);
            if (e != null) e(t1, t2, t3);
        }), new { action = action, arg1 = arg1, arg2 = arg2, arg3 = arg3 });
    }
    public void PostMainThreadAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        _mainThreadSynContext.Post(new SendOrPostCallback((o) =>
        {
            Action<T1, T2, T3, T4> e = (Action<T1, T2, T3, T4>)o.GetType().GetProperty("action").GetValue(o);
            T1 t1 = (T1)o.GetType().GetProperty("arg1").GetValue(o);
            T2 t2 = (T2)o.GetType().GetProperty("arg2").GetValue(o);
            T3 t3 = (T3)o.GetType().GetProperty("arg3").GetValue(o);
            T4 t4 = (T4)o.GetType().GetProperty("arg4").GetValue(o);
            if (e != null) e(t1, t2, t3, t4);
        }), new { action = action, arg1 = arg1, arg2 = arg2, arg3 = arg3, arg4 = arg4 });
    }
}
