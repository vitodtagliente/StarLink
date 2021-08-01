using System;
using System.Net;

using StarLink;

namespace Network
{
    [ComponentSettings(Id = "Network")]
    public class ClientComponent : StarLink.ClientComponent
    {
        public ClientComponent(StarClient client)
            : base(client)
        {
        }

        public HttpStatusCode Ping(out int ping)
        {
            DateTime callTime = DateTime.Now;
            HttpStatusCode error = Call(BaseCommand.CommandId<Commands.PingCommand>(), new EmptyCommandData(), out EmptyCommandData response);
            ping = (DateTime.Now - callTime).Milliseconds;
            return error;
        }

        protected override void RegisterCommands()
        {

        }
    }
}
