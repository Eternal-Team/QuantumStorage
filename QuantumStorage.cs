using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace QuantumStorage;

public class QuantumStorage : Mod
{
	public const string AssetPath = "QuantumStorage/Assets/";
	public const string TexturePath = AssetPath + "Textures/";

	// public override void HandlePacket(BinaryReader reader, int whoAmI) => Net.HandlePacket(reader, whoAmI);
}

public class RecipeSystem : ModSystem
{
	public override void AddRecipes()
	{
		Recipe.Create(ItemID.Leather).AddIngredient(ItemID.Vertebrae, 5).Register();
	}

	public override void PostSetupContent()
	{
		Utility.Load();
	}
}