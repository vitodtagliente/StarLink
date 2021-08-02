using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace StarLink
{
    abstract class CommandProcessor
    {
        public CommandProcessor(NetworkLink link)
        {
            Commands = new CommandRegister();
            _link = link;
            _requests = new ConcurrentDictionary<StarId, TaskCompletionSource<StarMessage>>();
        }

        public CommandRegister Commands { get; private set; }

        protected NetworkLink _link;
        private ConcurrentDictionary<StarId, TaskCompletionSource<StarMessage>> _requests;

        protected HttpStatusCode Call<RequestType, ResponseType>(string commandId, RequestType request, out ResponseType response, Action<StarMessage> sendAction, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            StarMessage requestMessage = StarMessage.Create();
            requestMessage.Header.Set(MessageHeader.HeaderType.Command, commandId);
            requestMessage.Body = StarSerializer.Serialize(request);

            var tcs = new TaskCompletionSource<StarMessage>();
            try
            {
                _requests.TryAdd(requestMessage.Header.Id, tcs);
                sendAction.Invoke(requestMessage);
                var task = tcs.Task;

                // Wait here until either the response has been received,
                // or we have reached the timeout limit.
                Task.WaitAll(new Task[] { task }, timeout);

                if (task.IsCompleted)
                {
                    // The response has been received.
                    StarMessage responseMessage = task.Result;
                    response = StarSerializer.Deserialize<ResponseType>(responseMessage.Body);
                    if (responseMessage.Header.TryGet(MessageHeader.HeaderType.StatusCode, out string statusCode)
                        && !string.IsNullOrEmpty(statusCode))
                    {
                        return Enum.Parse<HttpStatusCode>(statusCode);
                    }
                    // cannot determine the status code
                    return HttpStatusCode.FailedDependency;
                }
                else // Timeout response.
                {
                    Console.WriteLine($"Request timeout of {timeout} milliseconds has expired, throwing TimeoutException");
                    response = new ResponseType();
                    return HttpStatusCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex}.");

                // Remove the request/response entry in the 'finally' block to avoid leaking memory.
                _requests.TryRemove(requestMessage.Header.Id, out tcs);
            }

            response = new ResponseType();
            return HttpStatusCode.InternalServerError;
        }

        public abstract void Process(UserSession session, StarMessage message);

        protected void Process(UserSession session, StarMessage message, Action<StarMessage> sendAction)
        {
            string commandId = message.Header.Get(MessageHeader.HeaderType.Command);
            if (!string.IsNullOrEmpty(commandId))
            {
                if (_requests.TryGetValue(message.Header.Id, out TaskCompletionSource<StarMessage> tcs))
                {
                    // response handler
                    tcs.TrySetResult(message);
                }
                else
                {
                    // handle requests execution
                    if (Commands.TryGet(commandId, out BaseCommand command))
                    {
                        StarMessage response;
                        HttpStatusCode error = command.Execute(session, message, out response);
                        if (error == HttpStatusCode.OK)
                        {
                            if (command.Settings.RequireResponse)
                            {
                                sendAction.Invoke(response);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Failed to execute the command {0} with error {1}", commandId, error);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot find the command[{0}]", commandId);
                    }
                }
            }
        }
    }

    sealed class ClientCommandProcessor : CommandProcessor
    {
        public ClientCommandProcessor(NetworkLink link)
            : base(link)
        {

        }

        public HttpStatusCode Call<RequestType>(string commandId, RequestType request)
            where RequestType : new()
        {
            StarMessage requestMessage = StarMessage.Create();
            requestMessage.Header.Set(MessageHeader.HeaderType.Command, commandId);
            requestMessage.Body = StarSerializer.Serialize(request);
            _link?.Send(MessageSerializer.Serialize(requestMessage));
            return HttpStatusCode.OK;
        }

        public HttpStatusCode Call<RequestType, ResponseType>(string commandId, RequestType request, out ResponseType response, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            return Call(commandId, request, out response, (StarMessage message) =>
            {
                _link?.Send(MessageSerializer.Serialize(message));
            }, timeout);
        }

        public override void Process(UserSession session, StarMessage message)
        {
            Process(session, message, (StarMessage message) =>
            {
                _link?.Send(MessageSerializer.Serialize(message));
            });
        }
    }

    sealed class ServerCommandProcessor : CommandProcessor
    {
        public ServerCommandProcessor(NetworkLink link)
            : base(link)
        {

        }

        public HttpStatusCode Call<RequestType>(StarNodeId nodeId, string commandId, RequestType request)
            where RequestType : new()
        {
            StarMessage requestMessage = StarMessage.Create();
            requestMessage.Header.Set(MessageHeader.HeaderType.Command, commandId);
            requestMessage.Body = StarSerializer.Serialize(request);
            _link?.Send(nodeId, MessageSerializer.Serialize(requestMessage));
            return HttpStatusCode.OK;
        }

        public HttpStatusCode Call<RequestType, ResponseType>(StarNodeId nodeId, string commandId, RequestType request, out ResponseType response, int timeout = 20000000 /* 20s */)
            where RequestType : new()
            where ResponseType : new()
        {
            return Call(commandId, request, out response, (StarMessage message) =>
            {
                _link?.Send(nodeId, MessageSerializer.Serialize(message));
            }, timeout);
        }

        public override void Process(UserSession session, StarMessage message)
        {
            Process(session, message, (StarMessage message) =>
            {
                _link?.Send(session.NodeId, MessageSerializer.Serialize(message));
            });
        }
    }
}
