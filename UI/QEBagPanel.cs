using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using QuantumStorage.Items;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace QuantumStorage.UI
{
	public class QEBagPanel : BaseUIPanel<QEBag>, IItemHandlerUI
	{
		private const int SlotSize = 44;
		private new const int Padding = 4;

		private UIGrid<UIContainerSlot> GridItems
		{
			get
			{
				if (gridItems != null) return gridItems;

				gridItems = new UIGrid<UIContainerSlot>(9)
				{
					Width = (0, 1),
					Height = (-28, 1),
					Top = (28, 0),
					OverflowHidden = true,
					ListPadding = Padding
				};

				gridItems.Clear();
				for (int i = 0; i < Container.Handler.Slots; i++)
				{
					UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i)
					{
						Width = (SlotSize, 0),
						Height = (SlotSize, 0)
					};
					gridItems.Add(slot);
				}

				return gridItems;
			}
		}

		public ItemHandler Handler => Container.Handler;
		public string GetTexture(Item item) => "QuantumStorage/Textures/Items/QEBag";
		private UITextButton buttonInitialize;

		private UIButton[] buttonsFrequency;

		private UIGrid<UIContainerSlot> gridItems;

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
						if (Container.frequency[pos] != Colors.None) Main.LocalPlayer.PutItemInInventory(Utility.ColorToItem(Container.frequency[pos]));

						Container.frequency[pos] = Utility.ValidItems[Main.mouseItem.type];
						buttonsFrequency[pos].texture = QuantumStorage.textureGemsMiddle;
						buttonsFrequency[pos].sourceRectangle = new Rectangle(8 * (int)Container.frequency[pos], 0, 8, 10);
						if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Container.item.whoAmI, 1f);

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

				Append(GridItems);
			};
		}

		public override void OnInitialize()
		{
			Width = (16 + (SlotSize + Padding) * 9 - Padding, 0);
			Height = (44 + (SlotSize + Padding) * 3 - Padding, 0);
			this.Center();

			UIText textLabel = new UIText(Container.DisplayName.GetTranslation())
			{
				HAlign = 0.5f,
				HorizontalAlignment = HorizontalAlignment.Center
			};
			Append(textLabel);

			UITextButton buttonReset = new UITextButton("R")
			{
				Size = new Vector2(20),
				RenderPanel = false,
				Padding = (0, 0, 0, 0),
				HoverText = Language.GetText("Mods.QuantumStorage.UI.Reset")
			};
			buttonReset.OnClick += (evt, element) =>
			{
				if (!Container.frequency.IsSet)
				{
					for (int i = 0; i < 3; i++) Main.LocalPlayer.PutItemInInventory(Utility.ColorToItem(Container.frequency[i]));

					Container.frequency = new Frequency();
					if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Container.item.whoAmI, 1f);
				}
				else
				{
					for (int i = 0; i < 3; i++) Main.LocalPlayer.PutItemInInventory(Utility.ColorToItem(Container.frequency[i]));

					Container.frequency = new Frequency();
					if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Container.item.whoAmI, 1f);

					RemoveChild(GridItems);

					InitializeFrequencySelection();
					for (int i = 0; i < 3; i++) Append(buttonsFrequency[i]);
					Append(buttonInitialize);
				}
			};
			Append(buttonReset);

			UIButton buttonLootAll = new UIButton(QuantumStorage.textureLootAll)
			{
				Left = (28,0),
				Size = new Vector2(20),
				HoverText = Language.GetText("LegacyInterface.29")
			};
			buttonLootAll.OnClick += (evt, element) => ItemUtility.LootAll(Container.Handler, Main.LocalPlayer);
			Append(buttonLootAll);

			UIButton buttonDepositAll = new UIButton(QuantumStorage.textureDepositAll)
			{
				Size = new Vector2(20),
				Left = (56, 0),
				HoverText = Language.GetText("LegacyInterface.30")
			};
			buttonDepositAll.OnClick += (evt, element) => ItemUtility.DepositAll(Container.Handler, Main.LocalPlayer);
			Append(buttonDepositAll);

			UIButton buttonQuickStack = new UIButton(QuantumStorage.textureQuickStack)
			{
				Size = new Vector2(20),
				Left = (84, 0),
				HoverText = Language.GetText("LegacyInterface.31")
			};
			buttonQuickStack.OnClick += (evt, element) => ItemUtility.QuickStack(Container.Handler, Main.LocalPlayer);
			Append(buttonQuickStack);

			UITextButton buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				RenderPanel = false,
				Padding = (0, 0, 0, 0),
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
			else Append(GridItems);
		}
	}
}