using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;using Common.ICommon;
using Common.ICommon;

namespace Common.Common
{
    public class clsEncrypt: IclsEncrypt
    {
        public  string Decrypt(string cipherString, bool useHashing)
        {
            try
            {
                byte[] buffer;
                byte[] inputBuffer = new byte[cipherString.Length];
                try
                {
                    inputBuffer = Convert.FromBase64String(cipherString);
                }
                catch (Exception)
                {
                    return "";
                }
                string s = "DUC_GIA_HUM";
                if (useHashing)
                {
                    MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
                    buffer = provider.ComputeHash(Encoding.UTF8.GetBytes(s));
                    provider.Clear();
                }
                else
                {
                    buffer = Encoding.UTF8.GetBytes(s);
                }
                TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider();
                provider2.Key = buffer;
                provider2.Mode = CipherMode.ECB;
                provider2.Padding = PaddingMode.PKCS7;
                byte[] bytes = provider2.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                provider2.Clear();
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public  string Encrypt(string toEncrypt, bool useHashing)
        {
            try
            {
                byte[] bytes;
                byte[] inputBuffer = new byte[toEncrypt.Length];
                try
                {
                    inputBuffer = Encoding.UTF8.GetBytes(toEncrypt);
                }
                catch (Exception)
                {
                    return "";
                }
                string s = "DUC_GIA_HUM";
                if (useHashing)
                {
                    MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
                    bytes = provider.ComputeHash(Encoding.UTF8.GetBytes(s));
                    provider.Clear();
                }
                else
                {
                    bytes = Encoding.UTF8.GetBytes(s);
                }
                TripleDESCryptoServiceProvider provider2 = new TripleDESCryptoServiceProvider();
                provider2.Key = bytes;
                provider2.Mode = CipherMode.ECB;
                provider2.Padding = PaddingMode.PKCS7;
                byte[] inArray = provider2.CreateEncryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                provider2.Clear();
                return Convert.ToBase64String(inArray, 0, inArray.Length);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
