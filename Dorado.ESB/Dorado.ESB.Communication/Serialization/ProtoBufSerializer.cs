using ProtoBuf;
using System;
using System.Collections;
using System.IO;

namespace Dorado.ESB.Communication
{
    public class ProtoBufSerializer : IObjectSerializer
    {
        #region ctor

        public ProtoBufSerializer()
        {
        }

        #endregion ctor

        #region IObjectSerializer Members

        public byte[] Serialize(object instance)
        {
            if (instance is IDictionary)
                return SerializeDictionary(instance as IDictionary);

            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.NonGeneric.Serialize(stream, instance);
                return stream.ToArray();
            }
        }

        private byte[] SerializeDictionary(IDictionary dict)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                int index = 1;
                foreach (DictionaryEntry entry in dict)
                {
                    if (entry.Value == null) continue;
                    Serializer.NonGeneric.SerializeWithLengthPrefix(stream, entry.Key, PrefixStyle.Base128, index);
                    Serializer.NonGeneric.SerializeWithLengthPrefix(stream, entry.Value, PrefixStyle.Base128, index + 1);
                    ++index;
                }
                return stream.ToArray();
            }
        }

        public object Deserialize(byte[] data, Type objectType)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return Serializer.NonGeneric.Deserialize(objectType, stream);
            }
        }

        #endregion IObjectSerializer Members
    }
}