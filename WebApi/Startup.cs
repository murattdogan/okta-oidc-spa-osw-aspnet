/*!
 * Copyright (c) 2016, Okta, Inc. and/or its affiliates. All rights reserved.
 * The Okta software accompanied by this notice is provided pursuant to the Apache License, Version 2.0 (the "License.")
 *
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *
 * See the License for the specific language governing permissions and limitations under the License.
 */


using Microsoft.Owin;
using Owin;
using System.Web.Configuration;
using System.IdentityModel.Tokens;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Owin.Security.Jwt;

[assembly: OwinStartup(typeof(Okta.Samples.OpenIDConnect.AspNet.Api.Startup))]

namespace Okta.Samples.OpenIDConnect.AspNet.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use((context, next) =>
            {
                PrintCurrentIntegratedPipelineStage(context, "Middleware 1");
                return next.Invoke();
            });

            var clientID = WebConfigurationManager.AppSettings["okta:ClientId"];
            var oauthIssuer = WebConfigurationManager.AppSettings["okta:OAuth_Issuer"];
            var oidcIssuer = WebConfigurationManager.AppSettings["okta:OIDC_Issuer"];
            var IDorAccess = WebConfigurationManager.AppSettings["okta:IDorAccessToken"];

            var issuer = oidcIssuer;

            if (IDorAccess == "access")
            {
                issuer = oauthIssuer;
            }

            TokenValidationParameters tvps = new TokenValidationParameters
            {
                ValidAudience = clientID,
                ValidateAudience = true,
                ValidIssuer = issuer,
                ValidateIssuer = true
            };

            //app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            //{
            //    TokenValidationParameters = tvps,
            //    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
            //    {
            //        new OpenIdConnectCachingSecurityTokenProvider(oidcIssuer + "/.well-known/openid-configuration")
            //    }
            //});

            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
                AccessTokenFormat = new JwtFormat(tvps,
                new OpenIdConnectCachingSecurityTokenProvider(oidcIssuer + "/.well-known/openid-configuration")),
            });


            //app.Use((context, next) =>
            //{
            //    PrintCurrentIntegratedPipelineStage(context, "2nd MW");
            //    return next.Invoke();
            //});
            //app.Run(context =>
            //{
            //    PrintCurrentIntegratedPipelineStage(context, "3rd MW");
            //    return context.Response.WriteAsync("Hello world");
            //});

        }

        private void PrintCurrentIntegratedPipelineStage(IOwinContext context, string msg)
        {
            var currentIntegratedpipelineStage = System.Web.HttpContext.Current.CurrentNotification;
            context.Get<System.IO.TextWriter>("host.TraceOutput").WriteLine(
                "Current IIS event: " + currentIntegratedpipelineStage
                + " Msg: " + msg);
        }
    }


}
