using DEMOAPP.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace DEMOAPP.Controllers
{
    public class SharePointController : Controller
    {
        private string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        // GET: SharePoint
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/Sharepoint" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else { 
            string siteUrl = "https://zalo.sharepoint.com/sites/spsne";
            AuthenticationManager am = new AuthenticationManager();
            using (ClientContext ctx = am.GetAzureADWebApplicationAuthenticatedContext(siteUrl, (url) =>
            {
                return GetTokenForApplication(url);
            }))
            {
                ctx.Load(ctx.Web.CurrentUser);
                ctx.ExecuteQuery();

                ViewBag.UserName = ctx.Web.CurrentUser.Title;





            }
            }

            return View();
        }


        public string GetTokenForApplication(string siteUrl)
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // get a token for the Graph without triggering any user interaction (from the cache, via multi-resource refresh token, etc)
            ClientCredential clientcred = new ClientCredential(clientId, appKey);
            // initialize AuthenticationContext with the token cache of the currently signed in user, as kept in the app's database
            AuthenticationContext authenticationContext = new AuthenticationContext(aadInstance + tenantID, new ADALTokenCache(signedInUserID));
            AuthenticationResult authenticationResult =  authenticationContext.AcquireTokenSilent(siteUrl, clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }
    }
}