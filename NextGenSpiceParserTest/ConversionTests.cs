using NextGenSpice.Parser.Utils;
using Xunit;

namespace NextGenSpiceParserTest
{
    public class ConversionTests
    {
        [Fact]
        public void ConvertsValues()
        {
            Assert.Equal(5.2, Parser.ConvertValue("5.2"));
            Assert.Equal(4.2, Parser.ConvertValue("4.2wefwef"));
            Assert.Equal(double.NaN, Parser.ConvertValue("4a.2wefwef"));
            Assert.Equal(double.NaN, Parser.ConvertValue("4.2.wefwef"));
            Assert.Equal(4.2e6, Parser.ConvertValue("4.2MEG"));
            Assert.Equal(1e-3, Parser.ConvertValue("1MVOLTS"));
            Assert.Equal(1e-6, Parser.ConvertValue("1.UV"));
        }
    }
}