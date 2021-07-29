﻿using System;
using System.Net;
using StarLink;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            StarServer server = new StarServer(StarProtocol.UDP);
            server.onClientMessage = (UserSession session, StarMessage message) =>
            {
                if (message.Header.Contains(MessageHeader.HeaderType.Command))
                {
                    return;
                }

                // echo server
                Console.WriteLine("The client {0} sent the message: {1}", session.User.Id, message.Body);
                StarMessage response = StarMessage.Create();
                response.Body = string.Format("Hello Client {0}", session.User.Id);
                server.Send(session, response);
            };
            server.Components.Add(new Authentication.ServerComponent(server));

            StarClient client = new StarClient(StarProtocol.UDP);
            client.OnMessage = (StarMessage message) =>
            {
                Console.WriteLine("The server sent the message: {0}", message.Body);
            };
            client.Components.Add(new Authentication.ClientComponent(client));

            if (server != null && client != null)
            {
                server.Listen(7000);

                client.Connect(new StarEndpoint(StarEndpoint.Localhost, 7000));

                // welcome message
                StarMessage message = StarMessage.Create();
                message.Body = "Hello Server";
                client.Send(message);

                HttpStatusCode error = HttpStatusCode.OK;
                if (client.Components.TryGet(out Authentication.ClientComponent authComponent))
                {
                    error = authComponent.Authenticate(new Authentication.AuthenticationRequest()
                    {
                        Username = "Vito"
                    });
                }

                Console.ReadKey(true);
                client.Close();
                server.Close();
            }
        }
    }
}
