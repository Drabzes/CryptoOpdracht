using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BasicSecurity_Crypto_Program.Utility
{
    public class FileUtility
    {
        //Giel added
        //Check if file exists.
        public static bool CheckFileExist(string fileName)
        {
            return File.Exists(fileName); ;
        }

        public static void loadfileList(User _selectedUser)
        {
            int fileVersion = 1;
            string _fileName = string.Format("{0}-MessageFile-{1}.dat", _selectedUser.getUserName(), fileVersion);
            while (FileUtility.CheckFileExist(_fileName))
            {
                Console.WriteLine(_fileName);
                fileVersion++;
                _fileName = string.Format("{0}-MessageFile-{1}.dat", _selectedUser.getUserName(), fileVersion);
            }
        }

        public static string getString(string nameFile)
        {
            byte[] bytes = File.ReadAllBytes(nameFile);
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private static RSAParameters stringToKey(string keyString)
        {
            RSAParameters key;

            //get a stream from the string
            var sr = new System.IO.StringReader(keyString);
            //we need a deserializer
            var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            //get the object back from the stream
            key = (RSAParameters)xs.Deserialize(sr);

            return key;
        }

        public static void saveByte(string str, string nameFile)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            File.WriteAllBytes(nameFile, bytes);
        }// set string om naar bytes en daarna in een file

        public static User loadUser(string _name)
        {
            try
            {
                User selectedUser;
                string _fileNamePriv = string.Format("{0}-RSAPrivKey.dat", _name);
                string _fileNamePub = string.Format("{0}-RSAPubKey.dat", _name);

                if (FileUtility.CheckFileExist(_fileNamePub) && FileUtility.CheckFileExist(_fileNamePriv))
                {
                    //lees keys
                    string privKeyString = getString(_fileNamePriv);
                    string pubKeyString = getString(_fileNamePub);

                    //how to get the private key
                    RSAParameters privKey = stringToKey(privKeyString);
                    RSAParameters pubKey = stringToKey(pubKeyString);

                    //Create user with selected name and loaded key and place it in the memory.
                    selectedUser = new User(_name, privKey, pubKey);
                    Console.WriteLine(string.Format("Loaded all data from user: {0} ", selectedUser.getUserName()));
                    return selectedUser;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
