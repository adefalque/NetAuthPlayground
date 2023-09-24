using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", o =>
    {
        o.Cookie.Name = "CookieAuthApp";
        //o.Cookie.Path = "/test";  // Cookie will be set only for /test urls
        //o.Cookie.HttpOnly = false;  // Cookie will also be accessible in document.cookie - Not secure! XSS vulnerable
        //o.Cookie.SecurePolicy = CookieSecurePolicy.Always;    // Cookie only sent when HTTPS (EXCEPT for localhost)
        
        // Cross-site request: hacker.github.io requests for example an image from your my-project.github.io
        // Same site request: my-project.github.io requests an image from my-project.github.io
        // None: cookie is always sent
        // Lax: the cookie is not sent on cross-site requests, such as on requests to load images or frames,
        //      but is sent when a user is navigating to the origin site from an external site
        //      (for example, when following a link)
        // Strict: cookie is never sent from cross-site requests
        //o.Cookie.SameSite = SameSiteMode.Lax;
        
        //o.ExpireTimeSpan = TimeSpan.FromSeconds(10);
    });

builder.Services.AddControllers();

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World2");
app.MapGet("/test", () => "Test");
app.MapGet("/secure", () => "Secured content").RequireAuthorization();

app.MapPost("/login", async (HttpContext ctx) =>
{
    await ctx.SignInAsync("cookie", new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                }, 
                "cookie"
            )
        ),
        new AuthenticationProperties()
        {
            //IsPersistent = true    // Cookie will persist even when browser is closed
        }
    );

    return "Ok";
});

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync("cookie", new AuthenticationProperties()
    {
        
    });

    return "You're logged out";
});

app.MapDefaultControllerRoute();

app.Run();