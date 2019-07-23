using BaseLibrary.Tiles.TileEntites;
using BaseLibrary.UI;
using ContainerLibrary;
using System;
using System.Linq;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace QuantumStorage.TileEntities
{
	public class QETank : BaseTE, IFluidHandler, IHasUI
	{
		public override Type TileType => typeof(Tiles.QETank);

		public Guid ID { get; set; }
		public BaseUIPanel UI { get; set; }
		public LegacySoundStyle CloseSound => SoundID.Item1;
		public LegacySoundStyle OpenSound => SoundID.Item1;

		public Frequency frequency;

		public FluidHandler Handler
		{
			get
			{
				if (!frequency.IsSet) return null;

				FluidPair pair = QSWorld.Instance.QEFluidHandlers.FirstOrDefault(fluidPair => Equals(fluidPair.Frequency, frequency));
				if (pair != null) return pair.Handler;

				pair = QSWorld.baseFluidPair.Clone();

				QSWorld.Instance.QEFluidHandlers.Add(pair);
				Net.SendFluidFrequency(frequency);
				return pair.Handler;
			}
		}

		public QETank()
		{
			ID = Guid.NewGuid();
			frequency = new Frequency();
		}

		public override TagCompound Save() => new TagCompound
		{
			["ID"] = ID.ToString(),
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag)
		{
			ID = Guid.Parse(tag.GetString("ID"));
			frequency = tag.Get<Frequency>("Frequency");
		}
	}
}