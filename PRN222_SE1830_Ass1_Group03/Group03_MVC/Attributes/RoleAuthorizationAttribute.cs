using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Group03_MVC.Attributes
{
    public class RoleAuthorizationAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizationAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userRole = session.GetString("Role");
            var userId = session.GetString("UserId");

            // Check if user is logged in
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Check if user has required role
            if (string.IsNullOrEmpty(userRole) || !_allowedRoles.Contains(userRole))
            {
                context.Result = new RedirectToActionResult("Forbidden", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}