using BaseLibrary;
using BaseLibrary.Tiles.TileEntites;
using BaseLibrary.UI;
using ContainerLibrary;
using System;
using System.IO;
using System.Linq;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace QuantumStorage.TileEntities
{
	public class QEChest : BaseTE, IItemHandler, IHasUI
	{
		public override Type TileType => typeof(Tiles.QEChest);

		public Guid UUID { get; set; }
		public BaseUIPanel UI { get; set; }
		public LegacySoundStyle CloseSound => SoundID.Item1;
		public LegacySoundStyle OpenSound => SoundID.Item1;

		public Frequency frequency;

		public ItemHandler Handler
		{
			get
			{
				if (!frequency.IsSet) return null;

				ItemPair pair = QSWorld.Instance.QEItemHandlers.FirstOrDefault(itemPair => Equals(itemPair.Frequency, frequency));
				if (pair != null) return pair.Handler;

				pair = QSWorld.baseItemPair.Clone();
				pair.Frequency = frequency;

				QSWorld.Instance.QEItemHandlers.Add(pair);
				Net.SendItemFrequency(frequency);
				return pair.Handler;
			}
		}

		public QEChest()
		{
			UUID = Guid.NewGuid();
			frequency = new Frequency();
		}

		public override TagCompound Save() => new TagCompound
		{
			["UUID"] = UUID,
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag)
		{
			UUID = tag.Get<Guid>("UUID");
			frequency = tag.Get<Frequency>("Frequency");
		}

		public override void NetSend(BinaryWriter writer, bool lightSend)
		{
			writer.Write(UUID);
			writer.Write(frequency);
		}

		public override void NetReceive(BinaryReader reader, bool lightReceive)
		{
			UUID = reader.ReadGUID();
			frequency = reader.ReadFrequency();
		}
	}
}