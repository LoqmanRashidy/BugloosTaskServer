using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.Users
{
    public class BearerTokensOptions
    {
        public string Key { set; get; }
        public string Issuer { set; get; }
        public string Audience { set; get; }

        public int AccessTokenExpiration { set; get; }
        public int RefreshTokenExpiration { set; get; }
        public bool AllowMultipleLoginsFromTheSameUser { set; get; }
        public bool AllowSignoutAllUserActiveClients { set; get; }

    }
}
