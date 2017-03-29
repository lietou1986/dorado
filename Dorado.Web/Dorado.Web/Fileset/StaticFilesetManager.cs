using Dorado.Web.Fileset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Dorado.Web.Fileset
{
    public static class StaticFilesetManager
    {
        public static class Consts
        {
            public const string GlobalSectionName = "PlatformStaticFileset";
            public const string ReferenceJavascript = "<script type=\"text/javascript\" src=\"{0}\" {1}></script>";
            public const string EmbedJavascript = "<script type=\"text/javascript\" {1}>{0}</script>";
            public const string ReferenceStyleSheet = "<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" {1}/>";
            public const string EmbedStyleSheet = "<style type=\"text/css\" {1}>{0}</style>";
        }

        public const string VarStringJs = "function vstr(str) {\r\n         if (typeof ($bs_vars) == 'undefine')\r\n             return str;\r\n         var result = str;\r\n         for (var v in $bs_vars) {\r\n             var regex = new RegExp('\\\\$\\\\{' + v.toString() + '\\\\}|\\\\{' + v.toString() + '\\\\}', 'igm');\r\n             result = result.replace(regex, $bs_vars[v]);\r\n         }\r\n         return result;\r\n     };";
        private static bool _enableMinify;
        public static object GlobalSyncRoot;
        public static object SyncRoot;
        private static bool _enableMinifierLogging;
        private static readonly StaticFileDictionary _files;
        private static readonly VariableDictionary _variables;
        private static bool _IsUseRemoteConfig = true;

        public static bool EnableMinify
        {
            get
            {
                Monitor.Enter(GlobalSyncRoot);
                try
                {
                    return _enableMinify;
                }
                finally
                {
                    Monitor.Exit(GlobalSyncRoot);
                }
            }
            set
            {
                Monitor.Enter(GlobalSyncRoot);
                try
                {
                    _enableMinify = value;
                }
                finally
                {
                    Monitor.Exit(GlobalSyncRoot);
                }
            }
        }

        public static bool EnableMinifierLogging
        {
            get
            {
                Monitor.Enter(GlobalSyncRoot);
                try
                {
                    return _enableMinifierLogging;
                }
                finally
                {
                    Monitor.Exit(GlobalSyncRoot);
                }
            }
            set
            {
                Monitor.Enter(GlobalSyncRoot);
                try
                {
                    _enableMinifierLogging = value;
                }
                finally
                {
                    Monitor.Exit(GlobalSyncRoot);
                }
            }
        }

        public static string VariableJavascriptCache
        {
            get;
            set;
        }

        public static StaticFileDictionary Files
        {
            get
            {
                return _files;
            }
        }

        public static VariableDictionary Variables
        {
            get
            {
                return _variables;
            }
        }

        public static FilesetConfiguration FilesetConfiguration
        {
            get { return _IsUseRemoteConfig ? FilesetRemoteConfigurationManager.Config : FilesetConfigurationManager.Config; }
        }

        static StaticFilesetManager()
        {
            _enableMinify = true;
            GlobalSyncRoot = new object();
            SyncRoot = new object();
            EnableMinify = true;
            EnableMinifierLogging = true;
            _files = new StaticFileDictionary(255);
            _variables = new VariableDictionary(128);

            ConfigChangedEvent(null, null);
            FilesetRemoteConfigurationManager.ConfigChangedEvent += ConfigChangedEvent;
        }

        public static void ConfigChangedEvent(object sender, EventArgs args)
        {
            UpdateVariables();
            UpdateFileset();
        }

        public static StaticFile GetStaticFile(string fileName)
        {
            StaticFile file = null;
            Files.TryGetValue(fileName, out file);
            return file;
        }

        public static string GetVariable(string varKey)
        {
            string str = null;
            Variables.TryGetValue(varKey, out str);
            return str;
        }

        public static string ReplaceVarible(string original)
        {
            string output = original;
            foreach (KeyValuePair<string, string> variable in _variables)
            {
                string reg = "\\$\\{" + variable.Key + "\\}|\\$" + variable.Key;
                output = Regex.Replace(output, reg, variable.Value, RegexOptions.IgnoreCase);
            }
            return output;
        }

        private static void UpdateVariables()
        {
            _variables.Clear();
            FilesetConfiguration.Variables = (FilesetConfiguration.Variables ?? new List<VariableItem>());
            FilesetConfiguration.Variables.ForEach(delegate(VariableItem v)
            {
                _variables.Add(v.VariableKey, v.VariableValue);
            }
            );
            StringBuilder jsCacheBuilder = new StringBuilder(255);
            jsCacheBuilder.Append("var $bs_vars={");
            int varCount = FilesetConfiguration.Variables.Count;
            int lastCount = varCount - 1;
            for (int i = 0; i < varCount; i++)
            {
                jsCacheBuilder.Append("'").Append(FilesetConfiguration.Variables[i].VariableKey).Append("':").Append("'").Append(FilesetConfiguration.Variables[i].VariableValue).Append("'");
                if (i != lastCount)
                {
                    jsCacheBuilder.Append(",");
                }
            }
            jsCacheBuilder.Append("};");
            VariableJavascriptCache = jsCacheBuilder.ToString();
        }

        private static void UpdateFileset()
        {
            _files.Clear();
            foreach (StaticFileset fileset in FilesetConfiguration.FilesetItems)
            {
                fileset.ConfigurationRoot = FilesetConfiguration;
                if (fileset.Enable)
                {
                    if (fileset.Implements != null && fileset.Implements.Length > 0)
                    {
                        StaticFileset[] impSets = GetImplements(fileset.Implements, FilesetConfiguration);
                        if (impSets != null && impSets.Length > 0)
                        {
                            StaticFileset[] array = impSets;
                            for (int i = 0; i < array.Length; i++)
                            {
                                StaticFileset impSet = array[i];
                                impSet.ConfigurationRoot = FilesetConfiguration;
                                StaticFile[] files = impSet.Files;
                                for (int j = 0; j < files.Length; j++)
                                {
                                    StaticFile impFile = files[j];
                                    impFile.Parent = impSet;
                                    impFile.Owner = _files;
                                    if (!_files.ContainsKey(impFile.Name))
                                    {
                                        _files.Add(impFile.Name, impFile);
                                    }
                                    else
                                    {
                                        _files[impFile.Name] = impFile;
                                    }
                                }
                            }
                        }
                    }
                    if (fileset.Files != null && fileset.Files.Length > 0)
                    {
                        StaticFile[] files2 = fileset.Files;
                        for (int k = 0; k < files2.Length; k++)
                        {
                            StaticFile file = files2[k];
                            file.Parent = fileset;
                            file.Owner = _files;
                            if (!_files.ContainsKey(file.Name))
                            {
                                _files.Add(file.Name, file);
                            }
                            else
                            {
                                _files[file.Name] = file;
                            }
                            file.ClearCache();
                        }
                    }
                }
            }
        }

        private static StaticFileset[] GetImplements(IEnumerable<string> imps, FilesetConfiguration config)
        {
            return (
                from item in config.FilesetItems
                let item1 = item
                where imps.Any((string imp) => imp.Equals(item1.Name, StringComparison.OrdinalIgnoreCase))
                select item).ToArray<StaticFileset>();
        }
    }
}