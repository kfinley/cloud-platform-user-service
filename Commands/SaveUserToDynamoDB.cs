using System;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using CloudPlatform.Core;
using CloudPlatform.Aws.DynamoDB;
using CloudPlatform.User.Models;
using CloudPlatform.User.Data;

namespace CloudPlatform.User.Commands {

  public class SaveUserToDynamoDB : IRequestHandler<SaveUserRequest, SaveUserResponse> {

    protected readonly IAmazonDynamoDB dynamoDbClient;

    public SaveUserToDynamoDB(IAmazonDynamoDB dynamoDbClient) {
      this.dynamoDbClient = dynamoDbClient;
    }


    public async Task<SaveUserResponse> Handle(SaveUserRequest request, CancellationToken cancellationToken) {

      
      // var user = await this.repository.SaveAsync(new Models.User {
      //   Id = request.Id,
      //   FirstName = request.FirstName,
      //   LastName = request.LastName,
      //   Email = request.Email
      // });


      return new SaveUserResponse();

    }
  }
}
