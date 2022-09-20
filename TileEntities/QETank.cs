using System;
using System.IO;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using FluidLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage.TileEntities;

public class QETank : BaseTileEntity, IFluidStorage, IHasUI
{
	protected override Type TileType => typeof(Tiles.QETank);

	private new Guid ID;
	public Frequency Frequency;

	public QETank()
	{
		ID = Guid.NewGuid();
		Frequency = new Frequency();
	}

	public override void OnKill()
	{
		for (int index = 0; index < 3; index++)
			Item.NewItem(new EntitySource_Misc("QuantumStorage.Reset"), Position.X * 16, Position.Y * 16, 32, 32, Utility.ColorToItem(Frequency[index]));
	}

	public override void SaveData(TagCompound tag)
	{
		tag["ID"] = ID;
		tag["Frequency"] = Frequency;
	}

	public override void LoadData(TagCompound tag)
	{
		ID = tag.Get<Guid>("UUID");
		Frequency = tag.Get<Frequency>("Frequency");
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(ID);
		writer.Write(Frequency);
	}

	public override void NetReceive(BinaryReader reader)
	{
		ID = reader.ReadGuid();
		Frequency = reader.ReadFrequency();
	}

	public FluidStorage GetFluidStorage()
	{
		if (!Frequency.IsSet) return null;

		QuantumStorageSystem system = ModContent.GetInstance<QuantumStorageSystem>();

		if (system.QEFluidHandlers.TryGetValue(Frequency, out FluidStorage storage))
			return storage;

		storage = new QuantumStorageSystem.BaseFluidStorage();
		system.QEFluidHandlers.Add(Frequency, storage);
		return storage;

		// Net.SendItemFrequency(frequency);
	}

	public Guid GetID() => ID;
}