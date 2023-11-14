/****************************************************
    文件：DeveloperExtension.cs
	作者：DULE
    邮箱: 1055767424@qq.com
    日期：2021/1/5 19:58:35
	功能：开发者 积累语法糖拓展
*****************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine.UI;

public static class DeveloperExtension
{
    #region 颜色、

    private const string ColorFormat = "";

    public static string SetColor(this string str, Color c)
    {
        return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(c), str);
    }

    public static string ToHtml(this Color c)
    {
        return ColorUtility.ToHtmlStringRGB(c);
    }

    #endregion

    /// <summary>
    /// 获取中间索引
    /// </summary>
    /// <param name="numbers">带排序数组</param>
    /// <param name="low">开始位置</param>
    /// <param name="high">结束位置</param>
    /// <returns></returns>
    private static int getMiddle<T>(T[] numbers, int low, int high) where T : IComparable
    {
        T value = numbers[low]; //数组的第一个作为中轴
        T temp = default(T);
        while (low < high)
        {
            while (low < high && numbers[high].CompareTo(value) == 1)
            {
                high--;
            }
            if (low < high)
            {
                temp = numbers[high];
                numbers[high] = value;
                numbers[low] = temp;
                low++;
            }
            while (low < high && numbers[low].CompareTo(value) == -1)
            {
                low++;
            }
            if (low < high)
            {
                temp = numbers[low];
                numbers[low] = value;
                numbers[high] = temp;
                high--;
            }
        }
        // numbers[low] = temp; //中轴记录到尾
        return low; // 返回中轴的位置
    }

    private static int getMiddle<T>(T[] numbers, int low, int high, Func<T, IComparable> func)
    {
        T value = numbers[low]; //数组的第一个作为中轴
        T temp = default(T);
        while (low < high)
        {
            while (low < high && func(numbers[high]).CompareTo(func(value)) == 1)
            {
                high--;
            }
            if (low < high)
            {
                temp = numbers[high];
                numbers[high] = value;
                numbers[low] = temp;
                low++;
            }
            while (low < high && func(numbers[low]).CompareTo(func(value)) == -1)
            {
                low++;
            }
            if (low < high)
            {
                temp = numbers[low];
                numbers[low] = value;
                numbers[high] = temp;
                high--;
            }
        }
        // numbers[low] = temp; //中轴记录到尾
        return low; // 返回中轴的位置
    }

    /// <summary>
    /// 获取中间索引
    /// </summary>
    /// <param name="numbers">带排序数组</param>
    /// <param name="low">开始位置</param>
    /// <param name="high">结束位置</param>
    private static void quickSort<T>(T[] numbers, int low, int high) where T : IComparable
    {
        if (low < high)
        {
            int middle = getMiddle(numbers, low, high); //将numbers数组进行一分为二
            quickSort(numbers, low, middle - 1); //对低字段表进行递归排序
            quickSort(numbers, middle + 1, high); //对高字段表进行递归排序
        }
    }



    private static void quickSort<T>(T[] numbers, int low, int high, Func<T, IComparable> func)
    {
        if (low < high)
        {
            int middle = getMiddle(numbers, low, high, func); //将numbers数组进行一分为二
            quickSort(numbers, low, middle - 1, func); //对低字段表进行递归排序
            quickSort(numbers, middle + 1, high, func); //对高字段表进行递归排序
        }
    }
    public static void Quick<T>(this T[] numbers, Func<T, IComparable> func)
    {
        if (numbers.Length > 0)
        { //查看数组是否为空{
            quickSort(numbers, 0, numbers.Length - 1, func);
        }
    }

    /// <summary>
    /// 获取中间索引
    /// </summary>
    public static void Quick<T>(this T[] numbers) where T : IComparable
    {
        if (numbers.Length > 0)
        { //查看数组是否为空{
            quickSort(numbers, 0, numbers.Length - 1);
        }
    }


    public static T Get<T>(this List<T> numbers, Predicate<T> func)
    {
        return numbers.Find(func);
    }
    public static string GetFormatTime(this int seconds)
    {
        StringBuilder res = new StringBuilder();
        TimeSpan ts = new TimeSpan(0, 0, seconds);
        string m = ts.Minutes.ToString();
        if (ts.Minutes < 10)
        {
            m = "0" + m;
        }

        string s = ts.Seconds.ToString();
        if (ts.Seconds < 10)
        {
            s = "0" + s;
        }

        if (ts.Hours > 0)
        {
            res.Append(ts.Hours.ToString()).Append(":").Append(m).Append(":").Append(s);
            //str = ts.Hours.ToString() + "小时 " + ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
        }
        if (ts.Hours == 0 && ts.Minutes > 0)
        {
            res.Append(m).Append(":").Append(s);
            //str = ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
        }
        if (ts.Hours == 0 && ts.Minutes == 0)
        {
            res.Append("00").Append(":").Append(s);

        }
        return res.ToString();
        //目前到分钟

        //int m = seconds / 60;
        //int s = seconds % 60;
        //if (m < 10) res = res.Append("0");
        //res.Append(m.ToString()).Append(":");
        //if (s < 10) res = res.Append("0");
        //res.Append(s.ToString());
        //return res.ToString();
    }
    //public static string GetFormatTime(this long seconds)
    //{

    //    StringBuilder res = new StringBuilder();
    //    TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
    //    string m = ts.Minutes.ToString();
    //    if (ts.Minutes < 10)
    //    {
    //        m = "0" + m;
    //    }

    //    string s = ts.Seconds.ToString();
    //    if (ts.Seconds < 10)
    //    {
    //        s = "0" + s;
    //    }
        
    //    if (ts.Hours > 0)
    //    {
    //        res.Append(ts.Hours.ToString()).Append(":").Append(m).Append(":").Append(s);
    //        //str = ts.Hours.ToString() + "小时 " + ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
    //    }
    //    if (ts.Hours == 0 && ts.Minutes > 0)
    //    {
    //        res.Append(m).Append(":").Append(s);
    //        //str = ts.Minutes.ToString() + "分钟 " + ts.Seconds + "秒";
    //    }
    //    if (ts.Hours == 0 && ts.Minutes == 0)
    //    {
    //        res.Append("00").Append(":").Append(s);

    //    }
    //    return res.ToString();
    //    ////目前到分钟
    //    //StringBuilder res = new StringBuilder();
    //    //int m =(int)(seconds / 60);
    //    //int s = (int)(seconds % 60);
    //    //if (m < 10) res = res.Append("0");
    //    //res.Append(m.ToString()).Append(":");
    //    //if (s < 10) res = res.Append("0");
    //    //res.Append(s.ToString());
    //    //return res.ToString();
    //}

    static List<int> res;
    /// <summary>
    ///  随机在数组中 取不相同的随机个数
    /// </summary>
    /// <param name="count">数组的个数</param>
    /// <param name="radCount">随机个数</param>
    /// <returns></returns>
    public static List<int> RandomForListCount(int  count , int radCount)//count表示要从数组中选多少个元素
    {
        res =null;
        if (count <= 1|| radCount > count) return null;
        res = new List<int>();
        
        var num= new List<int>(count);
        for (int i = 0; i < count; i++)
        {
            num.Add(i);
        }

        int length = num.Count;
        for (int i = 0; i < radCount; i++)
        {
            int index = (int)(UnityEngine.Random.Range(0, length));//产生下标
            Debug.Log(" " + num[index]);
            res.Add(num[index]);
            num[index] = num[length - 1];
            length--;
        }
        return res;
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="path"></param>
    public static void OpenDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return;

        path = path.Replace("/", "\\");
        if (!Directory.Exists(path))
        {
            Debug.LogError("No Directory: " + path);
            return;
        }
        //可能360不信任
        System.Diagnostics.Process.Start("explorer.exe", path);
    }

    public static Texture2D Base64ToTexture2D(this string Base64STR)
    {
        Texture2D pic = new Texture2D(190, 190, TextureFormat.RGBA32, false);
        byte[] data = System.Convert.FromBase64String(Base64STR);
        pic.LoadImage(data);
        return pic;
    }

    public static String Texture2DToBase64JPG(this Texture2D t2d)
    {
        byte[] bytesArr = t2d.EncodeToJPG();
        string strbaser64 = Convert.ToBase64String(bytesArr);
        return strbaser64;
    }

    public static void SaveCacheImage(this Texture2D image, string folderPath, string imageName)
    {
        if (!string.IsNullOrEmpty(folderPath) && image != null && !string.IsNullOrEmpty(imageName))
        {
            if (Directory.Exists(folderPath))
            {
                try
                {
                    string texturePath = Path.Combine(folderPath, string.Format("{0}", imageName));
                    byte[] bytes = image.EncodeToPNG();
                    System.IO.File.WriteAllBytes(texturePath, bytes);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("cls:PageViewShowControl cls:SaveCacheImage ExceptionInfo:" + ex);
                }
            }
            else
            {
                Debug.LogError("cls:PageViewShowControl cls:SaveCacheImage info:CacheFoder  isn't exist, path:!" + folderPath);
            }
        }
    }


    public static IEnumerator UpdateLayout(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        yield return new WaitForEndOfFrame();
        if (rect.rect.width == 0|| rect.rect.height==0)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            yield return new WaitForEndOfFrame();
        }
    }

    public static bool IsPointerOverUGUI()
    {
        if (UnityEngine.EventSystems.EventSystem.current)
        {
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }
        return false;
    }


    public static void RestructuringContnet(this TMPro.TMP_InputField inputField, string content)
    {

        if (string.IsNullOrEmpty( inputField.text))
        {
            inputField.text = content;
            inputField.MoveTextEnd(false);
        }
        else
        {

            var postion = inputField.selectionStringFocusPosition;
            var beforTxt = inputField.text.Substring(0, postion);
            var endTxt = "";
            var isMoveEnd = true;
            if (postion != inputField.text.Length)
            {
                isMoveEnd = false;
                endTxt = inputField.text.Substring(postion, inputField.text.Length - postion);
            }

            if (isMoveEnd)
            {
                inputField.text = beforTxt + content + endTxt;
                inputField.MoveTextEnd(false);
            }
            else
            {
                inputField.text = beforTxt + content;
                inputField.MoveTextEnd(false);
                inputField.text = beforTxt + content + endTxt;
            }
        }
    }


    public static void DeleteContnet(this TMPro.TMP_InputField inputField)
    {

        if (string.IsNullOrEmpty(inputField.text))
        {
            inputField.MoveTextEnd(false);
        }
        else
        {

            var postion = inputField.selectionStringFocusPosition;
            var beforTxt = inputField.text.Substring(0, postion);
            var endTxt = "";
            if (postion != 0 && postion != inputField.text.Length)
            {
                endTxt = inputField.text.Substring(postion, inputField.text.Length - postion);
            }
            beforTxt = beforTxt.Substring(0, beforTxt.Length - 1);

            if (string.IsNullOrEmpty(beforTxt))
            {
                inputField.text = beforTxt;
            }
            inputField.MoveTextEnd(false);
            inputField.text = beforTxt + endTxt;
        }
    }

    public static void Clear(this TMPro.TMP_InputField inputfield)
    {
        inputfield.Select();
        inputfield.text = "";
    }

    #region Long

    /// <summary>
    /// 将一个long 类型格式话 12000= 1.2w
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string format2String(this long number)
    {
        if (number <= 0)
        {
            return "0";
        }
        else if (number < 10000)
        {
            return number.ToString();
        }
        else
        {
            double d = (double)number;
            double num = d / 10000.0;
            return num.ToString("#0.0") + "w";
        }
    }

    #endregion

    #region String
    /// <summary>
    /// Base64加密，采用utf8编码方式加密
    /// </summary>
    /// <param name="source">待加密的明文</param>
    /// <returns>加密后的字符串</returns>
    public static string Base64Encode(this string source)
    {
        return source.Base64Encode(Encoding.UTF8);
    }

    /// <summary>
    /// Base64加密
    /// </summary>
    /// <param name="encodeType">加密采用的编码方式</param>
    /// <param name="source">待加密的明文</param>
    /// <returns></returns>
    public static string Base64Encode(this string source,Encoding encodeType )
    {
        string encode = string.Empty;
        byte[] bytes = encodeType.GetBytes(source);
        try
        {
            encode = Convert.ToBase64String(bytes);
        }
        catch
        {
            encode = source;
        }
        return encode;
    }

    /// <summary>
    /// Base64解密，采用utf8编码方式解密
    /// </summary>
    /// <param name="result">待解密的密文</param>
    /// <returns>解密后的字符串</returns>
    public static string Base64Decode(this string result)
    {
        return result.Base64Decode(Encoding.UTF8);
    }

    /// <summary>
    /// Base64解密
    /// </summary>
    /// <param name="encodeType">解密采用的编码方式，注意和加密时采用的方式一致</param>
    /// <param name="result">待解密的密文</param>
    /// <returns>解密后的字符串</returns>
    public static string Base64Decode(this string result, Encoding encodeType)
    {
        string decode = string.Empty;
        byte[] bytes = Convert.FromBase64String(result);
        try
        {
            decode = encodeType.GetString(bytes);
        }
        catch
        {
            decode = result;
        }
        return decode;
    }
    public static long toLong(this string s)
    {

        long a = 0;
        long.TryParse(s, out a);
        return a;
    }

    #endregion

}
