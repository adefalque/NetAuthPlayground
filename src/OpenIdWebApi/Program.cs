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
    var authenticateResult = await ctx.AuthenticateAsync();
    
    return new
    {
        UserClaims = ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList(),
        AuthClaims = authenticateResult?.Principal?.Claims.Select(x => new { x.Type, x.Value }).ToList(),
        AuthMetadata = authenticateResult?.Properties?.Items
    };
}).RequireAuthorization();

app.MapGet("/login", async (HttpContext ctx) =>
{
    await ctx.ChallengeAsync(new AuthenticationProperties
    {
        RedirectUri = "/user"
    });
});

app.Run();
