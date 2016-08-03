namespace Okta.Samples.OpenIDConnect.AspNet.Api
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;

    public class JsonWebTokenValidationHandler : DelegatingHandler
    {
        public string SymmetricKey { get; set; }

        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string Modulus { get; set; }

        public string Exponent { get; set; }

        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            IEnumerable<string> authzHeaders;

            if (!request.Headers.TryGetValues("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                // Fail if no Authorization header or more than one Authorization headers  
                // are found in the HTTP request  
                return false;
            }

            // Remove the bearer token scheme prefix and return the rest as ACS token  
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;

            return true;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string strToken;
            HttpResponseMessage errorResponse = null;

            if (TryRetrieveToken(request, out strToken))
            {
                try
                {
                    RSACryptoServiceProvider rsaPublicKey = CreatePublicKey(this.Modulus, this.Exponent);

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKeyResolver = (token, securityToken2, keyIdentifier, validationParameters2) => {
                            return new RsaSecurityKey(rsaPublicKey);
                        },
                        ValidateAudience = true,
                        ValidAudience = this.Audience,
                        ValidateIssuer = true,
                        ValidIssuer = this.Issuer
                    };
                    SecurityToken secToken = new JwtSecurityToken();

                    Thread.CurrentPrincipal = tokenHandler.ValidateToken(strToken, validationParameters, out secToken);


                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.User = Thread.CurrentPrincipal;
                    }
                }
                //catch (JWT.SignatureVerificationException ex)
                //{
                //    errorResponse = request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex);
                //}
                //catch (JsonWebToken.TokenValidationException ex)
                //{
                //    errorResponse = request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex);
                //}
                catch (Exception ex)
                {
                    errorResponse = request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                }
            }

            return errorResponse != null ?
                Task.FromResult(errorResponse) :
                base.SendAsync(request, cancellationToken);
        }

        public static RSACryptoServiceProvider CreatePublicKey(string modulus, string exponent)
        {
            var cryptoProvider = new RSACryptoServiceProvider();

            cryptoProvider.ImportParameters(new RSAParameters()
            {
                Exponent = Base64UrlEncoder.DecodeBytes(exponent),
                Modulus = Base64UrlEncoder.DecodeBytes(modulus),
            });

            return cryptoProvider;
        }
    }
}
