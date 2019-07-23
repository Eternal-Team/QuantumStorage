using BaseLibrary.Items;
using BaseLibrary.UI;
using ContainerLibrary;
using FluidLibrary.Content;
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
	public class QEBucket : BaseItem, IHasUI, IFluidHandler
	{
		public override string Texture => "QuantumStorage/Textures/Items/QEBucket";

		public Guid ID { get; set; }
		public BaseUIPanel UI { get; set; }
		public LegacySoundStyle CloseSound => SoundID.Item1;
		public LegacySoundStyle OpenSound => SoundID.Item1;

		public Frequency frequency;

		public FluidHandler Handler
		{
			get
			{
				if (QSWorld.Instance.QEFluidHandlers.TryGetValue(frequency, out FluidHandler handler)) return handler;

				handler = QSWorld.baseFluidHandler.Clone();
				QSWorld.Instance.QEFluidHandlers.Add((Frequency)frequency.Clone(), handler);
				return handler;
			}
		}

		public QEBucket()
		{
			frequency = new Frequency();
		}

		public override ModItem Clone()
		{
			QEBucket clone = (QEBucket)base.Clone();
			clone.ID = ID;
			clone.frequency = (Frequency)frequency.Clone();
			return clone;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quantum Entangled Bucket");
		}

		public override void SetDefaults()
		{
			ID = Guid.NewGuid();
			item.useTime = 5;
			item.useAnimation = 5;
			item.useStyle = 1;
			item.rare = 0;
			item.autoReuse = true;
		}

		public override bool AltFunctionUse(Player player) => true;

		public override bool UseItem(Player player)
		{
			ref ModFluid fluid = ref Handler.GetFluidInSlotByRef(0);
			int targetX = Player.tileTargetX;
			int targetY = Player.tileTargetY;
			Tile tile = Main.tile[targetX, targetY];
			int liquidType = FluidLibrary.FluidLibrary.GetFluidIDByName(fluid?.Name);

			if (player.altFunctionUse == 2)
			{
				if (fluid != null)
				{
					if (!tile.nactive() || !Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type])
					{
						if (tile.liquid == 0 || tile.liquidType() == liquidType)
						{
							Main.PlaySound(19, (int)player.position.X, (int)player.position.Y);

							if (tile.liquid == 0) tile.liquidType(liquidType);

							int volume = Math.Min(fluid.volume, 255 - tile.liquid);
							tile.liquid += (byte)volume;
							fluid.volume -= volume;
							if (fluid.volume <= 0) fluid = null;

							WorldGen.SquareTileFrame(targetX, targetY);

							if (Main.netMode == 1) NetMessage.sendWater(targetX, targetY);

							return true;
						}
					}
				}
			}
			else
			{
				if ((fluid == null || liquidType == tile.liquidType()) && tile.liquid > 0)
				{
					Main.PlaySound(19, (int)player.position.X, (int)player.position.Y);

					if (fluid == null) fluid = FluidLoader.GetFluid(FluidLibrary.FluidLibrary.GetFluidNameByID(tile.liquidType()));

					int drain = Math.Min(tile.liquid, Handler.GetSlotLimit(0) - fluid.volume);
					fluid.volume += drain;

					tile.liquid -= (byte)drain;

					if (tile.liquid <= 0)
					{
						tile.lava(false);
						tile.honey(false);
					}

					WorldGen.SquareTileFrame(targetX, targetY, false);
					if (Main.netMode == 1) NetMessage.sendWater(targetX, targetY);
					else Liquid.AddWater(targetX, targetY);
				}
			}

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

			spriteBatch.Draw(QuantumStorage.textureRingBig, position + new Vector2(4, 14) * scale, new Rectangle(0, 4 * (int)frequency[0], 22, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureRingBig, position + new Vector2(4, 18) * scale, new Rectangle(0, 4 * (int)frequency[1], 22, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureRingSmall, position + new Vector2(6, 22) * scale, new Rectangle(0, 4 * (int)frequency[2], 18, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			if (!frequency.IsSet) return;

			Vector2 position = item.position - Main.screenPosition;
			Vector2 origin = new Vector2(15, 16);

			spriteBatch.Draw(QuantumStorage.textureRingBig, position + origin, new Rectangle(0, 4 * (int)frequency[0], 22, 4), alphaColor, rotation, origin - new Vector2(4, 16), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureRingBig, position + origin, new Rectangle(0, 4 * (int)frequency[1], 22, 4), alphaColor, rotation, origin - new Vector2(4, 20), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureRingSmall, position + origin, new Rectangle(0, 4 * (int)frequency[2], 18, 4), alphaColor, rotation, origin - new Vector2(6, 24), scale, SpriteEffects.None, 0f);
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