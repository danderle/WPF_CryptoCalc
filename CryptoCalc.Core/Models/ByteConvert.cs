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
        public static byte[] StringToAsciiBytes(string text)
        {
            return string.IsNullOrEmpty(text) ? null : Encoding.ASCII.GetBytes(text);
        }

        /// <summary>
        /// Converts bytes to a hex string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", string.Empty);
        }

        /// <summary>
        /// Converts bytes to a ascii string
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToAsciiString(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>
        /// Verifies if only hex characters are used in given text
        /// </summary>
        /// <param name="text">The text to check</param>
        /// <returns>True if all chars are hex values</returns>
        public static bool OnlyHexInString(string text)
        {
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
        }
        #endregion
    }
}
