using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Mvc.Security;

public class BeUserRequirement : IAuthorizationRequirement
{
    public BeUserRequirement(string email)
    {
        Email = email;
    }

    public string Email { get; }
}

public class BeUserHandler : AuthorizationHandler<BeUserRequirement>
{
    IHttpContextAccessor _httpContextAccessor = null;
    public BeUserHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BeUserRequirement requirement)
    {
        HttpContext httpContext = _httpContextAccessor.HttpContext; // Access context here

        string user = httpContext.Session.GetString("Email");

        var redirectContext = context.Resource as AuthorizationFilterContext;

        if (!string.IsNullOrWhiteSpace(user) && user != requirement.Email)
        {
            context.Succeed(requirement);
        }
        else
        {
            redirectContext.Result = new RedirectToActionResult("Login", "User", null);
            context.Fail();
        }

        return Task.CompletedTask;
    }
}