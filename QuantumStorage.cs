using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	public class QuantumStorage : Mod
	{
		internal static Texture2D textureEmptySocket;
		internal static Texture2D textureGemsSide;
		internal static Texture2D textureGemsMiddle;
		internal static Texture2D textureRingSmall;
		internal static Texture2D textureRingBig;

		// todo: syncing

		public override void Load()
		{
			Utility.Initialize();

			TagSerializer.AddSerializer(new FrequencySerializer());

			if (!Main.dedServ)
			{
				textureEmptySocket = ModContent.GetTexture("QuantumStorage/Textures/UI/EmptySocket");
				textureGemsMiddle = ModContent.GetTexture("QuantumStorage/Textures/Tiles/GemMiddle_0");
				textureGemsSide = ModContent.GetTexture("QuantumStorage/Textures/Tiles/GemSide_0");
				textureRingSmall = ModContent.GetTexture("QuantumStorage/Textures/Items/RingSmall");
				textureRingBig = ModContent.GetTexture("QuantumStorage/Textures/Items/RingBig");
			}
		}

		public override void Unload() => BaseLibrary.Utility.UnloadNullableTypes();
	}
}