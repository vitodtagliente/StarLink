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
        string netIp = "localhost";
        [SerializeField]
        int netPort = 7777;
        [SerializeField]
        ProtocolType netProtocol = ProtocolType.Tcp;
        
        void Awake()
        {
            singleton = this;
            DontDestroyOnLoad(gameObject);

            Host = new StarClient();
            Host.Address = netIp;
            Host.Port = netPort;
            Host.Protocol = netProtocol;
        }

        void Start()
        {
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

        public virtual void OnMessage(int messageType, StarMessage message)
        {;
            switch (messageType)
            {
                case ((int)MessageType.Disconnect):
                    Stop();
                    break;
                default:
                    Debug.Log("Unknow message type");
                    break;
            }
        }
    }
}
