using BlazorGraphApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.Threading.Tasks;

namespace BlazorGraphApi.Pages
{
    [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })]
    public class _Host : PageModel
    {
        readonly ITokenAcquisition tokenAcquisition;
        public _Host(ITokenAcquisition tokenAcquisition)
        {
            this.tokenAcquisition = tokenAcquisition;
        }

        public async Task OnGetAsync()
        {
            // Get a token, 
            // If token cache is not populated the authorizeForScopes attribute will re-authorize which will populate the cache
            string token = await tokenAcquisition.GetAccessTokenForUserAsync(new[] { Constants.ScopeUserRead });
        }
    }
}