using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using StarLink;

namespace UnityStarLink.Component
{
    public class StarIdentity : StarBehaviour
    {
        [SerializeField]
        int starId = 0;

        public int Id { get; private set; }

        void Awake()
        {
            if (starId != 0)
                Id = starId;
        }
    }
}
