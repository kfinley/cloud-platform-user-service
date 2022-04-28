using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MediatR;

using CloudPlatform.User.Commands;
using CloudPlatform.WebApi;

namespace CloudPlatform.User.Api.Controllers {
  [ApiController]
  [AllowAnonymous]

  public class RegistrationController : ApiControllerBase {

    public RegistrationController(IMediator mediator) : base(mediator) { }

    [HttpPost]
    [Route("/user/v1/register")]
    public async Task<IActionResult> Register(RegistrationRequest request)
      => Ok(await base.Send(request));
  }
}

