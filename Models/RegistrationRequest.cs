using MediatR;

namespace CloudPlatform.User.Models {
  public class RegistrationRequest : IRequest<RegistrationResponse> {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
  }
}
