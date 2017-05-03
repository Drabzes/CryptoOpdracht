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
        private RSAParameters privKey;
        private RSAParameters pubKey;


        public User(string userName)
        {
            this._userName = userName;
        }

        public User(string userName,  RSAParameters privKey, RSAParameters pubKey)
        {
            this._userName = userName;
            this.privKey = privKey;
            this.pubKey = pubKey;
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
