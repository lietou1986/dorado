using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Package.Component.Base;
using Dorado.Package.ServiceImp.Persistence;
using Dorado.Package.ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Dorado.Package.ServiceImp
{
    internal abstract class BasePackageDispatcher : IDisposable
    {
        protected bool isStopped = false;

        protected List<PackageInfo> ListPackageInfo;

        public BasePackageDispatcher()
        {
            try
            {
                InitBaseDirectories();
                RegisterConfigChanged();
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("PackageDispatcher", ex);
            }
        }

        private void InitBaseDirectories()
        {
            try
            {
                if (!Directory.Exists(PackageSettings.Instance.PackagePath))
                    Directory.CreateDirectory(PackageSettings.Instance.PackagePath);

                if (!Directory.Exists(PackageSettings.Instance.PackageTempPath))
                    Directory.CreateDirectory(PackageSettings.Instance.PackageTempPath);
            }
            catch (ArgumentNullException anEx)
            {
                LoggerWrapper.Logger.Error("PackageDispatcher", anEx);
                throw anEx;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("PackageDispatcher", ex);
                throw ex;
            }
        }

        public void Running()
        {
            LoggerWrapper.Logger.Info("package service begin...");
            isStopped = false;
            Package();
        }

        public void Stop()
        {
            isStopped = true;
            LoggerWrapper.Logger.Info("package service stop...");
        }

        private void Package()
        {
            while (true)
            {
                if (!isStopped)
                {
                    try
                    {
                        AssignTasks();
                        TasksIntervalSleep();
                    }
                    catch (Exception ex)
                    {
                        LoggerWrapper.Logger.Error("BasePackageDispatcher", ex);
                    }
                }
            }
        }

        private void TasksIntervalSleep()
        {
            Thread.Sleep((int)PackageSettings.Instance.TaskInterval);
        }

        private void DoPackage()
        {
            foreach (PackageInfo packageInfo in ListPackageInfo)
            {
                try
                {
                    PackageDao.UpdatePackageStatus(new Guid(packageInfo.PackageNo), 1);
                    BasePackage basePackage = GeneratePackageInstance(packageInfo.PackageType.ToLower(), packageInfo.PackageStruct, packageInfo.PackageNotice);
                    basePackage.Package();
                    PackageDao.UpdatePackageDataByNo(new Guid(packageInfo.PackageNo), basePackage.ZipFileFullPath);
                    PackageDao.UpdatePackageStatus(new Guid(packageInfo.PackageNo), 2);
                }
                catch (Exception ex)
                {
                    PackageDao.UpdatePackageStatus(new Guid(packageInfo.PackageNo), 3);
                    LoggerWrapper.Logger.Error("PackageDispatcher", ex);
                }
            }
        }

        protected abstract BasePackage GeneratePackageInstance(string packageType, PackageStruct packageStruct,
                                                               PackageNotice packageNotice);

        protected virtual void AssignTasks()
        {
            foreach (int packagePriority in Enum.GetValues(typeof(PackagePriority)).Cast<int>().Reverse())
            {
                this.ListPackageInfo = PackageDao.GetPackageTask(packagePriority);
                if (this.ListPackageInfo.Count > 0)
                {
                    DoPackage();
                }
            }
        }

        protected virtual void PackageSettings_ConfigChanged(object sender, EventArgs e)
        {
            LoggerWrapper.Logger.Info("Configuration file changes, service re-initialization");
        }

        private void RegisterConfigChanged()
        {
            PackageSettings.ConfigChanged += new EventHandler(PackageSettings_ConfigChanged);
        }

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Members
    }
}