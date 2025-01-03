﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager.AES
{
    public class SecretKey
    {
        public static string GenerateKey()
        {
            SymmetricAlgorithm symmAlgorithm = AesCryptoServiceProvider.Create();

            return symmAlgorithm == null ? String.Empty : ASCIIEncoding.ASCII.GetString(symmAlgorithm.Key);
        }

        #region Store Secret Key

        /// <summary>
        /// Store a secret key as string value in a specified file.
        /// </summary>
        /// <param name="secretKey"> a symmetric key value </param>
        /// <param name="outFile"> file location to store a secret key </param>
        public static void StoreKey(string secretKey, string outFile)
        {
            string outFilePath = "../../../Keys/" + outFile;
            FileStream fOutput = new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Write);
            byte[] buffer = Encoding.ASCII.GetBytes(secretKey);

            try
            {
                fOutput.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("SecretKeys.StoreKey:: ERROR {0}", e.Message);
            }
            finally
            {
                fOutput.Close();
            }
        }

        #endregion

        #region Load Secret Key

        /// <summary>
        /// Load a symmetric key value from a file
        /// </summary>
        /// <param name="inFile"> file location of a secret key </param>
        /// <returns> a secret key value </returns>
        public static string LoadKey(string inFile)
        {

            string inFilePath = "../../../Keys/" + inFile;
            FileStream fInput = new FileStream(inFilePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[(int)fInput.Length];

            try
            {
                fInput.Read(buffer, 0, (int)fInput.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("SecretKeys.LoadKey:: ERROR {0}", e.Message);
            }
            finally
            {
                fInput.Close();
            }

            return ASCIIEncoding.ASCII.GetString(buffer);



        }

        #endregion


    }
}
