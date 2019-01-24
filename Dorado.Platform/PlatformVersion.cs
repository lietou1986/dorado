using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dorado.Platform
{
    public static class PlatformVersion
    {
        private static readonly Version _infoVersion = new Version("1.0.0.0");

        private static readonly List<Version> _breakingChangesHistory = new List<Version>
        {
            // IMPORTANT: Add app versions from low to high
            // NOTE: do not specify build & revision unless you have good reasons for it.
            //       A release with breaking changes should definitely have at least
            //       a greater minor version.
            new Version("2.0"),
        };

        static PlatformVersion()
        {
            _breakingChangesHistory.Reverse();

            // get informational version
            var infoVersionAttr = Assembly.GetExecutingAssembly().GetAttribute<AssemblyInformationalVersionAttribute>(false);
            if (infoVersionAttr != null)
            {
                _infoVersion = new Version(infoVersionAttr.InformationalVersion);
            }
        }

        /// <summary>
        /// Gets the app version
        /// </summary>
        public static string CurrentVersion
        {
            get
            {
                return "{0}.{1}".FormatInvariant(_infoVersion.Major, _infoVersion.Minor);
            }
        }

        /// <summary>
        /// Gets the app full version
        /// </summary>
        public static string CurrentFullVersion
        {
            get
            {
                return _infoVersion.ToString();
            }
        }

        public static Version Version
        {
            get
            {
                return _infoVersion;
            }
        }

        /// <summary>
        /// Gets a list of Dorado.Platform.NET versions in which breaking changes occured,
        /// which could lead to plugins malfunctioning.
        /// </summary>
        /// <remarks>
        /// A plugin's <c>MinAppVersion</c> is checked against this list to assume
        /// it's compatibility with the current app version.
        /// </remarks>
        internal static IEnumerable<Version> BreakingChangesHistory
        {
            get
            {
                return _breakingChangesHistory.AsEnumerable();
            }
        }
    }
}