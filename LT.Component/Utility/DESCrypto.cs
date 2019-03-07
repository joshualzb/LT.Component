using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LT.Component.Utility
{
    /// <summary>
    /// DESCrypto 通用类
    /// </summary>
    public static class DESCrypto
    {

        /// <summary>
        /// DESC 加密字符串
        /// </summary>
        /// <param name="sKey">加密密码，8位字符</param>
        /// <param name="pToEncrypt">需要加密的内容</param>
        /// <returns></returns>
        public static string Encrypt(string pToEncrypt, string sKey = "sft&fdb_")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// DESC 解密字符串
        /// 如果失败，则返回空值
        /// </summary>
        /// <param name="sKey">解密密码，8位字符</param>
        /// <param name="pToDecrypt">需要解密的内容</param>
        /// <returns></returns>
        public static string Decrypt(string pToDecrypt, string sKey = "sft&fdb_")
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                StringBuilder ret = new StringBuilder();
                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }
    }
}
