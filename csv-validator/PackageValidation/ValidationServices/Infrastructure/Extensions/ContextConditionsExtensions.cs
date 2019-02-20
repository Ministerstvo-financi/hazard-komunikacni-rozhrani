using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ValidationPilotServices.ConfigService;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.Infrastructure.Attributes;
using ValidationPilotServices.Infrastructure.Conditions;

namespace ValidationPilotServices.Infrastructure.Extensions
{
    public static class ContextConditionsExtensions
    {
        [ContextCondition(MethodToProcess = "EQUALSOPERATORID")]
        [ContextCondition(MethodToProcess = "EQUALSGAMETYPE")]
        [ContextCondition(MethodToProcess = "EQUALSIDLOCATION")]
        public static bool FieldEquals(object valueToValidate, object valueValidateWith)
        {
            return valueValidateWith.ToString().Equals(valueToValidate.ToString());
        }

        [ContextCondition(MethodToProcess = "STARTSWITHOPERATORID")]
        public static bool FieldStartsWith(object valueToValidate, object valueValidateWith)
        {
            return valueToValidate.ToString().StartsWith(valueValidateWith.ToString());
        }

        //withinPackageTimespan
        [ContextCondition(MethodToProcess = "WITHINPACKAGETIMESPAN")]
        public static bool FieldWithinPackageTimespan(object valueToValidate, object valueValidateWith)
        {
            return true;
            //FIXME - fix implementation
            // if (valueToValidate == null || valueToValidate.GetType() != typeof(DateTime))
            // {
            //     return false;
            // }

            // string target = valueValidateWith.ToString().Trim();
            // string value = target.Length == ConfigSettings.remote_reporting_period_parameter_format.Length
            //     ? ((DateTime) valueToValidate).ToString(ConfigSettings.remote_reporting_period_parameter_format)
            //     : ((DateTime) valueToValidate).ToString(ConfigSettings.daily_reporting_period_parameter_format);

            // return value.Equals(target);
        }

        [ContextCondition(MethodToProcess = "DECIMALSNUMBERSGREATE")]
        public static bool FieldDigitsNumbersGreaterThen(object valueToValidate, object valueValidateWith)
        {
            string source = valueToValidate == null ? string.Empty : valueToValidate.ToString();

            if (string.IsNullOrEmpty(source))
                //means this value already validated by nullable property.
                return true;

            int numbLen = (int)valueValidateWith;
            int position = source.IndexOf(',');

            if (position == -1)
            {
                return false;
            }

            string value = source.Substring(position + 1);

            if (string.IsNullOrEmpty(value) || value.Length < numbLen)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// This extension defines the function to validate parameter by context condition.
        /// </summary>
        /// <param name="item">The field structure defined.</param>
        public static void GetContextConditionMethodDefine(this FieldItem item)
        {
            if (string.IsNullOrEmpty(item.ContextConditionParameter))
            {
                return;
            }

            string fn = IsPredefinedConditionExpression(item.ContextConditionParameter)
                ? GetPredefinedConditionFunction(item.ContextConditionParameter).ToUpper()
                : item.ContextConditionParameter.ToUpper();

            MethodInfo method = typeof(ContextConditionsExtensions)
                .GetMethods()
                .FirstOrDefault(p => p.GetCustomAttributes<ContextConditionAttribute>()
                                         .FirstOrDefault(a => a.MethodToProcess.Equals(fn)) != null);

            if (method == null)
            {
                throw new ArgumentOutOfRangeException(
                    $"The Context condition {item.ContextConditionParameter} is not defined for the field {item.FieldName}.");
            }

            item.ContextCondition = (ContextConditionValidationsHandler)Delegate.CreateDelegate(typeof(ContextConditionValidationsHandler), null, method);
        }

        //TODO: hardcoded part - I thing it's fine, because argument is predefined. In this case we don't need to define any property to get value.
        public static object GetContextConditionParameter(this PackageReaderService source, string contextCondition)
        {
            if (string.IsNullOrEmpty(contextCondition))
            {
                return null;
            }

            //if the predefined expression like DigitsNumbers(3), Len(20)
            if (IsPredefinedConditionExpression(contextCondition))
            {
                int param = GetPredefinedConditionArgument(contextCondition);
                if (param == -1)
                {
                   return null;
                }

                return param;
            }

            //other cases
            PropertyInfo pi = GetPropertyForCondition(source, contextCondition);

            if (pi == null)
            {
                return null;
            }

            return pi.GetValue(source);
        }

        public static string GetContextValidationErrorMessageTemplate(this PackageReaderService source, string contextCondition)
        {
            if (string.IsNullOrEmpty(contextCondition))
            {
                return string.Empty;
            }

            PropertyInfo pi = GetPropertyForCondition(source, contextCondition);
            ContextConditionAttribute attribute = pi
                .GetCustomAttributes<ContextConditionAttribute>()
                .FirstOrDefault(p => p.MethodToProcess.Equals(contextCondition.ToUpper()));

            if (attribute == null)
            {
                return string.Empty;
            }

            return attribute.ConditionErrorMessageTemplate;
        }

        private static PropertyInfo GetPropertyForCondition(PackageReaderService source, string contextCondition)
        {
            return source.GetType()
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes<ContextConditionAttribute>()
                                         .FirstOrDefault(a => a.MethodToProcess.Equals(contextCondition.ToUpper())) != null);

        }

        private static Match MatchPredefinedExpression(string contextCondition)
        {
            Regex regex = new Regex(ConfigSettings.predefined_expression_regex, RegexOptions.Compiled);
            return regex.Match(contextCondition, 0, contextCondition.Length);
        }

        public static bool IsPredefinedConditionExpression(string contextCondition)
        {
            return MatchPredefinedExpression(contextCondition).Success;
        }

        public static int GetPredefinedConditionArgument(string contextCondition)
        {
            Match match = MatchPredefinedExpression(contextCondition);
            if (!match.Success || match.Groups.Count != 3)
            {
                return -1;
            }

            return int.Parse(match.Groups[2].ToString());
        }

        public static string GetPredefinedConditionFunction(string contextCondition)
        {
            Match match = MatchPredefinedExpression(contextCondition);
            if (!match.Success || match.Groups.Count != 3)
            {
                return string.Empty;
            }

            return match.Groups[1].ToString();
        }

    }
}

