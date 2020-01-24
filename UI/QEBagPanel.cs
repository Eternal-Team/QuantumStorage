using BaseLibrary;
using BaseLibrary.UI;
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
		private const int Padding = 4;

		private UIGrid<UIContainerSlot> GridItems
		{
			get
			{
				if (gridItems != null) return gridItems;

				gridItems = new UIGrid<UIContainerSlot>(9)
				{
					Width = { Percent = 100 },
					Height = { Pixels = -28, Percent = 100 },
					Y = { Pixels = 28 },
					ItemMargin = SlotMargin
				};

				gridItems.Clear();
				for (int i = 0; i < Container.Handler.Slots; i++)
				{
					UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i)
					{
						Width = { Pixels = SlotSize },
						Height = { Pixels = SlotSize }
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
						if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, Container.item.whoAmI, 1f);

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

				Add(GridItems);
			};
		}

		public QEBagPanel(QEBag bag) : base(bag)
		{
			Width.Pixels = 16 + (SlotSize + SlotMargin) * 9 - SlotMargin;
			Height.Pixels = 44 + (SlotSize + SlotMargin) * 3 - SlotMargin;

			UIText textLabel = new UIText(Container.DisplayName.GetTranslation())
			{
				X = { Percent = 50 },
				HorizontalAlignment = HorizontalAlignment.Center
			};
			Add(textLabel);

			UITextButton buttonReset = new UITextButton("R")
			{
				Size = new Vector2(20),
				RenderPanel = false,
				Padding = BaseLibrary.UI.Padding.Zero,
				HoverText = Language.GetText("Mods.QuantumStorage.UI.Reset")
			};
			buttonReset.OnClick += args =>
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

					Remove(GridItems);

					InitializeFrequencySelection();
					for (int i = 0; i < 3; i++) Add(buttonsFrequency[i]);
					Add(buttonInitialize);
				}
			};
			Add(buttonReset);

			UIButton buttonLootAll = new UIButton(QuantumStorage.textureLootAll)
			{
				X = { Pixels = 28 },
				Size = new Vector2(20),
				HoverText = Language.GetText("LegacyInterface.29")
			};
			buttonLootAll.OnClick += args => ItemUtility.LootAll(Container.Handler, Main.LocalPlayer);
			Add(buttonLootAll);

			UIButton buttonDepositAll = new UIButton(QuantumStorage.textureDepositAll)
			{
				Size = new Vector2(20),
				X = { Pixels = 56 },
				HoverText = Language.GetText("LegacyInterface.30")
			};
			buttonDepositAll.OnClick += args => ItemUtility.DepositAll(Container.Handler, Main.LocalPlayer);
			Add(buttonDepositAll);

			UIButton buttonQuickStack = new UIButton(QuantumStorage.textureQuickStack)
			{
				Size = new Vector2(20),
				X = { Pixels = 84 },
				HoverText = Language.GetText("LegacyInterface.31")
			};
			buttonQuickStack.OnClick += args => ItemUtility.QuickStack(Container.Handler, Main.LocalPlayer);
			Add(buttonQuickStack);

			UITextButton buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				X = { Percent = 100 },
				RenderPanel = false,
				Padding = BaseLibrary.UI.Padding.Zero,
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
			else Add(GridItems);
		}
	}
}