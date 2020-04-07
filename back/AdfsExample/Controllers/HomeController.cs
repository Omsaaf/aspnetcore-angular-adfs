using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdfsExample.Controllers
{
    [Authorize(Policy = "Given Policy")]
    [Produces("application/json")]
    [Route("api/Home")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("private")]
        public IActionResult GetPrivate()
        {
            var name = User.Identity.Name;
            var email = User.Claims.FirstOrDefault(x => x.Type.ToLower().Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"))?.Value.ToString().ToLower();
            var upn = User.Claims.First(x =>
                    x.Type.ToLowerInvariant().Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn"))
                ?.Value
                .ToString();
            var firstName = User.Claims.First(x =>
                    x.Type.ToLowerInvariant().Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"))
                ?.Value
                .ToString();
            var lastName = User.Claims.First(x =>
                    x.Type.ToLowerInvariant().Contains("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"))
                ?.Value
                .ToString();
            var userGuid=User.Claims.First(x =>
                    x.Type.ToLowerInvariant().Contains("objectguid"))
                ?.Value
                .ToString();
            if (!string.IsNullOrEmpty(userGuid))
            {
                byte[] encoded = System.Convert.FromBase64String(userGuid);
                userGuid=new System.Guid(encoded).ToString();
            }
            return Ok(new
            {
                message = $"Welcome to the private area! You are {firstName} {lastName} - {userGuid} - {email} - {upn}"
            });
        }

        [HttpGet]
        [Route("public")]
        [AllowAnonymous]
        public IActionResult GetPublic()
        {
            return Ok(new
            {
                message = "Welcome to the public area!"
            });
        }

    }
}