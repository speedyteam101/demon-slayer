using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace DemonSlayerMod.Common.Swords
{
	// Draws a Nichirin Blade from its layers. Every layer is a 64x64 sprite pointing up and to the right (like vanilla swords),
	// with the pommel in the bottom-left corner, so they all share one origin and rotation.
	//   Blade_<Shape>  greyscale, tinted with the Nichirin colour
	//   Edge_<Shape>   the silver cutting edge, drawn untinted
	//   Hilt           greyscale wrap, tinted with the hilt wrap colour
	//   HiltDiamonds   the white diamonds showing through the wrap, untinted
	//   Guard_<Guard>  greyscale, tinted with the guard finish
	public static class SwordDrawer
	{
		public const int Size = 64;
		private const string Path = "DemonSlayerMod/Common/Swords/Parts/";

		private static Texture2D Get(string name) {
			return ModContent.Request<Texture2D>(Path + name, AssetRequestMode.ImmediateLoad).Value;
		}

		// Calls `layer` once per sprite, bottom to top, with the colour to draw it in.
		public static void ForEachLayer(SwordBuild build, Color light, float glow, System.Action<Texture2D, Color> layer) {
			BladeShape shape = SwordParts.Shapes[build.Shape];
			Guard guard = SwordParts.Guards[build.Guard];
			Color blade = SwordParts.Colours[build.Colour].Color;
			Color wrap = SwordParts.Wraps[build.Wrap].Color;
			Color finish = SwordParts.GuardFinishes[build.GuardFinish].Color;

			layer(Get("Blade_" + shape.Texture), blade.MultiplyRGBA(light));
			if (glow > 0f) {
				// A faint additive copy makes the Nichirin colour shine while swinging.
				layer(Get("Blade_" + shape.Texture), (blade * glow) with { A = 0 });
			}
			layer(Get("Edge_" + shape.Texture), light);
			layer(Get("Hilt"), wrap.MultiplyRGBA(light));
			layer(Get("HiltDiamonds"), light);
			if (guard.Texture != "None") {
				layer(Get("Guard_" + guard.Texture), finish.MultiplyRGBA(light));
			}
		}

		public static void Draw(SpriteBatch spriteBatch, SwordBuild build, Vector2 position, Color light, float rotation, Vector2 origin, float scale, SpriteEffects effects, float glow = 0f) {
			ForEachLayer(build, light, glow, (texture, color) =>
				spriteBatch.Draw(texture, position, null, color, rotation, origin, scale, effects, 0f));
		}

		// Draws the whole sword centred on a point (inventory, forge preview, dropped item).
		public static void DrawCentered(SpriteBatch spriteBatch, SwordBuild build, Vector2 center, Color light, float scale, float rotation = 0f) {
			Draw(spriteBatch, build, center, light, rotation, new Vector2(Size / 2f), scale, SpriteEffects.None);
		}
	}
}
