using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
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

		private UITank TankFluid =>
			tankFluid ?? (tankFluid = new UITank(Container)
			{
				Width = (40, 0),
				Height = (-44, 1),
				Top = (36, 0),
				HAlign = 0.5f
			});

		private UIButton[] buttonsFrequency;
		private UITextButton buttonInitialize;

		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (172, 0);
			this.Center();

			UIText textLabel = new UIText(Language.GetText("Mods.QuantumStorage.MapObject.QETank"))
			{
				HAlign = 0.5f
			};
			Append(textLabel);

			UITextButton buttonReset = new UITextButton("R")
			{
				Size = new Vector2(20),
				RenderPanel = false,
				HoverText = Language.GetText("Mods.QuantumStorage.UI.Reset")
			};
			buttonReset.OnClick += (evt, element) =>
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

					RemoveChild(TankFluid);

					InitializeFrequencySelection();
					for (int i = 0; i < 3; i++) Append(buttonsFrequency[i]);
					Append(buttonInitialize);
				}
			};
			Append(buttonReset);

			UITextButton buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				RenderPanel = false,
				HoverText = Language.GetText("Mods.BaseLibrary.UI.Close")
			};
			buttonClose.OnClick += (evt, element) => BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(Container);
			Append(buttonClose);

			if (!Container.frequency.IsSet)
			{
				InitializeFrequencySelection();

				for (int i = 0; i < 3; i++) Append(buttonsFrequency[i]);
				Append(buttonInitialize);
			}
			else Append(TankFluid);
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
					HAlign = 0.4f + 0.1f * pos,
					VAlign = 0.5f
				};
				buttonsFrequency[i].OnClick += (evt, element) =>
				{
					if (Utility.ValidItems.ContainsKey(Main.mouseItem.type))
					{
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
				Width = (-64, 1),
				Height = (40, 0),
				VAlign = 1,
				HAlign = 0.5f
			};
			buttonInitialize.OnClick += (evt, element) =>
			{
				if (!Container.frequency.IsSet) return;

				for (int i = 0; i < 3; i++) RemoveChild(buttonsFrequency[i]);
				RemoveChild(buttonInitialize);

				Append(TankFluid);
			};
		}
	}
}