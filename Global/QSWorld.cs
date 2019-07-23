using ContainerLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	public class ItemPair
	{
		public Frequency Frequency = new Frequency();
		public ItemHandler Handler;

		public Action<Frequency, int> OnContentsChanged = (frequency, slot) => { };

		public ItemPair Clone() => new ItemPair
		{
			Frequency = (Frequency)Frequency.Clone(),
			Handler = Handler.Clone()
		};
	}

	public class ItemPairSerializer : TagSerializer<ItemPair, TagCompound>
	{
		public override TagCompound Serialize(ItemPair value) => new TagCompound
		{
			["Frequency"] = value.Frequency,
			["Items"] = value.Handler
		};

		public override ItemPair Deserialize(TagCompound tag)
		{
			ItemPair pair = QSWorld.baseItemPair.Clone();
			pair.Frequency = tag.Get<Frequency>("Frequency");
			pair.Handler = tag.Get<ItemHandler>("Items");
			return pair;
		}
	}

	public class FluidPair
	{
		public Frequency Frequency = new Frequency();
		public FluidHandler Handler;

		public Action<Frequency, int> OnContentsChanged = (frequency, slot) => { };

		public FluidPair Clone() => new FluidPair
		{
			Frequency = (Frequency)Frequency.Clone(),
			Handler = Handler.Clone()
		};
	}

	public class FluidPairSerializer : TagSerializer<FluidPair, TagCompound>
	{
		public override TagCompound Serialize(FluidPair value) => new TagCompound
		{
			["Frequency"] = value.Frequency,
			["Fluids"] = value.Handler
		};

		public override FluidPair Deserialize(TagCompound tag)
		{
			FluidPair pair = new FluidPair
			{
				Frequency = tag.Get<Frequency>("Frequency"),
				Handler = tag.Get<FluidHandler>("Fluids")
			};
			return pair;
		}
	}

	public class QSWorld : ModWorld
	{
		public static QSWorld Instance;

		public List<ItemPair> QEItemHandlers;
		public List<FluidPair> QEFluidHandlers;

		internal static ItemPair baseItemPair;
		internal static FluidPair baseFluidPair;

		public override void Initialize()
		{
			Instance = this;

			baseItemPair = new ItemPair
			{
				Handler = new ItemHandler(27)
			};
			baseItemPair.OnContentsChanged += (frequency, slot) => Net.SendItem(frequency, slot);

			baseFluidPair = new FluidPair
			{
				Handler = new FluidHandler()
			};
			baseItemPair.OnContentsChanged += (frequency, slot) => Net.SendFluid(frequency, slot);
			baseFluidPair.Handler.GetSlotLimit += slot => 255 * 4;

			QEItemHandlers = new List<ItemPair>();
			QEFluidHandlers = new List<FluidPair>();
		}

		public override TagCompound Save() => new TagCompound
		{
			["QEItems"] = QEItemHandlers,
			["QEFluids"] = QEFluidHandlers
		};

		public override void Load(TagCompound tag)
		{
			QEItemHandlers = tag.GetList<ItemPair>("QEItems").ToList();
			QEFluidHandlers = tag.GetList<FluidPair>("QEFluids").ToList();
		}
	}
}