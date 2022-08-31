using BaseLibrary;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuantumStorage.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace QuantumStorage.UI;

public class QEChestPanel : BaseUIPanel<QEChest>, IItemStorageUI
{
	protected const int SlotSize = 44;
	protected const int SlotMargin = 4;

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
				Settings = { ItemMargin = SlotMargin }
			};

			gridItems.Clear();
			for (int i = 0; i < Container.GetItemStorage().Count; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(Container.GetItemStorage(), i)
				{
					Width = { Pixels = SlotSize },
					Height = { Pixels = SlotSize }
				};
				gridItems.Add(slot);
			}

			return gridItems;
		}
	}

	private UIText buttonInitialize;
	private UITexture[] buttonsFrequency;
	private UIGrid<UIContainerSlot> gridItems;

	public QEChestPanel(QEChest chest) : base(chest)
	{
		Width.Pixels = 16 + (SlotSize + SlotMargin) * 9 - SlotMargin;
		Height.Pixels = 44 + (SlotSize + SlotMargin) * 3 - SlotMargin;

		UIText textLabel = new UIText(Language.GetText("Mods.QuantumStorage.MapObject.QEChest"))
		{
			X = { Percent = 50 },
			Settings = { HorizontalAlignment = HorizontalAlignment.Center }
		};
		Add(textLabel);

		UIText buttonReset = new UIText("R")
		{
			Height = { Pixels = 20 },
			Width = { Pixels = 20 },
			HoverText = Language.GetText("Mods.QuantumStorage.UI.Reset")
		};
		buttonReset.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;

			if (!Container.Frequency.IsSet)
			{
				for (int i = 0; i < 3; i++) Main.LocalPlayer.QuickSpawnItem(new EntitySource_Misc("QuantumStorage.Reset"), Utility.ColorToItem(Container.Frequency[i]));

				Container.Frequency = new Frequency();
				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ((ModTileEntity)Container).ID, Container.Position.X, Container.Position.Y);
			}
			else
			{
				for (int i = 0; i < 3; i++) Main.LocalPlayer.QuickSpawnItem(new EntitySource_Misc("QuantumStorage.Reset"), Utility.ColorToItem(Container.Frequency[i]));

				Container.Frequency = new Frequency();
				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ((ModTileEntity)Container).ID, Container.Position.X, Container.Position.Y);

				Remove(GridItems);

				InitializeFrequencySelection();
				for (int i = 0; i < 3; i++) Add(buttonsFrequency[i]);
				Add(buttonInitialize);
			}
		};
		Add(buttonReset);

		UIText buttonClose = new UIText("X")
		{
			Height = { Pixels = 20 },
			Width = { Pixels = 20 },
			X = { Percent = 100 },
			HoverText = Language.GetText("Mods.BaseLibrary.UI.Close")
		};
		buttonClose.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;

			PanelUI.Instance.CloseUI(Container);
			args.Handled = true;
		};
		buttonClose.OnMouseEnter += _ => buttonClose.Settings.TextColor = Color.Red;
		buttonClose.OnMouseLeave += _ => buttonClose.Settings.TextColor = Color.White;
		Add(buttonClose);

		if (!Container.Frequency.IsSet)
		{
			InitializeFrequencySelection();
			for (int i = 0; i < 3; i++) Add(buttonsFrequency[i]);
			Add(buttonInitialize);
		}
		else Add(GridItems);
	}

	private void InitializeFrequencySelection()
	{
		buttonsFrequency = new UITexture[3];
		for (int i = 0; i < 3; i++)
		{
			int pos = i;
			buttonsFrequency[i] = new UITexture(ModContent.Request<Texture2D>(QuantumStorage.TexturePath+"Tiles/GemMiddle_0"))
			{
				Size = new Vector2(16, 20),
				X = { Percent = 40 + 10 * pos },
				Y = { Percent = 50 },
				Settings = {SourceRectangle = new Rectangle(8 * (int)Container.Frequency[pos], 0, 8, 10)}
			};
			buttonsFrequency[i].OnMouseDown += args =>
			{
				if (args.Button != MouseButton.Left) return;
				args.Handled = true;

				if (Utility.ValidItems.ContainsKey(Main.mouseItem.type))
				{
					if (Container.Frequency[pos] != Colors.None) Main.LocalPlayer.QuickSpawnItem(new EntitySource_Misc("QuantumStorage.Reset"), Utility.ColorToItem(Container.Frequency[pos]));
					
					Container.Frequency[pos] = Utility.ValidItems[Main.mouseItem.type];
					// buttonsFrequency[pos].Texture = QuantumStorage.textureGemsMiddle;
					buttonsFrequency[pos].Settings.SourceRectangle = new Rectangle(8 * (int)Container.Frequency[pos], 0, 8, 10);
					if (Main.netMode == NetmodeID.MultiplayerClient) NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null,((ModTileEntity)Container).ID, Container.Position.X, Container.Position.Y);

					Main.mouseItem.stack--;
					if (Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();

					if (Container.Frequency.IsSet) buttonInitialize.Text = Language.GetTextValue("Mods.QuantumStorage.UI.Initialize");
				}
			};
		}

		// todo: render inside panel
		buttonInitialize = new UIText(Language.GetText("Mods.QuantumStorage.UI.InsertGems"))
		{
			Width = { Pixels = -64, Percent = 100 },
			Height = { Pixels = 40 },
			Y = { Percent = 100 },
			X = { Percent = 50 },
			Settings = { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center}
		};
		buttonInitialize.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;
			
			if (!Container.Frequency.IsSet) return;

			for (int i = 0; i < 3; i++) Remove(buttonsFrequency[i]);
			Remove(buttonInitialize);

			Add(GridItems);
		};
	}

	public ItemStorage GetItemStorage() => Container.GetItemStorage();

	public string GetCursorTexture(Item item) => QuantumStorage.TexturePath + "Items/QEChest";
}