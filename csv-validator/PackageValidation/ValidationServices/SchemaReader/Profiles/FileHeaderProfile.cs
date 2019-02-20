using System;

namespace ValidationPilotServices.SchemaReader.Profiles
{
    public class FileHeaderProfile
    {
        public string PackageNumber { get; set; }
        public string Model { get; set; }
        public DateTime CreatedAt { get;  set; }
        public string Version { get; set; }


        public override string ToString()
        {
            return
                $"PackageNumber: {this.PackageNumber}, Model: {this.Model}, ReportPeriod: {this.CreatedAt.ToLongDateString()}, Version: {this.Version}";
        }
    }
}
