using System;
using UnityEngine;
using StarLink;

namespace StarLink.Unity3D
{
	public class StarBehaviour : MonoBehaviour
	{
		[SerializeField]
		float notifyTime = 0.5f;
		float time;

		void Start()
		{
			time = notifyTime;
			OnStart ();
		}

		public virtual void OnStart(){}

		void Update()
		{
			time -= Time.deltaTime;
			if (time < 0f) {

				Notify ();

				time = notifyTime;
			}
			OnUpdate ();
		}

		public virtual void OnUpdate(){}

		public virtual void Notify()
		{
			
		}
	}
}
