using System;
using System.IO;
using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.Utility;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuantumStorage.Global;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage.Items;

public class QEBag : BaseItem, IItemStorage, IHasUI
{
	public override string Texture => QuantumStorage.TexturePath + "Items/QEBag";

	public Guid ID;
	public Frequency Frequency;

	public override void OnCreate(ItemCreationContext context)
	{
		ID = Guid.NewGuid();
		Frequency = new Frequency();

		// BagSyncSystem.Instance.AllBags.Add(ID, this);
	}

	public override ModItem Clone(Item item)
	{
		QEBag clone = (QEBag)base.Clone(item);
		clone.ID = ID;
		clone.Frequency = Frequency;
		// if (BagSyncSystem.Instance.AllBags.ContainsKey(ID)) BagSyncSystem.Instance.AllBags.Remove(ID);
		// BagSyncSystem.Instance.AllBags.Add(ID, clone);
		return clone;
	}

	public override void SetStaticDefaults()
	{
		SacrificeTotal = 1;
	}

	public override void SetDefaults()
	{
		Item.useTime = 5;
		Item.useAnimation = 5;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.rare = ItemRarityID.Pink;
		Item.value = Item.sellPrice(gold: 6);

		OnCreate(null);
	}

	public override bool? UseItem(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
			PanelUI.Instance?.HandleUI(this);

		return true;
	}

	public override bool ConsumeItem(Player player) => false;

	public override bool CanRightClick() => true;

	public override void RightClick(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
			PanelUI.Instance?.HandleUI(this);
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

	public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
	{
		spriteBatch.Draw(ModContent.Request<Texture2D>(QuantumStorage.TexturePath + "Tiles/GemSide_0").Value, position + new Vector2(2, 12) * scale, new Rectangle(6 * (int)Frequency[0], 0, 6, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModContent.Request<Texture2D>(QuantumStorage.TexturePath + "Tiles/GemMiddle_0").Value, position + new Vector2(12, 12) * scale, new Rectangle(8 * (int)Frequency[1], 0, 8, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModContent.Request<Texture2D>(QuantumStorage.TexturePath + "Tiles/GemSide_0").Value, position + new Vector2(24, 12) * scale, new Rectangle(6 * (int)Frequency[2], 0, 6, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0f);
	}

	public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
	{
		Vector2 position = new Vector2(Item.position.X - Main.screenPosition.X + Item.width * 0.5f, Item.position.Y - Main.screenPosition.Y + Item.height - 17f + 2f);

		spriteBatch.Draw(ModContent.Request<Texture2D>(QuantumStorage.TexturePath + "Tiles/GemSide_0").Value, position, new Rectangle(6 * (int)Frequency[0], 0, 6, 10), alphaColor, rotation, new Vector2(14, 5), scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModContent.Request<Texture2D>(QuantumStorage.TexturePath + "Tiles/GemMiddle_0").Value, position, new Rectangle(8 * (int)Frequency[1], 0, 8, 10), alphaColor, rotation, new Vector2(4, 5), scale, SpriteEffects.None, 0f);
		spriteBatch.Draw(ModContent.Request<Texture2D>(QuantumStorage.TexturePath + "Tiles/GemSide_0").Value, position, new Rectangle(6 * (int)Frequency[2], 0, 6, 10), alphaColor, rotation, new Vector2(-8, 5), scale, SpriteEffects.FlipHorizontally, 0f);
	}

	public override void AddRecipes()
	{
		CreateRecipe()
			.AddIngredient(ItemID.Leather, 12)
			.AddIngredient(ItemID.HallowedBar, 4)
			.AddIngredient(ItemID.SoulofMight, 5)
			.AddTile(TileID.SteampunkBoiler)
			.Register();
	}

	public ItemStorage GetItemStorage()
	{
		if (!Frequency.IsSet) return null;

		QuantumStorageSystem system = ModContent.GetInstance<QuantumStorageSystem>();

		if (system.QEItemHandlers.TryGetValue(Frequency, out ItemStorage storage))
			return storage;

		storage = new QuantumStorageSystem.BaseItemStorage();
		system.QEItemHandlers.Add(Frequency, storage);
		return storage;

		// Net.SendItemFrequency(Frequency);
	}

	public Guid GetID() => ID;
}