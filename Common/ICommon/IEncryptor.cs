using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.ICommon
{
    public interface IEncryptor
    {
         string MD5Hash(string text);

         string HmacSha256Hash(string rawData);
    }
}
