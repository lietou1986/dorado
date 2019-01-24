using Dorado.Core;
using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Dorado.Platform
{
    public partial class PlatformSettings
    {
        private static readonly ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        private static PlatformSettings _current = null;
        private static Func<PlatformSettings> _settingsFactory = new Func<PlatformSettings>(() => new PlatformSettings());
        private static bool? _installed = null;
        private static bool _testMode = false;

        protected const char SEPARATOR = ':';
        protected const string FILENAME = "Settings.txt";

        private PlatformSettings()
        {
            RawDataSettings = new Dictionary<string, string>();
        }

        #region Static members

        public static void SetDefaultFactory(Func<PlatformSettings> factory)
        {
            Guard.ArgumentNotNull(() => factory);

            lock (rwLock.Write())
            {
                _settingsFactory = factory;
            }
        }

        public static PlatformSettings Current
        {
            get
            {
                using (rwLock.Upgrade())
                {
                    if (_current == null)
                    {
                        using (rwLock.Write())
                        {
                            if (_current == null)
                            {
                                _current = _settingsFactory();
                                _current.Load();
                            }
                        }
                    }
                }

                return _current;
            }
        }

        public static bool DatabaseIsInstalled()
        {
            if (_testMode)
                return false;

            if (!_installed.HasValue)
            {
                _installed = Current.IsValid();
            }

            return _installed.Value;
        }

        internal static void SetTestMode(bool isTestMode)
        {
            _testMode = isTestMode;
        }

        public static void Reload()
        {
            using (rwLock.Write())
            {
                _current = null;
                _installed = null;
            }
        }

        public static void Delete()
        {
            using (rwLock.Write())
            {
                string filePath = Path.Combine(CommonHelper.MapPath("~/App_Data/"), FILENAME);
                File.Delete(filePath);
                _current = null;
                _installed = null;
            }
        }

        #endregion Static members

        #region Instance members

        public Version AppVersion
        {
            get;
            set;
        }

        public IDictionary<string, string> RawDataSettings
        {
            get;
            private set;
        }

        public bool IsValid()
        {
            return true;
        }

        public virtual bool Load()
        {
            using (rwLock.Write())
            {
                string filePath = Path.Combine(CommonHelper.MapPath("~/App_Data/"), FILENAME);

                this.Reset();

                if (File.Exists(filePath))
                {
                    string text = File.ReadAllText(filePath);
                    var settings = ParseSettings(text);
                    if (settings.Any())
                    {
                        this.RawDataSettings.AddRange(settings);
                        if (settings.ContainsKey("AppVersion"))
                        {
                            this.AppVersion = new Version(settings["AppVersion"]);
                        }

                        return this.IsValid();
                    }
                }

                return false;
            }
        }

        public void Reset()
        {
            using (rwLock.Write())
            {
                this.RawDataSettings.Clear();
                this.AppVersion = null;

                _installed = null;
            }
        }

        public virtual bool Save()
        {
            if (!this.IsValid())
                return false;

            using (rwLock.Write())
            {
                string filePath = Path.Combine(CommonHelper.MapPath("~/App_Data/"), FILENAME);
                if (!File.Exists(filePath))
                {
                    using (File.Create(filePath))
                    {
                        // we use 'using' to close the file after it's created
                    }
                }

                var text = SerializeSettings();
                File.WriteAllText(filePath, text);

                return true;
            }
        }

        #endregion Instance members

        #region Instance helpers

        protected virtual IDictionary<string, string> ParseSettings(string text)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (text.IsEmpty())
                return result;

            var settings = new List<string>();
            using (var reader = new StringReader(text))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                    settings.Add(str);
            }

            foreach (var setting in settings)
            {
                var separatorIndex = setting.IndexOf(SEPARATOR);
                if (separatorIndex == -1)
                {
                    continue;
                }
                string key = setting.Substring(0, separatorIndex).Trim();
                string value = setting.Substring(separatorIndex + 1).Trim();

                if (key.HasValue() && value.HasValue())
                {
                    result.Add(key, value);
                }
            }

            return result;
        }

        protected virtual string SerializeSettings()
        {
            return string.Format("AppVersion: {0}{1}",
                this.AppVersion.ToString(),
                Environment.NewLine);
        }

        #endregion Instance helpers
    }
}