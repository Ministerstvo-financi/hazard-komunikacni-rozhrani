using System;

namespace ValidationPilotServices.DataTypes
{
    public class Text : BaseValidator
    {

        public Text(string minLen, string maxLen)
        {
            if (string.IsNullOrEmpty(minLen))
            {
                this.min_len = 0;
            }
            else
            {
                if (!uint.TryParse(minLen, out this.min_len))
                {
                    throw new ArgumentOutOfRangeException($"Minimal length parameter has invalid value.");
                }
            }

            if (string.IsNullOrEmpty(maxLen) || !uint.TryParse(maxLen, out this.max_len))
            {
                throw new ArgumentOutOfRangeException($"Maximal length parameter has invalid value.");
            }
        }

        protected override bool Validate(string fieldValue)
        {
            this.IsNullable = this.min_len == 0 ? true : false;
            return base.Validate(fieldValue);
        }
    }
}
