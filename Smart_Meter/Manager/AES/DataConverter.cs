using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Common;

namespace Manager.AES
{
    public static class DataConverter
    {

        public static byte[] StringToBytes(string input)
        {
            return Encoding.UTF8.GetBytes(input);
        }

        public static string BytesToString(byte[] input)
        {
            return Encoding.UTF8.GetString(input);
        }

        public static byte[] DoubleToBytes(double input)
        {
            return BitConverter.GetBytes(input);
        }

        public static double BytesToDouble(byte[] input)
        {
            return BitConverter.ToDouble(input, 0);
        }


    }
}
