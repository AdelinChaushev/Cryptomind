using Cryptomind.Core.Rooms.Enums;
using Cryptomind.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptomind.Core.Rooms
{
	public class RaceRoom
	{
		public string Code { get; set; }
		public string Player1Id { get; set; }
		public string Player2Id { get; set; }
		public bool Player1Ready { get; set; }
		public bool Player2Ready { get; set; }
		public int Player1Score { get; set; }
		public int Player2Score { get; set; }
		public int CurrentRound { get; set; }
		public RoomStatus Status { get; set; }
		public string EncryptedText { get; set; }
		public CipherType CipherType { get; set; }
	}
}
