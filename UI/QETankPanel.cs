using BaseLibrary;
using BaseLibrary.UI;
using BaseLibrary.UI.Elements;
using ContainerLibrary;
using Microsoft.Xna.Framework;
using QuantumStorage.TileEntities;

namespace QuantumStorage.UI
{
	public class QETankPanel : BaseUIPanel<TEQETank>
	{
		public UITank tankFluid;

		public override void OnInitialize()
		{
			Width = (408, 0);
			Height = (172, 0);
			this.Center();

			UIText textLabel = new UIText("Quantum Entangled Tank")
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

			tankFluid = new UITank(Container)
			{
				Width = (40, 0),
				Height = (-44, 1),
				Top = (36, 0),
				HAlign = 0.5f
			};
			Append(tankFluid);
		}
	}
}