using MediatR;

namespace CloudPlatform.User.Models {
  public class CreateCognitoUserRequest : IRequest<CreateCognitoUserResponse> {
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
  }
}
