using CloudPlatform.Core.Data;

namespace CloudPlatform.User.Models {
  public class User : BaseEntity {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
  }
}
