using Shouldly;
using Xunit;

namespace Service.Test.Providers
{
    // protected class TestFixture
    // {
    //         // protected readonly ...
    //         protected readonly IPasswordAdapter Fixture;

    //         public TestFixture
    //         {
    //             Fixture = new Mock<IPasswordAdapter>();
    //         }
    // }

    // public class BitwardenTest : TestFixture
    public class BitwardenTest
    {
        [Fact]
        public void WhenUserSuppliesCorrectUserAndPassword_SetLoginVariables()
        {
            //Given
            string x = "test";
            //When

            //Then
            x.ShouldBe("test");
        }
    }
}