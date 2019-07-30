using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ID;

namespace QuantumStorage
{
	internal static class Utility
	{
		internal static Dictionary<int, Colors> ValidItems;

		internal static void Load()
		{
			ValidItems = new Dictionary<int, Colors>
			{
				{ ItemID.Diamond, Colors.White },
				{ ItemID.Ruby, Colors.Red },
				{ ItemID.Emerald, Colors.Green },
				{ ItemID.Topaz, Colors.Yellow },
				{ ItemID.Amethyst, Colors.Purple },
				{ ItemID.Sapphire, Colors.Blue },
				{ ItemID.Amber, Colors.Orange }
			};
		}

		internal static int ColorToItem(Colors color) => color == Colors.None ? 0 : ValidItems.First(pair => pair.Value == color).Key;

		internal static void Write(this BinaryWriter writer, Frequency frequency)
		{
			writer.Write((int)frequency[0]);
			writer.Write((int)frequency[1]);
			writer.Write((int)frequency[2]);
		}

		internal static Frequency ReadFrequency(this BinaryReader reader) => new Frequency((Colors)reader.ReadInt32(), (Colors)reader.ReadInt32(), (Colors)reader.ReadInt32());
	}
}