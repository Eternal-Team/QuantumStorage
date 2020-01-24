using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using QuantumStorage.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace QuantumStorage.UI
{
	public class QETankPanel : BaseUIPanel<QETank>
	{
		private UITank tankFluid;

		private UITank TankFluid => tankFluid ?? (tankFluid = new UITank(Container)
		{
			Width = { Pixels = 40 },
			Height = { Pixels = -44, Percent = 100 },
			Y = { Pixels = 36 },
			X = { Percent = 50 }
		});

		private UIButton[] buttonsFrequency;
		private UITextButton buttonInitialize;

		public QETankPanel(QETank tank) : base(tank)
		{
			Width.Percent = 408;
			Height.Pixels = 172;


			UIText textLabel = new UIText(Language.GetText("Mods.QuantumStorage.MapObject.QETank"))
			{
				X = { Percent = 50 },
				HorizontalAlignment = HorizontalAlignment.Center
			};
			Add(textLabel);

			UITextButton buttonReset = new UITextButton("R")
			{
				Size = new Vector2(20),
				RenderPanel = false,
				Padding = Padding.Zero,
				HoverText = Language.GetText("Mods.QuantumStorage.UI.Reset")
			};
			buttonReset.OnClick += args =>
			{
				if (!Container.frequency.IsSet)
				{
					for (int i = 0; i < 3; i++) Main.LocalPlayer.PutItemInInventory(Utility.ColorToItem(Container.frequency[i]));

					Container.frequency = new Frequency();
					if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, Container.ID, Container.Position.X, Container.Position.Y);
				}
				else
				{
					for (int i = 0; i < 3; i++) Main.LocalPlayer.PutItemInInventory(Utility.ColorToItem(Container.frequency[i]));

					Container.frequency = new Frequency();
					if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, Container.ID, Container.Position.X, Container.Position.Y);

					Remove(TankFluid);

					InitializeFrequencySelection();
					for (int i = 0; i < 3; i++) Add(buttonsFrequency[i]);
					Add(buttonInitialize);
				}
			};
			Add(buttonReset);

			UITextButton buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				X = { Percent = 100 },
				RenderPanel = false,
				Padding = Padding.Zero,
				HoverText = Language.GetText("Mods.BaseLibrary.UI.Close")
			};
			buttonClose.OnClick += args => PanelUI.Instance.CloseUI(Container);
			Add(buttonClose);

			if (!Container.frequency.IsSet)
			{
				InitializeFrequencySelection();

				for (int i = 0; i < 3; i++) Add(buttonsFrequency[i]);
				Add(buttonInitialize);
			}
			else Add(TankFluid);
		}

		private void InitializeFrequencySelection()
		{
			buttonsFrequency = new UIButton[3];
			for (int i = 0; i < 3; i++)
			{
				int pos = i;
				buttonsFrequency[i] = new UIButton(QuantumStorage.textureGemsMiddle, new Rectangle(8 * (int)Container.frequency[pos], 0, 8, 10))
				{
					Size = new Vector2(16, 20),
					X = { Percent = 40 + 10 * pos },
					Y = { Percent = 50 }
				};
				buttonsFrequency[i].OnClick += args =>
				{
					if (Utility.ValidItems.ContainsKey(Main.mouseItem.type))
					{
						if (Container.frequency[pos] != Colors.None) Main.LocalPlayer.PutItemInInventory(Utility.ColorToItem(Container.frequency[pos]));

						Container.frequency[pos] = Utility.ValidItems[Main.mouseItem.type];
						buttonsFrequency[pos].texture = QuantumStorage.textureGemsMiddle;
						buttonsFrequency[pos].sourceRectangle = new Rectangle(8 * (int)Container.frequency[pos], 0, 8, 10);
						if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, Container.ID, Container.Position.X, Container.Position.Y);

						Main.mouseItem.stack--;
						if (Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();

						if (Container.frequency.IsSet) buttonInitialize.text = Language.GetTextValue("Mods.QuantumStorage.UI.Initialize");
					}
				};
			}

			buttonInitialize = new UITextButton(Language.GetText("Mods.QuantumStorage.UI.InsertGems"))
			{
				Width = { Pixels = -64, Percent = 100 },
				Height = { Pixels = 40 },
				Y = { Percent = 100 },
				X = { Percent = 50 }
			};
			buttonInitialize.OnClick += args =>
			{
				if (!Container.frequency.IsSet) return;

				for (int i = 0; i < 3; i++) Remove(buttonsFrequency[i]);
				Remove(buttonInitialize);

				Add(TankFluid);
			};
		}
	}
}