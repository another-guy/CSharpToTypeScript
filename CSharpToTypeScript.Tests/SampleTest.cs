using Xunit;

namespace CSharpToTypeScript.Tests
{
    public class SampleTest
    {
        [Fact]
        public void TestThatIsSupposedToFail()
        {
            Assert.True(false, "TEST FAILS AS EXPECTED");
        }
    }
}
