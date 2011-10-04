﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpUtils.Extensions;

namespace SimpleMassiveRealtimeRankingServer.Server.PacketHandlers
{
	public class GetVersionHandler : IPacketHandler
	{
		public void HandlePacket(ServerManager ServerManager, Packet ReceivedPacket, Packet PacketToSend)
		{
			PacketToSend.Stream.WriteStruct(ServerManager.Version);
		}
	}
}
