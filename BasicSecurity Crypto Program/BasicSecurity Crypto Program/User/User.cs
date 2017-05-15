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

        public byte[] HashAndSign(byte[] encrypted)
        {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData;

            rsaCSP.ImportParameters(_privKey);

            hashedData = hash.ComputeHash(encrypted);
            return rsaCSP.SignHash(hashedData, CryptoConfig.MapNameToOID("SHA1"));
        }

        public bool VerifyHash(RSAParameters rsaParams, byte[] signedData, byte[] signature)
        {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider();
            SHA1Managed hash = new SHA1Managed();
            byte[] hashedData;

            rsaCSP.ImportParameters(rsaParams);
            bool dataOK = rsaCSP.VerifyData(signedData, CryptoConfig.MapNameToOID("SHA1"), signature);
            hashedData = hash.ComputeHash(signedData);
            return rsaCSP.VerifyHash(hashedData, CryptoConfig.MapNameToOID("SHA1"), signature);
        }
    }
}
