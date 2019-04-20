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
	public class QEChest : BaseTile
	{
		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(mod.GetTileEntity<TEQEChest>().Hook_AfterPlacement, -1, 0, false);
			TileObjectData.addTile(Type);
			disableSmartCursor = true;

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Quantum Entangled Chest");
			AddMapEntry(Color.Purple, name);
		}

		public override void RightClick(int i, int j)
		{
			TEQEChest qeChest = mod.GetTileEntity<TEQEChest>(i, j);
			if (qeChest == null) return;

			//PortableStorage.Instance.PanelUI.UI.HandleUI(qeChest);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			TEQEChest qeChest = mod.GetTileEntity<TEQEChest>(i, j);
			//PortableStorage.Instance.PanelUI.UI.CloseUI(qeChest);

			Item.NewItem(i * 16, j * 16, 32, 32, mod.ItemType<Items.QEChest>());
			qeChest.Kill(i, j);
		}
	}
}