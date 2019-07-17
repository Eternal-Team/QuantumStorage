using System.Collections.Generic;
using Terraria.ID;

namespace QuantumStorage.Global
{
	internal static class Utility
	{
		internal static Dictionary<int, Colors> ValidItems;

		internal static void Initialize()
		{
			ValidItems = new Dictionary<int, Colors>
			{
				{ItemID.Diamond, Colors.White},
				{ItemID.Ruby, Colors.Red},
				{ItemID.Emerald, Colors.Green},
				{ItemID.Topaz, Colors.Yellow},
				{ItemID.Amethyst, Colors.Purple},
				{ItemID.Sapphire, Colors.Blue},
				{ItemID.Amber, Colors.Orange}
			};
		}
	}
}