using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Manager.AES
{
    public class AES_Symm_Algorithm
    {
        public static byte[] EncryptData(string secretKey, byte[] data)
        {
            byte[] encryptedData = null;
            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7  //automatski dodaje padding za nedostatne bajtove
            };

            ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    encryptedData = memoryStream.ToArray();
                }
            }

            return encryptedData;
            
        }

        public static byte[] DecryptData(string secretKey, byte[] encryptedData)
        {
            byte[] decryptedData = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();
            using (MemoryStream memoryStream = new MemoryStream(encryptedData))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                    decryptedData = new byte[encryptedData.Length];
                    cryptoStream.Read(decryptedData, 0, decryptedData.Length);
                }
            }
            return decryptedData;
        }



    }
}
