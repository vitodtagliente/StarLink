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
            Client = client;
        }

        protected HttpStatusCode Call<RequestType>(string commandId, RequestType request)
            where RequestType : new()
        {
            return Client.Call(commandId, request);
        }

        protected HttpStatusCode Call<RequestType, ResponseType>(string commandId, RequestType request, out ResponseType response, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            return Client.Call(commandId, request, out response, timeout);
        }

        public StarClient Client { get; private set; }
    }

    [ComponentSettings]
    public abstract class ServerComponent : Component
    {
        public ServerComponent(StarServer server)
               : base()
        {
            Server = server;
        }

        protected HttpStatusCode Call<RequestType>(StarNodeId nodeId, string commandId, RequestType request)
            where RequestType : new()
        {
            return Server.Call(nodeId, commandId, request);
        }

        protected HttpStatusCode Call<RequestType, ResponseType>(StarNodeId nodeId, string commandId, RequestType request, out ResponseType response, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            return Server.Call(nodeId, commandId, request, out response, timeout);
        }

        public StarServer Server { get; private set; }
    }
}
