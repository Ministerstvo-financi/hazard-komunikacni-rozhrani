using System.Text.RegularExpressions;


namespace ValidationPilotServices.DataTypes
{
    public class IntegerNumber : BaseValidator
    {
        public IntegerNumber()
        {
            this.pattern = new Regex(@"(\-)?(\d){1,10}",RegexOptions.Compiled);
        }

        protected override bool Validate(string fieldValue)
        {
            bool flagValid = base.Validate(fieldValue);
            if (flagValid && !string.IsNullOrEmpty(fieldValue) && !int.TryParse(fieldValue, out int data))
            {
                this.ErrorMessage = $"The value {fieldValue} can't be convert to IntegerNumber format.";
                return false;
            }

            return flagValid;
        }
    }
}
