using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Amazon.CognitoIdentityProvider;
using Amazon.SimpleEmail;

using AppSettings.FeatureFlags;
using CloudPlatform.WebApi;
using CloudPlatform.Core;
using Microsoft.AspNetCore.Builder;
using System;
using CloudPlatform.User.Commands;

namespace CloudPlatform.User.Api {
  public class Startup : StartupBase {
    public Startup(IConfiguration configuration)
      : base(configuration) {
      base.ServiceName = "User";
      // base.MediatorAssembly = Commands.CommandsAssembly.Value;
    }

    public void ConfigureServices(IServiceCollection services) {

      services
        .Configure<Models.SiteOptions>(Configuration.GetSection("Service:Site"))
        .Configure<Models.CognitoOptions>(Configuration.GetSection("Service:Cognito"))
        .Configure<Models.ContactOptions>(Configuration.GetSection("Service:ContactInfo"))
        .AddAWSService<IAmazonCognitoIdentityProvider>(Configuration.GetAWSOptions("Service:Cognito"))
        .AddAWSService<IAmazonSimpleEmailService>(Configuration.GetAWSOptions("Service:SES"))
        // Pass a func that returns the service collection so we can use it later if we want
        // this allows us to pull the service collection in the Configure method or in other classes
        .AddSingleton<Func<IServiceCollection>>((sc) => () => services);
    }

    public override void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IWebHostEnvironment env) {

      var features = app.ApplicationServices.GetService<IAppFeatures>();
      var services = app.ApplicationServices.GetService<Func<IServiceCollection>>().Invoke();

      services
        .AddTransient<SendRegistrationConfirmation>()
        .AddTransient<Registration>()
        .AddTransient<CreateCognitoUser>();

      if (features.Flags.IsEnabled(Features.UserService_UseMySql)) {

        base.ConfigureCoreServices<CloudPlatform.User.Data.UserDataContext>(services);
        services.AddTransient<SaveUserToDatabase>();

      } else if (features.Flags.IsEnabled(Features.UserService_UseDynamoDB)) {

        services.AddTransient<SaveUserToDynamoDB>();
        base.ConfigureCoreServices(services);

      }

      base.Configure(app, env);

    }
  }
}
