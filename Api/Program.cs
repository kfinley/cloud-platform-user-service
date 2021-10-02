using CloudPlatform.WebApi;

namespace CloudPlatform.User.Api {
  public class Program : ApiProgramBase {

    public static void Main(string[] args) =>
      ApiProgramBase.Run<Startup>(args);
  }
}
