using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PMapCore.Common
{
    
    public static class BinarySerializer
    {
        public static void Serialize(Stream stream, object value)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, value);
        }

        public static void Serialize(FileInfo file, object value)
        {
            FileStream stream = null;
            try
            {
                stream = File.Create(file.FullName);
                Serialize(stream, value);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
        public static object Deserialize(Stream stream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
        }

        public static object Deserialize(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
                return Deserialize(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
    }
}
