using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

public static class RsaHelper
{
    public static string Encrypt(string plaintext, string publicKeyPem)
    {
        // 解析 PEM 格式的公钥
        RSAParameters publicKey = ParsePemPublicKey(publicKeyPem);

        // 加密数据
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
        byte[] ciphertextBytes = RsaEncrypt(plaintextBytes, publicKey);

        // 返回加密后的 Base64 编码字符串
        return Convert.ToBase64String(ciphertextBytes);
    }

    public static string Decrypt(string ciphertext, string privateKeyPem)
    {
        // 解析 PEM 格式的私钥
        RSAParameters privateKey = ParsePemPrivateKey(privateKeyPem);

        // 解密数据
        byte[] ciphertextBytes = Convert.FromBase64String(ciphertext);
        byte[] decryptedBytes = RsaDecrypt(ciphertextBytes, privateKey);
        string decryptedText = Encoding.UTF8.GetString(decryptedBytes);

        // 返回解密后的字符串
        return decryptedText;
    }

    private static RSAParameters ParsePemPublicKey(string publicKeyPem)
    {
        StringReader stringReader = new StringReader(publicKeyPem);
        PemReader pemReader = new PemReader(stringReader);
        RsaKeyParameters rsaParams = (RsaKeyParameters)pemReader.ReadObject();

        RSAParameters rsaPublicKey = new RSAParameters
        {
            Modulus = rsaParams.Modulus.ToByteArrayUnsigned(),
            Exponent = rsaParams.Exponent.ToByteArrayUnsigned()
        };

        return rsaPublicKey;
    }

    private static RSAParameters ParsePemPrivateKey(string privateKeyPem)
    {
        StringReader stringReader = new StringReader(privateKeyPem);
        PemReader pemReader = new PemReader(stringReader);


        AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
        RsaPrivateCrtKeyParameters rsaParams = (RsaPrivateCrtKeyParameters)keyPair.Private;

        RSAParameters rsaPrivateKey = new RSAParameters
        {
            Modulus = rsaParams.Modulus.ToByteArrayUnsigned(),
            Exponent = rsaParams.PublicExponent.ToByteArrayUnsigned(),
            D = rsaParams.Exponent.ToByteArrayUnsigned(),
            P = rsaParams.P.ToByteArrayUnsigned(),
            Q = rsaParams.Q.ToByteArrayUnsigned(),
            DP = rsaParams.DP.ToByteArrayUnsigned(),
            DQ = rsaParams.DQ.ToByteArrayUnsigned(),
            InverseQ = rsaParams.QInv.ToByteArrayUnsigned()
        };

        return rsaPrivateKey;
    }

    private static byte[] RsaEncrypt(byte[] plaintextBytes, RSAParameters publicKey)
    {
        using (RSA rsa = RSA.Create())
        {
            rsa.KeySize = 2048;
            rsa.ImportParameters(publicKey);
            return rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.Pkcs1);
        }
    }

    private static byte[] RsaDecrypt(byte[] ciphertextBytes, RSAParameters privateKey)
    {

        using (RSA rsa = RSA.Create())
        {
            rsa.KeySize = 2048;

            rsa.ImportParameters(privateKey);
            return rsa.Decrypt(ciphertextBytes, RSAEncryptionPadding.Pkcs1);
        }
    }


}