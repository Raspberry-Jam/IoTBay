using IoTBay.Models.DTOs;
using IoTBay.Models.Entities;
using IoTBay.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IoTBay.Controllers;

/// <summary>
/// This filter intercepts all requests going to Action methods marked with the AuthenticationFilter attribute,
/// to verify if the browser session is logged in and the user that it is logged in as has the appropriate permissions
/// to access the Action that they are requesting.
/// </summary>
/// <param name="userRole">Minimum level of permission required for access</param>
public class AuthenticationFilter(Role userRole = Role.Customer) : ActionFilterAttribute
{
    public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get the current context
        var httpContext = context.HttpContext;
        
        // Deserialize the session-critical user info from the currentUser string as a UserSessionDto object
        var currentUser = SessionUtils.GetObjectFromJson<UserSessionDto>(httpContext.Session, "currentUser");

        // If the user is trying to access something that requires them to be logged in, redirect them to the login
        // page. This is used most effectively by tagging an action with [AuthenticationFilter] with no specified role,
        // falling back on the default Customer role.
        if (currentUser == null)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "User" },
                { "action", "Index" }    
            });
            return base.OnActionExecutionAsync(context, next);
        }

        // If the user is trying to access something that they do not have permission for, redirect them to an
        // AccessDenied page.
        if (currentUser.Role < userRole)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Home" },
                { "action", "AccessDenied" } // TODO: Add an AccessDenied action to the Home controller
            });
        }
        
        return base.OnActionExecutionAsync(context, next);
    }
}