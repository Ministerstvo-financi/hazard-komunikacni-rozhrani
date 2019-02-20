using System;
using System.Text.RegularExpressions;

namespace ValidationPilotServices.DataTypes
{
    public class DateAndTime : BaseValidator
    {
        public DateAndTime()
        {
            string yearPattern = @"(\d\d\d\d)";
            string monthPattern = @"(01|02|03|04|05|06|07|08|09|10|11|12)";
            string dayPattern = @"(0[1-9]|[12][0-9]|3[01])";
            string hourPattern = @"([01][0-9]|2[0123])";
            string minutePattern = @"([0-5][0-9])";
            string secondPattern = @"(([0-5][0-9])|(60))";
            string fractionPattern = @"(\.[0-9])";
            string offsetPattern = $@"(([+-]{hourPattern}:{minutePattern})|Z)";

            string dateTimePattern = $"^{yearPattern}-{monthPattern}-{dayPattern}T{hourPattern}:{minutePattern}:{secondPattern}{fractionPattern}{offsetPattern}$";

            this.pattern = new Regex(dateTimePattern, RegexOptions.Compiled);
        }

        public DateAndTime(string regexp)
        {

            this.pattern = new Regex(regexp ?? throw new ArgumentNullException($"DateAndTime type must has a regex pattern defined."),RegexOptions.Compiled);
        }

        protected override bool Validate(string fieldValue)
        {
            bool validBase = base.Validate(fieldValue);

            if (!validBase) return false;

            if (!string.IsNullOrEmpty(fieldValue) && !DateTime.TryParse(fieldValue, out DateTime date))
            {
                this.ErrorMessage =
                    $"The Value: {fieldValue} is invalid. It's impossible to convert it to DateTime format.";
                return false;
            }

            return true;
        }
    }
}