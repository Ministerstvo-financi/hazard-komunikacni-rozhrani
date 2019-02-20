namespace ValidationPilotServices.ConfigService
{
    public static class ConfigSettings
    {
        public static string remote_reporting_period_parameter_format = "yyyyMMddHH";
        public static string daily_reporting_period_parameter_format = "yyyyMM";

        //Reg expression to check model validity (V or M on second position)
        public static string model_regex = @"^(.*)\-([V,M]{1})\-.*$";

        //Reg expression to parse Remote Access Package Name
        public static string remote_regex = @"^([A-Za-z0-9_]{1,20})-(V)-([0-9]{10})\-([a-zA-Z]{1})\-([0-9]{2})$";

        //Reg expression to parse Daily reports Package Name
        public static string daily_regex = @"^([A-Za-z0-9_]{1,20})-(M)-([0-9]{6})\-([a-zA-Z]{1})\-([A-Za-z0-9_]{1,50})\-([0-9]{2})$";

        public static string predefined_expression_regex = @"^([A-Za-z]*)\(([0-9]{1,})\)$";
    }
}
