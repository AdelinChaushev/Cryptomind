using System.Collections.Concurrent;

namespace Cryptomind.Core.Rooms
{
	public class RoomStore
	{
		public ConcurrentDictionary<string, RaceRoom> Rooms { get; } = new();
	}
}