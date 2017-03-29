using System;

namespace Dorado.Package.ServiceInterface.Model
{
    public class PackBizException : ApplicationException
    {
        public PackBizException(string message)
            : base(message)
        {
        }

        public PackBizException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class PackageException : ApplicationException
    {
        public PackageException(string message)
            : base(message)
        {
        }

        public PackageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class PackageStructParamException : ApplicationException
    {
        public PackageStructParamException(string message)
            : base(message)
        {
        }

        public PackageStructParamException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class PackageComponentInstanceException : ApplicationException
    {
        public PackageComponentInstanceException(string message)
            : base(message)
        {
        }

        public PackageComponentInstanceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class PackageConfigurationException : ApplicationException
    {
        public PackageConfigurationException(string message)
            : base(message)
        {
        }

        public PackageConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class PackageNoticeException : ApplicationException
    {
        public PackageNoticeException(string message)
            : base(message)
        {
        }

        public PackageNoticeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}