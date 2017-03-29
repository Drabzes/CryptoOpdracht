using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BasicSecurity_Crypto_Program
{
    public class User
    {
        private string _userName;
        private byte[] _KeyAes;


        public User(string userName)
        {
            this._userName = userName;
        }

        public void setKeyAesByte(byte[] key)
        {
            this._KeyAes = key;
        }

        public byte[] getKeyAesByte()
        {
            return this._KeyAes;
        }

        public string getUserName()
        {
            return _userName;
        }

        public string getAesStringkey()
        {
            string AesKeyString = System.Text.Encoding.UTF8.GetString(this._KeyAes);
            return AesKeyString;
        }

    }
}
