using System.Threading;
using ValidationPilotServices.DataTypes;
using ValidationPilotServices.SchemaReader;
using Xunit;
using Xunit.Abstractions;

namespace ValidationPilotTests
{
    public class CodeBookTests : CoreTest
    {
        public CodeBookTests(ITestOutputHelper output) : base(output)
        {
            var culture = new System.Globalization.CultureInfo("en-us");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        [Fact]
        public void CodeBookServiceIniTest()
        {
            CodeBookReaderService service = new CodeBookReaderService();
            Assert.Equal("codebook.csv", service.FileName);
        }

        [Fact]
        public void CodeBookServiceGetTest()
        {
            CodeBookReaderService service = new CodeBookReaderService();
            CodeBookCollection source = service.Get();

            Assert.NotNull(source);
            Assert.NotEmpty(source.Items);
        }

        [Theory]
        [InlineData("Hotovost", "0")]
        [InlineData("TypTokyOprava", "G")]
        [InlineData("PokladnaStulTurnajBingo", "S")]
        public void CodeBookValidateItemTest(string key, string value)
        {
            CodeBookReaderService service = new CodeBookReaderService();
            CodeBookCollection source = service.Get();

            bool flag = source.IsValidValue(key, value, out string message);
            Assert.True(flag, message);
        }

        [Theory]
        [InlineData("Hotoost", "0")]
        [InlineData("TypTokyOprava", "GF")]
        [InlineData("PokladnaStulTurnajBingo", "")]
        public void CodeBookFailedItemTest(string key, string value)
        {
            CodeBookReaderService service = new CodeBookReaderService();
            CodeBookCollection source = service.Get();

            bool flag = source.IsValidValue(key, value, out string message);
            Assert.False(flag);

            this.Output.WriteLine(message);
        }

    }
}
