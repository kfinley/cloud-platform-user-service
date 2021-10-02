using System.Threading;
using System.Threading.Tasks;

using MediatR;
using EFQuerySpecs;

using CloudPlatform.Core;
using CloudPlatform.Core.Data;
using CloudPlatform.User.Models;
using CloudPlatform.User.Data;

namespace CloudPlatform.User.Commands {
  public class SaveUserHandler : IRequestHandler<SaveUserRequest, SaveUserResponse> {

    private readonly IAsyncRepository<UserDataContext, IEntity> repository;

    public SaveUserHandler(IAsyncRepository<UserDataContext, IEntity> repository) {
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
