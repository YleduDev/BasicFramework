using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Framework
{
    public static class HttpEx
    {
        /// <summary>
        /// 获得加密后的p参数
        /// </summary>
        /// <param name="pJson">p对象json</param>
        /// <param name="uniqueString16">加密因子 16位的唯一随机数</param>
        /// <returns></returns>
        public static string EncryptP(string pJson, string uniqueString16)
        {
            return EncryptionExtension.AESEncryptedString(pJson, uniqueString16, uniqueString16);
        }

        /// <summary>
        /// 获得加密后的Si
        /// </summary>
        /// <param name="uniqueString16">加密因子 16位的唯一随机数</param>
        /// <param name="publicKeyPem">pem格式的公钥</param>
        /// <returns></returns>
        public static string EncryptSi(string uniqueString16, string publicKeyPem)
        {
            return RsaHelper.Encrypt(uniqueString16, publicKeyPem);
        }

        /// <summary>
        /// 获得sk加密后的数据
        /// </summary>
        /// <param name="uniqueString16">si加密前</param>
        /// <param name="rv">接口版本号</param>
        /// <param name="rt">当前时间</param>
        /// <param name="pk">包名</param>
        /// <param name="ti">32的唯一数据数</param>
        /// <param name="pJson">p的对象</param>
        /// <param name="form">form 表单对象</param>
        /// <param name="sJson">(json_encode(敏感数据组))</param>
        /// <returns></returns>
        public static string GetSK(string uniqueString16, string rv, string rt, string pk, string ti, string pJson, Dictionary<string, IComparable> form, string sJson = null)
        {
            string header = rv + rt + pk + ti + pJson + sJson;

            //Log.E("header:" + header);
            string headerMd5 = MD5Helper.MD5Str(header);
            //Log.E("headerMd5:" + headerMd5);
            var keys = form.Keys.ToList();
            keys.Sort();

            string body = "";
            for (int i = 0; i < keys.Count; i++)
            {
                body += form[keys[i]];
            }
            //Log.E("body:" + body);
            string bodyMd5 = MD5Helper.MD5Str(body);
            //Log.E("bodyMd5:" + bodyMd5);

            // Log.E("all:" + headerMd5 + bodyMd5 + uniqueString16);

            string allMd5 = MD5Helper.MD5Str(headerMd5 + bodyMd5 + uniqueString16);

            //Log.E("all Md5:" + allMd5);

            return allMd5;

        }

        /// <summary>
        /// 生成唯一随机数32位
        /// </summary>
        /// <returns></returns>
        public static string GenerateUniqueString32()
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        /// <summary>
        /// 生成唯一随机数16位
        /// </summary>
        /// <returns></returns>
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
}