using BaseLibrary.UI;
using BaseLibrary.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace QuantumStorage.Tiles;

public class QEChest : ModTile
{
	public override string Texture => QuantumStorage.TexturePath + "Tiles/QEChest";

	public override void SetStaticDefaults()
	{
		Main.tileSolidTop[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TileEntities.QEChest>().Hook_AfterPlacement, -1, 0, false);
		TileObjectData.addTile(Type);

		ModTranslation name = CreateMapEntryName();
		AddMapEntry(Color.Purple, name);
	}

	public override bool RightClick(int i, int j)
	{
		if (!TileEntityUtility.TryGetTileEntity(i, j, out TileEntities.QEChest qeChest)) return false;

		PanelUI.Instance?.HandleUI(qeChest);

		return true;
	}

	public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
	{
		if (drawData.tileFrameX % 18 == 0 && drawData.tileFrameY % 18 == 0)
		{
			Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
		}
	}

	public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (!TileEntityUtility.TryGetTileEntity(i, j, out TileEntities.QEChest qeChest)) return;

		Tile tile = Main.tile[i, j];
		if (!tile.IsTopLeft()) return;

		Vector2 position = new Point16(i, j).ToScreenCoordinates();

		spriteBatch.Draw(QuantumStorage.TextureGemsSide.Value, position + new Vector2(5, 9), new Rectangle(6 * (int)qeChest.Frequency[0], 0, 6, 10), Color.White, 0f, new Vector2(3, 5), 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(QuantumStorage.TextureGemsMiddle.Value, position + new Vector2(12, 4), new Rectangle(8 * (int)qeChest.Frequency[1], 0, 8, 10), Color.White);
		spriteBatch.Draw(QuantumStorage.TextureGemsSide.Value, position + new Vector2(24, 4), new Rectangle(6 * (int)qeChest.Frequency[2], 0, 6, 10), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (!TileEntityUtility.TryGetTileEntity(i, j, out TileEntities.QEChest qeChest)) return;

		PanelUI.Instance?.CloseUI(qeChest);

		qeChest.Kill(i, j);

		Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.QEChest>());
	}
}