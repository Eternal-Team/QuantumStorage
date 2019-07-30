using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage
{
	// todo: sprites

	public class QuantumStorage : Mod
	{
		internal static QuantumStorage Instance;

		internal static Texture2D textureGemsSide;
		internal static Texture2D textureGemsMiddle;
		internal static Texture2D textureRingSmall;
		internal static Texture2D textureRingBig;

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

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Vertebrae, 5);
			recipe.SetResult(ItemID.Leather);
			recipe.AddRecipe();
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI) => Net.HandlePacket(reader, whoAmI);
	}
}