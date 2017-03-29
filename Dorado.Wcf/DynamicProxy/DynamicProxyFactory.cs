#region Copyright Notice

// ----------------------------------------------------------------------------
// Copyright (C) 2006 Microsoft Corporation, All rights reserved.
// ----------------------------------------------------------------------------

// Author: Vipul Modi (vipul.modi@microsoft.com)

#endregion Copyright Notice

namespace Dorado.Wcf.DynamicProxy
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Data;
    using System.Data.Design;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.Text;
    using System.Web.Services.Discovery;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using WsdlNS = System.Web.Services.Description;

    public class DynamicProxyFactory
    {
        private string wsdlUri;
        private DynamicProxyFactoryOptions options;

        private CodeCompileUnit codeCompileUnit;
        private CodeDomProvider codeDomProvider;
        private ServiceContractGenerator contractGenerator;

        private Collection<MetadataSection> metadataCollection;
        private IEnumerable<Binding> bindings;
        private IEnumerable<ContractDescription> contracts;
        private ServiceEndpointCollection endpoints;
        private IEnumerable<MetadataConversionError> importWarnings;
        private IEnumerable<MetadataConversionError> codegenWarnings;
        private IEnumerable<CompilerError> compilerWarnings;

        private Assembly proxyAssembly;
        private string proxyCode;

        public DynamicProxyFactory(string wsdlUri, DynamicProxyFactoryOptions options)
        {
            if (wsdlUri == null)
                throw new ArgumentNullException("wsdlUri");

            if (options == null)
                throw new ArgumentNullException("options");

            wsdlUri = wsdlUri;
            options = options;

            DownloadMetadata();
            ImportMetadata();
            CreateProxy();
            WriteCode();
            CompileProxy();
        }

        public DynamicProxyFactory(string wsdlUri)
            : this(wsdlUri, new DynamicProxyFactoryOptions())
        {
        }

        private void DownloadMetadata()
        {
            EndpointAddress epr = new EndpointAddress(wsdlUri);

            DiscoveryClientProtocol disco = new DiscoveryClientProtocol();
            disco.AllowAutoRedirect = true;
            disco.UseDefaultCredentials = true;
            disco.DiscoverAny(wsdlUri);
            disco.ResolveAll();

            Collection<MetadataSection> results = new Collection<MetadataSection>();
            foreach (object document in disco.Documents.Values)
            {
                AddDocumentToResults(document, results);
            }
            metadataCollection = results;
        }

        private void AddDocumentToResults(object document, Collection<MetadataSection> results)
        {
            WsdlNS.ServiceDescription wsdl = document as WsdlNS.ServiceDescription;
            XmlSchema schema = document as XmlSchema;
            XmlElement xmlDoc = document as XmlElement;

            if (wsdl != null)
            {
                results.Add(MetadataSection.CreateFromServiceDescription(wsdl));
            }
            else if (schema != null)
            {
                results.Add(MetadataSection.CreateFromSchema(schema));
            }
            else if (xmlDoc != null && xmlDoc.LocalName == "Policy")
            {
                results.Add(MetadataSection.CreateFromPolicy(xmlDoc, null));
            }
            else
            {
                MetadataSection mexDoc = new MetadataSection();
                mexDoc.Metadata = document;
                results.Add(mexDoc);
            }
        }

        private void ImportMetadata()
        {
            codeCompileUnit = new CodeCompileUnit();
            CreateCodeDomProvider();

            WsdlImporter importer = new WsdlImporter(new MetadataSet(metadataCollection));
            AddStateForDataContractSerializerImport(importer);
            AddStateForXmlSerializerImport(importer);

            bindings = importer.ImportAllBindings();
            contracts = importer.ImportAllContracts();
            endpoints = importer.ImportAllEndpoints();
            importWarnings = importer.Errors;

            bool success = true;
            if (importWarnings != null)
            {
                foreach (MetadataConversionError error in importWarnings)
                {
                    if (!error.IsWarning)
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (!success)
            {
                DynamicProxyException exception = new DynamicProxyException(
                    Constants.ErrorMessages.ImportError);
                exception.MetadataImportErrors = importWarnings;
                throw exception;
            }
        }

        private void AddStateForXmlSerializerImport(WsdlImporter importer)
        {
            XmlSerializerImportOptions importOptions =
                new XmlSerializerImportOptions(codeCompileUnit);
            importOptions.CodeProvider = codeDomProvider;

            importOptions.WebReferenceOptions = new WsdlNS.WebReferenceOptions();
            importOptions.WebReferenceOptions.CodeGenerationOptions =
                CodeGenerationOptions.GenerateProperties |
                CodeGenerationOptions.GenerateOrder;

            importOptions.WebReferenceOptions.SchemaImporterExtensions.Add(
                typeof(TypedDataSetSchemaImporterExtension).AssemblyQualifiedName);
            importOptions.WebReferenceOptions.SchemaImporterExtensions.Add(
                typeof(DataSetSchemaImporterExtension).AssemblyQualifiedName);

            importer.State.Add(typeof(XmlSerializerImportOptions), importOptions);
        }

        private void AddStateForDataContractSerializerImport(WsdlImporter importer)
        {
            XsdDataContractImporter xsdDataContractImporter =
                new XsdDataContractImporter(codeCompileUnit);
            xsdDataContractImporter.Options = new ImportOptions();
            xsdDataContractImporter.Options.ImportXmlType =
                (options.FormatMode ==
                    DynamicProxyFactoryOptions.FormatModeOptions.DataContractSerializer);

            xsdDataContractImporter.Options.CodeProvider = codeDomProvider;
            importer.State.Add(typeof(XsdDataContractImporter),
                    xsdDataContractImporter);

            foreach (IWsdlImportExtension importExtension in importer.WsdlImportExtensions)
            {
                DataContractSerializerMessageContractImporter dcConverter =
                    importExtension as DataContractSerializerMessageContractImporter;

                if (dcConverter != null)
                {
                    if (options.FormatMode ==
                        DynamicProxyFactoryOptions.FormatModeOptions.XmlSerializer)
                        dcConverter.Enabled = false;
                    else
                        dcConverter.Enabled = true;
                }
            }
        }

        private void CreateProxy()
        {
            CreateServiceContractGenerator();

            foreach (ContractDescription contract in contracts)
            {
                contractGenerator.GenerateServiceContractType(contract);
            }

            bool success = true;
            codegenWarnings = contractGenerator.Errors;
            if (codegenWarnings != null)
            {
                foreach (MetadataConversionError error in codegenWarnings)
                {
                    if (!error.IsWarning)
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (!success)
            {
                DynamicProxyException exception = new DynamicProxyException(
                 Constants.ErrorMessages.CodeGenerationError);
                exception.CodeGenerationErrors = codegenWarnings;
                throw exception;
            }
        }

        private void CompileProxy()
        {
            // reference the required assemblies with the correct path.
            CompilerParameters compilerParams = new CompilerParameters();

            AddAssemblyReference(
                typeof(System.ServiceModel.ServiceContractAttribute).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(
                typeof(System.Web.Services.Description.ServiceDescription).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(
                typeof(System.Runtime.Serialization.DataContractAttribute).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(typeof(System.Xml.XmlElement).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(typeof(System.Uri).Assembly,
                compilerParams.ReferencedAssemblies);

            AddAssemblyReference(typeof(System.Data.DataSet).Assembly,
                compilerParams.ReferencedAssemblies);

            CompilerResults results =
                codeDomProvider.CompileAssemblyFromSource(
                    compilerParams,
                    proxyCode);

            if ((results.Errors != null) && (results.Errors.HasErrors))
            {
                DynamicProxyException exception = new DynamicProxyException(
                    Constants.ErrorMessages.CompilationError);
                exception.CompilationErrors = ToEnumerable(results.Errors);

                throw exception;
            }

            compilerWarnings = ToEnumerable(results.Errors);
            proxyAssembly = Assembly.LoadFile(results.PathToAssembly);
        }

        private void WriteCode()
        {
            using (StringWriter writer = new StringWriter())
            {
                CodeGeneratorOptions codeGenOptions = new CodeGeneratorOptions();
                codeGenOptions.BracingStyle = "C";
                codeDomProvider.GenerateCodeFromCompileUnit(
                        codeCompileUnit, writer, codeGenOptions);
                writer.Flush();
                proxyCode = writer.ToString();
            }

            // use the modified proxy code, if code modifier is set.
            if (options.CodeModifier != null)
                proxyCode = options.CodeModifier(proxyCode);
        }

        private void AddAssemblyReference(Assembly referencedAssembly,
            StringCollection refAssemblies)
        {
            string path = Path.GetFullPath(referencedAssembly.Location);
            string name = Path.GetFileName(path);
            if (!(refAssemblies.Contains(name) ||
                  refAssemblies.Contains(path)))
            {
                refAssemblies.Add(path);
            }
        }

        public ServiceEndpoint GetEndpoint(string contractName)
        {
            return GetEndpoint(contractName, null);
        }

        public ServiceEndpoint GetEndpoint(string contractName,
                string contractNamespace)
        {
            ServiceEndpoint matchingEndpoint = null;

            foreach (ServiceEndpoint endpoint in Endpoints)
            {
                if (ContractNameMatch(endpoint.Contract, contractName) &&
                    ContractNsMatch(endpoint.Contract, contractNamespace))
                {
                    matchingEndpoint = endpoint;
                    break;
                }
            }

            if (matchingEndpoint == null)
                throw new ArgumentException(string.Format(
                    Constants.ErrorMessages.EndpointNotFound,
                    contractName, contractNamespace));

            return matchingEndpoint;
        }

        private bool ContractNameMatch(ContractDescription cDesc, string name)
        {
            return (string.Compare(cDesc.Name, name, true) == 0);
        }

        private bool ContractNsMatch(ContractDescription cDesc, string ns)
        {
            return ((ns == null) ||
                    (string.Compare(cDesc.Namespace, ns, true) == 0));
        }

        public DynamicProxy CreateProxy(string contractName)
        {
            return CreateProxy(contractName, null);
        }

        public DynamicProxy CreateProxy(string contractName,
                string contractNamespace)
        {
            ServiceEndpoint endpoint = GetEndpoint(contractName,
                    contractNamespace);

            return CreateProxy(endpoint);
        }

        public DynamicProxy CreateProxy(ServiceEndpoint endpoint)
        {
            Type contractType = GetContractType(endpoint.Contract.Name,
                endpoint.Contract.Namespace);

            Type proxyType = GetProxyType(contractType);

            return new DynamicProxy(proxyType, endpoint.Binding,
                    endpoint.Address);
        }

        private Type GetContractType(string contractName,
                string contractNamespace)
        {
            Type[] allTypes = proxyAssembly.GetTypes();
            ServiceContractAttribute scAttr = null;
            Type contractType = null;
            XmlQualifiedName cName;
            foreach (Type type in allTypes)
            {
                // Is it an interface?
                if (!type.IsInterface)
                    continue;

                // Is it marked with ServiceContract attribute?
                object[] attrs = type.GetCustomAttributes(
                    typeof(ServiceContractAttribute), false);
                if ((attrs == null) || (attrs.Length == 0))
                    continue;

                // is it the required service contract?
                scAttr = (ServiceContractAttribute)attrs[0];
                cName = GetContractName(type, scAttr.Name, scAttr.Namespace);

                if (string.Compare(cName.Name, contractName, true) != 0)
                    continue;

                if (string.Compare(cName.Namespace, contractNamespace,
                            true) != 0)
                    continue;

                contractType = type;
                break;
            }

            if (contractType == null)
                throw new ArgumentException(
                    Constants.ErrorMessages.UnknownContract);

            return contractType;
        }

        internal const string DefaultNamespace = "http://tempuri.org/";

        internal static XmlQualifiedName GetContractName(Type contractType,
            string name, string ns)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = contractType.Name;
            }

            if (ns == null)
            {
                ns = DefaultNamespace;
            }
            else
            {
                ns = Uri.EscapeUriString(ns);
            }

            return new XmlQualifiedName(name, ns);
        }

        private Type GetProxyType(Type contractType)
        {
            Type clientBaseType = typeof(ClientBase<>).MakeGenericType(
                    contractType);

            Type[] allTypes = ProxyAssembly.GetTypes();
            Type proxyType = null;

            foreach (Type type in allTypes)
            {
                // Look for a proxy class that implements the service
                // contract and is derived from ClientBase<service contract>
                if (type.IsClass && contractType.IsAssignableFrom(type)
                    && type.IsSubclassOf(clientBaseType))
                {
                    proxyType = type;
                    break;
                }
            }

            if (proxyType == null)
                throw new DynamicProxyException(string.Format(
                            Constants.ErrorMessages.ProxyTypeNotFound,
                            contractType.FullName));

            return proxyType;
        }

        private void CreateCodeDomProvider()
        {
            codeDomProvider = CodeDomProvider.CreateProvider(options.Language.ToString());
        }

        /// <summary>
        /// 代码生成选项
        /// </summary>
        private void CreateServiceContractGenerator()
        {
            contractGenerator = new ServiceContractGenerator(
                codeCompileUnit);
            contractGenerator.Options |= ServiceContractGenerationOptions.AsynchronousMethods;
        }

        public IEnumerable<MetadataSection> Metadata
        {
            get
            {
                return metadataCollection;
            }
        }

        public IEnumerable<Binding> Bindings
        {
            get
            {
                return bindings;
            }
        }

        public IEnumerable<ContractDescription> Contracts
        {
            get
            {
                return contracts;
            }
        }

        public IEnumerable<ServiceEndpoint> Endpoints
        {
            get
            {
                return endpoints;
            }
        }

        public Assembly ProxyAssembly
        {
            get
            {
                return proxyAssembly;
            }
        }

        public string ProxyCode
        {
            get
            {
                return proxyCode;
            }
        }

        public IEnumerable<MetadataConversionError> MetadataImportWarnings
        {
            get
            {
                return importWarnings;
            }
        }

        public IEnumerable<MetadataConversionError> CodeGenerationWarnings
        {
            get
            {
                return codegenWarnings;
            }
        }

        public IEnumerable<CompilerError> CompilationWarnings
        {
            get
            {
                return compilerWarnings;
            }
        }

        public static string ToString(IEnumerable<MetadataConversionError>
            importErrors)
        {
            if (importErrors != null)
            {
                StringBuilder importErrStr = new StringBuilder();

                foreach (MetadataConversionError error in importErrors)
                {
                    if (error.IsWarning)
                        importErrStr.AppendLine("Warning : " + error.Message);
                    else
                        importErrStr.AppendLine("Error : " + error.Message);
                }

                return importErrStr.ToString();
            }
            else
            {
                return null;
            }
        }

        public static string ToString(IEnumerable<CompilerError> compilerErrors)
        {
            if (compilerErrors != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (CompilerError error in compilerErrors)
                    builder.AppendLine(error.ToString());

                return builder.ToString();
            }
            else
            {
                return null;
            }
        }

        private static IEnumerable<CompilerError> ToEnumerable(
                CompilerErrorCollection collection)
        {
            if (collection == null)
                return null;

            List<CompilerError> errorList = new List<CompilerError>();
            foreach (CompilerError error in collection)
                errorList.Add(error);

            return errorList;
        }
    }
}