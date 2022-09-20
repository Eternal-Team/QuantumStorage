using System.Collections.Generic;
using System.IO;
using System.Linq;
using ContainerLibrary;
using FluidLibrary;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage;

public class QuantumStorageSystem : ModSystem
{
	public class BaseItemStorage : ItemStorage
	{
		public BaseItemStorage() : base(27)
		{
			// OnContentsChanged += (user, operation, slot) =>
			// {
			// 	/*Net.SendItem(frequency, slot)*/
			// };
		}
	}

	public class BaseFluidStorage : FluidStorage
	{
		public BaseFluidStorage() : base(255 * 10)
		{
			// OnContentsChanged += (user, operation, slot) =>
			// {
			/*Net.SendFluid(frequency, slot)*/
			// };
		}
	}

	public Dictionary<Frequency, ItemStorage> QEItemHandlers;
	public Dictionary<Frequency, FluidStorage> QEFluidHandlers;

	public QuantumStorageSystem()
	{
		QEItemHandlers = new Dictionary<Frequency, ItemStorage>();
		QEFluidHandlers = new Dictionary<Frequency, FluidStorage>();
	}

	public override void SaveWorldData(TagCompound tag)
	{
		tag["QEItems"] = new TagCompound
		{
			["Keys"] = QEItemHandlers.Keys.ToList(),
			["Values"] = QEItemHandlers.Values.Select(x => x.Save()).ToList()
		};

		tag["QEFluids"] = new TagCompound
		{
			["Keys"] = QEFluidHandlers.Keys.ToList(),
			["Values"] = QEFluidHandlers.Values.Select(x => x.Save()).ToList()
		};
	}

	public override void LoadWorldData(TagCompound tag)
	{
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
		}

		{
			TagCompound fluids = tag.GetCompound("QEFluids");
			var keys = fluids.GetList<Frequency>("Keys");
			var values = fluids.GetList<TagCompound>("Values").Select(tag =>
			{
				FluidStorage storage = new BaseFluidStorage();
				storage.Load(tag);
				return storage;
			}).ToList();
			QEFluidHandlers = keys.Zip(values).ToDictionary(x => x.First, x => x.Second);
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(QEItemHandlers.Count);
		foreach (var (frequency, storage) in QEItemHandlers)
		{
			writer.Write(frequency);
			storage.Write(writer);
		}

		writer.Write(QEFluidHandlers.Count);
		foreach (var (frequency, storage) in QEFluidHandlers)
		{
			writer.Write(frequency);
			// storage.Write(writer);
		}
	}

	public override void NetReceive(BinaryReader reader)
	{
		QEItemHandlers.Clear();

		int count = reader.ReadInt32();
		for (int i = 0; i < count; i++)
		{
			Frequency frequency = reader.ReadFrequency();

			ItemStorage storage = new BaseItemStorage();
			storage.Read(reader);
			QEItemHandlers.Add(frequency, storage);
		}

		QEFluidHandlers.Clear();

		 count = reader.ReadInt32();
		for (int i = 0; i < count; i++)
		{
			Frequency frequency = reader.ReadFrequency();

			FluidStorage storage = new BaseFluidStorage();
			// storage.Read(reader);
			QEFluidHandlers.Add(frequency, storage);
		}
	}
}