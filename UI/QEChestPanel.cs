using BaseLibrary;
using BaseLibrary.UI;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QuantumStorage.TileEntities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace QuantumStorage.UI;

public class QEChestPanel : BaseUIPanel<QEChest>, IItemStorageUI
{
	protected const int SlotSize = 44;
	protected const int SlotMargin = 4;

	private ref Frequency Frequency => ref Container.Frequency;

	private BaseElement initializePage;
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

		// todo: sprites
		UIText buttonReset = new UIText("R")
		{
			Height = { Pixels = 20 },
			Width = { Pixels = 20 },
			HoverText = Language.GetText("Mods.BaseLibrary.UI.Reset")
		};
		buttonReset.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;

			if (!Frequency.IsSet)
			{
				for (int i = 0; i < 3; i++)
					Utility.SpawnItem(Main.LocalPlayer, Utility.ColorToItem(Frequency[i]));

				Frequency = new Frequency();
				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ((ModTileEntity)Container).ID, Container.Position.X, Container.Position.Y);
			}
			else
			{
				for (int i = 0; i < 3; i++)
					Utility.SpawnItem(Main.LocalPlayer, Utility.ColorToItem(Frequency[i]));

				Frequency = new Frequency();
				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ((ModTileEntity)Container).ID, Container.Position.X, Container.Position.Y);

				Remove(gridItems);

				InitializeFrequencySelection();
				Add(initializePage);
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
			args.Handled = true;

			PanelUI.Instance.CloseUI(Container);
		};
		buttonClose.OnMouseEnter += _ => buttonClose.Settings.TextColor = Color.Red;
		buttonClose.OnMouseLeave += _ => buttonClose.Settings.TextColor = Color.White;
		Add(buttonClose);

		if (!Frequency.IsSet)
		{
			InitializeFrequencySelection();
			Add(initializePage);
		}
		else Add(gridItems = GetGrid());
	}

	private void InitializeFrequencySelection()
	{
		initializePage = new BaseElement
		{
			Width = { Percent = 100 },
			Height = { Percent = 100, Pixels = -28 }
		};

		// todo: render inside panel
		UIText buttonInitialize = new UIText(Language.GetText("Mods.QuantumStorage.UI.InsertGems"))
		{
			Width = { Pixels = -64, Percent = 100 },
			Height = { Pixels = 40 },
			Y = { Percent = 100 },
			X = { Percent = 50 },
			Settings = { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
		};
		buttonInitialize.OnMouseDown += args =>
		{
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;

			if (!Frequency.IsSet) return;

			Remove(initializePage);
			Add(gridItems = GetGrid());
		};
		initializePage.Add(buttonInitialize);

		for (int i = 0; i < 3; i++)
		{
			int pos = i;
			// note: size is kind of small
			UITexture buttonFrequency = new UITexture(ModContent.Request<Texture2D>(QuantumStorage.TexturePath + "Tiles/GemMiddle_0"))
			{
				Size = new Vector2(16, 20),
				X = { Percent = 40 + 10 * pos },
				Y = { Percent = 50 },
				Settings = { SourceRectangle = new Rectangle(8 * (int)Frequency[pos], 0, 8, 10) }
			};
			buttonFrequency.OnMouseDown += args =>
			{
				if (args.Button != MouseButton.Left) return;
				args.Handled = true;

				if (!Utility.ValidItems.ContainsKey(Main.mouseItem.type))
					return;

				Utility.SpawnItem(Main.LocalPlayer, Utility.ColorToItem(Frequency[pos]));

				Frequency[pos] = Utility.ValidItems[Main.mouseItem.type];
				buttonFrequency.Settings.SourceRectangle = new Rectangle(8 * (int)Frequency[pos], 0, 8, 10);

				if (Main.netMode == NetmodeID.MultiplayerClient)
					NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ((ModTileEntity)Container).ID, Container.Position.X, Container.Position.Y);

				Main.mouseItem.stack--;
				if (Main.mouseItem.stack <= 0) Main.mouseItem.TurnToAir();

				if (Frequency.IsSet) buttonInitialize.Text = Language.GetTextValue("Mods.BaseLibrary.UI.Initialize");
			};
			initializePage.Add(buttonFrequency);
		}
	}

	private UIGrid<UIContainerSlot> GetGrid()
	{
		UIGrid<UIContainerSlot> grid = new UIGrid<UIContainerSlot>(9)
		{
			Width = { Percent = 100 },
			Height = { Pixels = -28, Percent = 100 },
			Y = { Pixels = 28 },
			Settings = { ItemMargin = SlotMargin }
		};

		ItemStorage storage = Container.GetItemStorage();
		for (int i = 0; i < storage.Count; i++)
		{
			UIContainerSlot slot = new UIContainerSlot(storage, i)
			{
				Width = { Pixels = SlotSize },
				Height = { Pixels = SlotSize }
			};
			grid.Add(slot);
		}

		return grid;
	}

	public ItemStorage GetItemStorage() => Container.GetItemStorage();

	public string GetCursorTexture(Item item) => QuantumStorage.TexturePath + "Items/QEChest";
}