using Microsoft.Owin.Security.Infrastructure;
using System; 
using System.Linq;
using System.Threading.Tasks; 

namespace Test_Web_Api.Providers
{
    // --------------------------------------------------------------------------------
    /// <summary>
    /// provider to refresh token 
    /// </summary>
    // --------------------------------------------------------------------------------
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        // ********************************************************************************
        /// <summary>
        /// crate refresh_token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns> 
        // ********************************************************************************
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            Create(context);
        }

        // ********************************************************************************
        /// <summary>
        /// validate token
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns> 
        // ********************************************************************************
        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            Receive(context);
        }

        // ********************************************************************************
        /// <summary>
        /// create refresh token helper
        /// </summary>
        /// <param name="context"></param> 
        // ********************************************************************************
        public void Create(AuthenticationTokenCreateContext context)
        {
            object inputs;
            context.OwinContext.Environment.TryGetValue("Microsoft.Owin.Form#collection", out inputs);

            var grantType = ((Microsoft.Owin.FormCollection)inputs)?.GetValues("grant_type");

            var grant = grantType.FirstOrDefault();
            //if (grant == null || grant.Equals("refresh_token")) return;
            if (grant == null) return;
            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(30);

            context.SetToken(context.SerializeTicket());
        }

        // ********************************************************************************
        /// <summary>
        /// validate token helper
        /// </summary>
        /// <param name="context"></param> 
        // ********************************************************************************
        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);

            if (context.Ticket == null)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "invalid token";
                return;
            }

            if (context.Ticket.Properties.ExpiresUtc <= DateTime.UtcNow)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                context.Response.ReasonPhrase = "unauthorized";
                return;
            }

            context.Ticket.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(30);
            context.SetTicket(context.Ticket);
        }
    }
}