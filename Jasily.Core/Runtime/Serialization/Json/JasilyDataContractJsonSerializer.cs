﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Runtime.Serialization.Json
{
    public static class JasilyDataContractJsonSerializer
    {
        public static T JsonToObject<T>(this Stream stream)
        {
            var ser = new DataContractJsonSerializer(typeof(T));
            T obj = (T)ser.ReadObject(stream);
            return obj;
        }

        public static T JsonToObject<T>(this byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                return ms.JsonToObject<T>();
            }
        }

        public static T JsonToObject<T>(this string jsonDoc, Encoding encoding)
        {
            return encoding.GetBytes(jsonDoc).JsonToObject<T>();
        }

        public static T JsonToObject<T>(this string jsonDoc)
        {
            return jsonDoc.JsonToObject<T>(Encoding.UTF8);
        }
    }
}
