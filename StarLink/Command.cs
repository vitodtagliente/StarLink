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
    public class EmptyCommandResponse
    {

    }

    [CommandSettings]
    public abstract class Command<Req, Res> : BaseCommand
        where Req : new()
        where Res : new()
    {
        public Command()
            : base()
        {

        }

        public Type RequestType { get { return typeof(Req); } }
        public Type ResponseType { get { return typeof(Res); } }

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

            Req request = JsonSerializer.Deserialize<Req>(requestMessage.Body);
            if (request == null)
            {
                Console.WriteLine("Failed to parse the request[{0}]", requestMessage.Body);
                return HttpStatusCode.BadRequest;
            }

            HttpStatusCode error = ProcessRequest(userSession, request, out Res response);
            responseMessage.Header.Set(MessageHeader.HeaderType.Command, Id);
            responseMessage.Header.Set(MessageHeader.HeaderType.StatusCode, error); 
            responseMessage.Body = JsonSerializer.Serialize(response);

            return HttpStatusCode.OK;
        }

        protected abstract HttpStatusCode ProcessRequest(UserSession userSession, Req request, out Res response);
    }
}
