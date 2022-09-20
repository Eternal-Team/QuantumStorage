using System;
using System.IO;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using FluidLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage.Items;

public class QEBucket : BaseItem, IHasUI, IFluidStorage
{
	public override string Texture => QuantumStorage.TexturePath + "Items/QEBucket";

	private Guid ID;
	public Frequency Frequency;

	public override ModItem Clone(Item item)
	{
		QEBucket clone = (QEBucket)base.Clone(item);
		clone.ID = ID;
		clone.Frequency = Frequency.Clone();
		return clone;
	}

	public override void OnCreate(ItemCreationContext context)
	{
		ID = Guid.NewGuid();
	}

	public override void SetDefaults()
	{
		Item.useTime = 5;
		Item.useAnimation = 5;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.sellPrice(gold: 6);
		Item.autoReuse = true;
	}

	public override bool AltFunctionUse(Player player) => true;

	public override bool? UseItem(Player player)
	{
		FluidStorage storage = GetFluidStorage();
		if (storage is null)
			return false;

		int targetX = Player.tileTargetX;
		int targetY = Player.tileTargetY;
		Tile tile = Main.tile[targetX, targetY];

		// place
		if (player.altFunctionUse == 2)
		{
			if (tile.HasUnactuatedTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] && tile.TileType != 546)
				return false;

			var fluidStack = storage[0];
			if (fluidStack.Fluid is null)
				return false;

			if (tile.LiquidAmount != 0 && tile.LiquidType != fluidStack.Fluid.Type)
				return false;

			if (!storage.RemoveFluid(player, 0, out FluidStack fluid, byte.MaxValue - tile.LiquidAmount))
				return false;

			SoundEngine.PlaySound(SoundID.Splash, player.position);

			tile.LiquidType = fluidStack.Fluid?.Type ?? 0;
			tile.LiquidAmount += (byte)fluid.Volume;

			WorldGen.SquareTileFrame(targetX, targetY);

			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.sendWater(targetX, targetY);
		}
		// remove
		else
		{
			if (tile.LiquidAmount <= 0)
				return false;

			FluidStack stack = new FluidStack(FluidLoader.GetFluid(tile.LiquidType), tile.LiquidAmount);
			if (!storage.InsertFluid(player, 0, ref stack))
				return false;

			SoundEngine.PlaySound(SoundID.Splash, player.position);

			byte remaining = (byte)stack.Volume;
			tile.LiquidAmount = remaining;
			if (remaining <= 0) tile.LiquidType = LiquidID.Water;

			WorldGen.SquareTileFrame(targetX, targetY, false);

			if (Main.netMode == NetmodeID.MultiplayerClient)
				NetMessage.sendWater(targetX, targetY);
			else
				Liquid.AddWater(targetX, targetY);
		}

		return true;
	}

	public override bool ConsumeItem(Player player) => false;

	public override bool CanRightClick() => true;

	public override void RightClick(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
			PanelUI.Instance?.HandleUI(this);
	}

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		spriteBatch.Draw(QuantumStorage.TextureRingBig.Value, position + new Vector2(4, 14) * scale, new Rectangle(0, 4 * (int)Frequency[0], 22, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(QuantumStorage.TextureRingBig.Value, position + new Vector2(4, 18) * scale, new Rectangle(0, 4 * (int)Frequency[1], 22, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(QuantumStorage.TextureRingSmall.Value, position + new Vector2(6, 22) * scale, new Rectangle(0, 4 * (int)Frequency[2], 18, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
	}

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
	{
		Vector2 position = new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height - 14f + 2f);

		spriteBatch.Draw(QuantumStorage.TextureRingBig.Value, position, new Rectangle(0, 4 * (int)Frequency[0], 22, 4), alphaColor, rotation, new Vector2(11, 0), scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(QuantumStorage.TextureRingBig.Value, position, new Rectangle(0, 4 * (int)Frequency[1], 22, 4), alphaColor, rotation, new Vector2(11, -4), scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(QuantumStorage.TextureRingSmall.Value, position, new Rectangle(0, 4 * (int)Frequency[2], 18, 4), alphaColor, rotation, new Vector2(9, -8), scale, SpriteEffects.FlipHorizontally, 0f);
	}

	public override void SaveData(TagCompound tag)
	{
		tag["ID"] = ID;
		tag["Frequency"] = Frequency;
	}

	public override void LoadData(TagCompound tag)
	{
		ID = tag.Get<Guid>("ID");
		Frequency = tag.Get<Frequency>("Frequency");
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(ID);
		writer.Write(Frequency);
	}

	public override void NetReceive(BinaryReader reader)
	{
		ID = reader.ReadGuid();
		Frequency = reader.ReadFrequency();
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.EmptyBucket)
			.AddIngredient(ItemID.HallowedBar, 4)
			.AddIngredient(ItemID.SoulofSight, 5)
			.AddTile(TileID.SteampunkBoiler)
			.Register();
	}

	public Guid GetID() => ID;

	public FluidStorage GetFluidStorage()
	{
		if (!Frequency.IsSet) return null;

		QuantumStorageSystem system = ModContent.GetInstance<QuantumStorageSystem>();

		if (system.QEFluidHandlers.TryGetValue(Frequency, out FluidStorage storage))
			return storage;

		storage = new QuantumStorageSystem.BaseFluidStorage();
		system.QEFluidHandlers.Add(Frequency, storage);
		return storage;

		// Net.SendItemFrequency(frequency);
	}
}