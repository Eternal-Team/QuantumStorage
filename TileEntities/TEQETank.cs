using BaseLibrary.Tiles.TileEntites;
using BaseLibrary.UI;
using ContainerLibrary;
using QuantumStorage.Global;
using QuantumStorage.Tiles;
using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace QuantumStorage.TileEntities
{
	public class TEQETank : BaseTE, IFluidHandler, IHasUI
	{
		public override Type TileType => typeof(QETank);

		public Guid ID { get; set; }
		public BaseUIPanel UI { get; set; }
		public LegacySoundStyle CloseSound => SoundID.Item1;
		public LegacySoundStyle OpenSound => SoundID.Item1;

		public Frequency frequency;

		public FluidHandler Handler
		{
			get
			{
				if (QSWorld.Instance.QEFluidHandlers.TryGetValue(frequency, out FluidHandler handler)) return handler;

				FluidHandler temp = QSWorld.baseFluidHandler.Clone();
				QSWorld.Instance.QEFluidHandlers.Add((Frequency)frequency.Clone(), temp);
				return temp;
			}
		}

		public TEQETank()
		{
			frequency = new Frequency();
		}

		public override TagCompound Save() => new TagCompound
		{
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag) => frequency = tag.Get<Frequency>("Frequency");
	}
}