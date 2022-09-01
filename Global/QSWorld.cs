using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContainerLibrary;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage.Global;

// 	public class FluidPair
// 	{
// 		public Frequency Frequency = new Frequency();
// 		public FluidHandler Handler;
//
// 		public Action<Frequency, int> OnContentsChanged = (frequency, slot) => { };
//
// 		public FluidPair Clone() => new FluidPair
// 		{
// 			Frequency = (Frequency)Frequency.Clone(),
// 			Handler = Handler.Clone()
// 		};
// 	}
//
// 	public class FluidPairSerializer : TagSerializer<FluidPair, TagCompound>
// 	{
// 		public override TagCompound Serialize(FluidPair value) => new TagCompound
// 		{
// 			["Frequency"] = value.Frequency,
// 			["Fluids"] = value.Handler.Save()
// 		};
//
// 		public override FluidPair Deserialize(TagCompound tag)
// 		{
// 			FluidPair pair = QSWorld.baseFluidPair.Clone();
// 			pair.Frequency = tag.Get<Frequency>("Frequency");
// 			pair.Handler.Load(tag.Get<TagCompound>("Fluids"));
// 			return pair;
// 		}
// 	}

public class QuantumStorageSystem : ModSystem
{
	public class BaseItemStorage : ItemStorage
	{
		public BaseItemStorage() : base(27)
		{
			OnContentsChanged += (user, operation, slot) =>
			{
				/*Net.SendItem(frequency, slot)*/
			};
		}
	}

	public Dictionary<Frequency, ItemStorage> QEItemHandlers;
	// public List<FluidPair> QEFluidHandlers;

	public QuantumStorageSystem()
	{
		// baseFluidPair = new FluidPair
		// {
		// 	Handler = new FluidHandler()
		// };
		// baseFluidPair.OnContentsChanged += (frequency, slot) => Net.SendFluid(frequency, slot);
		// baseFluidPair.Handler.GetSlotLimit += slot => 255 * 4;

		QEItemHandlers = new Dictionary<Frequency, ItemStorage>();
		// QEFluidHandlers = new List<FluidPair>();
	}

	public override void SaveWorldData(TagCompound tag)
	{
		tag["QEItems"] = new TagCompound
		{
			["Keys"] = QEItemHandlers.Keys.ToList(),
			["Values"] = QEItemHandlers.Values.Select(x => x.Save()).ToList()
		};

		// tag["QEFluids"] = QEFluidHandlers;
	}

	public override void LoadWorldData(TagCompound tag)
	{
		TagCompound items = tag.GetCompound("QEItems");
		var keys = items.GetList<Frequency>("Keys");
		var values = items.GetList<TagCompound>("Values").Select(tag =>
		{
			ItemStorage storage = new BaseItemStorage();
			storage.Load(tag);
			return storage;
		}).ToList();
		QEItemHandlers = keys.Zip(values).ToDictionary(x => x.First, x => x.Second);

		// QEFluidHandlers = tag.GetList<FluidPair>("QEFluids").ToList();
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(QEItemHandlers.Count);
		foreach (var (frequency, storage) in QEItemHandlers)
		{
			writer.Write(frequency);
			storage.Write(writer);
		}
		
		// writer.Write(QEFluidHandlers.Count);
		// foreach (FluidPair pair in QEFluidHandlers) writer.Write(pair);
	}

	public override void NetReceive(BinaryReader reader)
	{
		int count = reader.ReadInt32();
		for (int i = 0; i < count; i++)
		{
			Frequency frequency = reader.ReadFrequency();
			
			ItemStorage storage = new BaseItemStorage();
			storage.Read(reader);
			QEItemHandlers.Add(frequency,  storage);
		}
		
		// count = reader.ReadInt32();
		// for (int i = 0; i < count; i++) QEFluidHandlers.Add(reader.ReadFluidPair());
	}
}