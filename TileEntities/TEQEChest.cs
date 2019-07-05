using BaseLibrary.Tiles.TileEntites;
using BaseLibrary.UI;
using ContainerLibrary;
using QuantumStorage.Global;
using QuantumStorage.Tiles;
using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Colors = QuantumStorage.Global.Colors;

namespace QuantumStorage.TileEntities
{
	public class TEQEChest : BaseTE, IItemHandler, IHasUI
	{
		public override Type TileType => typeof(QEChest);

		public Guid ID { get; set; }
		public BaseUIPanel UI { get; set; }
		public LegacySoundStyle CloseSound => SoundID.Item1;
		public LegacySoundStyle OpenSound => SoundID.Item1;

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

		public TEQEChest()
		{
			frequency = new Frequency(Colors.White, Colors.White, Colors.White);
		}

		public override TagCompound Save() => new TagCompound
		{
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag) => frequency = tag.Get<Frequency>("Frequency");
	}
}