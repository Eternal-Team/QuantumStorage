using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using QuantumStorage.Items;
using Terraria;

namespace QuantumStorage.UI
{
	public class QEBagPanel : BaseUIPanel<QEBag>, IItemHandlerUI
	{
		public ItemHandler Handler => Container.Handler;
		public string GetTexture(Item item) => "QuantumStorage/Textures/Items/QEBag";

		private UIButton[] buttonsFrequency;
		private UITextButton buttonInitialize;

		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (172, 0);
			this.Center();

			UIText textLabel = new UIText("Quantum Entangled Bag")
			{
				HAlign = 0.5f
			};
			Append(textLabel);

			UITextButton buttonClose = new UITextButton("X")
			{
				Size = new Vector2(20),
				Left = (-20, 1),
				RenderPanel = false
			};
			buttonClose.OnClick += (evt, element) => BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(Container);
			Append(buttonClose);

			if (!Container.frequency.IsSet)
			{
				buttonsFrequency = new UIButton[3];
				for (int i = 0; i < 3; i++)
				{
					int pos = i;
					buttonsFrequency[i] = new UIButton(QuantumStorage.textureEmptySocket)
					{
						Size = new Vector2(16, 20),
						HAlign = 0.17f + 0.33f * pos,
						VAlign = 0.5f
					};
					buttonsFrequency[i].OnClick += (evt, element) =>
					{
						if (Utility.ValidItems.ContainsKey(Main.mouseItem.type))
						{
							Container.frequency[pos] = Utility.ValidItems[Main.mouseItem.type];
							buttonsFrequency[pos].texture = QuantumStorage.textureGemsMiddle;
							buttonsFrequency[pos].sourceRectangle = new Rectangle(8 * (int)Container.frequency[pos], 0, 8, 10);
						}
					};
					Append(buttonsFrequency[i]);
				}

				buttonInitialize = new UITextButton("Initialize")
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

					AddGrid();
				};
				Append(buttonInitialize);
			}
			else AddGrid();
		}

		private void AddGrid()
		{
			UIGrid<UIContainerSlot> gridItems = new UIGrid<UIContainerSlot>(9)
			{
				Width = (0, 1),
				Height = (-28, 1),
				Top = (28, 0),
				OverflowHidden = true,
				ListPadding = 4f
			};
			Append(gridItems);

			for (int i = 0; i < Container.Handler.Slots; i++)
			{
				UIContainerSlot slot = new UIContainerSlot(() => Container.Handler, i);
				gridItems.Add(slot);
			}
		}
	}
}