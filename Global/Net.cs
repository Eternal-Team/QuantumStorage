using System.IO;
using Terraria.ModLoader;

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
			ModPacket packet = QuantumStorage.Instance.GetPacket();
			packet.Write((byte)packetType);
			return packet;
		}

		internal static void HandlePacket(BinaryReader reader, int whoAmI)
		{
			PacketType packetType = (PacketType)reader.ReadByte();

			switch (packetType)
			{
				case PacketType.AddItemFrequency: break;
				case PacketType.AddFluidFrequency: break;
				case PacketType.SyncItemFrequency: break;
				case PacketType.SyncFluidFrequency: break;
			}
		}
	}
}