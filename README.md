# BlazorGraphApi

Blazor Server App with Azure AD Authentication, that calls the Microsoft Graph API on-behalf of the signed-in user.

This code uses [Microsoft.Indentity.Web](https://github.com/AzureAD/microsoft-identity-web)

You will need to register your app in Azure and modify appsettings.json to include your details

```
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "your.domain",
    "TenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "ClientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
    "CallbackPath": "/signin-oidc",
    "ClientSecret": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "GraphApiUrl": "https://graph.microsoft.com/beta"
}
```

# Issues

I had some problems the first time the user loads the Blazor MS Graph profile page.

In this situation the browser cookies exist, but the in-memory cache does not. This causes a MsalUiRequiredException to be thrown. 
Deleting the browser cookies and reloaded the page sort of worked, but I was looking for a better solution.

In the samples provided with Microsoft.Indentity.Web 
they add ``` [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })] ``` attribute to the controller, which catches the exception, signs the user out and then signs them back in, which populated the token cache. This also works for razor pages, so I added this to __Host.cshtml PageModel.


__Hosts.cshtml.cs

```
using BlazorGraphApi.Services;
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

```

Seems to work, I'd be interested to hear if anyone has a better solution.
