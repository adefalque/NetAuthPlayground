using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

const string authScheme = "cookie";

builder.Services.AddAuthentication(authScheme)
    .AddCookie(authScheme, o =>
    {
        o.Cookie.Name = "AuthorizationPoliciesApp";
    });

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("EU Passport", p =>
    {
        p.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(authScheme)
            .RequireClaim("passport_type", "EUR");
    });
    o.AddPolicy("NOR Passport", p =>
    {
        p.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(authScheme)
            .RequireClaim("passport_type", "NOR");
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/sweden", (HttpContext ctx) =>
{
    return "Welcome to Sweden";
}).RequireAuthorization("EU Passport");

app.MapGet("/norway", (HttpContext ctx) =>
{
    return "Welcome to Norway";
}).RequireAuthorization("NOR Passport");

app.MapGet("/user", (HttpContext ctx) =>
{
    var userName = ctx.User.FindFirstValue(ClaimTypes.Name);
    var passportType = ctx.User.FindFirstValue("passport_type");
    
    return Results.Content($"Welcome {userName}. Your passport type is {passportType}");
}).RequireAuthorization();

app.MapGet("/login", async (string passportType, HttpContext ctx) =>
{
    await ctx.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(
        new []
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "jdoe"),
            new Claim("passport_type", string.IsNullOrEmpty(passportType) ? "EU" : passportType)
        }, 
        authScheme))
    );

    return Results.Ok();
});

app.Run();
