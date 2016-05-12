﻿using System;
using System.Net;
using System.Net.Sockets;

namespace StarLink
{
	public static class StarUtility
	{
		public static bool IsConnected(this Socket socket, int pollTime = 1000)
		{
			try {
				bool part1 = socket.Poll (pollTime, SelectMode.SelectRead);
				bool part2 = (socket.Available == 0);
				if (part1 & part2) {
					// connection is closed 
					return false;
				}
				return true;
			} catch (Exception) {
				return false;
			}
		}
	}
}

