using BaseLibrary.Items;
using BaseLibrary.UI;
using ContainerLibrary;
using FluidLibrary.Content;
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
				if (!frequency.IsSet) return null;

				FluidPair pair = QSWorld.Instance.QEFluidHandlers.FirstOrDefault(fluidPair => Equals(fluidPair.Frequency, frequency));
				if (pair != null) return pair.Handler;

				pair = QSWorld.baseFluidPair.Clone();
				pair.Frequency = frequency;

				QSWorld.Instance.QEFluidHandlers.Add(pair);
				Net.SendFluidFrequency(frequency);
				return pair.Handler;
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
			if (Handler == null) return false;

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

							Handler.Shrink(0,volume);

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

					if (fluid == null) fluid = FluidLoader.GetFluid(FluidLibrary.FluidLibrary.GetFluidNameByID(tile.liquidType())).Clone();

					int drain = Math.Min(tile.liquid, Handler.GetSlotLimit(0) - fluid.volume);

					tile.liquid -= (byte)drain;
					Handler.Grow(0, drain);

					if (tile.liquid <= 0)
					{
						tile.lava(false);
						tile.honey(false);
					}

					WorldGen.SquareTileFrame(targetX, targetY, false);
					if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.sendWater(targetX, targetY);
					else Liquid.AddWater(targetX, targetY);
				}
			}

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
			spriteBatch.Draw(QuantumStorage.textureRingBig, position + new Vector2(4, 14) * scale, new Rectangle(0, 4 * (int)frequency[0], 22, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureRingBig, position + new Vector2(4, 18) * scale, new Rectangle(0, 4 * (int)frequency[1], 22, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureRingSmall, position + new Vector2(6, 22) * scale, new Rectangle(0, 4 * (int)frequency[2], 18, 4), Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Vector2 position = new Vector2(item.position.X - Main.screenPosition.X + item.width * 0.5f, item.position.Y - Main.screenPosition.Y + item.height - 14f + 2f);

			spriteBatch.Draw(QuantumStorage.textureRingBig, position, new Rectangle(0, 4 * (int)frequency[0], 22, 4), alphaColor, rotation, new Vector2(11, 0), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureRingBig, position, new Rectangle(0, 4 * (int)frequency[1], 22, 4), alphaColor, rotation, new Vector2(11, -4), scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(QuantumStorage.textureRingSmall, position, new Rectangle(0, 4 * (int)frequency[2], 18, 4), alphaColor, rotation, new Vector2(9, -8), scale, SpriteEffects.FlipHorizontally, 0f);
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