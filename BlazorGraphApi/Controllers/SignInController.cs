using BlazorGraphApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using System.Threading.Tasks;

namespace BlazorGraphApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignInController : ControllerBase
    {
        readonly ITokenAcquisition tokenAcquisition;

        public SignInController(ITokenAcquisition tokenAcquisition)
        {
            this.tokenAcquisition = tokenAcquisition;
        }

        [HttpGet]
        [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })]
        public async Task<ActionResult<string>> SignInAsync(string redirectUrl)
        {
            // force a sign in, get a token, populates the token cache
            string result = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { Constants.ScopeUserRead });
            // go back to where we came from
            Response.Redirect(redirectUrl);

            return result;
        }
    }
}