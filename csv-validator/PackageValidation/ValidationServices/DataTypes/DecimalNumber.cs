using System;
using System.Text.RegularExpressions;

namespace ValidationPilotServices.DataTypes
{
    public class DecimalNumber : BaseValidator
    {
        public DecimalNumber()
        {
            this.pattern = new Regex(@"^(\-)?\d{1,}(,\d{0,})?$", RegexOptions.Compiled);
        }

        public DecimalNumber(string DomainType):this()
        {
            if (string.IsNullOrEmpty(DomainType)){
                return;
            }

            switch (DomainType.ToUpper()){
                case "D_DECIMAL":
                    this.pattern = new Regex(@"^(\-)?\d{1,13}(,\d{0,2})?$", RegexOptions.Compiled);
                    break;
                case "D_GPS":
                    this.pattern = new Regex(@"^(\-)?\d{1,4},\d{4,7}$", RegexOptions.Compiled);
                    break;
                case "D_MONEY":
                    this.pattern = new Regex(@"^(\-)?\d{1,13}(,\d{0,2})?$", RegexOptions.Compiled);
                    break;
                case "D_COURSE":
                    this.pattern = new Regex(@"^(\-)?\d{1,18}(,\d{0,3})?$", RegexOptions.Compiled);
                    break;
                case "D_COURSE_EXTENDED":
                    this.pattern = new Regex(@"^(\-)?\d{1,}(,\d{0,})?$", RegexOptions.Compiled);
                    break;
            }
        }

        protected override bool Validate(string fieldValue)
        {
            bool flag = base.Validate(fieldValue);

            // if (flag && !string.IsNullOrEmpty(fieldValue) && !float.TryParse(fieldValue, out float data))
            // {
            //     this.ErrorMessage = $"The value {fieldValue} can't be convert to DecimalNumber format.";
            //     return false;
            // }

            return flag;
        }
    }
}
