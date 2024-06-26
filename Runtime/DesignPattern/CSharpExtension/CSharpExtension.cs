/****************************************************
    文件：CSharpExtension.cs
    日期：2020/11/9 17:41:6
	功能：C#/Unity 基础api语法糖
*****************************************************/

namespace Framework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Reflection;
    using System.Text;

#if UNITY_5_6_OR_NEWER
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;
    using Object = UnityEngine.Object;

#endif

    /// <summary>
    /// 一些基础类型的扩展
    /// </summary>
    public static class BasicValueExtension
    {
        /// <summary>
        /// 是否相等
        /// 
        /// 示例：
        /// <code>
        /// if (this.Is(player))
        /// {
        ///     ...
        /// }
        /// </code>
        /// </summary>
        /// <param name="selfObj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Is(this object selfObj, object value)
        {
            return selfObj == value;
        }

        public static bool Is<T>(this T selfObj, Func<T, bool> condition)
        {
            return condition(selfObj);
        }

        /// <summary>
        /// 表达式成立 则执行 Action
        /// 
        /// 示例:
        /// <code>
        /// (1 == 1).Do(()=>Log.I("1 == 1");
        /// </code>
        /// </summary>
        /// <param name="selfCondition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool Do(this bool selfCondition, Action action)
        {
            if (selfCondition)
            {
                action();
            }

            return selfCondition;
        }

        /// <summary>
        /// 不管表达成不成立 都执行 Action，并把结果返回
        /// 
        /// 示例:
        /// <code>
        /// (1 == 1).Do((result)=>Log.I("1 == 1:" + result);
        /// </code>
        /// </summary>
        /// <param name="selfCondition"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool Do(this bool selfCondition, Action<bool> action)
        {
            action(selfCondition);

            return selfCondition;
        }
    }

    /// <summary>
    /// 通用的扩展，类的扩展
    /// </summary>
    public static class ClassExtention
    {
        /// <summary>
        /// 功能：判断是否为空
        /// 
        /// 示例：
        /// <code>
        /// var simpleObject = new object();
        ///
        /// if (simpleObject.IsNull()) // 等价于 simpleObject == null
        /// {
        ///     // do sth
        /// }
        /// </code>
        /// </summary>
        /// <param name="selfObj">判断对象(this)</param>
        /// <typeparam name="T">对象的类型（可不填）</typeparam>
        /// <returns>是否为空</returns>
        public static bool IsNull<T>(this T selfObj) where T : class
        {
            return null == selfObj;
        }

        /// <summary>
        /// 功能：判断不是为空
        /// 示例：
        /// <code>
        /// var simpleObject = new object();
        ///
        /// if (simpleObject.IsNotNull()) // 等价于 simpleObject != null
        /// {
        ///    // do sth
        /// }
        /// </code>
        /// </summary>
        /// <param name="selfObj">判断对象（this)</param>
        /// <typeparam name="T">对象的类型（可不填）</typeparam>
        /// <returns>是否不为空</returns>
        public static bool IsNotNull<T>(this T selfObj) where T : class
        {
            return null != selfObj;
        }

        public static void DoIfNotNull<T>(this T selfObj, Action<T> action) where T : class
        {
            if (selfObj != null)
            {
                action(selfObj);
            }
        }
    }

    /// <summary>
    /// Func、Action、delegate 的扩展
    /// </summary>
    public static class FuncOrActionOrEventExtension
    {
        #region Func Extension

        /// <summary>
        /// 功能：不为空则调用 Func
        /// 
        /// 示例:
        /// <code>
        /// Func<int> func = ()=> 1;
        /// var number = func.InvokeGracefully(); // 等价于 if (func != null) number = func();
        /// </code>
        /// </summary>
        /// <param name="selfFunc"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T InvokeGracefully<T>(this Func<T> selfFunc)
        {
            return null != selfFunc ? selfFunc() : default(T);
        }

        #endregion

        #region Action

        /// <summary>
        /// 功能：不为空则调用 Action
        /// 
        /// 示例:
        /// <code>
        /// System.Action action = () => Log.I("action called");
        /// action.InvokeGracefully(); // if (action != null) action();
        /// </code>
        /// </summary>
        /// <param name="selfAction"> action 对象 </param>
        /// <returns> 是否调用成功 </returns>
        public static bool InvokeGracefully(this Action selfAction)
        {
            if (null != selfAction)
            {
                selfAction();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用 Action<T>
        /// 
        /// 示例:
        /// <code>
        /// System.Action<int> action = (number) => Log.I("action called" + number);
        /// action.InvokeGracefully(10); // if (action != null) action(10);
        /// </code>
        /// </summary>
        /// <param name="selfAction"> action 对象</param>
        /// <typeparam name="T">参数</typeparam>
        /// <returns> 是否调用成功</returns>
        public static bool InvokeGracefully<T>(this Action<T> selfAction, T t)
        {
            if (null != selfAction)
            {
                selfAction(t);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用 Action<T,K>
        ///
        /// 示例
        /// <code>
        /// System.Action<int,string> action = (number,name) => Log.I("action called" + number + name);
        /// action.InvokeGracefully(10,"Framework"); // if (action != null) action(10,"Framework");
        /// </code>
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        public static bool InvokeGracefully<T, K>(this Action<T, K> selfAction, T t, K k)
        {
            if (null != selfAction)
            {
                selfAction(t, k);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 不为空则调用委托
        ///
        /// 示例：
        /// <code>
        /// // delegate
        /// TestDelegate testDelegate = () => { };
        /// testDelegate.InvokeGracefully();
        /// </code>
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call suceed </returns>
        public static bool InvokeGracefully(this Delegate selfAction, params object[] args)
        {
            if (null != selfAction)
            {
                selfAction.DynamicInvoke(args);
                return true;
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// 泛型工具
    /// 
    /// 实例：
    /// <code>
    /// 示例：
    /// var typeName = GenericExtention.GetTypeName<string>();
    /// typeName.LogInfo(); // string
    /// </code>
    /// </summary>
    public static class GenericUtil
    {
        /// <summary>
        /// 获取泛型名字
        /// <code>
        /// var typeName = GenericExtention.GetTypeName<string>();
        /// typeName.LogInfo(); // string
        /// </code>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetTypeName<T>()
        {
            return typeof(T).ToString();
        }
    }

    /// <summary>
    /// 可枚举的集合扩展（Array、List<T>、Dictionary<K,V>)
    /// </summary>
    public static class IEnumerableExtension
    {
        #region Array Extension

        /// <summary>
        /// 遍历数组
        /// <code>
        /// var testArray = new[] { 1, 2, 3 };
        /// testArray.ForEach(number => number.LogInfo());
        /// </code>
        /// </summary>
        /// <returns>The each.</returns>
        /// <param name="selfArray">Self array.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        /// <returns> 返回自己 </returns>
        public static T[] ForEach<T>(this T[] selfArray, Action<T> action)
        {
            Array.ForEach(selfArray, action);
            return selfArray;
        }

        /// <summary>
        /// 遍历 IEnumerable
        /// <code>
        /// // IEnumerable<T>
        /// IEnumerable<int> testIenumerable = new List<int> { 1, 2, 3 };
        /// testIenumerable.ForEach(number => number.LogInfo());
        /// // 支持字典的遍历
        /// new Dictionary<string, string>()
        ///         .ForEach(keyValue => Log.I("key:{0},value:{1}", keyValue.Key, keyValue.Value));
        /// </code>
        /// </summary>
        /// <returns>The each.</returns>
        /// <param name="selfArray">Self array.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> selfArray, Action<T> action)
        {
            if (action == null) throw new ArgumentException();
            foreach (var item in selfArray)
            {
                action(item);
            }

            return selfArray;
        }

        #endregion

        #region List Extension

        /// <summary>
        /// 倒序遍历
        /// <code>
        /// var testList = new List<int> { 1, 2, 3 };
        /// testList.ForEachReverse(number => number.LogInfo()); // 3, 2, 1
        /// </code>
        /// </summary>
        /// <returns>返回自己</returns>
        /// <param name="selfList">Self list.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T> action)
        {
            if (action == null) throw new ArgumentException();

            for (var i = selfList.Count - 1; i >= 0; --i)
                action(selfList[i]);

            return selfList;
        }

        /// <summary>
        /// 倒序遍历（可获得索引)
        /// <code>
        /// var testList = new List<int> { 1, 2, 3 };
        /// testList.ForEachReverse((number,index)=> number.LogInfo()); // 3, 2, 1
        /// </code>
        /// </summary>
        /// <returns>The each reverse.</returns>
        /// <param name="selfList">Self list.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T, int> action)
        {
            if (action == null) throw new ArgumentException();

            for (var i = selfList.Count - 1; i >= 0; --i)
                action(selfList[i], i);

            return selfList;
        }

        /// <summary>
        /// 遍历列表(可获得索引）
        /// <code>
        /// var testList = new List<int> {1, 2, 3 };
        /// testList.Foreach((number,index)=>number.LogInfo()); // 1, 2, 3,
        /// </code>
        /// </summary>
        /// <typeparam name="T">列表类型</typeparam>
        /// <param name="list">目标表</param>
        /// <param name="action">行为</param>
        public static void ForEach<T>(this List<T> list, Action<int, T> action)
        {
            for (var i = 0; i < list.Count; i++)
            {
                action(i, list[i]);
            }
        }

        #endregion

        #region Dictionary Extension

        /// <summary>
        /// 合并字典
        /// <code>
        /// // 示例
        /// var dictionary1 = new Dictionary<string, string> { { "1", "2" } };
        /// var dictionary2 = new Dictionary<string, string> { { "3", "4" } };
        /// var dictionary3 = dictionary1.Merge(dictionary2);
        /// dictionary3.ForEach(pair => Log.I("{0}:{1}", pair.Key, pair.Value));
        /// </code>
        /// </summary>
        /// <returns>The merge.</returns>
        /// <param name="dictionary">Dictionary.</param>
        /// <param name="dictionaries">Dictionaries.</param>
        /// <typeparam name="TKey">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            params Dictionary<TKey, TValue>[] dictionaries)
        {
            return dictionaries.Aggregate(dictionary,
                (current, dict) => current.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value));
        }

        /// <summary>
        /// 遍历字典
        /// <code>
        /// var dict = new Dictionary<string,string> {{"name","liangxie},{"age","18"}};
        /// dict.ForEach((key,value)=> Log.I("{0}:{1}",key,value);//  name:liangxie    age:18
        /// </code>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="action"></param>
        public static void ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V> action)
        {
            var dictE = dict.GetEnumerator();

            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                action(current.Key, current.Value);
            }

            dictE.Dispose();
        }

        /// <summary>
        /// 字典添加新的词典
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dict"></param>
        /// <param name="addInDict"></param>
        /// <param name="isOverride"></param>
        public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> addInDict,
            bool isOverride = false)
        {
            var dictE = addInDict.GetEnumerator();

            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                if (dict.ContainsKey(current.Key))
                {
                    if (isOverride)
                        dict[current.Key] = current.Value;
                    continue;
                }

                dict.Add(current.Key, current.Value);
            }

            dictE.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// 对 System.IO 的一些扩展
    /// </summary>
    public static class IOExtension
    {
        /// <summary>
        /// 检测路径是否存在，如果不存在则创建
        /// </summary>
        /// <param name="path"></param>
        public static string CreateDirIfNotExists4FilePath(this string path)
        {
            var direct = Path.GetDirectoryName(path);

            if (!Directory.Exists(direct))
            {
                Directory.CreateDirectory(direct);
            }

            return path;
        }


        /// <summary>
        /// 创建新的文件夹,如果存在则不创建
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.CreateDirIfNotExists();
        /// // 结果为，在 Assets 目录下创建 TestFolder
        /// </code>
        /// </summary>
        public static string CreateDirIfNotExists(this string dirFullPath)
        {
            if (!Directory.Exists(dirFullPath))
            {
                Directory.CreateDirectory(dirFullPath);
            }

            return dirFullPath;
        }

        /// <summary>
        /// 删除文件夹，如果存在
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.DeleteDirIfExists();
        /// // 结果为，在 Assets 目录下删除了 TestFolder
        /// </code>
        /// </summary>
        public static void DeleteDirIfExists(this string dirFullPath)
        {
            if (Directory.Exists(dirFullPath))
            {
                Directory.Delete(dirFullPath, true);
            }
        }

        /// <summary>
        /// 清空 Dir（保留目录),如果存在。
        /// <code>
        /// var testDir = "Assets/TestFolder";
        /// testDir.EmptyDirIfExists();
        /// // 结果为，清空了 TestFolder 里的内容
        /// </code>
        /// </summary>
        public static void EmptyDirIfExists(this string dirFullPath)
        {
            if (Directory.Exists(dirFullPath))
            {
                Directory.Delete(dirFullPath, true);
            }

            Directory.CreateDirectory(dirFullPath);
        }

        /// <summary>
        /// 删除文件 如果存在
        /// <code>
        /// // 示例
        /// var filePath = "Assets/Test.txt";
        /// File.Create("Assets/Test);
        /// filePath.DeleteFileIfExists();
        /// // 结果为，删除了 Test.txt
        /// </code>
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <returns> 是否进行了删除操作 </returns>
        public static bool DeleteFileIfExists(this string fileFullPath)
        {
            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 合并路径
        /// <code>
        /// // 示例：
        /// Application.dataPath.CombinePath("Resources").LogInfo();  // /projectPath/Assets/Resources
        /// </code>
        /// </summary>
        /// <param name="selfPath"></param>
        /// <param name="toCombinePath"></param>
        /// <returns> 合并后的路径 </returns>
        public static string CombinePath(this string selfPath, string toCombinePath)
        {
            return Path.Combine(selfPath, toCombinePath);
        }

        #region 未经过测试

#if UNITY_EDITOR
        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFolder(string path)
        {
#if UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start("explorer.exe", path);
#endif
        }
#endif


        /// <summary>
        /// 获取文件夹名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string fileName)
        {
            fileName = MakePathStandard(fileName);
            return fileName.Substring(0, fileName.LastIndexOf('/'));
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <param name="path"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetFileName(string path, char separator = '/')
        {
            path = IOExtension.MakePathStandard(path);
            return path.Substring(path.LastIndexOf(separator) + 1);
        }

        /// <summary>
        /// 获得文件夹下 所有指定文件
        /// </summary>
        /// <param name="dirPath">"(*.jpg|*.bmp)"  "*.*" </param>
        /// <param name="searchPattern">搜索条件</param>
        /// <param name="searchOption">搜索文件夹类型</param>
        /// <returns></returns>
        public static FileInfo[] GetAllFile(string dirPath, string searchPattern, SearchOption searchOption= SearchOption.AllDirectories)
        {
            if (!Directory.Exists(dirPath)) return null;

            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);

            FileInfo[] files = directoryInfo.GetFiles(searchPattern, searchOption);
            return files;
        }


        /// <summary>
        /// 获得文件夹下 所有指定文件
        /// </summary>
        /// <param name="dirPath">"(*.jpg|*.bmp)"  "*.*" </param>
        /// <param name="searchPattern">搜索条件</param>
        /// <param name="searchOption">搜索文件夹类型</param>
        /// <returns></returns>
        public static FileInfo[] GetAllFile(string dirPath, string[] searchPattern, SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(dirPath)) return null;
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            FileInfo[] files = searchPattern.SelectMany(pattern => directoryInfo.GetFiles(pattern, searchOption)).ToArray();

            //FileInfo[] files = directoryInfo.GetFiles(searchPattern, searchOption);
            return files;
        }


        /// <summary>
        /// 获得文件夹下 所有指定文件
        /// </summary>
        /// <param name="dirPath">"(*.jpg|*.bmp)"  "*.*" </param>
        /// <param name="searchPattern">搜索条件</param>
        /// <param name="searchOption">搜索文件夹类型</param>
        /// <returns></returns>
        public static FileInfo[] GetAllFileFilter(string dirPath, string[] searchPattern, string[] filter, SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(dirPath)) return null;
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            DirectoryInfo[] dirs = directoryInfo.GetDirectories();
           
            List<FileInfo> fileInfos = new List<FileInfo>();

            FileInfo[] files = searchPattern.SelectMany(pattern => directoryInfo.GetFiles(pattern, SearchOption.TopDirectoryOnly)).ToArray();
            files.ForEach(fileInfo => { fileInfos.Add(fileInfo); });

            dirs.ForEach(dirInfo=> {
                if (!filter.Contains(dirInfo.Name)) {
                    FileInfo[] files = searchPattern.SelectMany(pattern => dirInfo.GetFiles(pattern, searchOption)).ToArray();
                    files.ForEach(fileInfo => { fileInfos.Add(fileInfo); });
                }
            });
            return fileInfos.ToArray();
        }


        public static DirectoryInfo[] GetAllDirectory(string dirPath) {
            if (!Directory.Exists(dirPath)) return null;

            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            return directoryInfo.GetDirectories();
        }

        /// <summary>
        /// 添加过滤的
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static DirectoryInfo[] GetAllDirectoryFilter(string dirPath,string[] filter)
        {
            if (!Directory.Exists(dirPath)) return null;
            List<DirectoryInfo> dirFiles = new List<DirectoryInfo>();
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            var dirs = directoryInfo.GetDirectories();
            dirs.ForEach(directoryInfo=> {
                if (!filter.Contains(directoryInfo.Name)) {
                    dirFiles.Add(directoryInfo);
                }
            });
            return dirFiles.ToArray();
        }

        /// <summary>
        /// 获取不带后缀的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtention(string fileName, char separator = '/')
        {
            return GetFilePathWithoutExtention(GetFileName(fileName, separator));
        }

        /// <summary>
        /// 获取不带后缀的文件路径
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetFilePathWithoutExtention(string fileName)
        {
            if (fileName.Contains("."))
                return fileName.Substring(0, fileName.LastIndexOf('.'));
            return fileName;
        }

        /// <summary>
        /// 使目录存在,Path可以是目录名必须是文件名
        /// </summary>
        /// <param name="path"></param>
        public static void MakeFileDirectoryExist(string path)
        {
            string root = Path.GetDirectoryName(path);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
        }

        /// <summary>
        /// 使目录存在
        /// </summary>
        /// <param name="path"></param>
        public static void MakeDirectoryExist(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 获取父文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPathParentFolder(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            return Path.GetDirectoryName(path);
        }


        /// <summary>
        /// 使路径标准化，去除空格并将所有'\'转换为'/'
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string MakePathStandard(string path)
        {
            return path.Trim().Replace("\\", "/");
        }

        public static List<string> GetDirSubFilePathList(this string dirABSPath, bool isRecursive = true,
            string suffix = "")
        {
            var pathList = new List<string>();
            var di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }

            var files = di.GetFiles();
            foreach (var fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }

                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                var dirs = di.GetDirectories();
                foreach (var d in dirs)
                {
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        public static List<string> GetDirSubDirNameList(this string dirABSPath)
        {
            var di = new DirectoryInfo(dirABSPath);

            var dirs = di.GetDirectories();

            return dirs.Select(d => d.Name).ToList();
        }

        public static string GetFileName(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");

            return lastIndex >= 0 ? name.Substring(lastIndex + 1) : name;
        }

        public static string GetFileNameWithoutExtend(this string absOrAssetsPath)
        {
            var fileName = GetFileName(absOrAssetsPath);
            var lastIndex = fileName.LastIndexOf(".");

            return lastIndex >= 0 ? fileName.Substring(0, lastIndex) : fileName;
        }

        public static string GetFileExtendName(this string absOrAssetsPath)
        {
            var lastIndex = absOrAssetsPath.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return absOrAssetsPath.Substring(lastIndex);
            }

            return string.Empty;
        }

        public static string GetDirPath(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }

        public static string GetLastDirName(this string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var dirs = name.Split('/');

            return absOrAssetsPath.EndsWith("/") ? dirs[dirs.Length - 2] : dirs[dirs.Length - 1];
        }
        /// <summary>
        /// 如果路径存在，返回文件的内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileConentIfExists(this string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }
            return null;
        }

        #endregion
    }
#if UNITY_5_6_OR_NEWER
    /// <summary>
    /// 简单的概率计算
    /// </summary>
    public static class ProbilityHelper
    {
        public static T RandomValueFrom<T>(params T[] values)
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }

        /// <summary>
        /// percent probability
        /// </summary>
        /// <param name="percent"> 0 ~ 100 </param>
        /// <returns></returns>
        public static bool PercentProbability(int percent)
        {
            return UnityEngine.Random.Range(0, 1000) * 0.001f < 50 * 0.01f;
        }
    }
#endif
    /// <summary>
    /// 面向对象扩展（继承、封装、多态)
    /// </summary>
    public static class OOPExtension
    {
        interface ExampleInterface
        {
        }

        public static void Example()
        {
            if (typeof(OOPExtension).ImplementsInterface<ExampleInterface>())
            {
            }

            if (new object().ImplementsInterface<ExampleInterface>())
            {
            }
        }


        /// <summary>
        /// Determines whether the type implements the specified interface
        /// and is not an interface itself.
        /// </summary>
        /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool ImplementsInterface<T>(this Type type)
        {
            return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
        }

        /// <summary>
        /// Determines whether the type implements the specified interface
        /// and is not an interface itself.
        /// </summary>
        /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool ImplementsInterface<T>(this object obj)
        {
            var type = obj.GetType();
            return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
        }
    }

    /// <summary>
    /// 程序集工具
    /// </summary>
    public class AssemblyUtil
    {
        /// <summary>
        /// 获取 Assembly-CSharp 程序集
        /// </summary>
        public static Assembly DefaultCSharpAssembly
        {
            get
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SingleOrDefault(a => a.GetName().Name == "Assembly-CSharp");
            }
        }

        /// <summary>
        /// 获取默认的程序集中的类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetDefaultAssemblyType(string typeName)
        {
            return DefaultCSharpAssembly.GetType(typeName);
        }
    }


    /// <summary>
    /// 反射扩展
    /// </summary>
    public static class ReflectionExtension
    {
        public static void Example()
        {
            // var selfType = ReflectionExtension.GetAssemblyCSharp().GetType("Framework.ReflectionExtension");
            // selfType.LogInfo();
        }

        public static Assembly GetAssemblyCSharp()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.FullName.StartsWith("Assembly-CSharp,"))
                {
                    return a;
                }
            }

//            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp.dll");
            return null;
        }

        public static Assembly GetAssemblyCSharpEditor()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.FullName.StartsWith("Assembly-CSharp-Editor,"))
                {
                    return a;
                }
            }

//            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp-Editor.dll");
            return null;
        }

        /// <summary>
        /// 通过反射方式调用函数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static object InvokeByReflect(this object obj, string methodName, params object[] args)
        {
            var methodInfo = obj.GetType().GetMethod(methodName);
            return methodInfo == null ? null : methodInfo.Invoke(obj, args);
        }

        /// <summary>
        /// 通过反射方式获取域值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">域名</param>
        /// <returns></returns>
        public static object GetFieldByReflect(this object obj, string fieldName)
        {
            var fieldInfo = obj.GetType().GetField(fieldName);
            return fieldInfo == null ? null : fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// 通过反射方式获取属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyByReflect(this object obj, string propertyName, object[] index = null)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, index);
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this PropertyInfo prop, Type attributeType, bool inherit)
        {
            return prop.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this FieldInfo field, Type attributeType, bool inherit)
        {
            return field.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this MethodInfo method, Type attributeType, bool inherit)
        {
            return method.GetCustomAttributes(attributeType, inherit).Length > 0;
        }


        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this MethodInfo method, bool inherit) where T : Attribute
        {
            var attrs = (T[]) method.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this FieldInfo field, bool inherit) where T : Attribute
        {
            var attrs = (T[]) field.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this PropertyInfo prop, bool inherit) where T : Attribute
        {
            var attrs = (T[]) prop.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var attrs = (T[]) type.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }
    }

    /// <summary>
    /// 类型扩展
    /// </summary>
    public static class TypeEx
    {
        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object DefaultForType(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }

    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtention
    {
        public static void Example()
        {
            var emptyStr = string.Empty;
            emptyStr.IsNotNullAndEmpty();
            emptyStr.IsNullOrEmpty();
            emptyStr = emptyStr.Append("appended").Append("1").ToString();
            emptyStr.IsNullOrEmpty();
        }

        /// <summary>
        /// Check Whether string is null or empty
        /// </summary>
        /// <param name="selfStr"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string selfStr)
        {
            return string.IsNullOrEmpty(selfStr);
        }

        /// <summary>
        /// Check Whether string is null or empty
        /// </summary>
        /// <param name="selfStr"></param>
        /// <returns></returns>
        public static bool IsNotNullAndEmpty(this string selfStr)
        {
            return !string.IsNullOrEmpty(selfStr);
        }

        /// <summary>
        /// Check Whether string trim is null or empty
        /// </summary>
        /// <param name="selfStr"></param>
        /// <returns></returns>
        public static bool IsTrimNotNullAndEmpty(this string selfStr)
        {
            return selfStr != null && !string.IsNullOrEmpty(selfStr.Trim());
        }

        public static bool IsTrimNullOrEmpty(this string selfStr)
        {
            return selfStr == null || string.IsNullOrEmpty(selfStr.Trim());
        }

        /// <summary>
        /// 缓存
        /// </summary>
        private static readonly char[] mCachedSplitCharArray = {'.'};

        /// <summary>
        /// Split
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="splitSymbol"></param>
        /// <returns></returns>
        public static string[] Split(this string selfStr, char splitSymbol)
        {
            mCachedSplitCharArray[0] = splitSymbol;
            return selfStr.Split(mCachedSplitCharArray);
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UppercaseFirst(this string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string LowercaseFirst(this string str)
        {
            return char.ToLower(str[0]) + str.Substring(1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUnixLineEndings(this string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        /// <summary>
        /// 转换成 CSV
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string ToCSV(this string[] values)
        {
            return string.Join(", ", values
                .Where(value => !string.IsNullOrEmpty(value))
                .Select(value => value.Trim())
                .ToArray()
            );
        }

        public static string[] ArrayFromCSV(this string values)
        {
            return values
                .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .ToArray();
        }

        public static string ToSpacedCamelCase(this string text)
        {
            var sb = new StringBuilder(text.Length * 2);
            sb.Append(char.ToUpper(text[0]));
            for (var i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    sb.Append(' ');
                }

                sb.Append(text[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 有点不安全,编译器不会帮你排查错误。
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FillFormat(this string selfStr, params object[] args)
        {
            return string.Format(selfStr, args);
        }

        /// <summary>
        /// 添加前缀
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="toAppend"></param>
        /// <returns></returns>
        public static StringBuilder Append(this string selfStr, string toAppend)
        {
            return new StringBuilder(selfStr).Append(toAppend);
        }

        /// <summary>
        /// 添加后缀
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="toPrefix"></param>
        /// <returns></returns>
        public static string AddPrefix(this string selfStr, string toPrefix)
        {
            return new StringBuilder(toPrefix).Append(selfStr).ToString();
        }

        /// <summary>
        /// 格式化
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="toAppend"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder AppendFormat(this string selfStr, string toAppend, params object[] args)
        {
            return new StringBuilder(selfStr).AppendFormat(toAppend, args);
        }

        /// <summary>
        /// 最后一个单词
        /// </summary>
        /// <param name="selfUrl"></param>
        /// <returns></returns>
        public static string LastWord(this string selfUrl)
        {
            return selfUrl.Split('/').Last();
        }

        /// <summary>
        /// 解析成数字类型
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="defaulValue"></param>
        /// <returns></returns>
        public static int ToInt(this string selfStr, int defaulValue = 0)
        {
            var retValue = defaulValue;
            return int.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }
        public static long ToLong(this string selfStr, long defaulValue = 0)
        {
            var retValue = defaulValue;
            return long.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }

        public static string ToBase64(this string selfStr)
        {
            if (selfStr == null) return null;
            var bys= System.Text.Encoding.Default.GetBytes(selfStr);
            return Convert.ToBase64String(bys);
        }
        public static string ToUTF8(this string selfStr)
        {
            if (selfStr == null) return null;
            var bys = System.Text.Encoding.Default.GetBytes(selfStr);
            return System.Text.Encoding.UTF8.GetString(bys);
        }
        //public static string ToDefault(this string selfStr)
        //{
        //    if (selfStr == null) return null;
        //    var bys = System.Text.ansi.Default.GetBytes(selfStr);

        //}

        public static string Base64ToDefault(this string selfStr)
        {
            if (selfStr == null) return null;
            return System.Text.Encoding.Default.GetString(Convert.FromBase64String(selfStr));
            
        }

        /// <summary>
        /// 解析到时间类型
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string selfStr, DateTime defaultValue = default(DateTime))
        {
            var retValue = defaultValue;
            return DateTime.TryParse(selfStr, out retValue) ? retValue : defaultValue;
        }

        /// <summary>
        /// 获得 时间的格式 如 12:03
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetFormatTime(this float seconds)
        {

            StringBuilder res = new StringBuilder();
            int m = (int)((seconds / 60) % 60);
            int s = (int)(seconds % 60);
            int h = (int)(seconds / 3600);
            if (h > 0)
            {
                if (h < 10) res = res.Append("0");
                res.Append(h.ToString()).Append(":");
            }
            if (m < 10) res = res.Append("0");
            res.Append(m.ToString()).Append(":");
            if (s < 10) res = res.Append("0");
            res.Append(s.ToString());
            return res.ToString();


        }

        /// <summary>
        /// 获得 时间的格式 如 12:03
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string GetFormatTime(this double seconds)
        {
            StringBuilder res = new StringBuilder();
            int m = (int)((seconds / 60)%60);
            int s = (int)(seconds % 60);
            int h = (int)(seconds / 3600);
            if (h > 0)
            {
                if (h < 10) res = res.Append("0");
                res.Append(h.ToString()).Append(":");
            }
            if (m < 10) res = res.Append("0");
            res.Append(m.ToString()).Append(":");
            if (s < 10) res = res.Append("0");
            res.Append(s.ToString());
            return res.ToString();
        }
        public static string GetFormatTime(this long millisecond)
        {
            var seconds = millisecond / 1000;

            StringBuilder res = new StringBuilder();
            int m = (int)((seconds / 60) % 60);
            int s = (int)(seconds % 60);
            int h = (int)(seconds / 3600);
            if (h > 0)
            {
                if (h < 10) res = res.Append("0");
                res.Append(h.ToString()).Append(":");
            }
            if (m < 10) res = res.Append("0");
            res.Append(m.ToString()).Append(":");
            if (s < 10) res = res.Append("0");
            res.Append(s.ToString());
            return res.ToString();
        }

        public static string DateAgo(this long millisecond)
        {
            if (millisecond == 0) return "00:00";
            var millisecondDataTime = MillisecondToDataTime(millisecond);

            if (millisecondDataTime.Day == DateTime.Now.Day)
            {
                return millisecondDataTime.ToString("HH:mm");
            }
            else
            {
                return millisecondDataTime.ToString("MM.dd HH:mm");
            }
        }


        public static DateTime MillisecondToDataTime(this long millisecond) {

            long begtime = millisecond * 10000;
            DateTime dt_1970 = new DateTime(1970, 1, 1, 8, 0, 0);
            long tricks_1970 = dt_1970.Ticks;//1970年1月1日刻度
            long time_tricks = tricks_1970 + begtime;//日志日期刻度
            DateTime dtNew = new DateTime(time_tricks);//转化为DateTime
            return dtNew;
        }

        /// <summary>
        /// 解析 Float 类型
        /// </summary>
        /// <param name="selfStr"></param>
        /// <param name="defaulValue"></param>
        /// <returns></returns>
        public static float ToFloat(this string selfStr, float defaulValue = 0)
        {
            var retValue = defaulValue;
            return float.TryParse(selfStr, out retValue) ? retValue : defaulValue;
        }

        /// <summary>
        /// 是否存在中文字符
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasChinese(this string input)
        {
            return Regex.IsMatch(input, @"[\u4e00-\u9fa5]");
        }

        /// <summary>
        /// 是否存在空格
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool HasSpace(this string input)
        {
            return input.Contains(" ");
        }

        /// <summary>
        /// 删除特定字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string RemoveString(this string str, params string[] targets)
        {
            return targets.Aggregate(str, (current, t) => current.Replace(t, string.Empty));
        }
    }


#if UNITY_5_6_OR_NEWER
    public static class BehaviourExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var component = gameObject.GetComponent<MonoBehaviour>();

            component.Enable(); // component.enabled = true
            component.Disable(); // component.enabled = false
        }

        public static T Enable<T>(this T selfBehaviour) where T : Behaviour
        {
            selfBehaviour.enabled = true;
            return selfBehaviour;
        }

        public static T Disable<T>(this T selfBehaviour) where T : Behaviour
        {
            selfBehaviour.enabled = false;
            return selfBehaviour;
        }
    }

    public static class CameraExtension
    {
        public static void Example()
        {
            var screenshotTexture2D = Camera.main.CaptureCamera(new Rect(0, 0, Screen.width, Screen.height));
            //Log.I(screenshotTexture2D.width);
        }

        public static Texture2D CaptureCamera(this Camera camera, Rect rect)
        {
            var renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
            camera.targetTexture = renderTexture;
            camera.Render();

            RenderTexture.active = renderTexture;

            var screenShot = new Texture2D((int) rect.width, (int) rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);
            screenShot.Apply();

            camera.targetTexture = null;
            RenderTexture.active = null;
            Object.Destroy(renderTexture);

            return screenShot;
        }

        public static void EnableLayer(this Camera camera, int mask)
        {
            camera.cullingMask |= 1 << mask;
        }
        /// <summary>
        /// 添加渲染层
        /// </summary>
        public static void EnableLayer(this Camera camera, string maskName)
        {
            camera.cullingMask |= 1 << LayerMask.NameToLayer(maskName);

        }
        /// <summary>
        /// 移除渲染层
        /// </summary>
        public static void DisableLayer(this Camera camera, int mask)
        {
            camera.cullingMask &= ~(1 << mask);
        }
        /// <summary>
        /// 移除渲染层
        /// </summary>
        public static void DisableLayer(this Camera camera, string maskName)
        {
            camera.cullingMask &= ~(1 << LayerMask.NameToLayer(maskName));

        }

    }

    public static class ColorExtension
    {
        public static void Example()
        {
            var color = "#C5563CFF".HtmlStringToColor();
            //Log.I(color);
        }

        /// <summary>
        /// #C5563CFF -> 197.0f / 255,86.0f / 255,60.0f / 255
        /// </summary>
        /// <param name="htmlString"></param>
        /// <returns></returns>
        public static Color HtmlStringToColor(this string htmlString)
        {
            Color retColor;
            var parseSucceed = ColorUtility.TryParseHtmlString(htmlString, out retColor);
            return parseSucceed ? retColor : Color.black;
        }

        /// <summary>
        /// unity's color always new a color
        /// </summary>
        public static Color White = Color.white;
    }

    /// <summary>
    /// GameObject's Util/Static This Extension
    /// </summary>
    public static class GameObjectExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var transform = gameObject.transform;
            var selfScript = gameObject.AddComponent<MonoBehaviour>();
            var boxCollider = gameObject.AddComponent<BoxCollider>();

            gameObject.Show(); // gameObject.SetActive(true)
            selfScript.Show(); // this.gameObject.SetActive(true)
            boxCollider.Show(); // boxCollider.gameObject.SetActive(true)
            gameObject.transform.Show(); // transform.gameObject.SetActive(true)

            gameObject.Hide(); // gameObject.SetActive(false)
            selfScript.Hide(); // this.gameObject.SetActive(false)
            boxCollider.Hide(); // boxCollider.gameObject.SetActive(false)
            transform.Hide(); // transform.gameObject.SetActive(false)

            selfScript.DestroyGameObj();
            boxCollider.DestroyGameObj();
            transform.DestroyGameObj();

            selfScript.DestroyGameObjGracefully();
            boxCollider.DestroyGameObjGracefully();
            transform.DestroyGameObjGracefully();

            selfScript.DestroyGameObjAfterDelay(1.0f);
            boxCollider.DestroyGameObjAfterDelay(1.0f);
            transform.DestroyGameObjAfterDelay(1.0f);

            selfScript.DestroyGameObjAfterDelayGracefully(1.0f);
            boxCollider.DestroyGameObjAfterDelayGracefully(1.0f);
            transform.DestroyGameObjAfterDelayGracefully(1.0f);

            gameObject.Layer(0);
            selfScript.Layer(0);
            boxCollider.Layer(0);
            transform.Layer(0);

            gameObject.Layer("Default");
            selfScript.Layer("Default");
            boxCollider.Layer("Default");
            transform.Layer("Default");
        }

        #region CEGO001 Show

        public static GameObject Show(this GameObject selfObj)
        {
            selfObj.SetActive(true);
            return selfObj;
        }

        public static T Show<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Show();
            return selfComponent;
        }

        #endregion

        #region CEGO002 Hide

        public static GameObject Hide(this GameObject selfObj)
        {
            selfObj.SetActive(false);
            return selfObj;
        }

        public static T Hide<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Hide();
            return selfComponent;
        }

        #endregion

        #region CEGO003 DestroyGameObj

        public static void DestroyGameObj<T>(this T selfBehaviour) where T : Component
        {
            selfBehaviour.gameObject.DestroySelf();
        }

        #endregion

        #region CEGO004 DestroyGameObjGracefully

        public static void DestroyGameObjGracefully<T>(this T selfBehaviour) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfGracefully();
            }
        }

        #endregion

        #region CEGO005 DestroyGameObjGracefully

        public static T DestroyGameObjAfterDelay<T>(this T selfBehaviour, float delay) where T : Component
        {
            selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            return selfBehaviour;
        }

        public static T DestroyGameObjAfterDelayGracefully<T>(this T selfBehaviour, float delay) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            }

            return selfBehaviour;
        }

        #endregion

        #region CEGO006 Layer

        public static GameObject Layer(this GameObject selfObj, int layer)
        {
            selfObj.layer = layer;
            return selfObj;
        }

        public static T Layer<T>(this T selfComponent, int layer) where T : Component
        {
            selfComponent.gameObject.layer = layer;
            return selfComponent;
        }

        public static GameObject Layer(this GameObject selfObj, string layerName)
        {
            selfObj.layer = LayerMask.NameToLayer(layerName);
            return selfObj;
        }

        public static T Layer<T>(this T selfComponent, string layerName) where T : Component
        {
            selfComponent.gameObject.layer = LayerMask.NameToLayer(layerName);
            return selfComponent;
        }

        /*
         * ——设置Layer
LayerMask mask=1<<需要开启的Layers层
LayerMask mask=0<<需要关闭的Layers层
开启第5层
LayerMask mask=1<<5或LayerMask mask=1<<LayerMask.NameToLayer("UI")
关闭第五层
LayerMask mask=0<<5或LayerMask mask=0<<LayerMask.NameToLayer("UI")
开启第5层和第六层
LayerMask mask=1<<5|1<<6
给相机添加渲染层
Component<Camera>().cullingMask |= 1 << 5
给相机移除渲染层
Component<Camera>().cullingMask &= ~(1 << 5)
相机只保留UI层
camera.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
开关UI层，如果是开的则关，如果是关的则开
camera.cullingMask ^= 1 << LayerMask.NameToLayer("UI");
————————————————
         * 
        */
        /// <summary>
        /// 添加渲染层
        /// </summary>
        public static void EnableLayer(this LayerMask layerMask , int mask)
        {
            layerMask |= 1 << mask;
        }
        /// <summary>
        /// 添加渲染层
        /// </summary>
        public static void EnableLayer(this LayerMask layerMask,  string maskName)
        {
            layerMask |= 1 << LayerMask.NameToLayer(maskName);

        }
        /// <summary>
        /// 移除渲染层
        /// </summary>
        public static void DisableLayer(this LayerMask layerMask,  int mask)
        {
            layerMask &= ~(1 << mask);
        } 
          /// <summary>
          /// 移除渲染层
          /// </summary>
        public static void DisableLayer(this LayerMask layerMask,  string maskName)
        {
            layerMask &= ~(1 << LayerMask.NameToLayer(maskName));

        }



        #endregion

        #region CEGO007 Component

        public static T GetOrAddComponent<T>(this GameObject selfComponent) where T : Component
        {
            var comp = selfComponent.gameObject.GetComponent<T>();
            return comp ? comp : selfComponent.gameObject.AddComponent<T>();
        }

        public static Component GetOrAddComponent(this GameObject selfComponent, Type type)
        {
            var comp = selfComponent.gameObject.GetComponent(type);
            return comp ? comp : selfComponent.gameObject.AddComponent(type);
        }

        #endregion
    }

    public static class GraphicExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var image = gameObject.AddComponent<Image>();
            var rawImage = gameObject.AddComponent<RawImage>();

            // image.color = new Color(image.color.r,image.color.g,image.color.b,1.0f);
            image.ColorAlpha(1.0f);
            rawImage.ColorAlpha(1.0f);
        }

        public static T ColorAlpha<T>(this T selfGraphic, float alpha) where T : Graphic
        {
            var color = selfGraphic.color;
            color.a = alpha;
            selfGraphic.color = color;
            return selfGraphic;
        }
    }

    public static class ImageExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();
            var image1 = gameObject.AddComponent<Image>();

            image1.FillAmount(0.0f); // image1.fillAmount = 0.0f;
        }

        public static Image FillAmount(this Image selfImage, float fillamount)
        {
            selfImage.fillAmount = fillamount;
            return selfImage;
        }
    }

    public static class LightmapExtension
    {
        public static void SetAmbientLightHTMLStringColor(string htmlStringColor)
        {
            RenderSettings.ambientLight = htmlStringColor.HtmlStringToColor();
        }
    }

    public static class ObjectExtension
    {
        public static void Example()
        {
            var gameObject = new GameObject();

            gameObject.Instantiate()
                .Name("ExtensionExample")
                .DestroySelf();

            gameObject.Instantiate()
                .DestroySelfGracefully();

            gameObject.Instantiate()
                .DestroySelfAfterDelay(1.0f);

            gameObject.Instantiate()
                .DestroySelfAfterDelayGracefully(1.0f);

            gameObject
                .ApplySelfTo(selfObj => Debug.Log(selfObj.name))
                .Name("TestObj")
                .ApplySelfTo(selfObj => Debug.Log(selfObj.name))
                .Name("ExtensionExample")
                .DontDestroyOnLoad();
        }


        #region CEUO001 Instantiate

        public static T Instantiate<T>(this T selfObj) where T : Object
        {
            return Object.Instantiate(selfObj);
        }

        public static T Instantiate<T>(this T selfObj, Vector3 position, Quaternion rotation) where T : Object
        {
            return (T) Object.Instantiate((Object) selfObj, position, rotation);
        }

        public static T Instantiate<T>(
            this T selfObj,
            Vector3 position,
            Quaternion rotation,
            Transform parent)
            where T : Object
        {
            return (T) Object.Instantiate((Object) selfObj, position, rotation, parent);
        }


        public static T InstantiateWithParent<T>(this T selfObj, Transform parent, bool worldPositionStays)
            where T : Object
        {
            return (T) Object.Instantiate((Object) selfObj, parent, worldPositionStays);
        }

        public static T InstantiateWithParent<T>(this T selfObj, Transform parent) where T : Object
        {
            return Object.Instantiate(selfObj, parent, false);
        }

        #endregion

        #region CEUO002 Name

        public static T Name<T>(this T selfObj, string name) where T : Object
        {
            selfObj.name = name;
            return selfObj;
        }

        #endregion

        #region CEUO003 Destroy Self

        public static void DestroySelf<T>(this T selfObj) where T : Object
        {
            Object.Destroy(selfObj);
        }

        public static T DestroySelfGracefully<T>(this T selfObj) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj);
            }

            return selfObj;
        }

        #endregion

        #region CEUO004 Destroy Self AfterDelay 

        public static T DestroySelfAfterDelay<T>(this T selfObj, float afterDelay) where T : Object
        {
            Object.Destroy(selfObj, afterDelay);
            return selfObj;
        }

        public static T DestroySelfAfterDelayGracefully<T>(this T selfObj, float delay) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj, delay);
            }

            return selfObj;
        }

        #endregion

        #region CEUO005 Apply Self To 

        public static T ApplySelfTo<T>(this T selfObj, System.Action<T> toFunction) where T : Object
        {
            toFunction.InvokeGracefully(selfObj);
            return selfObj;
        }

        #endregion

        #region CEUO006 DontDestroyOnLoad

        public static T DontDestroyOnLoad<T>(this T selfObj) where T : Object
        {
            Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }

        #endregion

        public static T As<T>(this object selfObj) where T : class
        {
            return selfObj as T;
        }
    }

    public static class RectTransformExtension
    {
        public static Vector2 GetPosInRootTrans(this RectTransform selfRectTransform, Transform rootTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(rootTrans, selfRectTransform).center;
        }

        public static RectTransform AnchorPosX(this RectTransform selfRectTrans, float anchorPosX)
        {
            var anchorPos = selfRectTrans.anchoredPosition;
            anchorPos.x = anchorPosX;
            selfRectTrans.anchoredPosition = anchorPos;
            return selfRectTrans;
        }

        public static RectTransform AnchorPosY(this RectTransform selfRectTrans, float anchorPosY)
        {
            var anchorPos = selfRectTrans.anchoredPosition;
            anchorPos.y = anchorPosY;
            selfRectTrans.anchoredPosition = anchorPos;
            return selfRectTrans;
        }

        public static RectTransform SetSizeWidth(this RectTransform selfRectTrans, float sizeWidth)
        {
            var sizeDelta = selfRectTrans.sizeDelta;
            sizeDelta.x = sizeWidth;
            selfRectTrans.sizeDelta = sizeDelta;
            return selfRectTrans;
        }

        public static RectTransform SetSizeHeight(this RectTransform selfRectTrans, float sizeHeight)
        {
            var sizeDelta = selfRectTrans.sizeDelta;
            sizeDelta.y = sizeHeight;
            selfRectTrans.sizeDelta = sizeDelta;
            return selfRectTrans;
        }

        public static Vector2 GetWorldSize(this RectTransform selfRectTrans)
        {
            return RectTransformUtility.CalculateRelativeRectTransformBounds(selfRectTrans).size;
        }
    }

    public static class SelectableExtension
    {
        public static T EnableInteract<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.interactable = true;
            return selfSelectable;
        }

        public static T DisableInteract<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.interactable = false;
            return selfSelectable;
        }

        public static T CancalAllTransitions<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.transition = Selectable.Transition.None;
            return selfSelectable;
        }
    }

    public static class ToggleExtension
    {
        public static void RegOnValueChangedEvent(this Toggle selfToggle, UnityAction<bool> onValueChangedEvent)
        {
            selfToggle.onValueChanged.AddListener(onValueChangedEvent);
        }
    }

    /// <summary>
    /// Transform's Extension
    /// </summary>
    public static class TransformExtension
    {
        public static void Example()
        {
            var selfScript = new GameObject().AddComponent<MonoBehaviour>();
            var transform = selfScript.transform;

            //transform
            //    .Parent(null)
            //    .LocalIdentity()
            //    .LocalPositionIdentity()
            //    .LocalRotationIdentity()
            //    .LocalScaleIdentity()
            //    .LocalPosition(Vector3.zero)
            //    .LocalPosition(0, 0, 0)
            //    .LocalPosition(0, 0)
            //    .LocalPositionX(0)
            //    .LocalPositionY(0)
            //    .LocalPositionZ(0)
            //    .LocalRotation(Quaternion.identity)
            //    .LocalScale(Vector3.one)
            //    .LocalScaleX(1.0f)
            //    .LocalScaleY(1.0f)
            //    .Identity()
            //    .PositionIdentity()
            //    .RotationIdentity()
            //    .Position(Vector3.zero)
            //    .PositionX(0)
            //    .PositionY(0)
            //    .PositionZ(0)
            //    .Rotation(Quaternion.identity)
            //    .DestroyAllChild()
            //    .AsLastSibling()
            //    .AsFirstSibling()
            //    .SiblingIndex(0);

            //selfScript
            //    .Parent(null)
            //    .LocalIdentity()
            //    .LocalPositionIdentity()
            //    .LocalRotationIdentity()
            //    .LocalScaleIdentity()
            //    .LocalPosition(Vector3.zero)
            //    .LocalPosition(0, 0, 0)
            //    .LocalPosition(0, 0)
            //    .LocalPositionX(0)
            //    .LocalPositionY(0)
            //    .LocalPositionZ(0)
            //    .LocalRotation(Quaternion.identity)
            //    .LocalScale(Vector3.one)
            //    .LocalScaleX(1.0f)
            //    .LocalScaleY(1.0f)
            //    .Identity()
            //    .PositionIdentity()
            //    .RotationIdentity()
            //    .Position(Vector3.zero)
            //    .PositionX(0)
            //    .PositionY(0)
            //    .PositionZ(0)
            //    .Rotation(Quaternion.identity)
            //    .DestroyAllChild()
            //    .AsLastSibling()
            //    .AsFirstSibling()
            //    .SiblingIndex(0);
        }


        /// <summary>
        /// 缓存的一些变量,免得每次声明
        /// </summary>
        private static Vector3 mLocalPos;

        private static Vector3 mScale;
        private static Vector3 mPos;

        #region CETR001 Parent

        public static T Parent<T>(this T selfComponent, Component parentComponent) where T : Component
        {
            selfComponent.transform.SetParent(parentComponent == null ? null : parentComponent.transform);
            return selfComponent;
        }

        /// <summary>
        /// 设置成为顶端 Transform
        /// </summary>
        /// <returns>The root transform.</returns>
        /// <param name="selfComponent">Self component.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T AsRootTransform<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetParent(null);
            return selfComponent;
        }

        #endregion

        #region CETR002 LocalIdentity

        public static T LocalIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            selfComponent.transform.localRotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR003 LocalPosition

        public static T LocalPosition<T>(this T selfComponent, Vector3 localPos) where T : Component
        {
            selfComponent.transform.localPosition = localPos;
            return selfComponent;
        }

        public static Vector3 GetLocalPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localPosition;
        }


        public static T LocalPosition<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            selfComponent.transform.localPosition = new Vector3(x, y, z);
            return selfComponent;
        }

        public static T LocalPosition<T>(this T selfComponent, float x, float y) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.x = x;
            mLocalPos.y = y;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionX<T>(this T selfComponent, float x) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.x = x;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionY<T>(this T selfComponent, float y) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.y = y;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }

        public static T LocalPositionZ<T>(this T selfComponent, float z) where T : Component
        {
            mLocalPos = selfComponent.transform.localPosition;
            mLocalPos.z = z;
            selfComponent.transform.localPosition = mLocalPos;
            return selfComponent;
        }


        public static T LocalPositionIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localPosition = Vector3.zero;
            return selfComponent;
        }

        #endregion

        #region CETR004 LocalRotation

        public static Quaternion GetLocalRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localRotation;
        }

        public static T LocalRotation<T>(this T selfComponent, Quaternion localRotation) where T : Component
        {
            selfComponent.transform.localRotation = localRotation;
            return selfComponent;
        }

        public static T LocalRotationIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localRotation = Quaternion.identity;
            return selfComponent;
        }

        #endregion

        #region CETR005 LocalScale

        public static T LocalScale<T>(this T selfComponent, Vector3 scale) where T : Component
        {
            selfComponent.transform.localScale = scale;
            return selfComponent;
        }

        public static Vector3 GetLocalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.localScale;
        }

        public static T LocalScale<T>(this T selfComponent, float xyz) where T : Component
        {
            selfComponent.transform.localScale = Vector3.one * xyz;
            return selfComponent;
        }

        public static T LocalScale<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            mScale.y = y;
            mScale.z = z;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScale<T>(this T selfComponent, float x, float y) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            mScale.y = y;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleX<T>(this T selfComponent, float x) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.x = x;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleY<T>(this T selfComponent, float y) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.y = y;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleZ<T>(this T selfComponent, float z) where T : Component
        {
            mScale = selfComponent.transform.localScale;
            mScale.z = z;
            selfComponent.transform.localScale = mScale;
            return selfComponent;
        }

        public static T LocalScaleIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR006 Identity

        public static T Identity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.position = Vector3.zero;
            selfComponent.transform.rotation = Quaternion.identity;
            selfComponent.transform.localScale = Vector3.one;
            return selfComponent;
        }

        #endregion

        #region CETR007 Position

        public static T Position<T>(this T selfComponent, Vector3 position) where T : Component
        {
            selfComponent.transform.position = position;
            return selfComponent;
        }

        public static Vector3 GetPosition<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.position;
        }

        public static T Position<T>(this T selfComponent, float x, float y, float z) where T : Component
        {
            selfComponent.transform.position = new Vector3(x, y, z);
            return selfComponent;
        }

        public static T Position<T>(this T selfComponent, float x, float y) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = x;
            mPos.y = y;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.position = Vector3.zero;
            return selfComponent;
        }

        public static T PositionX<T>(this T selfComponent, float x) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = x;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionX<T>(this T selfComponent, Func<float, float> xSetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.x = xSetter(mPos.x);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionY<T>(this T selfComponent, float y) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.y = y;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionY<T>(this T selfComponent, Func<float, float> ySetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.y = ySetter(mPos.y);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionZ<T>(this T selfComponent, float z) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.z = z;
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        public static T PositionZ<T>(this T selfComponent, Func<float, float> zSetter) where T : Component
        {
            mPos = selfComponent.transform.position;
            mPos.z = zSetter(mPos.z);
            selfComponent.transform.position = mPos;
            return selfComponent;
        }

        #endregion

        #region CETR008 Rotation

        public static T RotationIdentity<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.rotation = Quaternion.identity;
            return selfComponent;
        }

        public static T Rotation<T>(this T selfComponent, Quaternion rotation) where T : Component
        {
            selfComponent.transform.rotation = rotation;
            return selfComponent;
        }

        public static Quaternion GetRotation<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.rotation;
        }

        #endregion

        #region CETR009 WorldScale/LossyScale/GlobalScale/Scale

        public static Vector3 GetGlobalScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetWorldScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        public static Vector3 GetLossyScale<T>(this T selfComponent) where T : Component
        {
            return selfComponent.transform.lossyScale;
        }

        #endregion

        #region CETR010 Destroy All Child

        [Obsolete("弃用啦 请使用 DestroyChildren")]
        public static T DestroyAllChild<T>(this T selfComponent) where T : Component
        {
            return selfComponent.DestroyChildren();
        }

        [Obsolete("弃用啦 请使用 DestroyChildren")]
        public static GameObject DestroyAllChild(this GameObject selfGameObj)
        {
            return selfGameObj.DestroyChildren();
        }

        public static T DestroyChildren<T>(this T selfComponent) where T : Component
        {
            var childCount = selfComponent.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfComponent.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfComponent;
        }

        public static GameObject DestroyChildren(this GameObject selfGameObj)
        {
            var childCount = selfGameObj.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfGameObj.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfGameObj;
        }

        #endregion

        #region CETR011 Sibling Index

        public static T AsLastSibling<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetAsLastSibling();
            return selfComponent;
        }

        public static T AsFirstSibling<T>(this T selfComponent) where T : Component
        {
            selfComponent.transform.SetAsFirstSibling();
            return selfComponent;
        }

        public static T SiblingIndex<T>(this T selfComponent, int index) where T : Component
        {
            selfComponent.transform.SetSiblingIndex(index);
            return selfComponent;
        }

        #endregion


        public static Transform FindByPath(this Transform selfTrans, string path)
        {
            return selfTrans.Find(path.Replace(".", "/"));
        }

        public static Transform SeekTrans(this Transform selfTransform, string uniqueName)
        {
            var childTrans = selfTransform.Find(uniqueName);

            if (null != childTrans)
                return childTrans;

            foreach (Transform trans in selfTransform)
            {
                childTrans = trans.SeekTrans(uniqueName);

                if (null != childTrans)
                    return childTrans;
            }

            return null;
        }

        public static T ShowChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
        {
            selfComponent.transform.Find(tranformPath).gameObject.Show();
            return selfComponent;
        }

        public static T HideChildTransByPath<T>(this T selfComponent, string tranformPath) where T : Component
        {
            selfComponent.transform.Find(tranformPath).Hide();
            return selfComponent;
        }

        public static void CopyDataFromTrans(this Transform selfTrans, Transform fromTrans)
        {
            selfTrans.SetParent(fromTrans.parent);
            selfTrans.localPosition = fromTrans.localPosition;
            selfTrans.localRotation = fromTrans.localRotation;
            selfTrans.localScale = fromTrans.localScale;
        }

        /// <summary>
        /// 递归遍历子物体，并调用函数
        /// </summary>
        /// <param name="tfParent"></param>
        /// <param name="action"></param>
        public static void ActionRecursion(this Transform tfParent, Action<Transform> action)
        {
            action(tfParent);
            foreach (Transform tfChild in tfParent)
            {
                tfChild.ActionRecursion(action);
            }
        }

        /// <summary>
        /// 递归遍历查找指定的名字的子物体
        /// </summary>
        /// <param name="tfParent">当前Transform</param>
        /// <param name="name">目标名</param>
        /// <param name="stringComparison">字符串比较规则</param>
        /// <returns></returns>
        public static Transform FindChildRecursion(this Transform tfParent, string name,
            StringComparison stringComparison = StringComparison.Ordinal)
        {
            if (tfParent.name.Equals(name, stringComparison))
            {
                //Log.I("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursion(name, stringComparison);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }

        /// <summary>
        /// 递归遍历查找相应条件的子物体
        /// </summary>
        /// <param name="tfParent">当前Transform</param>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public static Transform FindChildRecursion(this Transform tfParent, Func<Transform, bool> predicate)
        {
            if (predicate(tfParent))
            {
                Debug.Log("Hit " + tfParent.name);
                return tfParent;
            }

            foreach (Transform tfChild in tfParent)
            {
                Transform tfFinal = null;
                tfFinal = tfChild.FindChildRecursion(predicate);
                if (tfFinal)
                {
                    return tfFinal;
                }
            }

            return null;
        }

        public static string GetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }


        public static T GetComponentOrAdd<T>(this Component component) where T : Component
        {
            var com = component.GetComponent<T>();
            if (com == null)
            {
                com=component.gameObject.AddComponent<T>();
            }
            return com;
        }

    }

    public static class UnityActionExtension
    {
        public static void Example()
        {
            UnityAction action = () => { };
            UnityAction<int> actionWithInt = num => { };
            UnityAction<int, string> actionWithIntString = (num, str) => { };

            action.InvokeGracefully();
            actionWithInt.InvokeGracefully(1);
            actionWithIntString.InvokeGracefully(1, "str");
        }

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        public static bool InvokeGracefully(this UnityAction selfAction)
        {
            if (null != selfAction)
            {
                selfAction();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool InvokeGracefully<T>(this UnityAction<T> selfAction, T t)
        {
            if (null != selfAction)
            {
                selfAction(t);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Call action
        /// </summary>
        /// <param name="selfAction"></param>
        /// <returns> call succeed</returns>
        public static bool InvokeGracefully<T, K>(this UnityAction<T, K> selfAction, T t, K k)
        {
            if (null != selfAction)
            {
                selfAction(t, k);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获得随机列表中元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <param name="list">列表</param>
        /// <returns></returns>
        public static T GetRandomItem<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }


        /// <summary>
        /// 根据权值来获取索引
        /// </summary>
        /// <param name="powers"></param>
        /// <returns></returns>
        public static int GetRandomWithPower(this List<int> powers)
        {
            var sum = 0;
            foreach (var power in powers)
            {
                sum += power; 
            }

            var randomNum = UnityEngine.Random.Range(0, sum);
            var currentSum = 0;
            for (var i = 0; i < powers.Count; i++)
            {
                var nextSum = currentSum + powers[i];
                if (randomNum >= currentSum && randomNum <= nextSum)
                {
                    return i;
                }

                currentSum = nextSum;
            }

            Debug.LogError("权值范围计算错误！");
            return -1;
        }

        /// <summary>
        /// 根据权值获取值，Key为值，Value为权值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="powersDict"></param>
        /// <returns></returns>
        public static T GetRandomWithPower<T>(this Dictionary<T, int> powersDict)
        {
            var keys = new List<T>();
            var values = new List<int>();

            foreach (var key in powersDict.Keys)
            {
                keys.Add(key);
                values.Add(powersDict[key]);
            }

            var finalKeyIndex = values.GetRandomWithPower();
            return keys[finalKeyIndex];
        }
    }

  

    public static class TimeExtension
    {

        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timeStamp).ToLocalTime();
        }

        /// <summary>
        /// 从现在过去 time毫秒 后的时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetLongDeteString(this long time)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(time);
            DateTime dt = Convert.ToDateTime(DateTime.Now); 
            DateTime dtNew = dt.AddDays(span.Days);
            return dtNew.ToLongDateString();  
        }
         
        /// <summary>
        /// 从标准时间（1970 开始）
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetLongDeteForStampString(this long time)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(time);
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime dtNew = dt.AddDays(span.Days);
            return dtNew.ToLongDateString();
        }



        /// <summary>
        /// 时间戳 北京时间 8 点
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static Int64 GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1,0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
        /// <summary>
        /// 时间戳转本地时间
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime GetLocalDateTime(double unixTimeStamp)
        {

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); //当地时区
            DateTime dtime = startTime.AddSeconds(unixTimeStamp);
            return dtime;
        }


       
    }

    public static class IDExtension
    {
        static IdUtil idUtil;
        public static Int64 GetNewId()
        {
            if (idUtil == null) idUtil = new IdUtil(20, 20, 20);
            return idUtil.GenerateNewId(100);
        }
    }


#endif
}