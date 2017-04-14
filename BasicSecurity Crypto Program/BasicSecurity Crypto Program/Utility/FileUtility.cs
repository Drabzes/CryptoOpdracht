﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicSecurity_Crypto_Program.Utility
{
    public class FileUtility
    {
        // write bytes to a file
        public static bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

        //Giel added
        //Read a byte file and get the value from it in byte[]
        public static byte[] ReadByteArrayFromFile(String _FileName)
        {
            //byte[] buff = null;

            return File.ReadAllBytes(_FileName);
        }

        public static byte[] ReadByteArrayFromFileRSA(String _FileName)
        {
            byte[] UnTrimmed = File.ReadAllBytes(_FileName);
            byte[] Trimmed = new byte[UnTrimmed.Length];
            int i = 0;

            foreach(byte bit in UnTrimmed)
            {
                if (bit != 0)
                {
                    Trimmed[i] = bit;
                    i++;
                }
                
            }

            return Trimmed;
        }

        //Giel added
        //Check if file exists.
        public static bool CheckFileExist(string fileName)
        {
            return File.Exists(fileName); ;
        }
    }
}
