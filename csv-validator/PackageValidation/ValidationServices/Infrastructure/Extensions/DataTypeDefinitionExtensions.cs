using System;
using System.Linq;
using System.Reflection;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.SchemaReader.Profiles;

namespace ValidationPilotServices.Infrastructure.Extensions
{
    public static class DataTypeDefinitionExtensions
    {
        public static Type GetTypeByName(string typename)
        {
            Type type = Assembly.GetExecutingAssembly().ExportedTypes.FirstOrDefault(p => p.Name.ToUpper().Equals(typename.ToUpper()));
            return type;
        }

        public static BaseValidator ValidatorFieldBuilder(this FileStructureProfile source)
        {
            BaseValidator target = null;
            Type sourceType = GetTypeByName(source.FieldType);

            if (sourceType == typeof(Identifier))
            {
                target = GetIdentifierType(source);
                target.IsNullable = !string.IsNullOrEmpty(source.IsNullable) && source.IsNullable.ToUpper().Equals("Y")
                    ? true
                    : false;
            }
            else if (sourceType == typeof(DecimalNumber))
            {
                target = GetDecimalNumberType(source);
                target.IsNullable = !string.IsNullOrEmpty(source.IsNullable) && source.IsNullable.ToUpper().Equals("Y")
                    ? true
                    : false;
            }
            else if (sourceType == typeof(IntegerNumber))
            {
                target = GetIntegerNumberType(source);
                target.IsNullable = !string.IsNullOrEmpty(source.IsNullable) && source.IsNullable.ToUpper().Equals("Y")
                    ? true
                    : false;
            }
            else if (sourceType == typeof(DateAndTime))
            {
                target = GetDateAndTimeType(source);
                target.IsNullable = !string.IsNullOrEmpty(source.IsNullable) && source.IsNullable.ToUpper().Equals("Y")
                    ? true
                    : false;
            }
            else if (sourceType == typeof(Date))
            {
                target = GetDateType(source);
                target.IsNullable = !string.IsNullOrEmpty(source.IsNullable) && source.IsNullable.ToUpper().Equals("Y")
                    ? true
                    : false;
            }
            else if (sourceType == typeof(Text))
            {
                target = GetTextType(source);
                target.IsNullable = !string.IsNullOrEmpty(source.IsNullable) && source.IsNullable.ToUpper().Equals("Y")
                    ? true
                    : false;
            }
            else if (sourceType == typeof(CodeBook))
            {
                target = GetCodeBookType(source);
            }
            else if (sourceType == typeof(ReferenceLink))
            {
                target = GetReferenceLinkType(source);
            }
            else
            {
                throw new ArgumentException($"Unknown field type [{source.FieldName}-{source.FieldName}]");
                //target = new Text(string.Empty, "100");
            }

            return target;
        }

        public static Identifier GetIdentifierType(FileStructureProfile source)
        {
            int maxlength = int.Parse(source.MaxLength);
            int.TryParse(source.MinLength, out var minLength);
            return new Identifier(minLength, maxlength);
        }

        public static DateAndTime GetDateAndTimeType(FileStructureProfile source)
        {
            return new DateAndTime();
        }

        
        public static Date GetDateType(FileStructureProfile source)
        {
            return new Date();
        }

        public static DecimalNumber GetDecimalNumberType(FileStructureProfile source)
        {
            return new DecimalNumber(source.DomainType);
        }

        public static IntegerNumber GetIntegerNumberType(FileStructureProfile source)
        {
            return new IntegerNumber();
        }

        public static Text GetTextType(FileStructureProfile source)
        {
            return new Text(source.MinLength, source.MaxLength);
        }

        public static CodeBook GetCodeBookType(FileStructureProfile source)
        {
            return new CodeBook() {CodeBookName = source.FieldNameKey};
        }

        
        public static ReferenceLink GetReferenceLinkType(FileStructureProfile source)
        {
             int maxlength = int.Parse(source.MaxLength);
            int.TryParse(source.MinLength, out var minLength);
            return new ReferenceLink(minLength, maxlength);
        }

    }
}
