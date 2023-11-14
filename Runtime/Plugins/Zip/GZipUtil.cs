using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.GZip;
using System.IO;
using System.Text;

/*
：
*/

public static class GZipUtil 
{
    public static byte[] ComZip(string data)
    {
        MemoryStream ms = new MemoryStream();
        GZipOutputStream gzip = new GZipOutputStream(ms);
        byte[] binary = Encoding.UTF8.GetBytes(data);
        gzip.Write(binary, 0, binary.Length);
        gzip.Close();
        byte[] press = ms.ToArray();

        return press;

    }

    public static byte[] DisComZip(byte[] press)
    {
        GZipInputStream gzi = new GZipInputStream(new MemoryStream(press));

        MemoryStream re = new MemoryStream();
        int count = 0;
        byte[] data = new byte[4096];
        while ((count = gzi.Read(data, 0, data.Length)) != 0)
        {
            re.Write(data, 0, count);
        }
        byte[] depress = re.ToArray();
        //Debug.Log(Encoding.UTF8.GetString(depress));
        return depress;
    }

    public static string DisComZipToStr(byte[] press)
    {
        return Encoding.UTF8.GetString(DisComZip(press));
    }

}
