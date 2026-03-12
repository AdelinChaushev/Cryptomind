using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Rooms.Enums
{
	public enum RoomStatus
	{
		WaitingForPlayers = 0,
		WaitingForReady = 1,
		InProgress = 2,
		Finished = 3
	}
}
