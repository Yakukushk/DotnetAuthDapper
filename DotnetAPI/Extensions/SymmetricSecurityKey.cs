using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DotnetAPI.Extensions
{
    internal class SymmetricSecurityKeyExtensions
    {
    

        private static SymmetricSecurityKeyExtensions _instance = new SymmetricSecurityKeyExtensions();
        private static object _instanceLock = new object();
        private SymmetricSecurityKeyExtensions()
        {

        }
        public static SymmetricSecurityKeyExtensions Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new SymmetricSecurityKeyExtensions();
                    }
                    return _instance;
                }
            }
        }
        public TokenValidationParameters TokenValidationParametersExtentions(string token)
        {
            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token));
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuer = false,  // Validation of the issuer mitigates forwarding attacks that can occur when an IdentityProvider represents multiple tenants and signs tokens with the same keys
                ValidateIssuerSigningKey = false, // It is possible for tokens to contain the public key needed to check the signature
                ValidateAudience = false // a site that receives a token, could not replay it to another site. 
            };
            return tokenValidationParameters;

        }

    }
}
