using System;
using ValidationPilotServices.DataReader;
using ValidationPilotServices.Infrastructure.Enums;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class DataReaderServiceTests : CoreTest
    {
        public DataReaderServiceTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void DataReaderIniTest()
        {
            string folderSource = "DataSource/30030030-V-2019012108-R-01";
            PackageReaderService package = this.GetPackageType(folderSource);
            DataReaderService service = new DataReaderService(package);

            Assert.False(string.IsNullOrEmpty(service.DataSourceFolder));
            Assert.Equal(folderSource, service.DataSourceFolder);
        }

        [Fact]
        public void DataReaderCreateFailedTest()
        {
            Assert.Throws<ArgumentNullException>(() => new DataReaderService(null));
        }

        //[Fact]
        //public void DataReaderReadSourceTest()
        //{
        //    DataReaderService service = this.GetDataReaderService();
        //    service.ReadFileData("provozovatel.csv");
        //    Assert.True(service.IsValid);
        //}


        #region SUPPORT FUNCTIONS AND METHODS

        public PackageReaderService GetPackageType(string packageNumber)
        {
            PackageReaderService service = new PackageReaderService(packageNumber);
            service.Ini();

            return service;
        }

        private DataReaderService GetDataReaderService()
        {
            PackageReaderService package = this.GetPackageType("223454T333-V-2018123116-B-14");
            return new DataReaderService(package);
        }

        #endregion
    }
}
