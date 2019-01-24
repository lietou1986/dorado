using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Extensions;
using Dorado.Platform.Plugins;
using NuGet;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Dorado.Platform.Packaging
{
    public sealed class AppUpdater : DisposableObject
    {
        public const string UpdatePackagePath = "~/App_Data/Update";

        private static readonly RwLocker _rwLock = new RwLocker();

        #region Package update

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public bool InstallablePackageExists()
        {
            string packagePath = null;
            var package = FindPackage(false, out packagePath);

            if (package == null)
                return false;

            if (!ValidatePackage(package))
                return false;

            if (!CheckEnvironment())
                return false;

            return true;
        }

        internal bool TryUpdateFromPackage()
        {
            // NEVER EVER (!!!) make an attempt to auto-update in a dev environment!!!!!!!
            if (CommonHelper.IsDevEnvironment)
                return false;

            using (_rwLock.Upgrade())
            {
                try
                {
                    string packagePath = null;
                    var package = FindPackage(true, out packagePath);

                    if (package == null)
                        return false;

                    if (!ValidatePackage(package))
                        return false;

                    if (!CheckEnvironment())
                        return false;

                    using (_rwLock.Write())
                    {
                        Backup();

                        var info = ExecuteUpdate(package);

                        if (info != null)
                        {
                            var newPath = packagePath + ".applied";
                            if (File.Exists(newPath))
                            {
                                File.Delete(packagePath);
                            }
                            else
                            {
                                File.Move(packagePath, newPath);
                            }
                        }

                        return info != null;
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("An error occured while updating the application: {0}".FormatCurrent(ex.Message), ex);
                    return false;
                }
            }
        }

        private IPackage FindPackage(bool createLogger, out string path)
        {
            path = null;
            var dir = CommonHelper.MapPath(UpdatePackagePath, false);

            if (!Directory.Exists(dir))
                return null;

            var files = Directory.GetFiles(dir, "Dorado.Platform.*.nupkg", SearchOption.TopDirectoryOnly);

            // TODO: allow more than one package in folder and return newest
            if (files.Length == 0 || files.Length > 1)
                return null;

            try
            {
                path = files[0];
                IPackage package = new ZipPackage(files[0]);
                if (createLogger)
                {
                    LoggerWrapper.Logger.Info("Found update package '{0}'".FormatInvariant(package.GetFullName()));
                }
                return package;
            }
            catch { }

            return null;
        }

        private bool ValidatePackage(IPackage package)
        {
            if (package.Id != "DoradoPlatform")
                return false;

            var currentVersion = new SemanticVersion(PlatformVersion.Version);
            return package.Version > currentVersion;
        }

        private bool CheckEnvironment()
        {
            // TODO: Check it :-)
            return true;
        }

        private void Backup()
        {
            var source = new DirectoryInfo(CommonHelper.MapPath("~/"));

            var tempPath = CommonHelper.MapPath("~/App_Data/_Backup/App/DoradoPlatform");
            string localTempPath = null;
            for (int i = 0; i < 50; i++)
            {
                localTempPath = tempPath + (i == 0 ? "" : "." + i.ToString());
                if (!Directory.Exists(localTempPath))
                {
                    Directory.CreateDirectory(localTempPath);
                    break;
                }
                localTempPath = null;
            }

            if (localTempPath == null)
            {
                var exception = new CoreException("Too many backups in '{0}'.".FormatInvariant(tempPath));
                LoggerWrapper.Logger.Error(exception.Message, exception);
                throw exception;
            }

            var backupFolder = new DirectoryInfo(localTempPath);
            var folderUpdater = new FolderUpdater();
            folderUpdater.Backup(source, backupFolder, "App_Data", "Media");

            LoggerWrapper.Logger.Info("Backup successfully created in folder '{0}'.".FormatInvariant(localTempPath));
        }

        private PackageInfo ExecuteUpdate(IPackage package)
        {
            var appPath = CommonHelper.MapPath("~/");

            var logger = new NugetLogger();

            var project = new FileBasedProjectSystem(appPath) { Logger = logger };

            var nullRepository = new NullSourceRepository();

            var projectManager = new ProjectManager(
                nullRepository,
                new DefaultPackagePathResolver(appPath),
                project,
                nullRepository
                )
            { Logger = logger };

            // Perform the update
            projectManager.AddPackageReference(package, true, false);

            var info = new PackageInfo
            {
                Id = package.Id,
                Name = package.Title ?? package.Id,
                Version = package.Version.ToString(),
                Type = "App",
                Path = appPath
            };

            LoggerWrapper.Logger.Info("Update '{0}' successfully executed.".FormatInvariant(info.Name));

            return info;
        }

        #endregion Package update

        #region Migrations

        internal void ExecuteMigrations()
        {
            if (!PlatformSettings.DatabaseIsInstalled())
                return;

            var currentVersion = PlatformVersion.Version;
            var prevVersion = PlatformSettings.Current.AppVersion ?? new Version(1, 0);

            if (prevVersion >= currentVersion)
                return;

            if (prevVersion < new Version(2, 1))
            {
                // we introduced app migrations in V2.1. So any version prior 2.1
                // has to perform the initial migration
                MigrateInitial();
            }

            PlatformSettings.Current.AppVersion = currentVersion;
            PlatformSettings.Current.Save();
        }

        private void MigrateInitial()
        {
            var installedPlugins = PluginFileParser.ParseInstalledPluginsFile();
            if (installedPlugins.Count == 0)
                return;

            var renamedPlugins = new List<string>();

            var pluginRenameMap = new Dictionary<string, string>
            {
                { "Test", null /* null means: remove it */ },
            };

            foreach (var name in installedPlugins)
            {
                if (pluginRenameMap.ContainsKey(name))
                {
                    string newName = pluginRenameMap[name];
                    if (newName != null && !renamedPlugins.Contains(newName))
                    {
                        renamedPlugins.Add(newName);
                    }
                }
                else
                {
                    renamedPlugins.Add(name);
                }
            }

            PluginFileParser.SaveInstalledPluginsFile(renamedPlugins);
        }

        #endregion Migrations

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}