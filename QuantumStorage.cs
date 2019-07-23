using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	public class QuantumStorage : Mod
	{
		internal static QuantumStorage Instance;

		internal static Texture2D textureGemsSide;
		internal static Texture2D textureGemsMiddle;
		internal static Texture2D textureRingSmall;
		internal static Texture2D textureRingBig;

		/*
		Bag and bucket need tooltip info

		PUMPS - Terra Firma
		*/

		public override void Load()
		{
			Instance = this;

			Utility.Load();

			TagSerializer.AddSerializer(new FrequencySerializer());
			TagSerializer.AddSerializer(new ItemPairSerializer());
			TagSerializer.AddSerializer(new FluidPairSerializer());

			if (!Main.dedServ)
			{
				textureGemsMiddle = ModContent.GetTexture("QuantumStorage/Textures/Tiles/GemMiddle_0");
				textureGemsSide = ModContent.GetTexture("QuantumStorage/Textures/Tiles/GemSide_0");
				textureRingSmall = ModContent.GetTexture("QuantumStorage/Textures/Items/RingSmall");
				textureRingBig = ModContent.GetTexture("QuantumStorage/Textures/Items/RingBig");
			}
		}

		public override void Unload() => BaseLibrary.Utility.UnloadNullableTypes();

		public override void HandlePacket(BinaryReader reader, int whoAmI) => Net.HandlePacket(reader, whoAmI);
	}
}