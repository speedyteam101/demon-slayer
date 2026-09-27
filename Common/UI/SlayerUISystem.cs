using DemonSlayerMod.Common.Breathing;
using DemonSlayerMod.Common.Players;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace DemonSlayerMod.Common.UI
{
	// Owns the forge and Slayer menu panels, and draws the Breath bar under the player.
	public class SlayerUISystem : ModSystem
	{
		private const float ForgeReach = 16f * 8f;

		private static UserInterface forgeInterface;
		private static UserInterface statsInterface;
		private static ForgeUI forgeUI;
		private static StatsUI statsUI;
		private static Point forgeTile;
		private GameTime lastGameTime;

		public override void Load() {
			if (Main.dedServ) {
				return;
			}
			forgeUI = new ForgeUI();
			forgeUI.Activate();
			statsUI = new StatsUI();
			statsUI.Activate();
			forgeInterface = new UserInterface();
			statsInterface = new UserInterface();
		}

		public override void Unload() {
			forgeUI = null;
			statsUI = null;
			forgeInterface = null;
			statsInterface = null;
		}

		public static void OpenForge(Point tile) {
			forgeTile = tile;
			statsInterface?.SetState(null);
			forgeInterface?.SetState(forgeUI);
			Main.playerInventory = true;
			SoundEngine.PlaySound(SoundID.MenuOpen);
		}

		public static void CloseForge() {
			if (forgeInterface?.CurrentState != null) {
				forgeInterface.SetState(null);
				SoundEngine.PlaySound(SoundID.MenuClose);
			}
		}

		public static void ToggleStats() {
			if (statsInterface == null) {
				return;
			}
			if (statsInterface.CurrentState != null) {
				CloseStats();
			}
			else {
				forgeInterface?.SetState(null);
				statsInterface.SetState(statsUI);
				SoundEngine.PlaySound(SoundID.MenuOpen);
			}
		}

		public static void CloseStats() {
			if (statsInterface?.CurrentState != null) {
				statsInterface.SetState(null);
				SoundEngine.PlaySound(SoundID.MenuClose);
			}
		}

		public override void UpdateUI(GameTime gameTime) {
			lastGameTime = gameTime;
			if (forgeInterface?.CurrentState != null) {
				// Walking away from the forge closes it.
				Vector2 forge = forgeTile.ToWorldCoordinates(24f, 16f);
				if (Main.LocalPlayer.dead || Vector2.Distance(Main.LocalPlayer.Center, forge) > ForgeReach) {
					CloseForge();
				}
				else {
					forgeInterface.Update(gameTime);
				}
			}
			if (statsInterface?.CurrentState != null) {
				statsInterface.Update(gameTime);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int mouseText = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
			if (mouseText < 0) {
				return;
			}
			layers.Insert(mouseText, new LegacyGameInterfaceLayer("DemonSlayerMod: Panels", () => {
				if (forgeInterface?.CurrentState != null) {
					forgeInterface.Draw(Main.spriteBatch, lastGameTime);
				}
				if (statsInterface?.CurrentState != null) {
					statsInterface.Draw(Main.spriteBatch, lastGameTime);
				}
				return true;
			}, InterfaceScaleType.UI));
			layers.Insert(mouseText, new LegacyGameInterfaceLayer("DemonSlayerMod: Breath Bar", () => {
				DrawBreathBar();
				return true;
			}, InterfaceScaleType.Game));
		}

		// A thin bar under the player, plus the selected form's name, while holding a Nichirin Blade or recovering Breath.
		private static void DrawBreathBar() {
			Player player = Main.LocalPlayer;
			if (player.dead || player.ghost) {
				return;
			}
			SlayerPlayer slayer = SlayerPlayer.Get(player);
			if (!slayer.HoldingBlade && slayer.Breath >= slayer.BreathMax) {
				return;
			}

			var spriteBatch = Main.spriteBatch;
			Color styleColor = BreathingStyles.ColorOf(slayer.Style);
			Vector2 anchor = player.Bottom + new Vector2(0f, 14f + player.gfxOffY) - Main.screenPosition;
			const int width = 60;
			const int height = 7;
			var back = new Rectangle((int)(anchor.X - width / 2f), (int)anchor.Y, width, height);
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, back, Color.Black * 0.7f);
			float fill = slayer.BreathMax > 0 ? slayer.Breath / slayer.BreathMax : 0f;
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(back.X + 1, back.Y + 1, (int)((width - 2) * fill), height - 2), styleColor);

			// Cooldown sweep in white under the Breath fill.
			if (slayer.TechniqueCooldown > 0) {
				float cooldown = (float)slayer.TechniqueCooldown / slayer.TechniqueCooldownMax;
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(back.X + 1, back.Bottom, (int)((width - 2) * cooldown), 2), Color.White * 0.8f);
			}

			if (slayer.HoldingBlade) {
				string label = slayer.CurrentForm == null ? "No breathing style (K)" : slayer.CurrentForm.Name;
				Color color = slayer.CurrentForm != null && slayer.Breath < slayer.CurrentForm.Cost ? Color.Gray : styleColor;
				Vector2 size = FontAssets.MouseText.Value.MeasureString(label) * 0.7f;
				Utils.DrawBorderString(spriteBatch, label, new Vector2(anchor.X - size.X / 2f, back.Bottom + 4f), color, 0.7f);
			}
		}
	}
}
