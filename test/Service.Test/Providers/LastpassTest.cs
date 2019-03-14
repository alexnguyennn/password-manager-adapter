using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CliWrap;
using CliWrap.Models;
using Moq;
using Newtonsoft.Json;
using PasswordManager.Model.Enums;
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
                //arrange
                var user = "bobdog@bob.com";
                var statusCliMock = new Mock<ICli>();
                _cli.Setup(x => x.SetArguments("status"))
                    .Returns(statusCliMock.Object);

                var expectedStatusExecutionResult = new ExecutionResult(0, 
                    $"Logged in as {user}.", "", default(DateTimeOffset), default(DateTimeOffset));

                statusCliMock.Setup(x => x.ExecuteAsync())
                    .Returns(Task.FromResult(expectedStatusExecutionResult));


                var getIdsArgs = "ls --format=\"%ai\"";
                var cliMock = new Mock<ICli>();
                _cli.Setup(x => x.SetArguments(getIdsArgs))
                    .Returns(cliMock.Object);

                var expectedIdExecutionResult = new ExecutionResult(0, GetExpectedRecordIds(),
                    "", default(DateTimeOffset), default(DateTimeOffset));

                cliMock.Setup(x => x.ExecuteAsync())
                    .Returns(Task.FromResult(expectedIdExecutionResult));

                var getJsonStringArgs = $"show --json {expectedIdExecutionResult.StandardOutput.Replace("\r\n", " ")}";
                var expectedJsonExecutionResult = new ExecutionResult(0, GetExpectedRecordJsonString(),
                    "", default(DateTimeOffset), default(DateTimeOffset));

                cliMock = new Mock<ICli>();
                _cli.Setup(x => x.SetArguments(getJsonStringArgs))
                    .Returns(cliMock.Object);
                cliMock.Setup(x => x.ExecuteAsync())
                    .Returns(Task.FromResult(expectedJsonExecutionResult));

                var expectedRecords = GetExpectedRecords();

                //act
                var result = await Fixture.GetRecords();
                //result.ShouldBe(GetExpectedRecords());
                result.ShouldAllBe(record => record.source == AdapterType.LastPass);
                for (var i = 0; i < result.Count; i++)
                {
                    CheckProperties(result[i], expectedRecords[i]);
                }

            }

            private string GetExpectedRecordIds()
            {
                return "1\r\n2\r\n3\r\n4\r\n5\r\n";
            }

            private string GetExpectedRecordJsonString()
            {
                return
                    "[{ \"id\": \"1\", \"name\": \"bobdog.com\", \"fullname\": \"bobdog.com\", \"username\": \"dog\", \"password\": \"bob\", \"last_modified_gmt\": \"1542032428\", \"last_touch\": \"0\", \"group\": \"\", \"url\": \"https://bobdog.com/user/actions\", \"note\": \"\" }, { \"id\": \"2\", \"name\": \"bobdog.com\", \"fullname\": \"bobdog.com\", \"username\": \"dog\", \"password\": \"bob\", \"last_modified_gmt\": \"1542032428\", \"last_touch\": \"0\", \"group\": \"\", \"url\": \"https://bobdog.com/user/actions\", \"note\": \"\" }, { \"id\": \"3\", \"name\": \"bobdog.com\", \"fullname\": \"bobdog.com\", \"username\": \"dog\", \"password\": \"bob\", \"last_modified_gmt\": \"1542032428\", \"last_touch\": \"0\", \"group\": \"\", \"url\": \"https://bobdog.com/user/actions\", \"note\": \"\" }, { \"id\": \"4\", \"name\": \"bobdog.com\", \"fullname\": \"bobdog.com\", \"username\": \"dog\", \"password\": \"bob\", \"last_modified_gmt\": \"1542032428\", \"last_touch\": \"0\", \"group\": \"\", \"url\": \"https://bobdog.com/user/actions\", \"note\": \"\" }, { \"id\": \"5\", \"name\": \"bobdog.com\", \"fullname\": \"bobdog.com\", \"username\": \"dog\", \"password\": \"bob\", \"last_modified_gmt\": \"1542032428\", \"last_touch\": \"0\", \"group\": \"\", \"url\": \"https://bobdog.com/user/actions\", \"note\": \"\" } ]";
            }

            private IList<Record> GetExpectedRecords()
            {
                return JsonConvert.DeserializeObject<IList<Record>>(GetExpectedRecordJsonString()).ToList();
            }

            private void CheckProperties(Record current, Record other)
            {
                Type type = typeof(Record);
                type.GetProperties().Select(property =>
                {
                    property.GetValue(current).ShouldBe(property.GetValue(other));
                    return property;
                });
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