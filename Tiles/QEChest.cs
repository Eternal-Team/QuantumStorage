using BaseLibrary;
using BaseLibrary.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace QuantumStorage.Tiles
{
	public class QEChest : BaseTile
	{
		public override string Texture => "QuantumStorage/Textures/Tiles/QEChest";

		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TileEntities.QEChest>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;

			ModTranslation name = CreateMapEntryName();
			AddMapEntry(Color.Purple, name);
		}

		public override void RightClick(int i, int j)
		{
			TileEntities.QEChest qeChest = BaseLibrary.Utility.GetTileEntity<TileEntities.QEChest>(i, j);
			if (qeChest == null) return;

			BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(qeChest);
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			TileEntities.QEChest qeChest = BaseLibrary.Utility.GetTileEntity<TileEntities.QEChest>(i, j);
			if (qeChest == null) return;

			Main.specX[nextSpecialDrawIndex] = i;
			Main.specY[nextSpecialDrawIndex] = j;
			nextSpecialDrawIndex++;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TileEntities.QEChest qeChest = BaseLibrary.Utility.GetTileEntity<TileEntities.QEChest>(i, j);
			if (qeChest == null) return;

			Tile tile = Main.tile[i, j];
			if (!tile.IsTopLeft()) return;

			Vector2 position = new Point16(i, j).ToScreenCoordinates();

			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + new Vector2(5, 9), new Rectangle(6 * (int)qeChest.frequency[0], 0, 6, 10), Color.White, 0f, new Vector2(3, 5), 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsMiddle, position + new Vector2(12, 4), new Rectangle(8 * (int)qeChest.frequency[1], 0, 8, 10), Color.White);
			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + new Vector2(24, 4), new Rectangle(6 * (int)qeChest.frequency[2], 0, 6, 10), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			TileEntities.QEChest qeChest = BaseLibrary.Utility.GetTileEntity<TileEntities.QEChest>(i, j);
			BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(qeChest);

			for (int index = 0; index < 3; index++) Item.NewItem(i * 16, j * 16, 32, 32, Utility.ColorToItem(qeChest.frequency[index]));

			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.QEChest>());
			qeChest.Kill(i, j);
		}
	}
}