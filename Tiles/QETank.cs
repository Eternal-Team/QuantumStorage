using BaseLibrary;
using BaseLibrary.Tiles;
using FluidLibrary.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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

			AddMapEntry(Color.Purple);
		}

		public override void RightClick(int i, int j)
		{
			TileEntities.QETank qeTank = mod.GetTileEntity<TileEntities.QETank>(i, j);
			if (qeTank == null) return;

			Main.LocalPlayer.noThrow = 2;

			if (qeTank.Handler == null) BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(qeTank);
			else
			{
				Item item = Main.LocalPlayer.GetHeldItem();
				ref ModFluid fluid = ref qeTank.Handler.GetFluidInSlotByRef(0);
				if (item.type == ItemID.EmptyBucket)
				{
					if (fluid == null || fluid.volume < 255) return;

					switch (fluid.Name)
					{
						case "Water":
							Main.LocalPlayer.PutItemInInventory(ItemID.WaterBucket);
							break;
						case "Lava":
							Main.LocalPlayer.PutItemInInventory(ItemID.LavaBucket);
							break;
						case "Honey":
							Main.LocalPlayer.PutItemInInventory(ItemID.HoneyBucket);
							break;
					}

					item.stack--;
					if (item.stack <= 0) item.TurnToAir();

					fluid.volume -= 255;
					if (fluid.volume <= 0) fluid = null;
				}
				else if (item.type == ItemID.WaterBucket)
				{
					if (fluid != null && (!fluid.Equals(FluidLoader.GetFluid<Water>()) || fluid.volume > 3 * 255)) return;

					if (fluid == null) fluid = FluidLoader.GetFluid<Water>().Clone();

					fluid.volume += 255;
					item.stack--;
					if (item.stack <= 0) item.TurnToAir();
					Main.LocalPlayer.PutItemInInventory(ItemID.EmptyBucket);
				}
				else if (item.type == ItemID.LavaBucket)
				{
					if (fluid != null && (!fluid.Equals(FluidLoader.GetFluid<Lava>()) || fluid.volume > 3 * 255)) return;

					if (fluid == null) fluid = new Lava();

					fluid.volume += 255;
					item.stack--;
					if (item.stack <= 0) item.TurnToAir();
					Main.LocalPlayer.PutItemInInventory(ItemID.EmptyBucket);
				}
				else if (item.type == ItemID.HoneyBucket)
				{
					if (fluid != null && (!fluid.Equals(FluidLoader.GetFluid<Honey>()) || fluid.volume > 3 * 255)) return;

					if (fluid == null) fluid = new Honey();

					fluid.volume += 255;
					item.stack--;
					if (item.stack <= 0) item.TurnToAir();
					Main.LocalPlayer.PutItemInInventory(ItemID.EmptyBucket);
				}
				else BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(qeTank);
			}
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			TileEntities.QETank qeTank = mod.GetTileEntity<TileEntities.QETank>(i, j);
			if (qeTank == null) return;

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

			ModFluid fluid = qeTank.Handler?.GetFluidInSlot(0);
			if (fluid != null)
			{
				Texture2D texture = ModContent.GetTexture(fluid.Texture);
				Vector2 scale = new Vector2(20f / texture.Width, 14f / texture.Height * (fluid.volume / (float)qeTank.Handler.GetSlotLimit(0)));
				spriteBatch.Draw(texture, position + new Vector2(6, 26), null, Color.White, 0f, new Vector2(0, texture.Height), scale, SpriteEffects.None, 0f);
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			TileEntities.QETank qeTank = mod.GetTileEntity<TileEntities.QETank>(i, j);
			BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(qeTank);

			for (int index = 0; index < 3; index++) Item.NewItem(i * 16, j * 16, 32, 32, Utility.ColorToItem(qeTank.frequency[index]));

			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.QETank>());
			qeTank.Kill(i, j);
		}
	}
}