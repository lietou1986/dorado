using System;

namespace Dorado.DataExpress.Utility
{
    public class ConvertFailedException : Exception
    {
        private string _errorMessage = string.Empty;
        private string _originalValue = string.Empty;
        private Type _targetType;

        public string OriginalValue
        {
            get
            {
                return this._originalValue;
            }
            set
            {
                this._originalValue = value;
            }
        }

        public Type TargetType
        {
            get
            {
                return this._targetType;
            }
            set
            {
                this._targetType = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return this._errorMessage;
            }
            set
            {
                this._errorMessage = value;
            }
        }

        public ConvertFailedException(Type t, string message, string original)
        {
            this.OriginalValue = original;
            this._errorMessage = message;
            this.TargetType = t;
        }
    }
}