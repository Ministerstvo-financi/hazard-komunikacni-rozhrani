using System;

namespace ValidationPilotServices.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class ContextConditionAttribute : Attribute
    {
        public string MethodToProcess { get; set; }

        public string ConditionErrorMessageTemplate { get; set; }
    }
}
