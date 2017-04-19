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

            Boolean exit = false; 

            //Opvragen username
            Console.Write("Username: ");
            //Username opslaan in de variabele username 
            string username = Console.ReadLine();
            //Opvragen wachtwoord
            Console.Write("Password: ");
            //Wachtwoord opslaan in de variabele password
            string password = Console.ReadLine();
            //Laat de gebruiker een keuze welk soort encryption gebruikt moet worden 
            while(!exit)
            {
                Console.WriteLine("Choose your encryption type : \n 1 : AES \n 2 : RSA");
                int option = Convert.ToInt32(Console.ReadLine());
                switch (option)
                {
                    case 1:
                        Console.WriteLine("You choose AES");
                        exit = true;
                        break;
                    case 2:
                        Console.WriteLine("You choose RSA");
                        exit = true;
                        break;
                }
            }

            //Keuzes geven aan de gebruiker 


            Console.WriteLine("Test Aes and AOS");
<<<<<<< HEAD
=======
            User Giel = new User("Giel");

>>>>>>> JorisBranch
            //Try aes encryption
            try
            {
                bool writeByteText = false;
                string original = "Here is some data to encrypt! AES";
                Console.WriteLine("In try of Aes");
                User selectedUser = null;

                Console.Write("Enter userName:  "); // Prompt
                string line = Console.ReadLine(); // Get string from user

                // Create a new instance of the Aes
                // class.  This generates a new key and initialization 
                // vector (IV).
                using (Aes myAes = Aes.Create())
                {
                    Console.WriteLine("Loading UserData..");
                    string _fileName;

                    //Giel added
                    //Get the fileName of the selected user his AOSkey.
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
                        selectedUser = new User(line, AOSKey);
                        Console.WriteLine(string.Format("Loaded all data from user: {0} ", selectedUser.getUserName()));
                    }
                    else
                    {
                        Console.WriteLine("AOSKey was not found!");
                        Console.WriteLine("Using default user Giel");
                        
                        //Convert AesKey bytes to a string
                        selectedUser = new User("Giel", myAes.Key);

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
                        Console.WriteLine("Encrypted Aeskey: " + selectedUser.getAesStringkey());
                    }

                    _fileName = "AesStringByteFile";

                    //Giel added
                    //Load the Aeskey from the user into myAes.key
                    myAes.Key = selectedUser.getKeyAesByte();
                    string AesKey = System.Text.Encoding.UTF8.GetString(myAes.Key);
                    Console.WriteLine(string.Format("AesKey is: {0}", AesKey));

                    //Giel added
                    //Check if the key from selectedUser is the same as myAes.key
                    //These need to be the same because myAes.key will be used to encrypt en decrypt the text
                    string AesKeyUser = System.Text.Encoding.UTF8.GetString(selectedUser.getKeyAesByte());
                    Console.WriteLine(string.Format("AesKeyUser is: {0}", AesKeyUser));

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
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e.Message);
            }

            try
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
                    //Get the RSA public in in xml from.
                    string publicKey = RSA.ToXmlString(false);
                    //Convert RSA public key string into bytes Array
                    byte[] publicKeyBytes = ByteConverter.GetBytes(publicKey);
                    //We name the file here where we want to put the public key in.
                    string fileName = "RSAPublicKey";

                    //Display the public key to check if it is still the same.
                    Console.WriteLine("xml form of the public key");
                    Console.WriteLine(publicKey);
                    Console.WriteLine("");

                    //Write the public key in bytes into a file.
                    Console.WriteLine("Creating public key file");
                    //Write the public key into a file
                    FileUtility.ByteArrayToFile(fileName, publicKeyBytes);
                    Console.WriteLine("Public ket file Created. Yay!!");

                    //Check if file exists
                    Console.WriteLine(string.Format("Check if file exists: {0}",FileUtility.CheckFileExist(fileName)));

                    Console.WriteLine(string.Format("Read file {0}", fileName));
                    //ReadFile
                    if (FileUtility.CheckFileExist(fileName) == true)
                    {
                        //read the file and put it into a byte[]
                        publicKeyBytes = FileUtility.ReadByteArrayFromFileRSA(fileName);
                        //convert the byte[] to a string
                        publicKey = System.Text.Encoding.UTF8.GetString(publicKeyBytes);
                        //show the string
                        Console.WriteLine("The public key from the file is: ");
                        publicKey.Trim();
                        Console.WriteLine(publicKey);
                    }

                    //Pass the data to ENCRYPT, the public key information 
                    //(using RSACryptoServiceProvider.ExportParameters(false),
                    //and a boolean flag specifying no OAEP padding.
                    encryptedData = SecurityRSA.RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);

                    //Pass the data to DECRYPT, the private key information 
                    //(using RSACryptoServiceProvider.ExportParameters(true),
                    //and a boolean flag specifying no OAEP padding.
                    decryptedData = SecurityRSA.RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

                    //Display the decrypted plaintext to the console. 
                    Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));
                }
            }
            catch (ArgumentNullException)
            {
                //Catch this exception in case the encryption did
                //not succeed.
                Console.WriteLine("Encryption failed.");

            }
            Console.ReadKey();
        }  
    }
}
