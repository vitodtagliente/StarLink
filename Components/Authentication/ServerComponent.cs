using StarLink;

namespace Authentication
{
    [ComponentSettings(Id = "Authentication")]
    public class ServerComponent : StarLink.ServerComponent
    {
        public ServerComponent(StarServer server)
            : base(server)
        {
        }


        protected override void RegisterCommands()
        {
            Server.Commands.Add(new Commands.AuthenticationCommand(this));
        }
    }
}
