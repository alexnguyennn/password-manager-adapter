using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Models;
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
                Fixture = new Lastpass(_cli.Object);
                //Fixture = new Lastpass(new Cli("lpass"));
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
                var cliMock = new Mock<ICli>();
                var user = "bob@dog.com";
                var loginArgs = $"login {user} --trust";
                _cli.Setup(x => x.SetArguments(loginArgs))
                    .Returns(new Mock<ICli>().Object);

                _cli.Setup(x => x.SetArguments("status"))
                    .Returns(cliMock.Object);

                var expectedExecutionResult = new ExecutionResult(0, "Logged in as bob@dog.com.", "", default(DateTimeOffset), default(DateTimeOffset));

                cliMock.Setup(x => x.ExecuteAsync())
                .Returns(Task.FromResult(expectedExecutionResult));

                //act
                var result = await Fixture.Login(user);
                //assert
                result.ShouldBeTrue();
            }

            [Fact]
            async Task WhenUserNameIsIncorrect_UserIsNotLoggedIn()
            {
                //arrange
                var cliMock = new Mock<ICli>();
                var user = "bob@dog.com";
                var loginArgs = $"login {user} --trust";
                _cli.Setup(x => x.SetArguments(loginArgs))
                    .Returns(new Mock<ICli>().Object);

                _cli.Setup(x => x.SetArguments("status"))
                    .Returns(cliMock.Object);

                var expectedExecutionResult = new ExecutionResult(0, "Not logged in.", "", default(DateTimeOffset), default(DateTimeOffset));

                cliMock.Setup(x => x.ExecuteAsync())
                .Returns(Task.FromResult(expectedExecutionResult));

                //act
                var result = await Fixture.Login(user);
                //assert
                result.ShouldBeFalse();
            }
        }

        public class GetStatus : TestFixture
        {
            [Fact]
            async Task WhenUserIsLoggedIn_ReturnTrueAndAccount()
            {
                //arrange
                var cliMock = new Mock<ICli>();
                var user = "bob@dog.com";

                _cli.Setup(x => x.SetArguments("status"))
                    .Returns(cliMock.Object);

                var expectedExecutionResult = new ExecutionResult(0, $"Logged in as {user}.", "", default(DateTimeOffset), default(DateTimeOffset));

                cliMock.Setup(x => x.ExecuteAsync())
                .Returns(Task.FromResult(expectedExecutionResult));

                //act
                var (result, accountId) = await Fixture.GetStatus();
                //assert
                result.ShouldBeTrue();
                accountId.ShouldBe(user);
            }

            [Fact]
            async Task WhenUserIsNotLoggedIn_ReturnFalseAndNoAccount()
            {
                //arrange
                var cliMock = new Mock<ICli>();
                var user = "bob@dog.com";

                _cli.Setup(x => x.SetArguments("status"))
                    .Returns(cliMock.Object);

                var expectedExecutionResult = new ExecutionResult(0, "Not logged in.", "", default(DateTimeOffset), default(DateTimeOffset));

                cliMock.Setup(x => x.ExecuteAsync())
                .Returns(Task.FromResult(expectedExecutionResult));

                //act
                var (result, accountId) = await Fixture.GetStatus();
                //assert
                result.ShouldBeFalse();
                accountId.ShouldBeNull();
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