using System.Text.RegularExpressions;
using ValidationPilotServices.Infrastructure.Enums;

namespace ValidationPilotServices.DataTypes
{
    public abstract class BaseValidator
    {

        protected Regex pattern = null;
        protected string value = string.Empty;
        protected uint min_len = 0;
        protected uint max_len = 0;

        public virtual bool IsValid { get; protected set; }

        public virtual bool IsNullable { get; set; }

        public virtual string ErrorMessage { get; protected set; }
        public virtual EnumValidationResult ErrorCode { get; protected set; }

        public virtual string Value
        {
            get => this.value;
            set
            {
                string valueOperate = string.IsNullOrEmpty(value) ? value : value.Trim();
                this.IsValid = this.Validate(valueOperate);
                this.value = this.IsValid ? valueOperate : string.Empty;
            }
        }

        protected virtual bool Validate(string fieldValue)
        {
            this.ErrorCode = EnumValidationResult.ERR_FIELD_DOMAIN_TYPE;

            if (string.IsNullOrEmpty(fieldValue) && this.IsNullable)
            {
                return true;
            }

            if (string.IsNullOrEmpty(fieldValue) && !this.IsNullable)
            {
                this.ErrorCode =EnumValidationResult.ERR_FIELD_NOT_NULL;
                this.ErrorMessage = $"The field must have a value. Currently this field is empty.";
                return false;
            }

            if (this.pattern==null)
            {
                return true;
            }

            Regex regex = this.pattern;
            if (fieldValue != null)
            {
                Match match = regex.Match(fieldValue, 0, fieldValue.Length);
                bool flag = match.Success && match.Value.Equals(fieldValue);
                if (!flag)
                {
                    this.ErrorCode = EnumValidationResult.ERR_FIELD_BAD_FORMAT_RE;
                    this.ErrorMessage = $"The field has the invalid value: {fieldValue} - regex: {regex.ToString()}.";
                }

                return flag;
            }
            else
            {
                this.ErrorCode =EnumValidationResult.ERR_FIELD_NOT_NULL;
                this.ErrorMessage = $"The field must have a value. Currently this field is empty.";
                return false;
            }
        }
    }
}
