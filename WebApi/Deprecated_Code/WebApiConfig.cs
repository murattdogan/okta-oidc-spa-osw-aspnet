
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using System.Runtime.Caching;
using System.IdentityModel.Tokens;

namespace Okta.Samples.OpenIDConnect.AspNet.Api
{
    public static class Old_WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //config.EnableCors();
            config.EnableCors(new System.Web.Http.Cors.EnableCorsAttribute("*", "*", "*"));
            // Configure Web API to use only bearer token authentication.
            // Must reference OWIN libraries for the following 2 lines to work
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            var clientID = WebConfigurationManager.AppSettings["okta:ClientId"];
            var issuer = WebConfigurationManager.AppSettings["okta:TenantUrl"];

            if (true)
            {
                var jwtKeyInfo = getOktaKeyModulusAndExponent(issuer); // WebConfigurationManager.AppSettings["okta:JWTCertificatePublicKey"];

                config.MessageHandlers.Add(new JsonWebTokenValidationHandler()
                {
                    Audience = clientID,  // client id
                    Exponent = jwtKeyInfo[0],
                    Modulus = jwtKeyInfo[1],
                    Issuer = issuer
                });
            }

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }


        private static string getKeysUri(string strOktaTenantUrl)
        {

            string strKeysUrl = string.Empty;

            WebRequest request = WebRequest.Create(string.Format("{0}/.well-known/openid-configuration", strOktaTenantUrl));

            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());

            string responseFromServer = reader.ReadToEnd();
            try
            {
                Models.OpenIdConfiguration openidConfig = JsonConvert.DeserializeObject<Models.OpenIdConfiguration>(responseFromServer);
                if (openidConfig != null && openidConfig.KeysUri != null)
                {
                    strKeysUrl = openidConfig.KeysUri;
                }
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }

            return strKeysUrl;

        }

        private static string[] getOktaKeyModulusAndExponent(string strOktaTenantUrl)
        {
            string[] keyInfo = null;

            //If the data exists in cache, pull it from there, otherwise make a call to database to get the data
            ObjectCache cache = MemoryCache.Default;

            keyInfo = cache.Get("OktaKeyInfo") as string[];
            if (keyInfo != null)
                return keyInfo;
            else
                keyInfo = new string[2];

            string strKeysUrl = getKeysUri(strOktaTenantUrl);
            // The request will be made to the authentication server.
            //WebRequest request = WebRequest.Create(string.Format("{0}/oauth2/v1/keys", strOktaTenantUrl));
            WebRequest request = WebRequest.Create(strKeysUrl);

            StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());


            string responseFromServer = reader.ReadToEnd();
            try
            {
                Models.JsonWebKeys keys = JsonConvert.DeserializeObject<Models.JsonWebKeys>(responseFromServer);
                if(keys!=null && keys.Keys!=null && keys.Keys.Count > 0)
                {
                    //strCertPublicKey = keys.Keys[0].X509CertificateChain[0];
                    keyInfo[0] = keys.Keys[0].Exponent;
                    keyInfo[1] = keys.Keys[0].Modulus;

                }
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }

            //adding the certificate to the cache
            CacheItemPolicy policy = new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(180) };
            cache.Add("OktaKeyInfo", keyInfo, policy);

            return keyInfo;
        }
    }
}