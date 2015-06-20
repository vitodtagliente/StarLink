using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using StarLink;
using UnityEngine;

namespace UnityStarLink
{
    [RequireComponent(typeof(StarScene))]
    public class StarManager : MonoBehaviour
    {
        public static StarManager singleton;

        public StarClient Host { get; private set; }

        [SerializeField]
        string ip = "localhost";
        [SerializeField]
        int port = 7777;
        [SerializeField]
        ProtocolType protocol = ProtocolType.Tcp;

        [SerializeField]
        bool dontDestroyOnLoad = true;
        
        void Awake()
        {
            singleton = this;
            if(dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            Host = new StarClient();
            Host.Address = ip;
            Host.Port = port;
            Host.Protocol = protocol;
        }

        public void Connect()
        {
            if (Host.Connected) return;

            Host.Connect();
            if (!Host.Connected)
            {
                Debug.Log(Host.Log);
                return;
            }

            var ListenThread = new Thread(ListenCallback);
            ListenThread.Start();
        }

        void Update()
        {
            if (Host.Connected)
            {

            }
        }

        public void Log(string text)
        {

        }

        public void Stop()
        {

            Host.Close();
        }

        void ListenCallback()
        {
            while (Host.Connected)
            {
                var message = Host.ReceiveMessage();
                if (message != null && !message.Empty)
                {
                    OnMessage(message.ReadInt(), message);
                }
            }
        }

        void OnMessage(int messageType, StarMessage message)
        {
            switch (messageType)
            {
                case ((int)MessageType.Normal):
                    
                    break;
                case ((int)MessageType.Command):

                    break;
                case ((int)MessageType.Behaviour):

                    int starId = message.ReadInt();
                    string starComponent = message.ReadString();

                    var gameobject = StarScene.singleton.Find(starId);
                    if (gameobject != null)
                    {
                        var behaviour = gameobject.GetComponent(starComponent);
                        if (behaviour != null)
                            ((StarBehaviour)behaviour).ReadMessage(message);
                    }                        

                    break;
                default:
                    Log("Unknown message type");
                    break;
            }
        }

        void OnApplicationQuit()
        {
            Host.Close();
        }
    }
}
