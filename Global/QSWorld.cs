using System.Collections.Generic;
using System.Linq;
using ContainerLibrary;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage.Global
{
	public class QSWorld : ModWorld
	{
		public static QSWorld Instance;

		public Dictionary<Frequency, ItemHandler> QEItemHandlers;
		public Dictionary<Frequency, FluidHandler> QEFluidHandlers;

		internal static ItemHandler baseItemHandler;
		internal static FluidHandler baseFluidHandler;

		public override void Initialize()
		{
			Instance = this;

			baseItemHandler = new ItemHandler(27);
			baseFluidHandler = new FluidHandler();
			baseFluidHandler.GetSlotLimit += slot => 255 * 4;

			QEItemHandlers = new Dictionary<Frequency, ItemHandler>();
			QEFluidHandlers = new Dictionary<Frequency, FluidHandler>();
		}

		public override TagCompound Save() => new TagCompound
		{
			["QEItems"] = QEItemHandlers.Select(pair => new TagCompound
			{
				["Frequency"] = pair.Key,
				["Items"] = pair.Value.Save()
			}).ToList(),
			["QEFluids"] = QEFluidHandlers.Select(pair => new TagCompound
			{
				["Frequency"] = pair.Key,
				["Fluids"] = pair.Value.Save()
			}).ToList()
		};

		public override void Load(TagCompound tag)
		{
			QEItemHandlers = tag.GetList<TagCompound>("QEItems").ToDictionary(c => c.Get<Frequency>("Frequency"), c =>
			{
				ItemHandler cloned = baseItemHandler.Clone();
				return cloned.Load(c.GetCompound("Items"));
			});
			QEFluidHandlers = tag.GetList<TagCompound>("QEFluids").ToDictionary(c => c.Get<Frequency>("Frequency"), c =>
			{
				FluidHandler cloned = baseFluidHandler.Clone();
				return cloned.Load(c.GetCompound("Fluids"));
			});
		}
	}
}