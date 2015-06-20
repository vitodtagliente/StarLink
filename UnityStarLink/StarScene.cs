using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using StarLink;
using UnityStarLink.Component;

namespace UnityStarLink
{
    public class StarScene : MonoBehaviour
    {
        public static StarScene singleton;

        Dictionary<int, GameObject> Objects;

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

        void Add(int starId, GameObject obj)
        {
            if (!Objects.ContainsKey(starId))
                Objects.Add(starId, obj);
        }

        public GameObject Find(int id)
        {
            if (Objects.ContainsKey(id))
                return Objects[id];

            var result = GameObject.FindObjectsOfType(typeof(StarIdentity)) as StarIdentity[];
            foreach (var starId in result)
            {
                Add(starId.Id, starId.gameObject);
                if (starId.Id.Equals(id))
                    return starId.gameObject;
            }

            return null;
        }

        void OnLevelWasLoaded(int level)
        {
            Objects.Clear();

        }
    }
}
