#region Copyright Notice

// ----------------------------------------------------------------------------
// Copyright (C) 2006 Microsoft Corporation, All rights reserved.
// ----------------------------------------------------------------------------

// Author: Vipul Modi (vipul.modi@microsoft.com)

#endregion Copyright Notice

namespace Dorado.Wcf.DynamicProxy
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ServiceModel.Description;
    using System.Text;

    public class DynamicProxyException : ApplicationException
    {
        private IEnumerable<MetadataConversionError> importErrors = null;
        private IEnumerable<MetadataConversionError> codegenErrors = null;
        private IEnumerable<CompilerError> compilerErrors = null;

        public DynamicProxyException(string message)
            : base(message)
        {
        }

        public DynamicProxyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public IEnumerable<MetadataConversionError> MetadataImportErrors
        {
            get
            {
                return importErrors;
            }

            internal set
            {
                importErrors = value;
            }
        }

        public IEnumerable<MetadataConversionError> CodeGenerationErrors
        {
            get
            {
                return codegenErrors;
            }

            internal set
            {
                codegenErrors = value;
            }
        }

        public IEnumerable<CompilerError> CompilationErrors
        {
            get
            {
                return compilerErrors;
            }

            internal set
            {
                compilerErrors = value;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(base.ToString());

            if (MetadataImportErrors != null)
            {
                builder.AppendLine("Metadata Import Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(
                            MetadataImportErrors));
            }

            if (CodeGenerationErrors != null)
            {
                builder.AppendLine("Code Generation Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(
                            CodeGenerationErrors));
            }

            if (CompilationErrors != null)
            {
                builder.AppendLine("Compilation Errors:");
                builder.AppendLine(DynamicProxyFactory.ToString(
                            CompilationErrors));
            }

            return builder.ToString();
        }
    }
}