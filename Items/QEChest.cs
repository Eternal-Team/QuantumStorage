using BaseLibrary.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace QuantumStorage.Items
{
	public class QEChest : BaseItem
	{
		public override string Texture => "QuantumStorage/Textures/Items/QEChest";

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
			item.rare = ItemRarityID.Pink;
			item.value = Item.sellPrice(gold: 8);
			item.createTile = mod.TileType<Tiles.QEChest>();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldenChest);
			recipe.AddIngredient(ItemID.HallowedBar, 7);
			recipe.AddIngredient(ItemID.SoulofMight, 5);
			recipe.AddTile(TileID.SteampunkBoiler);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}