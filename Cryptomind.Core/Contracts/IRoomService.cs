using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Contracts
{
	public interface IRoomService
	{
		string CreateRoom(string creatorId);
		bool JoinRoom(string roomCode, string joinerId);
		bool SetReady(string roomCode, string readyUserId);
		string StartRoom(string roomCode);
	}
}
