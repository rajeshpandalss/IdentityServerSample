using IdentityServer.WindowsAuthentication.Configuration;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Models;
using IdentityServerSample.Models;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Helpers;

namespace IdentityServerSample.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    SigningCertificate = LoadCertificate(),

                    Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(StandardScopes.All)
                });
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://localhost:44307/identity",
                ClientId = "mvc",
                RedirectUri = "https://localhost:44307/",
                ResponseType = "id_token",

                SignInAsAuthenticationType = "Cookies"
            });

            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();
        }

        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}