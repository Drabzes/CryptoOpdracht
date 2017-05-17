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
                

                _selectedUser = FileUtility.loadUser(_name);

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

                    string pubKeyString = EncryptionUtility.keyToString(pubKey);

                    string privKeyString = EncryptionUtility.keyToString(privKey);

                    Console.WriteLine(String.Format("Saving keys"));
                    string _nameFile = string.Format("{0}-RSAPrivKey.dat", _name);
                    FileUtility.saveByte(privKeyString, _nameFile);
                    _nameFile = string.Format("{0}-RSAPubKey.dat", _name);
                    FileUtility.saveByte(pubKeyString, _nameFile);
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
                            _name = Console.ReadLine(); // name of the user
                            _fileNamePriv = string.Format("{0}-RSAPrivKey.dat", _name); // RSAPrivate key filename
                            _fileNamePub = string.Format("{0}-RSAPubKey.dat", _name); // RSAPublic key filename
                            if (FileUtility.CheckFileExist(_fileNamePub) && FileUtility.CheckFileExist(_fileNamePriv)) //Checking if the Pub en Priv exist
                            {
                                Console.Write("Message to send: ");
                                string message = Console.ReadLine();

                                _sendUser = FileUtility.loadUser(_name); //user gegevesn laden
                                EncryptionUtility.aosEncryption(message, _selectedUser, _sendUser, IV); // Aos excryption doen.
                            }
                            else
                            {
                                Console.WriteLine(string.Format("Could not find {0} ", _name));
                                Console.WriteLine("");
                            }
                            break;
                        case 2:
                            //Show a list of all message files
                            FileUtility.loadfileList(_selectedUser); // Show file lists

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
                                    _sendUser = FileUtility.loadUser(_name);

                                    //Get AES key
                                    byte[] _aesKeyBytes = EncryptionUtility.decryptFileWithRSA(_aesKeyFile, _selectedUser.getPrivKey());
                                    //Decrypt message
                                    string _message = EncryptionUtility.decryptMessage(_messageFile, _aesKeyBytes, IV);

                                    //Hash stuff
                                    EncryptionUtility.checkHash(_hashFile, _sendUser, _message);                                    

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

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }
            Console.ReadKey();
        }
    }
}
