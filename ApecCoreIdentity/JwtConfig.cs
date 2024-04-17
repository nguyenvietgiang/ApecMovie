using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApecCoreIdentity
{
    public class JwtConfig
    {
        public const string SecretKey = "this is the Secret key for apec authentication";
        public const string Issuer = "apecmovie2024";
        public const string Audience = "apec-audience";
    }

}
