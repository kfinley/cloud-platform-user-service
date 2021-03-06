using System;
using System.Net;
using System.Threading;

using Microsoft.Extensions.Options;

using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

using Machine.Specifications;
using Machine.Specifications.Model;

using FluentAssertions;
using Xunit;
using Moq;
using It = Machine.Specifications.It;
using Argument = Moq.It;
using SUT;

using CloudPlatform.Tests.Common.Specs;
using CloudPlatform.Tests.Common;
using CloudPlatform.User.Commands;
using CloudPlatform.User.Models;

namespace CloudPlatform.User.Tests {
  [Subject("Create Cognito User")]
  public class When_CreateCognitoUser_Requested : SpecBase {
    public When_CreateCognitoUser_Requested(MSpecFixture fixture)
      : base(fixture) {
      Setup(this, context, of);
    }

    static Sut<CreateCognitoUser> Sut = new Sut<CreateCognitoUser, CreateCognitoUserResponse>();

    static CreateCognitoUserRequest Request;
    static CreateCognitoUserResponse Result;

    Establish context = () => {
      Request = new CreateCognitoUserRequest {
        UserId = Guid.NewGuid().ToString(),
        FirstName = "Bob",
        LastName = "Jones",
        Email = "Bob@Jones.com"
      };

      Sut.SetupAsync<IAmazonCognitoIdentityProvider, AdminCreateUserResponse>(p =>
          p.AdminCreateUserAsync(Argument.IsAny<AdminCreateUserRequest>(), Argument.IsAny<CancellationToken>()))
        .ReturnsAsync(
        new AdminCreateUserResponse() {
          HttpStatusCode = HttpStatusCode.OK,
        });

      Sut.Setup<IOptions<CognitoOptions>, CognitoOptions>(o => o.Value).Returns(new CognitoOptions {
        UserPoolId = "test-user-pool-id"
      });

    };

    Because of = async () => Result = await Sut.Target.Handle(Request, new CancellationTokenSource().Token);

    It should_return_a_successful_result = () => {
      Result.Should().NotBeNull();
      Result.Success.Should().BeTrue();
    };

    [Fact]
    public void It_should_return_a_successful_result() =>
        should_return_a_successful_result();

    [Fact]
    public void It_should_create_a_new_Cognito_account() => should_create_a_new_Cognito_account();
    It should_create_a_new_Cognito_account = () => {
      Sut.Verify<IAmazonCognitoIdentityProvider>(p => p.AdminCreateUserAsync(Argument.IsAny<AdminCreateUserRequest>(), Argument.IsAny<CancellationToken>()), Times.Once());
    };

  }
}
