using System;

namespace Dorado.ESB.Communication
{
    public interface IObjectSerializer
    {
        byte[] Serialize(object instance);

        object Deserialize(byte[] data, Type objectType);
    }
}