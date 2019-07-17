using BaseLibrary;
using BaseLibrary.Tiles;
using Microsoft.Xna.Framework;
using QuantumStorage.TileEntities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace QuantumStorage.Tiles
{
	public class QETank : BaseTile
	{
		public override string Texture => "QuantumStorage/Textures/Tiles/QETank";

		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = false;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] {16, 16};
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEQETank>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;
			mineResist = 5f;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Quantum Entangled Tank");
			AddMapEntry(Color.Purple, name);
		}

		public override void RightClick(int i, int j)
		{
			TEQETank qeTank = mod.GetTileEntity<TEQETank>(i, j);
			if (qeTank == null) return;

			//if (Main.keyState.IsKeyDown(Keys.RightShift))
			//{
			//	qeTank.Handler.ExtractFluid(0, 100);

			//	ModFluid f = FluidLoader.GetFluid<Water>().NewInstance();
			//	f.volume = 255;

			//	qeTank.Handler.InsertFluid(0, f);

			//	return;
			//}

			BaseLibrary.BaseLibrary.PanelGUI.UI.HandleUI(qeTank);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			TEQETank qeChest = mod.GetTileEntity<TEQETank>(i, j);
			BaseLibrary.BaseLibrary.PanelGUI.UI.CloseUI(qeChest);

			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.QETank>());
			qeChest.Kill(i, j);
		}
	}
}