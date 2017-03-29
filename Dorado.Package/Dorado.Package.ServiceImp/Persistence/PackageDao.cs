using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Data;
using Dorado.Data.Extensions;
using Dorado.Package.ServiceInterface.Model;
using Dorado.Utils;
using System;
using System.Collections.Generic;

namespace Dorado.Package.ServiceImp.Persistence
{
    public sealed class PackageDao
    {
        private static Database GetDatabase()
        {
            return Database.GetDatabase("Message");
        }

        public static bool AddPackage(Guid packageNo, int productType, string packageType, string packageStruct, string packageNotice,
            int packageStatus, int packagePriority)
        {
            try
            {
                GetDatabase().ExecuteNonQuery(
                
                "dbo.AddPackage",
                packageNo,
                productType,
                packageType,
                packageStruct,
                packageNotice,
                packageStatus,
                packagePriority);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool DeletePackageByNo(Guid packageNo)
        {
            try
            {
                GetDatabase().ExecuteNonQuery(
               
               "[dbo].[DeletePackageByNo]",
               packageNo);
                return true;
            }
            catch (Exception e)
            {
                LoggerWrapper.Logger.Error("ReportEngineDao", e);
                return false;
            }
        }

        public static string GetPackageAddressByNo(Guid packageNo)
        {
            try
            {
                object obj = GetDatabase().ExecuteScalar( "[dbo].[GetPackageAddressByNo]",
                 delegate(IParameterSet parameterSet)
                 {
                     parameterSet.AddWithValue("@PackageNo", packageNo);
                 });
                if (obj == null || Convert.IsDBNull(obj))
                {
                    return string.Empty;
                }
                else
                {
                    return (string)obj;
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("PackageDao", ex);
                return string.Empty;
            }
        }

        public static PackageStatus GetPackageStatusByNo(Guid packageNo)
        {
            try
            {
                object obj = GetDatabase().ExecuteScalar( "[dbo].[GetPackageStatusByNo]",
                 delegate(IParameterSet parameterSet)
                 {
                     parameterSet.AddWithValue("@PackageNo", packageNo);
                 });
                if (obj == null || Convert.IsDBNull(obj))
                {
                    return PackageStatus.UnProcessed;
                }
                else
                {
                    return (PackageStatus)obj;
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("PackageDao", ex);
                return PackageStatus.Faulted;
            }
        }

        public static void UpdatePackageStatus(Guid packageNo, int packageStatus)
        {
            try
            {
                GetDatabase().ExecuteNonQuery(
               
               "dbo.UpdatePackageStatus",
               delegate(IParameterSet parameters)
               {
                   parameters.AddWithValue("@PackageNo", packageNo);
                   parameters.AddWithValue("@PackageStatus", packageStatus);
               },
                 null);
            }
            catch (Exception e)
            {
                LoggerWrapper.Logger.Error("PackageDao", e);
            }
        }

        public static void UpdatePackageDataByNo(Guid packageNo, string packageAddress)
        {
            //update status
            //update completedtime
            try
            {
                GetDatabase().ExecuteNonQuery(
          
          "dbo.UpdatePackageDataByNo",
          delegate(IParameterSet parameters)
          {
              parameters.AddWithValue("@PackageNo", packageNo);
              parameters.AddWithValue("@PackageAddress", packageAddress);
          },
            null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static List<PackageInfo> GetPackageTask(int packagePriority)
        {
            try
            {
                return GetDatabase().ExecuteAndGetInstanceList<PackageInfo>( "[dbo].[GetPackageTask]",
               delegate(IParameterSet parameterSet)
               {
                   parameterSet.AddWithValue("@PackagePriority", packagePriority);
               },
               delegate(IRecord record, PackageInfo context)
               {
                   context.PackageNo = record.Get<Guid>("PackageNo").ToString();
                   string tmpXml = record.GetOrDefault<String>("PackageNotice", "");
                   if (!string.IsNullOrEmpty(tmpXml))
                       context.PackageNotice = SerializeUtility.DeserializeIt<PackageNotice>(tmpXml, typeof(PackageNotice));
                   context.PackagePriority = (PackagePriority)record.Get<int>("PackagePriority");
                   context.PackageStruct = SerializeUtility.DeserializeIt<PackageStruct>(record.Get<String>("PackageStruct"),
                                                               typeof(PackageStruct));
                   context.ProductType = record.Get<int>("ProductType");
                   context.PackageType = record.Get<string>("PackageType");
               });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static PackageInfo GetPackageDataByNo(Guid packageNo)
        {
            try
            {
                PackageInfo packageInfo = GetDatabase().ExecuteAndGetInstance<PackageInfo>( "[dbo].[GetPackageDataByNo]",
               delegate(IParameterSet parameterSet)
               {
                   parameterSet.AddWithValue("@PackageNo", packageNo);
               },
               delegate(IRecord record, PackageInfo context)
               {
                   context.PackageNo = record.Get<Guid>("PackageNo").ToString();
                   string tmpXml = record.GetOrDefault<String>("PackageNotice", "");
                   if (!string.IsNullOrEmpty(tmpXml))
                       context.PackageNotice = SerializeUtility.DeserializeIt<PackageNotice>(tmpXml, typeof(PackageNotice));
                   context.PackagePriority = (PackagePriority)record.Get<int>("PackagePriority");
                   context.PackageStruct = SerializeUtility.DeserializeIt<PackageStruct>(record.Get<String>("PackageStruct"),
                                                               typeof(PackageStruct));
                   context.ProductType = record.Get<int>("ProductType");
                   context.PackageType = record.Get<string>("PackageType");
               });
                return packageInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}