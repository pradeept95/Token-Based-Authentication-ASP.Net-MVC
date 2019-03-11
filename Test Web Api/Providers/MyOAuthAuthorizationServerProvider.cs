using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.OAuth; 
using System.Security.Claims;
using System.Threading.Tasks; 
using Test_Web_Api.Models;

namespace Test_Web_Api.Providers
{
    // --------------------------------------------------------------------------------
    /// <summary>
    /// OAuth Service Provider
    /// </summary>
    // --------------------------------------------------------------------------------
    public class MyOAuthAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        // ********************************************************************************
        /// <summary>
        /// validate client authentication
        /// </summary> 
        /// <param name="context"></param>
        /// <returns></returns> 
        // ********************************************************************************
        public override Task ValidateClientAuthentication(
            OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }
            return Task.FromResult<object>(null);
        }

        // ********************************************************************************
        /// <summary>
        /// grant resource owner credentials
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns> 
        // ********************************************************************************
        public override async Task GrantResourceOwnerCredentials(
            OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            var userManager =
                context.OwinContext.GetUserManager<ApplicationUserManager>();

            ApplicationUser user;
            try
            {
                user = await userManager.FindAsync(context.UserName, context.Password);
            }
            catch
            {
                // Could not retrieve the user.
                context.SetError("The user name or password is incorrect.");
                //context.SetError("server_error");
                context.Rejected();

                // Return here so that we don't process further. Not ideal but needed to be done here.
                return;
            }

            if (user != null)
            {
                try
                {
                    // User is found. Signal this by calling context.Validated
                    ClaimsIdentity identity = await userManager.CreateIdentityAsync(
                        user,
                        DefaultAuthenticationTypes.ExternalBearer);

                    context.Validated(identity);
                }
                catch
                {
                    // The ClaimsIdentity could not be created by the UserManager.
                    context.SetError("The user name or password is incorrect.");
                    //context.SetError("server_error");
                    context.Rejected();
                }
            }
            else
            {
                // The resource owner credentials are invalid or resource owner does not exist.
                context.SetError(
                    "access_denied",
                    "The resource owner credentials are invalid or resource owner does not exist.");

                context.Rejected();
            }
        }
    }

}