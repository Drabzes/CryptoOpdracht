using BasicSecurity_Crypto_Program.Cryptos;
using BasicSecurity_Crypto_Program.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BasicSecurity_Crypto_Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test Aes and AOS");
            User _selectedUser = null;
            User _sendUser = null;
            var IV = System.Text.ASCIIEncoding.ASCII.GetBytes("adfqsffafdsfsqdf");

            //Try aes encryption
            try
            {
                Console.Write("Enter userName:  "); // Prompt
                string _name = Console.ReadLine(); // Get string from user
                

                _selectedUser = loadUser(_name);

                if (!(_selectedUser == null))
                {
                    
                    Console.WriteLine(String.Format("Naam persoon: {0}", _selectedUser.getUserName()));
                }
                else
                {
                    Console.WriteLine(String.Format("Could not load user"));
                    Console.WriteLine(String.Format("Creating user {0}", _name));

                    //lets take a new CSP with a new 2048 bit rsa key pair
                    RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);

                    //how to get the private key
                    var privKey = csp.ExportParameters(true);

                    //and the public key ...
                    var pubKey = csp.ExportParameters(false);

                    // user aanmaken
                    _selectedUser = new User(_name, privKey, pubKey);

                    string pubKeyString = keyToString(pubKey);

                    string privKeyString = keyToString(privKey);

                    Console.WriteLine(String.Format("Saving keys"));
                    string _nameFile = string.Format("{0}-RSAPrivKey.dat", _name);
                    saveByte(privKeyString, _nameFile);
                    _nameFile = string.Format("{0}-RSAPubKey.dat", _name);
                    saveByte(pubKeyString, _nameFile);
                }


                //Ask to send message to... or read message
                int inputvalue = 0;
                string value;
                do
                {
                    Console.WriteLine("press 1 to send a message");
                    Console.WriteLine("Press 2 to read a message");
                    Console.WriteLine("Press 0 to close this program");
                    Console.Write("Enter input value:  "); // Prompt
                    value = Console.ReadLine(); // Get string form message
                    inputvalue = Convert.ToInt32(value);
                    //declare file name variable for switch case
                    string _fileNamePriv;
                    string _fileNamePub;
                    string _messageFile;
                    string _aesKeyFile;
                    string _hashFile;
                    switch (inputvalue)
                    {
                        case 1:
                            Console.Write("Send message to?: ");
                            _name = Console.ReadLine();
                            _fileNamePriv = string.Format("{0}-RSAPrivKey.dat", _name);
                            _fileNamePub = string.Format("{0}-RSAPubKey.dat", _name);
                            if (FileUtility.CheckFileExist(_fileNamePub) && FileUtility.CheckFileExist(_fileNamePriv))
                            {
                                Console.Write("Message to send: ");
                                string message = Console.ReadLine();

                                _sendUser = loadUser(_name);
                                aosEncryption1(message, _selectedUser, _sendUser, IV);
                            }
                            else
                            {
                                Console.WriteLine(string.Format("Could not find {0} ", _name));
                                Console.WriteLine("");
                            }
                            break;
                        case 2:
                            //Show a list of all message files
                            Console.WriteLine("Blaah");
                            loadfileList(_selectedUser);

                            Console.Write("select message by number: ");
                            int _fileNumber = Convert.ToInt32(Console.ReadLine());
                            //Load all the file names into the memory
                            _messageFile = string.Format("{0}-MessageFile-{1}.dat", _selectedUser.getUserName(), _fileNumber);
                            _aesKeyFile = string.Format("{0}-EncryptedAesKey-{1}.dat", _selectedUser.getUserName(), _fileNumber);
                            _hashFile = string.Format("{0}-EncryptedHash-{1}.dat", _selectedUser.getUserName(), _fileNumber);
                            if (FileUtility.CheckFileExist(_messageFile) && FileUtility.CheckFileExist(_aesKeyFile))
                            {
                                Console.WriteLine("You selected file: {0}", _messageFile);
                                //select the user that wrote the text to you
                                Console.Write("Select user: ");
                                _name = Console.ReadLine();

                                //Read the users keys
                                _fileNamePriv = string.Format("{0}-RSAPrivKey.dat", _name);
                                _fileNamePub = string.Format("{0}-RSAPubKey.dat", _name);
                                //check if those keys exists
                                if (FileUtility.CheckFileExist(_fileNamePub) && FileUtility.CheckFileExist(_fileNamePriv))
                                {
                                    //load the user into the system
                                    _sendUser = loadUser(_name);

                                    //Get AES key
                                    byte[] _aesKeyBytes = decryptFileWithRSA(_aesKeyFile, _selectedUser.getPrivKey());
                                    //Decrypt message
                                    string _message = decryptMessage(_messageFile, _aesKeyBytes, IV);

                                    //Hash stuff
                                    checkHash(_hashFile, _sendUser, _message);                                    

                                    //Show decrypted message
                                    Console.WriteLine("Decrypted message: {0}", _message);
                                    
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("Could not find {0} ", _name));
                                    Console.WriteLine("");
                                }
                            }
                            break;
                             
                    }
                            

                } while (!(inputvalue == 0));

                //if person is found.
                
                //ask what message to encrypt

                //Make md5 of the message.

                //Generate aos key

                //encrypt with aos key

                //Encrypt aos key with private key.

                


                //aosEncryption(_selectedUser);
                //rsaEncryption(_selectedUser);


            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            Console.ReadKey();
        }

        private static void checkHash(string _hashFile, User _senderUser, string _message)
        {
            Console.WriteLine(string.Format("Testing hash:"));

            byte[] _encrypted= File.ReadAllBytes(_hashFile);
            byte[] _messageBytes = Encoding.ASCII.GetBytes(_message);
            ASCIIEncoding ascii = new ASCIIEncoding();
            var text = ascii.GetString(_messageBytes);

            var d = _senderUser.VerifyHash(_senderUser.getPubKey(), _messageBytes, _encrypted);

            Console.WriteLine(" signature correct: {0}", d);

            /*
            SHA1Managed sha = new SHA1Managed();
            var teststring2 = Convert.ToBase64String(_senderUser.GetHash(_senderUser.getPubKey(), _encrypted, singedData));
            Console.WriteLine(" signature {0}", teststring2);
            */

            }

        private static void loadfileList(User _selectedUser)
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

        private static string decryptMessage(string _messageFile, byte[] aesKeyBytes, byte[] IV)
        {
            using (Aes myAes = Aes.Create())
            {
                myAes.Key = aesKeyBytes;
                myAes.IV = IV;

                var test = File.ReadAllBytes(_messageFile);

                string filevalue = getString(_messageFile);

                var encryptedFromFile = System.Text.Encoding.Unicode.GetBytes(filevalue);

               

                var testdata = SecurityAes.DecryptStringFromBytes_Aes(test,
                 myAes.Key, myAes.IV);

                return testdata;
            }
        }

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
            catch(Exception e)
            {
                return null;
            }
            
        }

        private static void aosEncryption1 (string text, User _senderUser, User _recieverUser, byte[] iv)
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
                

                /*
                var bytesPlainTextData = FileUtility.ReadByteArrayFromFileRSA(_fileName);
                var morebytes = getString(_fileName);
                var conv = System.Text.Encoding.Unicode.GetBytes(morebytes);
                byte[] buff = File.ReadAllBytes(_fileName);

                csp = new RSACryptoServiceProvider();
                csp.ImportParameters(_user.getPrivKey());
                var aeKeydecrypted = csp.Decrypt(buff, false);
                */



            }
        }

        public static void aosEncryption(User selectedUser, string line)
        {
            bool writeByteText = false;
            string original = "Here is some data to encrypt! AES";
            Console.WriteLine("In try of Aes");
            
            // Create a new instance of the Aes
            // class.  This generates a new key and initialization 
            // vector (IV).
            using (Aes myAes = Aes.Create())
            {
                Console.WriteLine("Loading UserData..");
                string _fileName;

                //Giel added
                
                _fileName = string.Format("{0}-AOSKey", line);

                //Giel added
                //Look for a file named Giel-AOSKey
                //If file is found don't make a new key for User
                if (FileUtility.CheckFileExist(_fileName))
                {
                    Console.WriteLine("AOSKey found!");
                    //Read file with the AesKey into memory
                    byte[] AOSKey = FileUtility.ReadByteArrayFromFile(_fileName);
                    //Create user with selected name and loaded key and place it in the memory.
                    //selectedUser = new User(line, AOSKey);
                    Console.WriteLine(string.Format("Loaded all data from user: {0} ", selectedUser.getUserName()));
                }
                else
                {
                    Console.WriteLine("AOSKey was not found!");
                    Console.WriteLine("Using default user Giel");

                    //Convert AesKey bytes to a string
                    //selectedUser = new User("Giel", myAes.Key);

                    //Create file with private key for Giel.
                    //Filename = "userName"-AOSKey.bit
                    string _nameFilePrivateKey = string.Format("{0}-AOSKey", selectedUser.getUserName());
                    if (FileUtility.ByteArrayToFile(_nameFilePrivateKey, myAes.Key) == true)
                    {
                        Console.WriteLine("Key saved");
                    }
                    else
                    {
                        Console.WriteLine("Something went wrong saving the key");
                    }

                    //get converted bytes key from User Giel
                    //Write in in the console for tests
                    //Console.WriteLine("Encrypted Aeskey: " + selectedUser.getAesStringkey());
                }

                _fileName = "AesStringByteFile";

                //Giel added
                //Load the Aeskey from the user into myAes.key
                //myAes.Key = selectedUser.getKeyAesByte();
                string AesKey = System.Text.Encoding.UTF8.GetString(myAes.Key);
                Console.WriteLine(string.Format("AesKey is: {0}", AesKey));

                //Giel added
                //Check if the key from selectedUser is the same as myAes.key
                //These need to be the same because myAes.key will be used to encrypt en decrypt the text
               // string AesKeyUser = System.Text.Encoding.UTF8.GetString(selectedUser.getKeyAesByte());
                //Console.WriteLine(string.Format("AesKeyUser is: {0}", AesKeyUser));

                // Encrypt the string to an array of bytes.
                byte[] encrypted = SecurityAes.EncryptStringToBytes_Aes(original,
                    myAes.Key, myAes.IV);

                //Giel added
                //write encrypted text bytes to file
                writeByteText = FileUtility.ByteArrayToFile(_fileName, encrypted);
                Console.WriteLine("Text succefully written to file ");

                //Giel added
                //Get the string of the encrypted texts
                string encryptedText = System.Text.Encoding.UTF8.GetString(encrypted);
                Console.WriteLine("Encryped Aestext: " + encryptedText);

                //Giel added
                //Get the bytes from the byte file and place them into encryptedFromFile byte array
                Console.WriteLine("Reading byte file into memory..");
                byte[] encryptedFromFile = FileUtility.ReadByteArrayFromFile(_fileName);
                Console.WriteLine("Success");

                // Decrypt the bytes to a string.
                string roundtrip = SecurityAes.DecryptStringFromBytes_Aes(encryptedFromFile,
                 myAes.Key, myAes.IV);

                //Display the original data and the decrypted data.
                Console.WriteLine("Original:   {0}", original);
                Console.WriteLine("Round Trip: {0}", roundtrip);
            }
        }

        public static void rsaEncryption (User selectedUser)
        {
            //Create a UnicodeEncoder to convert between byte array and string.
            UnicodeEncoding ByteConverter = new UnicodeEncoding();

            //Create byte arrays to hold original, encrypted, and decrypted data.
            byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt AOS");
            byte[] encryptedData;
            byte[] decryptedData;

            //Create a new instance of RSACryptoServiceProvider to generate
            //public and private key data.
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                string fileName = null;
                //Get the RSA public in in xml from.
                string publicKey = RSA.ToXmlString(false);
                string privateKey = RSA.ToXmlString(true);
                //Convert RSA public key string into bytes Array
                byte[] publicKeyBytes = ByteConverter.GetBytes(publicKey);
                byte[] privateKeyBytes = ByteConverter.GetBytes(privateKey);

                //We name the file here where we want to put the public key in.
                fileName = string.Format("RSAPublicKey-{0}.xml", selectedUser.getUserName());

                //Display the public key to check if it is still the same.
                Console.WriteLine("xml form of the public key");
                Console.WriteLine(publicKey);
                Console.WriteLine("");



                //Check if file exists
                Console.WriteLine(string.Format("Check if file exists: {0}", FileUtility.CheckFileExist(fileName)));

                Console.WriteLine(string.Format("Read file {0}", fileName));
                //ReadFile
                if (FileUtility.CheckFileExist(fileName) == true)
                {
                    //read the file and put it into a byte[]
                    //publicKeyBytes = FileUtility.ReadByteArrayFromFileRSA(fileName);
                    //convert the byte[] to a string
                    //publicKey = Convert.ToBase64String(publicKeyBytes);

                    publicKey = FileUtility.readXmlFile(fileName);
                    //show the string
                    Console.WriteLine("The public key from the file is: ");
                    Console.WriteLine(publicKey);
                }
                else
                {
                    //Write the public key in bytes into a file.
                    Console.WriteLine("Creating public key file");
                    //Write the public key into a file
                    //FileUtility.ByteArrayToFile(fileName, publicKeyBytes);
                    var test = FileUtility.writeXmlFile(fileName, publicKey);
                    Console.WriteLine("Public ket file Created. Yay!!");
                }

                //change the fileName to private file
                fileName = string.Format("RSAPrivateKey-{0}.xml", selectedUser.getUserName());

                //Check if file exists
                Console.WriteLine(string.Format("Check if file exists: {0}", FileUtility.CheckFileExist(fileName)));

                Console.WriteLine(string.Format("Read file {0}", fileName));

                if (FileUtility.CheckFileExist(fileName) == true)
                {
                    //read the file and put it into a byte[]
                    //privateKeyBytes = FileUtility.ReadByteArrayFromFileRSA(fileName);
                    //convert the byte[] to a string
                    //privateKey = Convert.ToBase64String(privateKeyBytes);
                    privateKey = FileUtility.readXmlFile(fileName);
                    //show the string
                    Console.WriteLine("The private key from the file is: ");
                    privateKey = privateKey.Replace(" ", string.Empty);
                    Console.WriteLine(privateKey);
                }
                else
                {
                    //Write the public key in bytes into a file.
                    Console.WriteLine("Creating private key file");
                    //Write the public key into a file
                    //FileUtility.ByteArrayToFile(fileName, privateKeyBytes);
                    var test = FileUtility.writeXmlFile(fileName, privateKey);
                    Console.WriteLine("Private key file Created. Yay!!");
                }



                //Pass the data to ENCRYPT, the public key information 
                //(using RSACryptoServiceProvider.ExportParameters(false),
                //and a boolean flag specifying no OAEP padding.



                //encryptedData = SecurityRSA.RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);
                //privateKey = "<RSAKeyValue><Modulus>" + privateKey + "</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
                RSA.FromXmlString(privateKey);

                encryptedData = RSA.Encrypt(dataToEncrypt, true);


                //Pass the data to DECRYPT, the private key information 
                //(using RSACryptoServiceProvider.ExportParameters(true),
                //and a boolean flag specifying no OAEP padding.

                //decryptedData = SecurityRSA.RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

                //TextReader reader = new StreamReader(string.Format("RSAPrivateKey-{0}.xml", selectedUser.getUserName()));


                //publicKey = "<RSAKeyValue><Modulus>" + privateKey + "</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
                RSA.FromXmlString(publicKey);

                decryptedData = RSA.Decrypt(encryptedData, false);

                //Display the decrypted plaintext to the console. 
                Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));

                string md5string = SecurityHash.CalculateMD5Hash("TestTekst");


                //Check md5 and sign
                MD5 md5 = System.Security.Cryptography.MD5.Create();

                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes("testtext");

                byte[] hash = md5.ComputeHash(inputBytes);

                byte[] singedhash = RSA.SignHash(hash, "MD5");

                Utility.FileUtility.ByteArrayToFile("singedHash", singedhash);

                md5string = ByteConverter.GetString(singedhash);

                Console.WriteLine(string.Format("hash code of testTeskt: {0}", md5string));

            }
        }

        private static string getString(string nameFile)
        {
            byte[] bytes = File.ReadAllBytes(nameFile);
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private static byte[] getByteFromFile(string nameFile)
        {
            byte[] bytes = File.ReadAllBytes(nameFile);
            
            return bytes;
        }

        private static void saveByte(string str, string nameFile)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            File.WriteAllBytes(nameFile, bytes);
        }

        private static string keyToString (RSAParameters key)
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

        public static string CalculateMD5Hash(string input)

        {

            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.Unicode.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);


            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("X2"));

            }

            return sb.ToString();

        }

        private static byte[] decryptFileWithRSA(string _fileName, RSAParameters key)
        {
            /*
            string plainTextData = getString(_fileName);

            var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);
            */

            var rsabytedata = getByteFromFile(_fileName);

            var csp = new RSACryptoServiceProvider(2048);
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(key);

            var testvalue = csp.Decrypt(rsabytedata, false);
            return testvalue;
        }
    }
}
