using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using StarLink;

namespace UnityStarLink
{
    public class StarScene : MonoBehaviour
    {
        public static StarScene singleton;

        public Dictionary<int, GameObject> Objects;

        public GameObject Player
        {
            get;
            private set;
        }

        void Awake()
        {
            Objects = new Dictionary<int, GameObject>();
            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {

        }

        void Update()
        {

        }

        void OnLevelWasLoaded(int level)
        {
            Objects.Clear();

        }
    }
}
