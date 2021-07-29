using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace StarLink
{
    abstract class MessageProcessor
    {
        public MessageProcessor(StarLink link)
        {
            _link = link;
            _requests = new ConcurrentDictionary<StarId, TaskCompletionSource<StarMessage>>();
        }

        protected StarLink _link;
        private ConcurrentDictionary<StarId, TaskCompletionSource<StarMessage>> _requests;

        protected HttpStatusCode Send(StarMessage request, out StarMessage response, Action<StarMessage> sendAction, int timeout = 20000000 /* 20s */)
        {
            var tcs = new TaskCompletionSource<StarMessage>();
            try
            {
                _requests.TryAdd(request.Header.Id, tcs);
                sendAction.Invoke(request);
                var task = tcs.Task;

                // Wait here until either the response has been received,
                // or we have reached the timeout limit.
                Task.WaitAll(new Task[] { task }, timeout);

                if (task.IsCompleted)
                {
                    // The response has been received.
                    response = task.Result;
                    return HttpStatusCode.OK;
                }
                else // Timeout response.
                {
                    Console.WriteLine($"Request timeout of {timeout} milliseconds has expired, throwing TimeoutException");
                    response = new StarMessage();
                    return HttpStatusCode.RequestTimeout;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex}.");

                // Remove the request/response entry in the 'finally' block to avoid leaking memory.
                _requests.TryRemove(request.Header.Id, out tcs);
            }

            response = new StarMessage();
            return HttpStatusCode.InternalServerError;
        }

        public void Process(StarMessage message)
        {
            if (_requests.TryGetValue(message.Header.Id, out TaskCompletionSource<StarMessage> tcs))
            {
                tcs.TrySetResult(message);
            }
        }
    }

    sealed class ClientMessageProcessor : MessageProcessor
    {
        public ClientMessageProcessor(StarLink link)
            : base(link)
        {

        }

        public HttpStatusCode Send(StarMessage request)
        {
            _link?.Send(MessageSerializer.Serialize(request));
            return HttpStatusCode.OK;
        }

        public HttpStatusCode Send(StarMessage request, out StarMessage response, int timeout = 20000000 /* 20s */)
        {
            return Send(request, out response, (StarMessage message) =>
            {
                _link?.Send(MessageSerializer.Serialize(message));
            }, timeout);
        }
    }

    sealed class ServerMessageProcessor : MessageProcessor
    {
        public ServerMessageProcessor(StarLink link)
            : base(link)
        {

        }

        public HttpStatusCode Send(StarNodeId nodeId, StarMessage request)
        {
            _link?.Send(nodeId, MessageSerializer.Serialize(request));
            return HttpStatusCode.OK;
        }

        public HttpStatusCode Send(StarNodeId nodeId, StarMessage request, out StarMessage response, int timeout = 20000000 /* 20s */)
        {
            return Send(request, out response, (StarMessage message) =>
            {
                _link?.Send(nodeId, MessageSerializer.Serialize(message));
            }, timeout);
        }
    }
}
