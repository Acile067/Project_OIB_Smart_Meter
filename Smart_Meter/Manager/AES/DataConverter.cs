using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

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

        public static byte[] IntToBytes(int input)
        {
            return BitConverter.GetBytes(input);
        }


        public static int BytesToInt(byte[] input)
        {
            return BitConverter.ToInt32(input, 0);
        }

        public static byte[] DoubleToBytes(double input)
        {
            return BitConverter.GetBytes(input);
        }

        public static double BytesToDouble(byte[] input)
        {
            return BitConverter.ToDouble(input, 0);
        }

        public static byte[] ObjectToBytes<T>(T obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        public static T BytesToObject<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            return JsonSerializer.Deserialize<T>(data);
        }

    }
}
