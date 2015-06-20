using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityStarLink
{
    [System.AttributeUsage(AttributeTargets.Field)]
    public class StarVar : System.Attribute
    {
        public int Dirty;
    }
}
