using System;
using System.Diagnostics;
using System.Reflection;
using System.Net;
using System.Text.Json;

namespace StarLink
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandSettings : Attribute
    {
        public string Id = string.Empty;
        public bool RequireAuthentication = false;
        public bool Reliable = false;
    }

    [CommandSettings]
    public abstract class BaseCommand
    {
        public string Id { get { return Settings.Id; } }

        public CommandSettings Settings
        {
            get
            {
                return GetType().GetCustomAttribute<CommandSettings>();
            }
        }

        public static string CommandId<T>() where T : BaseCommand
        {
            return typeof(T).GetCustomAttribute<CommandSettings>()?.Id;
        }

        public abstract HttpStatusCode Execute(UserSession userSession, StarMessage request, out StarMessage response);
    }

    [Serializable]
    public class EmptyCommandData
    {

    }

    [CommandSettings]
    public abstract class Command<ComponentT, RequestT, ResponseT> : BaseCommand
        where ComponentT : Component
        where RequestT : new()
        where ResponseT : new()
    {
        public Command(ComponentT component)
            : base()
        {
            _component = component;
        }

        protected ComponentT _component { get; private set; }

        public Type ComponentType { get { return typeof(ComponentT); } }
        public Type RequestType { get { return typeof(RequestT); } }
        public Type ResponseType { get { return typeof(ResponseT); } }

        public override HttpStatusCode Execute(UserSession userSession, StarMessage requestMessage, out StarMessage responseMessage)
        {
            responseMessage = StarMessage.Create(requestMessage.Header.Id);

            Debug.Assert(Id == requestMessage.Header.Get(MessageHeader.HeaderType.Command),
                "Cannot process the message with commandId[{0}]", requestMessage.Header.Get(MessageHeader.HeaderType.Command)
            );

            if (Settings.RequireAuthentication && userSession.IsAuthenticated == false)
            {
                Console.WriteLine("user[{0}] Unauthorized to run the command[{1}]", userSession.User.Id, Id);
                return HttpStatusCode.Unauthorized;
            }

            RequestT request = JsonSerializer.Deserialize<RequestT>(requestMessage.Body);
            if (request == null)
            {
                Console.WriteLine("Failed to parse the request[{0}]", requestMessage.Body);
                return HttpStatusCode.BadRequest;
            }

            HttpStatusCode error = ProcessRequest(userSession, request, out ResponseT response);
            responseMessage.Header.Set(MessageHeader.HeaderType.Command, Id);
            responseMessage.Header.Set(MessageHeader.HeaderType.StatusCode, error);
            responseMessage.Body = JsonSerializer.Serialize(response);

            return HttpStatusCode.OK;
        }

        protected abstract HttpStatusCode ProcessRequest(UserSession userSession, RequestT request, out ResponseT response);
    }
}
