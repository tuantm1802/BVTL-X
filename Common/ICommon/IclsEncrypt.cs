using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.ICommon
{
    public interface IclsEncrypt
    {
        string Decrypt(string cipherString, bool useHashing);
        string Encrypt(string toEncrypt, bool useHashing);
    }
}
