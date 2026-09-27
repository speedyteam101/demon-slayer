using DemonSlayerMod.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DemonSlayerMod.Common.Swords
{
	// Draws a swinging Nichirin Blade from its parts, in the same place and pose vanilla would draw a sword
	// (see PlayerDrawLayers.DrawPlayer_27_HeldItem). The item itself has noUseGraphic so vanilla draws nothing.
	public class NichirinBladeDrawLayer : PlayerDrawLayer
	{
		public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.HeldItem);

		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) {
			Player player = drawInfo.drawPlayer;
			return drawInfo.shadow == 0f && player.itemAnimation > 0 && !player.dead && !player.frozen
				&& drawInfo.heldItem?.ModItem is NichirinBlade;
		}

		protected override void Draw(ref PlayerDrawSet drawInfo) {
			Player player = drawInfo.drawPlayer;
			var blade = (NichirinBlade)drawInfo.heldItem.ModItem;
			Color light = Lighting.GetColor(player.Center.ToTileCoordinates());
			Vector2 position = (drawInfo.ItemLocation - Main.screenPosition).Floor();
			var origin = new Vector2(SwordDrawer.Size * 0.5f - SwordDrawer.Size * 0.5f * player.direction, SwordDrawer.Size);
			float scale = player.GetAdjustedItemScale(drawInfo.heldItem);
			float rotation = player.itemRotation;
			var effects = drawInfo.itemEffect;

			var cache = drawInfo.DrawDataCache;
			SwordDrawer.ForEachLayer(blade.Build, light, 0.35f, (texture, color) =>
				cache.Add(new DrawData(texture, position, null, color, rotation, origin, scale, effects)));
		}
	}
}
