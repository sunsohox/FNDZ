using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Security.Cryptography;
using System.IO;

namespace Common
{
    public class PasswordLocker
    {
        /// <summary>
        /// 加密内容
        /// </summary>
        /// <param name="toEncrpt">待加密的内容</param>
        /// <returns>返回已加密过的内容</returns>
        public static string DESEncrypt(string toEncrpt)
        {
            string key = "LYLM8888";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(toEncrpt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                sb.AppendFormat("{0:X2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="toDecrypt">待解密的内容</param>
        /// <returns>已解密的内容</returns>
        public static string DESDecrypt(string toDecrypt)
        {
            string key = "LYLM8888";
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[toDecrypt.Length / 2];
            for (int x = 0; x < toDecrypt.Length / 2; x++)
            {
                int i = Convert.ToInt32(toDecrypt.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(key);
            des.IV = ASCIIEncoding.ASCII.GetBytes(key);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Encoding.Default.GetString(ms.ToArray());

        }
    }
}
