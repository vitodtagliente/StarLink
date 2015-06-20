using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

using UnityEngine;
using StarLink;

namespace UnityStarLink
{
    public abstract class StarBehaviour : MonoBehaviour
    {
        public bool Dirty { get; set; }

        void Update()
        {
            // reflection
            Attribute[] attrs = Attribute.GetCustomAttributes(typeof(StarVar));
            foreach (var attr in attrs) {
                Debug.Log(attr.GetType());
            }

        }

        protected void Bind()
        {

        }

        public virtual void ReadMessage(StarMessage message)
        {
        }

        public virtual StarMessage WriteMessage()
        {
            return null;
        }
    }
}
