using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(o =>
    {
        o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
    {
        o.Cookie.Name = "OpenIdWebApi";
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, o =>
    {
        o.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        o.Authority = "https://localhost:8001";
        o.ClientId = "WebAppCodeFlow";
        o.ClientSecret = "secret";
        o.ResponseType = "code";
        //o.CallbackPath = new PathString("signin-oidc");
        o.Scope.Add("openid");
        o.Scope.Add("profile");
        o.SaveTokens = true;
        o.GetClaimsFromUserInfoEndpoint = true;
    });
builder.Services.AddAuthorization();    

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/user",  async (HttpContext ctx) =>
{
    var sb = new StringBuilder();
    foreach (var claim in ctx.User.Claims)
    {
        sb.Append($"{claim.Type} = {claim.Value}\r\n");
    }

    var idToken = await ctx.GetTokenAsync(OpenIdConnectParameterNames.IdToken);

    sb.Append($"IdToken = {idToken}");
    
    return Results.Content(sb.ToString());
}).RequireAuthorization();

app.MapGet("/login", async (HttpContext ctx) =>
{
    await ctx.ChallengeAsync();
});

app.Run();
