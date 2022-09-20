using BaseLibrary.UI;
using BaseLibrary.Utility;
using FluidLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace QuantumStorage.Tiles;

public class QETank : ModTile
{
	public override string Texture => QuantumStorage.TexturePath + "Tiles/QETank";

	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = false;
		Main.tileLavaDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
		TileObjectData.newTile.Origin = new Point16(0, 1);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
		TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TileEntities.QETank>().Hook_AfterPlacement, -1, 0, false);
		TileObjectData.addTile(Type);

		MineResist = 5f;

		ModTranslation name = CreateMapEntryName();
		AddMapEntry(Color.Purple, name);
	}

	public override bool RightClick(int i, int j)
	{
		if (!TileEntityUtility.TryGetTileEntity(i, j, out TileEntities.QETank qeTank)) return false;

		Main.LocalPlayer.noThrow = 2;

		FluidStorage storage = qeTank.GetFluidStorage();
		if (storage is null)
		{
			PanelUI.Instance?.HandleUI(qeTank);
			return true;
		}

		Item item = Main.LocalPlayer.HeldItem;
		FluidStack stack = storage[0];
		int maxVolume = storage.MaxVolumeFor(0);
		if (item.type == ItemID.EmptyBucket)
		{
			if (stack.Fluid is null || stack.Volume < 255) return false;

			if (stack.Fluid is Water)
				Utility.SpawnItem(Main.LocalPlayer, ItemID.WaterBucket);
			else if (stack.Fluid is Lava)
				Utility.SpawnItem(Main.LocalPlayer, ItemID.LavaBucket);
			else if (stack.Fluid is Honey)
				Utility.SpawnItem(Main.LocalPlayer, ItemID.HoneyBucket);

			item.stack--;
			if (item.stack <= 0) item.TurnToAir();

			storage.ModifyStackSize(Main.LocalPlayer, 0, -255);
		}
		else if (item.type == ItemID.WaterBucket)
		{
			FluidStack toInsert = new FluidStack(FluidLoader.GetFluid(FluidLoader.FluidType<Water>()), 255);
			if (!storage.InsertFluid(Main.LocalPlayer, 0, ref toInsert))
				return false;

			item.stack--;
			if (item.stack <= 0) item.TurnToAir();
			Utility.SpawnItem(Main.LocalPlayer, ItemID.EmptyBucket);
		}
		else if (item.type == ItemID.LavaBucket)
		{
			FluidStack toInsert = new FluidStack(FluidLoader.GetFluid(FluidLoader.FluidType<Lava>()), 255);
			if (!storage.InsertFluid(Main.LocalPlayer, 0, ref toInsert))
				return false;

			item.stack--;
			if (item.stack <= 0) item.TurnToAir();
			Utility.SpawnItem(Main.LocalPlayer, ItemID.EmptyBucket);
		}
		else if (item.type == ItemID.HoneyBucket)
		{
			FluidStack toInsert = new FluidStack(FluidLoader.GetFluid(FluidLoader.FluidType<Honey>()), 255);
			if (!storage.InsertFluid(Main.LocalPlayer, 0, ref toInsert))
				return false;

			item.stack--;
			if (item.stack <= 0) item.TurnToAir();
			Utility.SpawnItem(Main.LocalPlayer, ItemID.EmptyBucket);
		}
		else PanelUI.Instance?.HandleUI(qeTank);

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
		if (!TileEntityUtility.TryGetTileEntity(i, j, out TileEntities.QETank qeTank)) return;

		Tile tile = Main.tile[i, j];
		if (!tile.IsTopLeft()) return;

		Vector2 position = new Point16(i, j).ToScreenCoordinates();

		spriteBatch.Draw(QuantumStorage.TextureGemsSide.Value, position + new Vector2(5, 5), new Rectangle(6 * (int)qeTank.Frequency[0], 0, 6, 10), Color.White, 0f, new Vector2(3, 5), 1f, SpriteEffects.None, 0f);
		spriteBatch.Draw(QuantumStorage.TextureGemsMiddle.Value, position + new Vector2(12, 0), new Rectangle(8 * (int)qeTank.Frequency[1], 0, 8, 10), Color.White);
		spriteBatch.Draw(QuantumStorage.TextureGemsSide.Value, position + new Vector2(24, 0), new Rectangle(6 * (int)qeTank.Frequency[2], 0, 6, 10), Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 0f);

		FluidStorage storage = qeTank.GetFluidStorage();
		FluidStack stack = storage[0];
		if (stack.Fluid is not null)
		{
			Texture2D texture = ModContent.Request<Texture2D>(stack.Fluid.Texture).Value;
			Vector2 scale = new Vector2(20f / texture.Width, 14f / texture.Height * (stack.Volume / (float)storage.MaxVolumeFor(0)));
			// Color color = fluid is Lava
			// 	? Color.White
			// 	: new[]
			// 	{
			// 		Lighting.GetColor(i, j),
			// 		Lighting.GetColor(i + 1, j),
			// 		Lighting.GetColor(i, j + 1),
			// 		Lighting.GetColor(i + 1, j + 1)
			// 	}.AverageColor();
			spriteBatch.Draw(texture, position + new Vector2(6, 26), null, Color.White, 0f, new Vector2(0, texture.Height), scale, SpriteEffects.None, 0f);
		}
	}

	public override void KillMultiTile(int i, int j, int frameX, int frameY)
	{
		if (!TileEntityUtility.TryGetTileEntity(i, j, out TileEntities.QETank qeTank)) return;

		PanelUI.Instance?.CloseUI(qeTank);

		qeTank.Kill(i, j);

		Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Items.QETank>());
	}
}