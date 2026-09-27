using DemonSlayerMod.Common.UI;
using DemonSlayerMod.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace DemonSlayerMod.Content.Tiles
{
	// A 3x2 forge. Right-click it to customise a Nichirin Blade; it is also the crafting station for new blades.
	public class SwordsmithForgeTile : ModTile
	{
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			Main.tileLighted[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = [16, 16];
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(190, 90, 40), CreateMapEntryName());
			DustType = DustID.Stone;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			r = 0.9f;
			g = 0.45f;
			b = 0.1f;
		}

		public override bool RightClick(int i, int j) {
			SlayerUISystem.OpenForge(new Point(i, j));
			return true;
		}

		public override void MouseOver(int i, int j) {
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<SwordsmithForge>();
		}
	}
}
