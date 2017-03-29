using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Dorado.DataExpress.Resources
{
    [DebuggerNonUserCode, CompilerGenerated, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    internal class DataExpressResources
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(DataExpressResources.resourceMan, null))
                {
                    ResourceManager temp = new ResourceManager("Dorado.DataExpress.Resources.DataExpressResources", typeof(DataExpressResources).Assembly);
                    DataExpressResources.resourceMan = temp;
                }
                return DataExpressResources.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return DataExpressResources.resourceCulture;
            }
            set
            {
                DataExpressResources.resourceCulture = value;
            }
        }

        internal static string ObtainTimeoutDescription
        {
            get
            {
                return DataExpressResources.ResourceManager.GetString("ObtainTimeoutDescription", DataExpressResources.resourceCulture);
            }
        }

        internal DataExpressResources()
        {
        }
    }
}