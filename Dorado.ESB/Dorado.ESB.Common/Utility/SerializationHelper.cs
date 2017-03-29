using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Dorado.ESB.Common.Utility
{
    /// <summary>Serialization Helper Class</summary>
    public static class SerializationHelper
    {
        #region Fields

        // Constants.
        private static string cXmlSearializableDictionaryRootElement = "Dictionary";

        private static string cXmlSearializableDictionaryItemElement = "Item";
        private static string cXmlSealializableDictionaryKeyAttribute = "key";
        private static string cXmlSealializableDictionaryValueAttribute = "value";

        #endregion Fields

        #region SOAP

        private static SoapFormatter SoapFormatter = new SoapFormatter();

        /// <summary>
        /// Returns the SOAP string representation of this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToSoap(object obj)
        {
            Check.NotNull(obj, "object");

            string str = null;

            using (MemoryStream stream = new MemoryStream())
            {
                SoapFormatter.Serialize(stream, obj);
                str = IOHelper.ReadContentsFromStream(stream);
            }

            return str;
        }

        /// <summary>
        /// Returns an object from this SOAP string.
        /// </summary>
        /// <param name="soap"></param>
        /// <returns></returns>
        public static object ToObjectFromSoap(string soap)
        {
            Check.NotEmpty(soap, "soap string");

            MemoryStream stream = null;
            object obj;

            try
            {
                stream = IOHelper.CreateMemoryStream(soap, Encoding.Default);
                obj = (Object)SoapFormatter.Deserialize(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return obj;
        }

        #endregion SOAP

        /// <summary>
        /// Creates an object from a Data Contract serialized string.
        /// </summary>
        /// <param name="dataContract"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object ToObjectFromDataContract(Type type, string dataContract)
        {
            Check.NotEmpty(dataContract, "dataContract");

            object obj;

            using (XmlTextReader xmlTextReader = new XmlTextReader(IOHelper.CreateMemoryStream(dataContract, Encoding.UTF8)))
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(type);
                obj = dataContractSerializer.ReadObject(xmlTextReader);
            }

            return obj;
        }

        /// <summary>
        /// Returns the Data Contract serialization of the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToDataContract(object obj)
        {
            Check.NotNull(obj, "object");

            using (MemoryStream stream = new MemoryStream())
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(obj.GetType());
                dataContractSerializer.WriteObject(stream, obj);

                return IOHelper.ReadContentsFromStream(stream);
            }
        }

        #region XML

        /// <summary>
        /// Returns the XML string representaion on this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXml(object obj)
        {
            return ToXml(obj, string.Empty);
        }

        /// <summary>Returns the XML string representaion on this object using this default namespace.</summary>
        /// <param name="obj">Object to convert</param>
        /// <param name="defaultNamespace">Default XML namespace</param>
        /// <returns>XML Formatted string represenataion of object</returns>
        public static string ToXml(object obj, string defaultNamespace)
        {
            Check.NotNull(obj, "object");

            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType(), defaultNamespace);
                xmlSerializer.Serialize(stream, obj);
                return IOHelper.ReadContentsFromStream(stream);
            }
        }

        /// <summary>Returns the object for this type and XML string.</summary>
        /// <param name="type">Object Type</param>
        /// <param name="xml">XML representation</param>
        /// <returns>Hydrated object</returns>
        public static object ToObjectFromXml(Type type, string xml)
        {
            return ToObjectFromXml(type, xml, null);
        }

        /// <summary>Returns the object for this type and XML string using this namespace.</summary>
        /// <param name="type">Object Type</param>
        /// <param name="xml">XML Representation</param>
        /// <param name="defaultNamespace">Default XML namespace</param>
        /// <returns>Hydrated object</returns>
        public static object ToObjectFromXml(Type type, string xml, string defaultNamespace)
        {
            Check.NotEmpty(xml, "xml");

            //	MemoryStream stream = null;
            object obj;

            using (StringReader reader = new StringReader(xml))
            {
                XmlSerializer serial = new XmlSerializer(type, defaultNamespace);
                obj = serial.Deserialize(reader);
            }

            return obj;
        }

        /// <summary>Returns the XML string representation of a dictionary object.</summary>
        /// <param name="dict">Dictionary object</param>
        /// <param name="rootElementName">Root Element Name</param>
        /// <param name="itemElementName">Item Element Name</param>
        /// <returns>Formatted XML string</returns>
        public static string ToXmlFromDictionary(IDictionary<string, object> dict, string rootElementName,
                                                 string itemElementName)
        {
            return ToXmlFromDictionary(dict, string.Empty, rootElementName, itemElementName);
        }

        /// <summary>Returns the XML string representaion on dictionary object using default namespace.</summary>
        /// <param name="dict">Dictionary object</param>
        /// <param name="defaultNamespace">Default XML namespace</param>
        /// <param name="rootElementName">Root Element Name</param>
        /// <param name="itemElementName">Item Element Name</param>
        /// <returns>Formatted XML string</returns>
        public static string ToXmlFromDictionary(IDictionary<string, object> dict, string defaultNamespace,
                                                 string rootElementName, string itemElementName)
        {
            Check.NotNull(dict, "dictionary");

            MemoryStream stream = new MemoryStream();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stream, Encoding.UTF8);

            /* Xml Format:
             * <Dictionary>
             *    <Item key="" value="" />
             *    <Item key="" value="" />
             * </Dictionary>
             */
            if (string.IsNullOrEmpty(rootElementName))
            {
                rootElementName = cXmlSearializableDictionaryRootElement;
            }
            if (string.IsNullOrEmpty(itemElementName))
            {
                itemElementName = cXmlSearializableDictionaryItemElement;
            }

            // Write root xml element.
            xmlTextWriter.WriteStartElement(rootElementName, defaultNamespace);
            foreach (string key in dict.Keys)
            {
                // Write each itme in dictionary to xml.
                xmlTextWriter.WriteStartElement(itemElementName, defaultNamespace);
                xmlTextWriter.WriteAttributeString(cXmlSealializableDictionaryKeyAttribute, key);
                xmlTextWriter.WriteAttributeString(cXmlSealializableDictionaryValueAttribute, StringHelper.ToString(dict[key]));
                xmlTextWriter.WriteEndElement();
            }
            xmlTextWriter.WriteFullEndElement();
            xmlTextWriter.Flush();

            // Read xml from stream.
            stream.Position = 0;
            string resultXml = IOHelper.ReadContentsFromStream(stream);
            xmlTextWriter.Close();

            // Return xml.
            return resultXml;
        }

        #endregion XML

        #region Cloning Methods

        /// <summary>
        /// Clones the object using XML serialization.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static object CloneObjectUsingXmlSerialization(object obj)
        {
            Check.NotNull(obj, "obj");

            string xml = ToXml(obj);
            object clonedObject = ToObjectFromXml(obj.GetType(), xml);

            return clonedObject;
        }

        /// <summary>
        /// Clone an object using DataContract serialization.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object CloneObjectUsingDataContractSerialization(object obj)
        {
            Check.NotNull(obj, "obj");

            string dataContract = ToDataContract(obj);
            object clonedObject = ToObjectFromDataContract(obj.GetType(), dataContract);

            return clonedObject;
        }

        /// <summary>Clone Object Using Binary Serialization</summary>
        /// <param name="obj">Object to clone</param>
        /// <returns>Clone of object</returns>
        public static object CloneObjectUsingBinarySerialization(object obj)
        {
            MemoryStream stream = null;
            object clonedObject = null;

            try
            {
                stream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(stream, obj);

                stream.Seek(0, SeekOrigin.Begin);

                clonedObject = binaryFormatter.Deserialize(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return clonedObject;
        }

        #endregion Cloning Methods

        #region Binary

        /// <summary>
        /// Toes the binary string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static string ToBinaryString(object obj)
        {
            Check.NotNull(obj, "obj");

            MemoryStream stream = null;
            string result = null;

            try
            {
                stream = new MemoryStream();
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(stream, obj);

                stream.Seek(0, SeekOrigin.Begin);

                //convert to base64
                result = Convert.ToBase64String(stream.ToArray());
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Toes the object from binary string.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public static object ToObjectFromBinaryString(string obj)
        {
            Check.NotNull(obj, "obj");

            MemoryStream stream = null;
            object result = null;

            try
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                stream = new MemoryStream(Convert.FromBase64String(obj));

                result = binaryFormatter.Deserialize(stream);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return result;
        }

        #endregion Binary
    }
}