using BaseLibrary.Items;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
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

		public Guid ID { get; set; }
		public BaseUIPanel UI { get; set; }
		public LegacySoundStyle CloseSound => SoundID.Item1;
		public LegacySoundStyle OpenSound => SoundID.Item1;

		public Frequency frequency;

		public ItemHandler Handler
		{
			get
			{
				if (QSWorld.Instance.QEItemHandlers.TryGetValue(frequency, out ItemHandler handler)) return handler;

				handler = QSWorld.baseItemHandler.Clone();
				QSWorld.Instance.QEItemHandlers.Add((Frequency)frequency.Clone(), handler);
				return handler;
			}
		}

		public QEBag()
		{
			frequency = new Frequency();
		}

		public override ModItem Clone()
		{
			QEBag clone = (QEBag)base.Clone();
			clone.ID = ID;
			clone.frequency = (Frequency)frequency.Clone();
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quantum Entangled Bag");
		}

		public override void SetDefaults()
		{
			ID = Guid.NewGuid();
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = 1;
			item.rare = 0;
		}

		public override bool UseItem(Player player)
		{
			if (player.whoAmI == Main.LocalPlayer.whoAmI) BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(this);

			return true;
		}

		public override bool CanRightClick() => true;

		public override void RightClick(Player player)
		{
			item.stack++;

			if (player.whoAmI == Main.LocalPlayer.whoAmI) BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(this);
		}

		public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
		{
			if (!frequency.IsSet) return;

			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + new Vector2(2, 12) * scale, new Rectangle(6 * (int)frequency[0], 0, 6, 10), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsMiddle, position + new Vector2(12, 12) * scale, new Rectangle(8 * (int)frequency[1], 0, 8, 10), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + new Vector2(24, 12) * scale, new Rectangle(6 * (int)frequency[2], 0, 6, 10), Color.White, 0f, origin, scale, SpriteEffects.FlipHorizontally, 0f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (!frequency.IsSet) return;

			Vector2 position = item.position - Main.screenPosition;
			Vector2 origin = new Vector2(16, 19);

			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + origin, new Rectangle(6 * (int)frequency[0], 0, 6, 10), alphaColor, rotation, origin - new Vector2(2, 14), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsMiddle, position + origin, new Rectangle(8 * (int)frequency[1], 0, 8, 10), alphaColor, rotation, origin - new Vector2(12, 14), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureGemsSide, position + origin, new Rectangle(6 * (int)frequency[2], 0, 6, 10), alphaColor, rotation, origin - new Vector2(24, 14), scale, SpriteEffects.FlipHorizontally, 0f);
		}

		public override TagCompound Save() => new TagCompound
		{
			["ID"] = ID.ToString(),
			["Frequency"] = frequency
		};

		public override void Load(TagCompound tag)
		{
			ID = Guid.Parse(tag.GetString("ID"));
			frequency = tag.Get<Frequency>("Frequency");
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(ID.ToString());
			writer.Write(frequency);
		}

		public override void NetRecieve(BinaryReader reader)
		{
			ID = Guid.Parse(reader.ReadString());
			frequency = reader.ReadFrequency();
		}
	}
}