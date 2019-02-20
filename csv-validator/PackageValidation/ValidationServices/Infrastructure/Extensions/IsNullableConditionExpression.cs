using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;

namespace ValidationPilotServices.Infrastructure.Extensions
{
    /// <summary>
    /// Expression Collection:
    ///     @FieldName.IsEmpty()          
    ///     @FieldName.NotEmpty()
    ///     @FieldName.InRange(value1, value2,...)
    ///     @FieldName.NotInRange(value1, value2,...)
    /// </summary>
    public static class IsNullableConditionExpression
    {
        private static readonly string FIELD_IS_EMPTY_EXPRESSION = "IsEmpty";
        private static readonly string FIELD_NOT_EMPTY_EXPRESSION = "NotEmpty";
        private static readonly string FIELD_IN_RANGE_EXPRESSION = "InRange";
        private static readonly string FIELD_NOT_IN_RANGE_EXPRESSION = "NotInRange";

        private static readonly string FIELD_IS_EMPTY_EXPRESSION_TEMPLATE = @"([A-Za-z0-9]{1,}).(IsEmpty\(\))";
        private static readonly string FIELD_NOT_EMPTY_EXPRESSION_TEMPLATE = @"([A-Za-z0-9]{1,}).(NotEmpty\(\))";
        private static readonly string FIELD_IN_RANGE_EXPRESSION_TEMPLATE = @"([A-Za-z0-9]{1,}).InRange(\(((\w|,?)*)\))";
        private static readonly string FIELD_NOT_IN_RANGE_EXPRESSION_TEMPLATE = @"([A-Za-z0-9]{1,}).NotInRange(\(((\w|,?)*)\))";
        
        /// <summary>
        /// This function returns template to parse expression.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static string GetRegTemplate(string expression)
        {
            if (expression.Contains(FIELD_IS_EMPTY_EXPRESSION))
            {
                return FIELD_IS_EMPTY_EXPRESSION_TEMPLATE;
            }
            else if (expression.Contains(FIELD_NOT_EMPTY_EXPRESSION))
            {
                return FIELD_NOT_EMPTY_EXPRESSION_TEMPLATE;
            }
            else if (expression.Contains(FIELD_IN_RANGE_EXPRESSION))
            {
                return FIELD_IN_RANGE_EXPRESSION_TEMPLATE;
            }
            else if (expression.Contains(FIELD_NOT_IN_RANGE_EXPRESSION))
            {
                return FIELD_NOT_IN_RANGE_EXPRESSION_TEMPLATE;
            }

            LoggerService.LoggerService.GetGlobalLog().Warn($"There is no any expressions found for {expression}");
            throw new ArgumentException($"There is no any expressions found for {expression}");
        }

        private static GroupCollection ParseExpression(string expression, string template, int groupsCountCompulsory = 2)
        {
            Regex regex = new Regex(template, RegexOptions.Compiled);
            Match match = regex.Match(expression, 0, expression.Length);

            if (match.Groups.Count < groupsCountCompulsory || string.IsNullOrEmpty(match.Groups[1].Value))
            {
                LoggerService.LoggerService.GetGlobalLog().Warn($"The expression {expression} for the field has been parsed with error.");
                throw new ArgumentException(
                    $"The expression {expression} for the field has been parsed with error.");
            }

            return match.Groups;
        }

        private static object GetRequestedFieldValue(string fieldName, ExpandoObject source)
        {
            var obj = (IDictionary<string, object>)source;
            if (!obj.ContainsKey(fieldName))
            {
                LoggerService.LoggerService.GetGlobalLog().Warn($"The Source Row doesn't contain field {fieldName}");
                throw new ArgumentException($"The Source Row doesn't contain field {fieldName}");
            }

            return obj[fieldName];
        }

        private static string[] GetCollectionArray(string source, string fieldName)
        {
            string[] array = source.Split(",");
            if (array == null || array.Length == 0)
            {
                throw new ArgumentNullException($"There is no any data to validate field {fieldName} are presented.");
            }

            return array;
        }

        private static bool ValueInRange(string rangeSource, object value, string fieldName)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                LoggerService.LoggerService.GetGlobalLog().Warn($"The value to validate {fieldName} is not presented.");
                throw new ArgumentException($"The value to validate {fieldName} is not presented.");
            }

            string[] array = GetCollectionArray(rangeSource, fieldName);
            return array.Contains(value.ToString());
        }
      
        /// <summary>
        /// This function returns TRUE if the expression in <see cref="FieldItem.IsNullableCondition"/> property
        /// of the <para>field</para> is not empty, parsed and executed correctly.
        /// If function returns FALSE the <para>errorMessage</para> contains of an explanation of the error.
        /// </summary>
        /// <param name="field">The field contained expression to parse</param>
        /// <param name="source"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool GetNullableFieldDefinition(this FieldItem field, ExpandoObject source,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(field.IsNullableCondition))
                {
                    LoggerService.LoggerService.GetGlobalLog().Warn( $"The field {field.FieldName} contains an expression reference, however the expression is empty.");
                    throw new ArgumentException(
                        $"The field {field.FieldName} contains an expression reference, however the expression is empty.");
                }

                string template = GetRegTemplate(field.IsNullableCondition);
                GroupCollection groups = ParseExpression(field.IsNullableCondition, template);

                string fieldToValidate = groups[1].Value;
                object fieldToValidateValue = GetRequestedFieldValue(fieldToValidate, source);

                if (template.Equals(FIELD_IS_EMPTY_EXPRESSION_TEMPLATE))
                {
                    //The field is mandatory if the expression is empty
                    if (fieldToValidateValue == null || string.IsNullOrEmpty(fieldToValidateValue.ToString()))
                    {
                        field.FieldType.IsNullable = false;
                    }
                    else
                    {
                        field.FieldType.IsNullable = true;
                    }
                }
                else if (template.Equals(FIELD_NOT_EMPTY_EXPRESSION_TEMPLATE))
                {
                    //The field is mandatory if the expression in not empty
                    if (fieldToValidateValue == null || string.IsNullOrEmpty(fieldToValidateValue.ToString()))
                    {
                        field.FieldType.IsNullable = true;
                    }
                    else
                    {
                        field.FieldType.IsNullable = false;
                    }
                }
                else if (template.Equals(FIELD_IN_RANGE_EXPRESSION_TEMPLATE))
                {
                    field.FieldType.IsNullable = !ValueInRange(groups[2].Value, fieldToValidateValue, field.FieldName);
                }
                else if (template.Equals(FIELD_NOT_IN_RANGE_EXPRESSION_TEMPLATE))
                {
                    field.FieldType.IsNullable = ValueInRange(groups[2].Value, fieldToValidateValue, field.FieldName);
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }
    }
}
