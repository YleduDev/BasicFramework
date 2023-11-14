using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MD5Helper
{
    public static string MD5Str(string str)
    {

        ////创建MD5对象 
        MD5 md5 = MD5.Create();
        ////开始加密： 通过传入的明文密码，转换为字节数组，还可以通过其他的方式来获取密文
        ////1. 需要将字符串转换成字节数组，
        byte[] buffer = Encoding.Default.GetBytes(str);
        //// 2.把明文字符串转换得到的字节数组，传给md5，获取密文（哈希值）得到一个字节数组
        byte[] MD5buffer = md5.ComputeHash(buffer);
        ////如果直接输出会出现乱码，原因：
        //// 如果是把 125 128 212 这个十六进制的数解析出来可能是 假定 我爱你
        ////我们要得到这段编码，那么给字节数组中每个元素都转换为字符串然后拼接起来就可以了

        ////将字节数组转换成字符串
        ////将字节数组转换为字符串三种方法：
        //// 1.将字节数组中每个元素按照指定的编码格式解析成字符串
        ////2.直接将数组Tostring（）；
        ////3.字节数组中每个元素ToString（）

        ////这里提供 string 和 stringbuider两种方法拼接
        ////string s = null;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < MD5buffer.Length; i++)
        {
            sb.Append(MD5buffer[i].ToString("x2"));  //ToString（）；传入参数目的是限定转换为字符串的格式
                                                     //s += MD5buffer[i].ToString();
        }
        return sb.ToString();
    }

}
