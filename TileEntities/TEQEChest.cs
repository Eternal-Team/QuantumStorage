using System;
using BaseLibrary.Tiles.TileEntites;
using ContainerLibrary;
using QuantumStorage.Global;
using QuantumStorage.Tiles;
using Terraria.ModLoader.IO;

namespace QuantumStorage.TileEntities
{
	public class TEQEChest : BaseTE, IItemHandler
	{
		public override Type TileType => typeof(QEChest);
		public Frequency frequency;

		public ItemHandler Handler
		{
			get
			{
				if (QSWorld.Instance.QEItemHandlers.TryGetValue(frequency, out ItemHandler handler)) return handler;

				ItemHandler temp = QSWorld.baseItemHandler.Clone();
				QSWorld.Instance.QEItemHandlers.Add(new Frequency(frequency.colors), temp);
				return temp;
			}
		}

		public override TagCompound Save() => new TagCompound
		{
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag) => frequency = tag.Get<Frequency>("Frequency");
	}
}