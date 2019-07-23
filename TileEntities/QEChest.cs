using BaseLibrary.Tiles.TileEntites;
using BaseLibrary.UI;
using ContainerLibrary;
using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace QuantumStorage.TileEntities
{
	public class QEChest : BaseTE, IItemHandler, IHasUI
	{
		public override Type TileType => typeof(Tiles.QEChest);

		public Guid ID { get; set; }
		public BaseUIPanel UI { get; set; }
		public LegacySoundStyle CloseSound => SoundID.Item1;
		public LegacySoundStyle OpenSound => SoundID.Item1;

		public Frequency frequency;

		public ItemHandler Handler
		{
			get
			{
				if (!frequency.IsSet) return null;

				if (QSWorld.Instance.QEItemHandlers.TryGetValue(frequency, out ItemHandler handler)) return handler;

				handler = QSWorld.baseItemHandler.Clone();
				QSWorld.Instance.QEItemHandlers.Add((Frequency)frequency.Clone(), handler);
				return handler;
			}
		}

		public QEChest()
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