namespace CloudPlatform.User.Models {
  public class CognitoOptions {
    public string RegionEndpoint { get; set; }
    public string UserPoolId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
  }
}
