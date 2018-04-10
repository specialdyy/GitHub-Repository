using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Common
{
    public static class SerializationHelper
    {
        /// <summary>
        /// 用序列化的方式对引用对象完成深拷贝（利用 System.Runtime.Serialization 来实现序列化与反序列化）。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RealObject"></param>
        /// <returns></returns>
        public static T BinaryClone<T>(this T RealObject)
        {
            using (Stream objectStream = new MemoryStream())
            {
                //利用 System.Runtime.Serialization 序列化与反序列化完成引用对象的复制
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, RealObject);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(objectStream);
            }
        }

        /// <summary>
        /// 用序列化的方式对引用对象完成深拷贝（利用 System.Xml.Serialization 来实现序列化与反序列化）。
        /// </summary>
        /// <typeparam name="T">需要序列化的对象类型。</typeparam>
        /// <param name="RealObject">需要序列化的对象。</param>
        /// <returns></returns>
        public static T XmlClone<T>(this T RealObject)
        {
            using (Stream stream = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, RealObject);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)serializer.Deserialize(stream);
            }
        }


        public static IEnumerable<T> Clone<T>(this IEnumerable<T> obj) where T : ICloneable
        {
            if (obj == null)
            {
                return null;
            }
            T[] arr = obj.ToArray();
            IEnumerable<T> result = from i in arr
                                    select (T)i.Clone();
            return result;
        }
    }
}
