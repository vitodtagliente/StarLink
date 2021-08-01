using System;
using System.Net;

using StarLink;

namespace Chat.Commands
{
    [CommandSettings(Id = "Write")]
    class WriteCommand : Command<ServerComponent, EmptyCommandData, EmptyCommandData>
    {
        public WriteCommand(ServerComponent component)
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
