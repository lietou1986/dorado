namespace Dorado.Data
{
    public class StoredProcedureParameter
    {
        private string key;
        private object value;
        private ParameterDirectionWrap parameterDirection;
        private int? size;

        public string Key
        {
            get
            {
                return this.key;
            }
            set
            {
                this.key = value;
            }
        }

        public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public ParameterDirectionWrap ParameterDirection
        {
            get
            {
                return this.parameterDirection;
            }
            set
            {
                this.parameterDirection = value;
            }
        }

        public int? Size
        {
            get
            {
                return this.size;
            }
            set
            {
                this.size = value;
            }
        }

        public StoredProcedureParameter(string key, object value, ParameterDirectionWrap parameterDirection, int size)
        {
            this.key = key;
            this.value = value;
            this.parameterDirection = parameterDirection;
            this.size = new int?(size);
        }

        public StoredProcedureParameter(string key, object value, ParameterDirectionWrap parameterDirection)
        {
            this.key = key;
            this.value = value;
            this.parameterDirection = parameterDirection;
            this.size = null;
        }

        public StoredProcedureParameter(object value, ParameterDirectionWrap parameterDirection, int size)
        {
            this.key = null;
            this.value = value;
            this.parameterDirection = parameterDirection;
            this.size = new int?(size);
        }

        public StoredProcedureParameter(object value, ParameterDirectionWrap parameterDirection)
        {
            this.key = null;
            this.value = value;
            this.parameterDirection = parameterDirection;
            this.size = null;
        }

        public StoredProcedureParameter(object value)
        {
            this.key = null;
            this.value = value;
            this.parameterDirection = ParameterDirectionWrap.Input;
            this.size = null;
        }
    }
}