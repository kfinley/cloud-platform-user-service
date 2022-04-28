using System;

using MediatR;

namespace CloudPlatform.User.Commands {
    public class SaveUserRequest : IRequest<SaveUserResponse> {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
  }

  public class SaveUserResponse {
    public bool Success { get; set; }
  }

}

