using System;
using System.Threading;

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
using CloudPlatform.Entities;

namespace CloudPlatform.User.Tests {

  [Subject("Save User to DynamoDB")]
  public class When_SaveUserToDynamoDB_Requested : SpecBase {

    public When_SaveUserToDynamoDB_Requested(MSpecFixture fixture)
      : base(fixture) {
        Setup(this, context, of);
    }

    static Sut<SaveUserToDynamoDB> Sut = new Sut<SaveUserToDynamoDB, SaveUserResponse>();
    static SaveUserRequest Request;
    static SaveUserResponse Result;

    Establish context = () => {
      Request = new SaveUserRequest {
        Id = Guid.NewGuid(),
        FirstName = "Bob",
        LastName = "Jones",
        Email = "Bob@Jones.com"
      };
    };

    Because of = async () => Result = await Sut.Target.Handle(Request, new CancellationTokenSource().Token);

    [Fact]
    public void It_should_a_new_User_to_a_DynamoDB_table()
      => should_a_new_User_to_a_DynamoDB_table();

    It should_a_new_User_to_a_DynamoDB_table = () => {
      // Sut.Verify
    };
  }
}
