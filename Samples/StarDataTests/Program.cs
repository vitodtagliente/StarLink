using System;
using System.Collections.Generic;
using StarLink;
using StarLink.Data;

namespace StarDataTests
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("StarData Tests");

			var data = new StarData ();
			data ["id"] = 9;
			data ["name"] = "Vito";

			var list = new List<object> ();
			list.Add ("ciao");
			list.Add (98989889);

			//data ["list"] = list;

			var position = new StarData ();
			position ["x"] = 10;
			position ["y"] = 30;

			data ["position"] = position;
			// bug, xke viene splittato per le virgone anche dentro le parentesi

			Console.WriteLine (data.ToString ());

			var cdata = StarData.Parse (data.ToString ());
			Console.WriteLine (cdata.ToString ());
			Console.WriteLine (cdata ["id"]);
			Console.WriteLine (cdata.Get("name").ToObject<string>());

			Console.ReadKey ();
		}
	}
}
