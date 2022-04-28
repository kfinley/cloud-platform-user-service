using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using EFQuerySpecs;

using CloudPlatform.Core;
using CloudPlatform.Entities;
using CloudPlatform.Core.Data;
using CloudPlatform.Aws.DynamoDB;
using CloudPlatform.User.Models;
using CloudPlatform.User.Data;

namespace CloudPlatform.User.Commands {

  public class SaveUserToDatabase : IRequestHandler<SaveUserRequest, SaveUserResponse> {


    private readonly IAsyncRepository<UserDataContext, IEntity> repository;

    public SaveUserToDatabase(IAsyncRepository<UserDataContext, IEntity> repository) {
      this.repository = repository;
    }


    public async Task<SaveUserResponse> Handle(SaveUserRequest request, CancellationToken cancellationToken) {
      var user = await this.repository.SaveAsync(new Models.User {
        Id = request.Id,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Email = request.Email
      });


      return new SaveUserResponse {
        Success = user.HasValue()
      };

    }
  }
}
