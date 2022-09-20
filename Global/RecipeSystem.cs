using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace QuantumStorage;

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