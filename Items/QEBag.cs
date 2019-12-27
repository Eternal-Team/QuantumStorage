using BaseLibrary;
using BaseLibrary.Items;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace QuantumStorage.Items
{
	public class QEBag : BaseItem, IItemHandler, IHasUI
	{
		public override string Texture => "QuantumStorage/Textures/Items/QEBag";

		public override bool CloneNewInstances => true;

		public Guid UUID { get; set; }
		public BaseUIPanel UI { get; set; }
		public LegacySoundStyle CloseSound => SoundID.Item1;
		public LegacySoundStyle OpenSound => SoundID.Item1;

		public Frequency frequency;

		public ItemHandler Handler
		{
			get
			{
				if (!frequency.IsSet) return null;

				ItemPair pair = ModContent.GetInstance<QSWorld>().QEItemHandlers.FirstOrDefault(itemPair => Equals(itemPair.Frequency, frequency));
				if (pair != null) return pair.Handler;

				pair = QSWorld.baseItemPair.Clone();
				pair.Frequency = frequency;

				ModContent.GetInstance<QSWorld>().QEItemHandlers.Add(pair);
				Net.SendItemFrequency(frequency);
				return pair.Handler;
			}
		}

		public QEBag()
		{
			frequency = new Frequency();
		}

		public override ModItem Clone()
		{
			QEBag clone = (QEBag)base.Clone();
			clone.UUID = UUID;
			clone.frequency = (Frequency)frequency.Clone();
			return clone;
		}

		public override void SetDefaults()
		{
			UUID = Guid.NewGuid();
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = 1;
			item.rare = ItemRarityID.Pink;
			item.value = Item.sellPrice(gold: 6);
		}

		public override bool UseItem(Player player)
		{
			if (player.whoAmI == Main.LocalPlayer.whoAmI) BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(this);

			return true;
		}

		public override bool ConsumeItem(Player player) => false;

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			if (player.whoAmI == Main.LocalPlayer.whoAmI) BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(this);
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + new Vector2(2, 12) * scale, new Rectangle(6 * (int)frequency[0], 0, 6, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsMiddle, position + new Vector2(12, 12) * scale, new Rectangle(8 * (int)frequency[1], 0, 8, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + new Vector2(24, 12) * scale, new Rectangle(6 * (int)frequency[2], 0, 6, 10), Color.White, 0f, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 position = new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - 17f + 2f);

			spriteBatch.Draw(QuantumStorage.textureGemsSide, position, new Rectangle(6 * (int)frequency[0], 0, 6, 10), alphaColor, rotation, new Vector2(14, 5), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsMiddle, position, new Rectangle(8 * (int)frequency[1], 0, 8, 10), alphaColor, rotation, new Vector2(4, 5), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsSide, position, new Rectangle(6 * (int)frequency[2], 0, 6, 10), alphaColor, rotation, new Vector2(-8, 5), scale, SpriteEffects.FlipHorizontally, 0f);
		}

		public override TagCompound Save() => new TagCompound
		{
			["UUID"] = UUID,
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag)
		{
			UUID = tag.Get<Guid>("UUID");
			frequency = tag.Get<Frequency>("Frequency");
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(UUID);
			writer.Write(frequency);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			UUID = reader.ReadGUID();
			frequency = reader.ReadFrequency();
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Leather, 12);
			recipe.AddIngredient(ItemID.HallowedBar, 4);
			recipe.AddIngredient(ItemID.SoulofMight, 5);
			recipe.AddTile(TileID.SteampunkBoiler);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}