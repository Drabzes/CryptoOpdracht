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
        private RSAParameters _privKey;
        private RSAParameters _pubKey;


        public User(string userName)
        {
            this._userName = userName;
        }

        public User(string userName,  RSAParameters privKey, RSAParameters pubKey)
        {
            this._userName = userName;
            this._privKey = privKey;
            this._pubKey = pubKey;
        }
        
        public string getUserName()
        {
            return _userName;
        }

        public RSAParameters getPrivKey()
        {
            return this._privKey;
        }

        public RSAParameters getPubKey()
        {
            return this._pubKey;
        }

    }
}
