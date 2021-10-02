using System;
using MediatR;

namespace CloudPlatform.User.Models {
  public class SaveUserRequest : IRequest<SaveUserResponse> {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
  }
}
