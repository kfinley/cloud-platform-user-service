namespace CloudPlatform.User.Models {
  public class CreateCognitoUserResponse {
    public bool Success { get; set; }
    public string TempPassword { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
  }
}
