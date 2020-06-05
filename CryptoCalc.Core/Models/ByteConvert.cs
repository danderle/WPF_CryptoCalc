using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CryptoCalc.Core
{
    public static class ByteConvert
    {
        #region Constructor

        static ByteConvert()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Reads all bytes from file
        /// </summary>
        /// <param name="filePath">path to the file</param>
        /// <returns>A byte array</returns>
        public static byte[] FileToBytes(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }
            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// Converts a hex string into byte array
        /// </summary>
        /// <param name="stringData">the hex string</param>
        /// <returns>A byte array</returns>
        public static byte[] HexStringToBytes(string stringData)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < stringData.Length; i += 2)
            {
                byte bite = Convert.ToByte(stringData.Substring(i, 2), 16);
                bytes.Add(bite);
            }
            return bytes.ToArray();
        }

        /// <summary>
        /// Converts text to byte array
        /// </summary>
        /// <param name="text">the string to convert to bytes</param>
        /// <returns>A byte array</returns>
        public static byte[] StringToBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        /// <summary>
        /// Converts bytes to a string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        #endregion
    }
}
