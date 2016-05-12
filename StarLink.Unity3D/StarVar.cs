using System;

namespace StarLink.Unity3D
{
	[System.AttributeUsage(AttributeTargets.Field)]
	public class StarVar : System.Attribute
	{
		public int Dirty;
	}
}
