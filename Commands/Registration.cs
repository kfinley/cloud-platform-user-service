using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using MediatR;

using CloudPlatform.User.Models;

namespace CloudPlatform.User.Commands {

  public class RegistrationRequest : IRequest<RegistrationResponse> {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
  }

  public class RegistrationResponse {
    public bool Success { get; set; }
    public string Error { get; set; }
  }

  public class Registration : IRequestHandler<RegistrationRequest, RegistrationResponse> {

    private readonly IMediator mediator;
    private readonly ILogger<Registration> logger;

    public Registration(IMediator mediator, ILogger<Registration> logger) {
      this.mediator = mediator;
      this.logger = logger;
    }

    public async Task<RegistrationResponse> Handle(RegistrationRequest request, CancellationToken cancellationToken) {

      /*
          User Registration

          Given a new user
          When a Registration is requested
          Then it should create a user in Cognito
           and it should create a user in the User data store
           and it should publish a message that a user has registered
           and it should send an email to the user

      */

      try {

        var userId = Guid.NewGuid();

        var createCognitoUserResponse = await mediator.Send(new CreateCognitoUserRequest {
          UserId = userId.ToString(),
          FirstName = request.FirstName,
          LastName = request.LastName,
          Email = request.Email
        });

        if (createCognitoUserResponse.Success) {

          var saveUserResponse = await mediator.Send(new SaveUserRequest {
            Id = userId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
          });
          if (saveUserResponse.Success) {
            await mediator.Publish(new SendRegistrationConfirmationRequest {
              UserId = userId,
              TempPassword = createCognitoUserResponse.TempPassword,
              FirstName = request.FirstName,
              LastName = request.LastName,
              Email = request.Email
            });
          } else {
            throw new Exception("Failed to create user.");
          }
          return new RegistrationResponse {
            Success = true
          };
        } else
          this.logger.LogError("Could not register user.");

      } catch (Exception e) {
        this.logger.LogError(e.Message);
        return new RegistrationResponse {
          Error = e.Message
        };
      }

      return new RegistrationResponse {
        Error = "Could not process user registration."
      };

    }
  }
}
