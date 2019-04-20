using System;
using BaseLibrary.Tiles.TileEntites;
using ContainerLibrary;
using QuantumStorage.Global;
using QuantumStorage.Tiles;
using Terraria.ModLoader.IO;

namespace QuantumStorage.TileEntities
{
	public class TEQETank : BaseTE, IFluidHandler
	{
		public override Type TileType => typeof(QETank);
		public Frequency frequency;

		public FluidHandler Handler
		{
			get
			{
				if (QSWorld.Instance.QEFluidHandlers.TryGetValue(frequency, out FluidHandler handler)) return handler;

				FluidHandler temp = QSWorld.baseFluidHandler.Clone();
				QSWorld.Instance.QEFluidHandlers.Add(new Frequency(frequency.colors), temp);
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