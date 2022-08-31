using BaseLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace QuantumStorage.Items;

public class QEChest : BaseItem
{
	public override string Texture => QuantumStorage.TexturePath + "Items/QEChest";

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.maxStack = 99;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = true;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.sellPrice(gold: 8);
		Item.createTile = ModContent.TileType<Tiles.QEChest>();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.GoldenChest)
			.AddIngredient(ItemID.HallowedBar, 7)
			.AddIngredient(ItemID.SoulofMight, 5)
			.AddTile(TileID.SteampunkBoiler)
			.Register();
	}
}