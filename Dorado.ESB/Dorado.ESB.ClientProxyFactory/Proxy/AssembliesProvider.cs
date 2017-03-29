using System;
using System.IO;
using System.Reflection;

namespace Dorado.ESB.ClientProxyFactory.Proxy
{
    public class AssembliesProvider
    {
        private static readonly AssembliesProvider instance = new AssembliesProvider();

        private Assembly[] Assemblies = null;

        private AssembliesProvider()
        {
            Assemblies = GetAssemblies();
        }

        public static AssembliesProvider Instance
        {
            get { return instance; }
        }

        public Assembly[] CurrentAssemblies
        {
            get
            {
                if (Assemblies != null && Assemblies.Length > 0)
                {
                    return Assemblies;
                }
                else
                {
                    Assemblies = GetAssemblies();
                    return Assemblies;
                }
            }
        }

        private Assembly[] GetAssemblies()
        {
            string baseDirectory = Assembly.GetExecutingAssembly().CodeBase;
            baseDirectory = System.IO.Path.GetDirectoryName(baseDirectory).Replace("file:\\", "");
            DirectoryInfo directoryInfo = new DirectoryInfo(baseDirectory);
            foreach (FileInfo file in directoryInfo.GetFiles("*.dll"))
            {
                string filename = file.Name.Replace(file.Extension, "");
                AppDomain.CurrentDomain.Load(filename);
            }
            Assembly[] assemblys = AppDomain.CurrentDomain.GetAssemblies();
            return assemblys;
        }
    }
}