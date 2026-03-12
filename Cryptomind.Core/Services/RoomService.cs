using Cryptomind.Common.Constants;
using Cryptomind.Common.Exceptions;
using Cryptomind.Common.Helpers;
using Cryptomind.Core.Contracts;
using Cryptomind.Core.Rooms;
using Cryptomind.Core.Rooms.Enums;
using Cryptomind.Data.Enums;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Cryptomind.Core.Services
{
	public class RoomService () : IRoomService
	{
		private ConcurrentDictionary<string, RaceRoom> rooms = new ConcurrentDictionary<string, RaceRoom>();
		public string CreateRoom(string creatorId)
		{
			string code;
			do
			{
				code = GenerateRoomCodeHelper.CodeGenerator();
			} while (rooms.ContainsKey(code));

			var raceRoom = new RaceRoom
			{
				Code = code,
				CurrentRound = 0,
				Player1Id = creatorId,
				Player2Id = string.Empty,
				Player1Ready = false,
				Player2Ready = false,
				Player1Score = 0,
				Player2Score = 0,
				Status = Rooms.Enums.RoomStatus.WaitingForPlayers
			};

			rooms.TryAdd(code, raceRoom);
			return code;
		}
		public bool JoinRoom(string roomCode, string joinerId)
		{
			if (!rooms.ContainsKey(roomCode))
			{
				throw new NotFoundException(RoomErrorConstants.RoomNotFound);
			}

			var room = rooms[roomCode];

			room.Player2Id = joinerId;
			room.Status = Rooms.Enums.RoomStatus.WaitingForReady;

			return true;
		}
		public bool SetReady(string roomCode, string readyUserId)
		{
			if (!rooms.ContainsKey(roomCode))
			{
				throw new NotFoundException(RoomErrorConstants.RoomNotFound);
			}

			var room = rooms[roomCode];

			lock (room)
			{
				if (readyUserId == room.Player1Id)
					room.Player1Ready = true;
				else if (readyUserId == room.Player2Id)
					room.Player2Ready = true;
				else
					throw new NotFoundException(RoomErrorConstants.PlayerNotInRoom);

				return room.Player1Ready && room.Player2Ready;
			}
		}
		public string StartRoom(string roomCode)
		{
			if (!rooms.ContainsKey(roomCode))
			{
				throw new NotFoundException(RoomErrorConstants.RoomNotFound);
			}
			var (cipherType, encryptedText) = CipherGeneratorHelper.GenerateRandom();

			var room = rooms[roomCode];
			room.Status = RoomStatus.InProgress;
			room.CipherType = Enum.Parse<CipherType>(cipherType);

			return encryptedText;
		}
	}
}
