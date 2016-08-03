using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Okta.Samples.OpenIDConnect.AspNet.Api.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonWebKeys
    {
        [JsonProperty("keys")]
        public List<JsonWebKey> Keys { get; set; }
    }

    public class JsonWebKey
    {
        [JsonProperty("e")]
        public string Exponent { get; set; }

        [JsonProperty("kty")]
        public string KeyType { get; set; }

        [JsonProperty("use")]
        public string PublicKeyUse { get; set; }

        [JsonProperty("kid")]
        public string KeyID { get; set; }

        [JsonProperty("x5c")]
        public List<string> X509CertificateChain { get; set; }

        [JsonProperty("x5t")]
        public string X509CertificateThumbprint { get; set; }

        [JsonProperty("n")]
        public string Modulus { get; set; }


    }

    [JsonObject(MemberSerialization.OptIn)]
    public class OpenIdConfiguration
    {
        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("authorization_endpoint")]
        public string AuthorizationEndPoint { get; set; }

        [JsonProperty("token_endpoint")]
        public string TokenEndPoint { get; set; }

        [JsonProperty("userinfo_endpoint")]
        public string UserInfoEndPoint { get; set; }

        [JsonProperty("jwks_uri")]
        public string KeysUri { get; set; }

        [JsonProperty("response_types_supported")]
        public string[] ResponseTypesSupported { get; set; }

        [JsonProperty("response_modes_supported")]
        public string[] ResponseModesSupported { get; set; }

        [JsonProperty("grant_types_supported")]
        public string[] GrantTypesSupported { get; set; }

        [JsonProperty("subject_types_supported")]
        public string[] SubjectTypesSupported { get; set; }

        [JsonProperty("id_token_signing_alg_values_supported")]
        public string[] IdTokenAlgorithmValuesSupported { get; set; }

        [JsonProperty("scopes_supported")]
        public string[] ScopesSupported { get; set; }

        [JsonProperty("token_endpoint_auth_methods_supported")]
        public string[] TokenEndPointAuthMethodsSupported { get; set; }

        [JsonProperty("claims_supported")]
        public string[] ClaimsSupported { get; set; }















    }
}