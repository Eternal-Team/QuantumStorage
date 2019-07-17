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
	public class QETank : BaseTile
	{
		public override string Texture => "QuantumStorage/Textures/Tiles/QETank";

		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] {16, 16};
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TileEntities.QETank>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;
			mineResist = 5f;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Quantum Entangled Tank");
			AddMapEntry(Color.Purple, name);
		}

		public override void RightClick(int i, int j)
		{
			TileEntities.QETank qeTank = mod.GetTileEntity<TileEntities.QETank>(i, j);
			if (qeTank == null) return;

			BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(qeTank);
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			Main.specX[nextSpecialDrawIndex] = i;
			Main.specY[nextSpecialDrawIndex] = j;
			nextSpecialDrawIndex++;
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			TileEntities.QETank qeTank = mod.GetTileEntity<TileEntities.QETank>(i, j);
			if (qeTank == null) return;

			Tile tile = Main.tile[i, j];
			if (!tile.IsTopLeft()) return;

			Vector2 position = new Point16(i, j).ToScreenCoordinates();

			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + new Vector2(5, 5), new Rectangle(6 * (int)qeTank.frequency[0], 0, 6, 10), Color.White, 0f, new Vector2(3, 5), 1f, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsMiddle, position + new Vector2(12, 0), new Rectangle(8 * (int)qeTank.frequency[1], 0, 8, 10), Color.White);
			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + new Vector2(24, 0), new Rectangle(6 * (int)qeTank.frequency[2], 0, 6, 10), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			TileEntities.QETank qeTank = mod.GetTileEntity<TileEntities.QETank>(i, j);
			BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(qeTank);

			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.QETank>());
			qeTank.Kill(i, j);
		}
	}
}