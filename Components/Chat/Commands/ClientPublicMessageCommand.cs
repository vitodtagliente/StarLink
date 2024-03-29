﻿using StarLink;
using System;
using System.Net;

namespace Chat.Commands
{
    [CommandSettings(Id = "PublicMessage", RequireAuthentication = true, RequireResponse = false)]
    class ClientPublicMessageCommand : Command<ClientComponent, ClientPublicMessageRequest, EmptyCommandData>
    {
        public ClientPublicMessageCommand(ClientComponent component)
            : base(component)
        {

        }

        protected override HttpStatusCode ProcessRequest(UserSession userSession, ClientPublicMessageRequest request, out EmptyCommandData response)
        {
            response = new EmptyCommandData();

            if (string.IsNullOrEmpty(request.Message))
            {
                Console.WriteLine("Chat messages cannot be empty, message {0}", request.Message);
                return HttpStatusCode.BadRequest;
            }

            _component.OnPublicMessage.Invoke(request.Username, request.Message);

            return HttpStatusCode.OK;
        }
    }
}
