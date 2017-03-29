using System;
using System.Configuration;
using System.Reflection;
using System.ServiceModel.Configuration;
using System.Xml;

namespace Dorado.ESB.Common.Config
{
    public sealed class ServiceModelSection : ConfigurationSection
    {
        public ServicesSection Services { get; set; }

        public ClientSection Client { get; set; }

        public BehaviorsSection Behaviors { get; set; }

        public BindingsSection Bindings { get; set; }

        public ExtensionsSection Extensions { get; set; }

        public DiagnosticSection Diagnostics { get; set; }

        protected override void DeserializeSection(XmlReader reader)
        {
            Type cfgType = typeof(ConfigurationSection);
            MethodInfo mi = cfgType.GetMethod("DeserializeElement", BindingFlags.Instance | BindingFlags.NonPublic);

            reader.ReadStartElement("configuration");
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.Name == "system.serviceModel")
                {
                    reader.ReadStartElement();
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        #region sections

                        string element = reader.Name;
                        try
                        {
                            if (reader.IsEmptyElement || reader.NodeType == XmlNodeType.Whitespace)
                            {
                                reader.Skip();
                                continue;
                            }

                            if (reader.Name == "diagnostics")
                            {
                                Diagnostics = new DiagnosticSection();
                                mi.Invoke(Diagnostics, new object[] { reader, false });
                                reader.ReadEndElement();
                            }
                            else if (reader.Name == "extensions")
                            {
                                Extensions = new ExtensionsSection();
                                mi.Invoke(Extensions, new object[] { reader, false });
                                reader.ReadEndElement();
                            }
                            else if (reader.Name == "services")
                            {
                                Services = new ServicesSection();
                                mi.Invoke(Services, new object[] { reader, false });
                                reader.ReadEndElement();
                            }
                            else if (reader.Name == "bindings")
                            {
                                Bindings = new BindingsSection();
                                /////
                                //if (Extensions != null && Extensions.BindingElementExtensions != null && Extensions.BindingElementExtensions.Count > 0)
                                //{
                                //    PropertyInfo fi = typeof(BindingsSection).GetProperty("Properties", BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic );
                                //    ConfigurationPropertyCollection properties = (ConfigurationPropertyCollection)fi.GetValue(Bindings, null);
                                //    if (properties != null)
                                //    {
                                //        foreach (ExtensionElement extensions in Extensions.BindingElementExtensions)
                                //        {
                                //            if (extensions != null)
                                //            {
                                //                ConfigurationProperty property = new ConfigurationProperty(extensions.Name, Type.GetType(extensions.Type, true), null, ConfigurationPropertyOptions.None);
                                //                properties.Add(property);
                                //            }
                                //        }
                                //    }
                                //}
                                /////
                                mi.Invoke(Bindings, new object[] { reader, false });
                                reader.ReadEndElement();
                            }
                            else if (reader.Name == "behaviors")
                            {
                                Behaviors = new BehaviorsSection();
                                mi.Invoke(Behaviors, new object[] { reader, false });
                                reader.ReadEndElement();
                            }
                            else if (reader.Name == "client")
                            {
                                Client = new ClientSection();
                                mi.Invoke(Client, new object[] { reader, false });
                                reader.ReadEndElement();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(string.Format("Deserializing '{0}' failed, {1}", element, ex.InnerException == null ? ex.Message : ex.InnerException.Message));
                        }

                        #endregion sections

                        reader.MoveToContent();
                    }
                }
                reader.Skip();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
    }
}