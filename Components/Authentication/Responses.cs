using System;
using StarLink;

namespace Authentication
{
    [Serializable]
    public class AuthenticationResponse
    {
        public UserSession Session { get; set; }
    }
}
