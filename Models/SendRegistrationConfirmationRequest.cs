using System;
using MediatR;

namespace CloudPlatform.User.Models {
  public class SendRegistrationConfirmationRequest : INotification {
    public Guid UserId { get; set; }
    public string TempPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
  }
}
