using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	public class QuantumStorage : Mod
	{
		internal static Texture2D textureGemsSide;
		internal static Texture2D textureGemsMiddle;
		internal static Texture2D textureRingSmall;
		internal static Texture2D textureRingBig;

		// todo: syncing

		/*
		Bag and bucket need tooltip info
		Black hole doesn't work with quantum bag
		Invs don't close when away from the storage.
		way to reinitialize storages

		PUMPS - Terra Firma
		*/

		public override void Load()
		{
			Utility.Initialize();

			TagSerializer.AddSerializer(new FrequencySerializer());

			if (!Main.dedServ)
			{
				textureGemsMiddle = ModContent.GetTexture("QuantumStorage/Textures/Tiles/GemMiddle_0");
				textureGemsSide = ModContent.GetTexture("QuantumStorage/Textures/Tiles/GemSide_0");
				textureRingSmall = ModContent.GetTexture("QuantumStorage/Textures/Items/RingSmall");
				textureRingBig = ModContent.GetTexture("QuantumStorage/Textures/Items/RingBig");
			}
		}

		public override void Unload() => BaseLibrary.Utility.UnloadNullableTypes();
	}
}