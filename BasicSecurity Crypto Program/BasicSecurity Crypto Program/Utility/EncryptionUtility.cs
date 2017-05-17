using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BasicSecurity_Crypto_Program.Cryptos;

namespace BasicSecurity_Crypto_Program.Utility
{
    public class EncryptionUtility
    {
        public static byte[] decryptFileWithRSA(string _fileName, RSAParameters key)
        {
            var rsabytedata = File.ReadAllBytes(_fileName);

            var csp = new RSACryptoServiceProvider(2048); //Create an RSA
            csp.ImportParameters(key); // Key importeren 

            return csp.Decrypt(rsabytedata, false); // get dycrypted message
        }

        public static string decryptMessage(string _messageFile, byte[] aesKeyBytes, byte[] IV)
        {
            using (Aes myAes = Aes.Create())
            {
                myAes.Key = aesKeyBytes;
                myAes.IV = IV;

                var test = File.ReadAllBytes(_messageFile);

                string filevalue = FileUtility.getString(_messageFile);

                var encryptedFromFile = System.Text.Encoding.Unicode.GetBytes(filevalue);

                var testdata = SecurityAes.DecryptStringFromBytes_Aes(test,
                 myAes.Key, myAes.IV);

                return testdata;
            }
        }

        public static string keyToString(RSAParameters key)
        {
            string keyString;

            //we need some buffer
            var sw = new System.IO.StringWriter();
            //we need a serializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //serialize the key into the stream
            xs.Serialize(sw, key);
            //get the string from the stream
            keyString = sw.ToString();

            return keyString;
        }

        public static void checkHash(string _hashFile, User _senderUser, string _message)
        {
            Console.WriteLine(string.Format("Testing hash:"));

            byte[] _encrypted = File.ReadAllBytes(_hashFile);
            byte[] _messageBytes = Encoding.ASCII.GetBytes(_message);
            ASCIIEncoding ascii = new ASCIIEncoding();
            var text = ascii.GetString(_messageBytes);

            var d = _senderUser.VerifyHash(_senderUser.getPubKey(), _messageBytes, _encrypted);

            Console.WriteLine(" signature correct: {0}", d);
        }

        public static void aosEncryption(string text, User _senderUser, User _recieverUser, byte[] iv)
        {
            var csp = new RSACryptoServiceProvider(2048);
            using (Aes myAes = Aes.Create())
            {
                myAes.IV = iv;
                //encrypt text with aes key
                byte[] encrypted = SecurityAes.EncryptStringToBytes_Aes(text,
                    myAes.Key, myAes.IV);

                Console.WriteLine("hulp: " + Convert.ToBase64String(myAes.IV));

                //add message
                string _encryptedText = Convert.ToBase64String(encrypted);
                int fileVersion = 0;
                string _fileName;
                do
                {
                    fileVersion++;
                    _fileName = string.Format("{0}-MessageFile-{1}.dat", _recieverUser.getUserName(), fileVersion);
                } while (FileUtility.CheckFileExist(_fileName));

                //Save encrypted message
                File.WriteAllBytes(_fileName, encrypted);
                Console.WriteLine(string.Format("Text encrypted and saved as: {0}", _fileName));


                //import pubkey to rsa
                csp = new RSACryptoServiceProvider();
                csp.ImportParameters(_recieverUser.getPubKey());
                // encrypt aesKey with pub RSAkey
                var aesKeyEncrypted = csp.Encrypt(myAes.Key, false);

                //string _encryptedAesKey = Convert.ToBase64String(aesKeyEncrypted);
                do
                {
                    _fileName = string.Format("{0}-EncryptedAesKey-{1}.dat", _recieverUser.getUserName(), fileVersion);
                } while (FileUtility.CheckFileExist(_fileName));

                //Save encrypted aesKey
                File.WriteAllBytes(_fileName, aesKeyEncrypted);
                Console.WriteLine(string.Format("AesKey encrypted and saved as: {0}", _fileName));

                //Chech for MD5 files
                do
                {
                    _fileName = string.Format("{0}-EncryptedHash-{1}.dat", _recieverUser.getUserName(), fileVersion);
                } while (FileUtility.CheckFileExist(_fileName));

                Console.WriteLine(string.Format("Testing hash:"));

                byte[] oorspronkelijkebootschap = System.Text.Encoding.ASCII.GetBytes(text);

                var singedData = _senderUser.HashAndSign(oorspronkelijkebootschap);

                var d = _senderUser.VerifyHash(_senderUser.getPubKey(), oorspronkelijkebootschap, singedData);

                Console.WriteLine(" signature {0}", d);

                //Creating file with name _FileName

                File.WriteAllBytes(_fileName, singedData);
                Console.WriteLine(string.Format("Hash saved as: {0}", _fileName));

                SHA1Managed sha = new SHA1Managed();
                var hashcode = sha.ComputeHash(encrypted);
                var teststring = Convert.ToBase64String(hashcode);
                Console.WriteLine("signature {0}", teststring);
            }
        }
    }
}
