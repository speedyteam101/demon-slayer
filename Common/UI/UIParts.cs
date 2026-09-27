using DemonSlayerMod.Common.Swords;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace DemonSlayerMod.Common.UI
{
	// A small clickable text button that lights up when hovered.
	public class TextButton : UITextPanel<string>
	{
		private static readonly Color Normal = new Color(63, 82, 151) * 0.9f;
		private static readonly Color Hover = new Color(100, 125, 210);
		private readonly Func<bool> enabled;

		public TextButton(string text, Action onClick, float width, float height = 26f, Func<bool> enabled = null) : base(text, 0.8f) {
			this.enabled = enabled;
			Width.Set(width, 0f);
			Height.Set(height, 0f);
			PaddingTop = PaddingBottom = 4f;
			PaddingLeft = PaddingRight = 4f;
			OnLeftClick += (_, _) => {
				if (this.enabled == null || this.enabled()) {
					SoundEngine.PlaySound(SoundID.MenuTick);
					onClick();
				}
			};
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			bool on = enabled == null || enabled();
			BackgroundColor = !on ? new Color(40, 40, 50) * 0.8f : IsMouseHovering ? Hover : Normal;
			TextColor = on ? Color.White : Color.Gray;
		}
	}

	// Draws a Nichirin Blade build, big, for the forge preview.
	public class SwordPreview : UIElement
	{
		public Func<SwordBuild> Build;

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			CalculatedStyle box = GetDimensions();
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, box.ToRectangle(), new Color(10, 10, 20) * 0.6f);
			SwordBuild build = Build?.Invoke();
			if (build == null) {
				return;
			}
			float scale = Math.Min(box.Width, box.Height) / SwordDrawer.Size * 0.9f;
			SwordDrawer.DrawCentered(spriteBatch, build, box.Center(), Color.White, scale);
		}
	}

	// A plain filled bar with a label (XP bar).
	public class Bar : UIElement
	{
		public Func<float> Fill;
		public Func<string> Label;
		public Color Color = new(90, 200, 120);

		protected override void DrawSelf(SpriteBatch spriteBatch) {
			CalculatedStyle box = GetDimensions();
			Rectangle rect = box.ToRectangle();
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, new Color(20, 20, 30) * 0.9f);
			float fill = MathHelper.Clamp(Fill?.Invoke() ?? 0f, 0f, 1f);
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + 2, rect.Y + 2, (int)((rect.Width - 4) * fill), rect.Height - 4), Color);
			string label = Label?.Invoke() ?? "";
			Vector2 size = FontAssets.MouseText.Value.MeasureString(label) * 0.8f;
			Utils.DrawBorderString(spriteBatch, label, box.Center() - size / 2f + new Vector2(0f, 2f), Color.White, 0.8f);
		}
	}

	public static class UIFactory
	{
		public static UIText Text(string text, float left, float top, float scale = 0.9f, bool large = false) {
			var element = new UIText(text, scale, large);
			element.Left.Set(left, 0f);
			element.Top.Set(top, 0f);
			return element;
		}

		public static T Place<T>(T element, float left, float top) where T : UIElement {
			element.Left.Set(left, 0f);
			element.Top.Set(top, 0f);
			return element;
		}
	}
}
