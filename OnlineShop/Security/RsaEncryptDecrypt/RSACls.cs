using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OnlineShop.Security.RsaEncryptDecrypt
{
    public class RSACls
    {
        public static Dictionary<string, string> RSAGenerator()
        {
            var dictionary = new Dictionary<string, string>();
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
            {
                string pubKey = rsa.ToXmlString(false);
                string prvKey = rsa.ToXmlString(true);

                dictionary.Add("PublicKey", pubKey);
                dictionary.Add("PrivateKey", prvKey);
            }
            return dictionary;
        }

        public static string RSAEncryption(string strText, string pubKey)
        {
            var byteData = Encoding.UTF8.GetBytes(strText);
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    rsa.FromXmlString(pubKey.ToString());
                    var encryptedData = rsa.Encrypt(byteData, true);
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string RSADecryption(string strText, string prvKey)
        {
            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    var base64Encrypted = strText;
                    // server decrypting data with private key                    
                    rsa.FromXmlString(prvKey);
                    var resultBytes = Convert.FromBase64String(base64Encrypted);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

    }
}