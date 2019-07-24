using BaseLibrary.Items;
using Terraria.ID;
using Terraria.ModLoader;

namespace QuantumStorage.Items
{
	public class QETank : BaseItem
	{
		public override string Texture => "QuantumStorage/Textures/Items/QETank";
		
		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.maxStack = 99;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.createTile = mod.TileType<Tiles.QETank>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Glass, 40);
			recipe.AddIngredient(ItemID.HallowedBar, 7);
			recipe.AddIngredient(ItemID.SoulofSight, 5);
			recipe.AddTile(TileID.SteampunkBoiler);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}