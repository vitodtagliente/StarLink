using System;
using System.Net;

using StarLink;

namespace Network.Commands
{
    [CommandSettings(Id = "Ping")]
    class PingCommand : Command<ServerComponent, EmptyCommandData, EmptyCommandData>
    {
        public PingCommand(ServerComponent component)
            : base(component)
        {
        }

        protected override HttpStatusCode ProcessRequest(UserSession userSession, EmptyCommandData request, out EmptyCommandData response)
        {
            response = new EmptyCommandData();
            return HttpStatusCode.OK;
        }
    }
}
