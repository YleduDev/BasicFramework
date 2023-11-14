using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;

public static class CryptoHelper
{
    /// <summary>
    /// RSAÃÜÔ¿×ªPemÃÜÔ¿
    /// </summary>
    /// <param name="RSAKey">RSAÃÜÔ¿</param>
    /// <param name="isPrivateKey">ÊÇ·ñÊÇË½Ô¿</param>
    /// <returns>PemÃÜÔ¿</returns>
    public static string RSAKeyToPem(string RSAKey, bool isPrivateKey)
    {
        string pemKey = string.Empty;
        var rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(RSAKey);
        RSAParameters rsaPara = new RSAParameters();
        RsaKeyParameters key = null;
        //RSAË½Ô¿
        if (isPrivateKey)
        {
            rsaPara = rsa.ExportParameters(true);
            key = new RsaPrivateCrtKeyParameters(
                new BigInteger(1, rsaPara.Modulus), new BigInteger(1, rsaPara.Exponent), new BigInteger(1, rsaPara.D),
                new BigInteger(1, rsaPara.P), new BigInteger(1, rsaPara.Q), new BigInteger(1, rsaPara.DP), new BigInteger(1, rsaPara.DQ),
                new BigInteger(1, rsaPara.InverseQ));
        }
        //RSA¹«Ô¿
        else
        {
            rsaPara = rsa.ExportParameters(false);
            key = new RsaKeyParameters(false,
                new BigInteger(1, rsaPara.Modulus),
                new BigInteger(1, rsaPara.Exponent));
        }
        using (TextWriter sw = new StringWriter())
        {
            var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
            pemWriter.WriteObject(key);
            pemWriter.Writer.Flush();
            pemKey = sw.ToString();
        }
        return pemKey;
    }
    /// <summary>
    /// PemÃÜÔ¿×ªRSAÃÜÔ¿
    /// </summary>
    /// <param name="pemKey">PemÃÜÔ¿</param>
    /// <param name="isPrivateKey">ÊÇ·ñÊÇË½Ô¿</param>
    /// <returns>RSAÃÜÔ¿</returns>
    public static string PemToRSAKey(string pemKey, bool isPrivateKey)
    {
        string rsaKey = string.Empty;
        object pemObject = null;
        RSAParameters rsaPara = new RSAParameters();
        using (StringReader sReader = new StringReader(pemKey))
        {
            var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sReader);

            pemObject = pemReader.ReadObject();
        }
        //RSAË½Ô¿
        if (isPrivateKey)
        {
            RsaPrivateCrtKeyParameters key = (RsaPrivateCrtKeyParameters)((AsymmetricCipherKeyPair)pemObject).Private;
            rsaPara = new RSAParameters
            {
                Modulus = key.Modulus.ToByteArrayUnsigned(),
                Exponent = key.PublicExponent.ToByteArrayUnsigned(),
                D = key.Exponent.ToByteArrayUnsigned(),
                P = key.P.ToByteArrayUnsigned(),
                Q = key.Q.ToByteArrayUnsigned(),
                DP = key.DP.ToByteArrayUnsigned(),
                DQ = key.DQ.ToByteArrayUnsigned(),
                InverseQ = key.QInv.ToByteArrayUnsigned(),
            };
        }
        //RSA¹«Ô¿
        else
        {
            RsaKeyParameters key = (RsaKeyParameters)pemObject;
            rsaPara = new RSAParameters
            {
                Modulus = key.Modulus.ToByteArrayUnsigned(),
                Exponent = key.Exponent.ToByteArrayUnsigned(),
            };
        }
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.ImportParameters(rsaPara);
        using (StringWriter sw = new StringWriter())
        {
            sw.Write(rsa.ToXmlString(isPrivateKey ? true : false));
            rsaKey = sw.ToString();
        }
        return rsaKey;
    }


    /// <summary>
    /// XML¹«Ô¿×ª³ÉPem¹«Ô¿
    /// </summary>
    /// <param name="xmlPublicKey"></param>
    /// <returns></returns>
    public static string XmlPublicKeyToPem(this string xmlPublicKey)
    {
        RSAParameters rsaParam;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(xmlPublicKey);
            rsaParam = rsa.ExportParameters(false);
        }
        RsaKeyParameters param = new RsaKeyParameters(false, new BigInteger(1, rsaParam.Modulus), new BigInteger(1, rsaParam.Exponent));

        string pemPublicKeyStr = null;
        using (var ms = new MemoryStream())
        {
            using (var sw = new StreamWriter(ms))
            {
                var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                pemWriter.WriteObject(param);
                sw.Flush();

                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, (int)ms.Length);
                pemPublicKeyStr = Encoding.UTF8.GetString(buffer);
            }
        }

        return pemPublicKeyStr;
    }

    /// <summary>
    /// Pem¹«Ô¿×ª³ÉXML¹«Ô¿
    /// </summary>
    /// <param name="pemPublicKeyStr"></param>
    /// <returns></returns>
    public static string PemPublicKeyToXml(this string pemPublicKeyStr)
    {
        RsaKeyParameters pemPublicKey;
        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(pemPublicKeyStr)))
        {
            using (var sr = new StreamReader(ms))
            {
                var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                pemPublicKey = (RsaKeyParameters)pemReader.ReadObject();
            }
        }

        var p = new RSAParameters
        {
            Modulus = pemPublicKey.Modulus.ToByteArrayUnsigned(),
            Exponent = pemPublicKey.Exponent.ToByteArrayUnsigned()
        };

        string xmlPublicKeyStr;
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(p);
            xmlPublicKeyStr = rsa.ToXmlString(false);
        }

        return xmlPublicKeyStr;
    }

    /// <summary>
    /// XMLË½Ô¿×ª³ÉPEMË½Ô¿
    /// </summary>
    /// <param name="xmlPrivateKey"></param>
    /// <returns></returns>
    public static string XmlPrivateKeyToPem(this string xmlPrivateKey)
    {
        RSAParameters rsaParam;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(xmlPrivateKey);
            rsaParam = rsa.ExportParameters(true);
        }

        var param = new RsaPrivateCrtKeyParameters(
        new BigInteger(1, rsaParam.Modulus), new BigInteger(1, rsaParam.Exponent), new BigInteger(1, rsaParam.D),
        new BigInteger(1, rsaParam.P), new BigInteger(1, rsaParam.Q), new BigInteger(1, rsaParam.DP), new BigInteger(1, rsaParam.DQ),
        new BigInteger(1, rsaParam.InverseQ));

        string pemPrivateKeyStr = null;
        using (var ms = new MemoryStream())
        {
            using (var sw = new StreamWriter(ms))
            {
                var pemWriter = new Org.BouncyCastle.OpenSsl.PemWriter(sw);
                pemWriter.WriteObject(param);
                sw.Flush();

                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, (int)ms.Length);
                pemPrivateKeyStr = Encoding.UTF8.GetString(buffer);
            }
        }

        return pemPrivateKeyStr;
    }

    /// <summary>
    /// PemË½Ô¿×ª³ÉXMLË½Ô¿
    /// </summary>
    /// <param name="pemPrivateKeyStr"></param>
    /// <returns></returns>
    public static string PemPrivateKeyToXml(this string pemPrivateKeyStr)
    {
        RsaPrivateCrtKeyParameters pemPrivateKey;
        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(pemPrivateKeyStr)))
        {
            using (var sr = new StreamReader(ms))
            {
                var pemReader = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                pemPrivateKey = (RsaPrivateCrtKeyParameters)keyPair.Private;
            }
        }

        var p = new RSAParameters
        {
            Modulus = pemPrivateKey.Modulus.ToByteArrayUnsigned(),
            Exponent = pemPrivateKey.PublicExponent.ToByteArrayUnsigned(),
            D = pemPrivateKey.Exponent.ToByteArrayUnsigned(),
            P = pemPrivateKey.P.ToByteArrayUnsigned(),
            Q = pemPrivateKey.Q.ToByteArrayUnsigned(),
            DP = pemPrivateKey.DP.ToByteArrayUnsigned(),
            DQ = pemPrivateKey.DQ.ToByteArrayUnsigned(),
            InverseQ = pemPrivateKey.QInv.ToByteArrayUnsigned(),
        };

        string xmlPrivateKeyStr;
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(p);
            xmlPrivateKeyStr = rsa.ToXmlString(true);
        }

        return xmlPrivateKeyStr;
    }

}