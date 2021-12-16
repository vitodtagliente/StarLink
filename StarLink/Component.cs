using System;
using System.Net;
using System.Reflection;

namespace StarLink
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentSettings : Attribute
    {
        public string Id = string.Empty;
    }

    [ComponentSettings]
    public abstract class Component
    {
        public string Id { get { return Settings.Id; } }

        public ComponentSettings Settings
        {
            get
            {
                return GetType().GetCustomAttribute<ComponentSettings>();
            }
        }

        public static string ComponentId<T>() where T : Component
        {
            return typeof(T).GetCustomAttribute<ComponentSettings>()?.Id;
        }

        public virtual void Initialize()
        {
            RegisterCommands();
        }

        protected virtual void RegisterCommands() { }
    }

    [ComponentSettings]
    public abstract class ClientComponent : Component
    {
        public ClientComponent(StarClient client)
            : base()
        {
            _client = client;
        }

        protected HttpStatusCode Call<RequestType>(string commandId, RequestType request)
            where RequestType : new()
        {
            return _client.Call(commandId, request);
        }

        protected HttpStatusCode Call<RequestType, ResponseType>(string commandId, RequestType request, out ResponseType response, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            return _client.Call(commandId, request, out response, timeout);
        }

        protected StarClient _client { get; private set; }
    }

    [ComponentSettings]
    public abstract class ServerComponent : Component
    {
        public ServerComponent(StarServer server)
               : base()
        {
            _server = server;
        }

        protected HttpStatusCode Call<RequestType>(StarNodeId nodeId, string commandId, RequestType request)
            where RequestType : new()
        {
            return _server.Call(nodeId, commandId, request);
        }

        protected HttpStatusCode Call<RequestType, ResponseType>(StarNodeId nodeId, string commandId, RequestType request, out ResponseType response, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            return _server.Call(nodeId, commandId, request, out response, timeout);
        }

        public virtual void OnClientConnection(UserSession userSession) { }
        public virtual void OnClientDisconnection(UserSession userSession) { }

        protected StarServer _server { get; private set; }
    }
}
