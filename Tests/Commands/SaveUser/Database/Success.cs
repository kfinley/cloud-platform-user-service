using System;
using System.Threading;

using FluentAssertions;
using Machine.Specifications;
using Machine.Specifications.Model;
using It = Machine.Specifications.It;
using Moq;
using Argument = Moq.It;
using Xunit;

using EFQuerySpecs;
using SUT;

using CloudPlatform.Entities;
using CloudPlatform.User.Commands;
using CloudPlatform.User.Data;
using CloudPlatform.Tests.Common.Specs;
using CloudPlatform.Tests.Common;

namespace CloudPlatform.User.Tests {

  [Subject("Save User to Database")]
  public class When_SaveUserToDatabase_Requested : SpecBase {
    public When_SaveUserToDatabase_Requested(MSpecFixture fixture)
      : base(fixture) {
      Setup(this, context, of);
    }

    static Sut<SaveUserToDatabase> Sut = new Sut<SaveUserToDatabase, SaveUserResponse>();
    static SaveUserRequest Request;
    static SaveUserResponse Result;

    Establish context = () => {
      Request = new SaveUserRequest {
        Id = Guid.NewGuid(),
        FirstName = "Bob",
        LastName = "Jones",
        Email = "Bob@Jones.com"
      };

      Sut.SetupAsync<IAsyncRepository<UserDataContext, IEntity>, Models.User>(r => r.SaveAsync(Argument.IsAny<Models.User>(), Argument.IsAny<CancellationToken>()))
        .ReturnsAsync(new Models.User {
          Id = Request.Id,
          FirstName = Request.FirstName,
          LastName = Request.LastName,
          Email = Request.Email,
          Status = EntityStatus.Active
        });
    };

    Because of = async () => Result = await Sut.Target.Handle(Request, new CancellationTokenSource().Token);

    [Fact]
    public void It_should_return_a_successful_result()
      => should_return_a_successful_result();
    It should_return_a_successful_result = () => {
      Result.Should().NotBeNull();
      Result.Success.Should().BeTrue();
    };


    [Fact]
    public void It_should_save_a_new_User_to_the_Data_Repository()
      => should_save_a_new_User_to_the_Data_Repository();
    It should_save_a_new_User_to_the_Data_Repository = () => {
      Sut.Verify<IAsyncRepository<UserDataContext, IEntity>>(p => p.SaveAsync(Argument.IsAny<Models.User>(), Argument.IsAny<CancellationToken>()), Times.Once());
    };

  }

}
