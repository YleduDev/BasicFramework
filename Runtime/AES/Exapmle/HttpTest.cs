using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UniRx;
using UnityEngine;

public class HttpTest : MonoBehaviour
{
    /*
    http://test-vr-yun.yizhentv.com/api/device/getInfo
    
    -----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAvXT2FCaULE2hlHIDDAw1
NECfcg9991yemalh1oaMuTGzIRFlRITzxjuWczlAenX05/Lzgdvst1FUFZM5fcWj
4wbqt/vEbMbsegRpvtRXsWPQj8UGvCOXIsPdJ0sLXocQemNtdjbEQuSw8ADwDRnF
AizqBNcsuXZzVMTAmHyt1XM65jSITMWL5JXcOTQhSpQ0pFEAe4vS6KJN/3UE86SV
b455x/lJxnaEtQ3t2gBvxP5J8JQ4T+eyA66uzkGnSZDKzbWcrFHCcLC3W0JnGHZe
2jRi6BPJQ2h60puw90c4dcE8HsLgIXaUtzfYkL54R2QaYWhqPKSEp5WwPVV7PCA4
gwIDAQAB
-----END PUBLIC KEY-----

     */

    public string pt;
    private string pricateKey, publicKey;
    public string url= "http://test-vr-yun.yizhentv.com/api/device/getInfo";
    void Start()
    {
        //当前时间
        //string curtime = TimeExtension.GetTimeStamp().ToString().ToUTF8();
        //获得pem格式 证书 公私密钥
        publicKey = Resources.Load<TextAsset>("publicKey").text;
       // pricateKey = Resources.Load<TextAsset>("pricateKey").text;

        //生成唯一数
        string uniqueString = GenerateUniqueString16().ToUTF8();

        Log.E("uniqueId:" + uniqueString);

        //加密
        string si = HttpEx.EncryptSi(uniqueString,publicKey);
        Log.E("si:" + si);
        string dsi = RsaHelper.Decrypt(si,pricateKey);
        Log.E("dsi:" + dsi);
        //aes 加密
        string p= HttpEx.EncryptP(pt, uniqueString);
        Log.E("P:" + p);
        string disP = EncryptionExtension.Decrypt(p, uniqueString, uniqueString);
        Log.E("disP:" + disP);

        string rt = "1679382409971";
        string ti = HttpEx.GenerateUniqueString32().ToUTF8();
        string pk = "com.onezhen.pcvr";
        string rv = "1.0.0";
        Dictionary<string, IComparable> form = new Dictionary<string, IComparable>();
        form.Add("1", "1");

        string sk = HttpEx.GetSK(uniqueString, rv, rt, pk, ti, pt, form);
        Log.E("sk:" + sk);

        var _dic_header = new Dictionary<string, string>();
        _dic_header.Add("ov", "29"); //客户端 系统 版本号 
        _dic_header.Add("rv", "1.0.0"); //客户端 版本号
        _dic_header.Add("rt", rt);//当前请求时间
        _dic_header.Add("ti", ti);//32随机数
        _dic_header.Add("pk", pk);

        _dic_header.Add("p", p.ToUTF8());
        _dic_header.Add("si", si.ToUTF8());
        _dic_header.Add("sk", sk.ToUTF8());
       // _dic_header.Add("User-Agent", VersionConfig.getUserAgent());
        _dic_header.Add("Content-Type", "application/x-www-form-urlencoded");

        var postData = new WWWForm();
        postData.AddField("1", "1");

        ObservableWWW.Post(url, postData, _dic_header).OnErrorRetry(onRetryErr, 2, TimeSpan.FromSeconds(0.2f)).CatchIgnore((WWWErrorException ex) =>
        {
            Debug.LogError("RawErrorMessage:" + ex.RawErrorMessage);


            if (ex.HasResponse)
            {
                Debug.LogError("StatusCode:" + ex.StatusCode);
            }
            foreach (var item in ex.ResponseHeaders)
            {
                Debug.LogError("ResponseHeaders:" + item.Key + ":" + item.Value);
            }

        }).Subscribe(
                content =>
                {

                    Log.E(content);
                },
               _ =>
               {
                   Log.E(_.Message);

               }, () => { Log.I(string.Format("URL:{0} Post请求已完成", url)); });
    }

    static Action<Exception> onRetryErr = e => { };

   public static string GenerateUniqueString32()
    {
        byte[] bytes = Guid.NewGuid().ToByteArray();
        return BitConverter.ToString(bytes).Replace("-", "");
    }

    public static string GenerateUniqueString16()
    {
        byte[] bytes = new byte[8];
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(bytes);
        }
        int l1 = BitConverter.ToInt32(bytes, 0);
        int l2 = BitConverter.ToInt32(bytes, 4);
        return $"{l1:X8}{l2:X8}";
    }

}
