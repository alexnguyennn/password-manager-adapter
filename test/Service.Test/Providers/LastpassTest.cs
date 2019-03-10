using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CliWrap;
using Moq;
using PasswordManager.Service.Providers;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using Record = PasswordManager.Model.Record;

namespace Service.Test.Providers
{
    public class LastpassTest
    {
        public class TestFixture
        {
            protected readonly Lastpass Fixture;
            protected readonly Mock<ICli> _cli;

            protected TestFixture()
            {
                _cli = new Mock<ICli>();
//                Fixture = new Lastpass(_cli.Object);
                Fixture = new Lastpass(new Cli("lpass"));
            }
        }


        public class Login : TestFixture
        {
            private readonly ITestOutputHelper _outputHelper;
            
            public Login(ITestOutputHelper outputHelper)
            {
                _outputHelper = outputHelper;
            }
            
            [Fact]
            async Task WhenUserNameIsCorrect_UserIsLoggedIn()
            {
                //arrange
                var x = "test";

                //act
                var result = await Fixture.Login("");
                //assert
                // TODO check status logged in
                _outputHelper.WriteLine($"Output: {result}");
                result.ExitCode.ShouldNotBe(1);
            }

            [Fact]
            void WhenUserNameIsIncorrect_UserIsNotLoggedIn()
            {
                var x = "test";
                x.ShouldBe("test");
            }
        }
        
        public class GetRecords : TestFixture
        {
            [Fact]
            async Task WhenUserIsLoggedIn_ReturnsAllRecords()
            {
                var result = await Fixture.GetRecords();
                result.ShouldBeOfType<IList<Record>>();
            }
        }
        
        public class GetField : TestFixture
        {
            [Fact]
            async Task WhenUserIsLoggedIn_CopyFieldToClipboard()
            {
                var result = await Fixture.GetField("<insert-id>", "password");
                // TODO check clipboard contents?
            }
        }
        
    }
}