using FluidLibrary.Content;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	internal static class Net
	{
		internal enum PacketType : byte
		{
			AddItemFrequency,
			AddFluidFrequency,
			SyncItemFrequency,
			SyncFluidFrequency
		}

		internal static ModPacket GetPacket(PacketType packetType)
		{
			ModPacket packet = ModContent.GetInstance<QuantumStorage>().GetPacket();
			packet.Write((byte)packetType);
			return packet;
		}

		internal static void HandlePacket(BinaryReader reader, int whoAmI)
		{
			PacketType packetType = (PacketType)reader.ReadByte();

			switch (packetType)
			{
				case PacketType.AddItemFrequency:
					ReceiveItemFrequency(reader, whoAmI);
					break;
				case PacketType.AddFluidFrequency:
					ReceiveFluidFrequency(reader, whoAmI);
					break;
				case PacketType.SyncItemFrequency:
					ReceiveItem(reader, whoAmI);
					break;
				case PacketType.SyncFluidFrequency:
					ReceiveFluid(reader, whoAmI);
					break;
			}
		}

		internal static void SendItemFrequency(Frequency frequency, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = GetPacket(PacketType.AddItemFrequency);
			packet.Write(frequency);
			packet.Send(ignoreClient: ignoreClient);
		}

		internal static void ReceiveItemFrequency(BinaryReader reader, int whoAmI)
		{
			Frequency frequency = reader.ReadFrequency();

			ItemPair handler = QSWorld.baseItemPair.Clone();
			handler.Frequency = frequency;
			ModContent.GetInstance<QSWorld>().QEItemHandlers.Add(handler);

			if (Main.netMode == NetmodeID.Server) SendItemFrequency(frequency, whoAmI);
		}

		internal static void SendFluidFrequency(Frequency frequency, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = GetPacket(PacketType.AddFluidFrequency);
			packet.Write(frequency);
			packet.Send(ignoreClient: ignoreClient);
		}

		internal static void ReceiveFluidFrequency(BinaryReader reader, int whoAmI)
		{
			Frequency frequency = reader.ReadFrequency();

			FluidPair handler = QSWorld.baseFluidPair.Clone();
			handler.Frequency = frequency;
			ModContent.GetInstance<QSWorld>().QEFluidHandlers.Add(handler);

			if (Main.netMode == NetmodeID.Server) SendFluidFrequency(frequency, whoAmI);
		}

		internal static void SendItem(Frequency frequency, int slot, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = GetPacket(PacketType.SyncItemFrequency);
			packet.Write(frequency);
			packet.Write(slot);
			packet.WriteItem(ModContent.GetInstance<QSWorld>().QEItemHandlers.FirstOrDefault(itemPair => Equals(itemPair.Frequency, frequency)).Handler.GetItemInSlot(slot), true);
			packet.Send(ignoreClient: ignoreClient);
		}

		internal static void ReceiveItem(BinaryReader reader, int whoAmI)
		{
			Frequency frequency = reader.ReadFrequency();
			int slot = reader.ReadInt32();
			Item item = reader.ReadItem(true);

			ItemPair pair = ModContent.GetInstance<QSWorld>().QEItemHandlers.FirstOrDefault(itemPair => Equals(itemPair.Frequency, frequency));
			pair?.Handler.SetItemInSlot(slot, item);

			if (Main.netMode == NetmodeID.Server) SendItem(frequency, slot, whoAmI);
		}

		internal static void SendFluid(Frequency frequency, int slot, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return;

			ModPacket packet = GetPacket(PacketType.SyncFluidFrequency);
			packet.Write(frequency);
			packet.Write(slot);
			packet.Write(ModContent.GetInstance<QSWorld>().QEFluidHandlers.FirstOrDefault(fluidPair => Equals(fluidPair.Frequency, frequency)).Handler.GetFluidInSlot(slot));
			packet.Send(ignoreClient: ignoreClient);
		}

		internal static void ReceiveFluid(BinaryReader reader, int whoAmI)
		{
			Frequency frequency = reader.ReadFrequency();
			int slot = reader.ReadInt32();
			ModFluid fluid = reader.ReadFluid();

			FluidPair pair = ModContent.GetInstance<QSWorld>().QEFluidHandlers.FirstOrDefault(fluidPair => Equals(fluidPair.Frequency, frequency));
			pair?.Handler.SetFluidInSlot(slot, fluid);

			if (Main.netMode == NetmodeID.Server) SendFluid(frequency, slot, whoAmI);
		}
	}
}