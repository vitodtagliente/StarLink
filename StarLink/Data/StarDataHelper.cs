﻿using System;
using System.Collections.Generic;

namespace StarLink.Data
{
	public static class StarDataHelper
	{
		public static StarData ToStarData(this object value){
			if (value.GetType () != typeof(StarData)) {
				if (value.GetType () == typeof(Dictionary<string, object>))
					return new StarData ((Dictionary<string, object>)value);
				else if (value.GetType () == typeof(List<object>))
					return new StarData ((List<object>)value);
				else
					return new StarData (value);
			} else {
				return ((StarData)value);
			}
		}
	}
}

