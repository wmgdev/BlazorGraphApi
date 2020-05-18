# BlazorGraphApi

Blazor Server App with AD Authentication, that calls the MS Graph API on-behalf of the signed-in user.

It works, but still working through some issues

This code uses [Microsoft.Indentity.Web](https://github.com/AzureAD/microsoft-identity-web)

You will need to register your app in Azure and modify appsettings.json and appsettings.Development.json to include your details

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

I had some problems the first time the user loads the blazor MS Graph profile page.

In this situation the browser cookies exist, but the in-memory cache does not. This causes a MsalUiRequiredException to be thrown. 
Deleting the browser cookies and reloaded the page sort of worked, but I was looking for a better solution.

In the samples provided with Microsoft.Indentity.Web 
they added [AuthorizeForScopes(Scopes = new[] { Constants.ScopeUserRead })] attribute to the controller, which caught the exception,
signed the user out and then signed them back in, which populated the token cache. Couldn't do this in Blazor, so came up with a fudge.


In the Blazor page I caught the MsalUiRequiredException and redirected to a normal webapi controller, passing the current page url

```
protected override async Task OnInitializedAsync()
{
    try
    {
        me = await graphApiService.GetProfileAsync();
    }
    catch (Exception ex)
    {
        if (ex.InnerException is MsalUiRequiredException)
        {
            var redirectUrl = Uri.EscapeDataString(NavigationManager.Uri);
            NavigationManager.NavigateTo($"/api/signin?redirectUrl={redirectUrl}", true);
        }
    }
}

```



The normal webapi controller in Controller/SignInController.cs forces a sign-in, populates the cache and then
redirects back to the page that called it.

```
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

```

Clunky, but seems to work, I'd be interested to hear if anyone has a better solution.
