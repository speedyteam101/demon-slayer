using DemonSlayerMod.Common.Breathing;
using DemonSlayerMod.Common.Players;
using DemonSlayerMod.Common.Swords;
using DemonSlayerMod.Content.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace DemonSlayerMod.Common.UI
{
	// The Swordsmith's Forge panel: cycle through every part of the Nichirin Blade you're holding
	// (or the first one in your inventory) and see the result live.
	public class ForgeUI : UIState
	{
		private const float PanelWidth = 700f;
		private const float PanelHeight = 560f;
		private const float RowsLeft = 250f;
		private const float RowHeight = 50f;

		private UIPanel panel;
		private UIText targetText;
		private UIText summaryText;
		private UIText steelHint;
		private readonly UIText[] valueTexts = new UIText[SwordBuild.Categories];
		private readonly UIText[] descriptionTexts = new UIText[SwordBuild.Categories];

		public override void OnInitialize() {
			panel = new UIPanel();
			panel.Width.Set(PanelWidth, 0f);
			panel.Height.Set(PanelHeight, 0f);
			panel.HAlign = 0.5f;
			panel.VAlign = 0.5f;
			Append(panel);

			panel.Append(UIFactory.Text("Swordsmith's Forge", 0f, 0f, 0.6f, true));
			var close = new TextButton("X", SlayerUISystem.CloseForge, 30f);
			close.Left.Set(-30f, 1f);
			panel.Append(close);

			targetText = UIFactory.Text("", 0f, 34f, 0.8f);
			panel.Append(targetText);

			var preview = new SwordPreview { Build = () => FindBlade(out _)?.Build };
			preview.Left.Set(0f, 0f);
			preview.Top.Set(60f, 0f);
			preview.Width.Set(230f, 0f);
			preview.Height.Set(230f, 0f);
			panel.Append(preview);

			var random = new TextButton("Randomise looks", RandomiseLooks, 230f, 30f, () => FindBlade(out _) != null);
			panel.Append(UIFactory.Place(random, 0f, 300f));
			var reset = new TextButton("Reset to plain katana", ResetBuild, 230f, 30f, () => FindBlade(out _) != null);
			panel.Append(UIFactory.Place(reset, 0f, 336f));

			steelHint = UIFactory.Text("", 0f, 378f, 0.75f);
			panel.Append(steelHint);

			for (int c = 0; c < SwordBuild.Categories; c++) {
				int category = c;
				float top = 60f + c * RowHeight;
				var left = new TextButton("<", () => Cycle(category, -1), 28f, 26f, () => FindBlade(out _) != null);
				var right = new TextButton(">", () => Cycle(category, 1), 28f, 26f, () => FindBlade(out _) != null);
				panel.Append(UIFactory.Place(left, RowsLeft, top));
				panel.Append(UIFactory.Place(right, PanelWidth - 60f, top));
				valueTexts[c] = UIFactory.Text("", RowsLeft + 38f, top + 4f, 0.9f);
				descriptionTexts[c] = UIFactory.Text("", RowsLeft + 38f, top + 26f, 0.7f);
				panel.Append(valueTexts[c]);
				panel.Append(descriptionTexts[c]);
			}

			summaryText = UIFactory.Text("", 0f, PanelHeight - 70f, 0.8f);
			panel.Append(summaryText);
		}

		// The blade being edited: the held one if it's a Nichirin Blade, otherwise the first in the inventory.
		public static NichirinBlade FindBlade(out int slot) {
			Player player = Main.LocalPlayer;
			if (player.HeldItem?.ModItem is NichirinBlade held && player.selectedItem < 58) {
				slot = player.selectedItem;
				return held;
			}
			for (int i = 0; i < 58; i++) {
				if (player.inventory[i].ModItem is NichirinBlade blade) {
					slot = i;
					return blade;
				}
			}
			slot = -1;
			return null;
		}

		private static void Changed(NichirinBlade blade, int slot) {
			blade.UpdateInventory(Main.LocalPlayer); // refreshes rarity
			if (Main.netMode == NetmodeID.MultiplayerClient && slot >= 0) {
				NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Main.myPlayer, PlayerItemSlotID.Inventory0 + slot, blade.Item.prefix);
			}
		}

		private static void Cycle(int category, int step) {
			NichirinBlade blade = FindBlade(out int slot);
			if (blade == null) {
				return;
			}
			SlayerPlayer slayer = SlayerPlayer.Get(Main.LocalPlayer);
			int count = SwordBuild.Count(category);
			int value = blade.Build.Get(category);
			for (int i = 0; i < count; i++) {
				value = ((value + step) % count + count) % count;
				if (category != 0 || slayer.SteelUnlocked(value)) {
					break; // steels you haven't earned are skipped
				}
			}
			blade.Build.Set(category, value);
			Changed(blade, slot);
		}

		private static void RandomiseLooks() {
			NichirinBlade blade = FindBlade(out int slot);
			if (blade == null) {
				return;
			}
			// Only the purely cosmetic categories, so your stats don't change.
			foreach (int category in new[] { 4, 5, 7 }) {
				blade.Build.Set(category, Main.rand.Next(SwordBuild.Count(category)));
			}
			Changed(blade, slot);
		}

		private static void ResetBuild() {
			NichirinBlade blade = FindBlade(out int slot);
			if (blade == null) {
				return;
			}
			int steel = blade.Build.Steel;
			blade.Build = new SwordBuild { Steel = steel };
			Changed(blade, slot);
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			if (panel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}

			Player player = Main.LocalPlayer;
			SlayerPlayer slayer = SlayerPlayer.Get(player);
			NichirinBlade blade = FindBlade(out int slot);
			if (blade == null) {
				targetText.SetText("[c/FF8080:Bring a Nichirin Blade (craft one here from 12 Iron or Lead Bars and 5 Wood)]");
				for (int c = 0; c < SwordBuild.Categories; c++) {
					valueTexts[c].SetText(SwordBuild.CategoryName(c) + ": -");
					descriptionTexts[c].SetText("");
				}
				summaryText.SetText("");
				steelHint.SetText("");
				return;
			}

			SwordBuild build = blade.Build;
			targetText.SetText(slot == player.selectedItem ? "Editing the blade in your hand" : $"Editing the Nichirin Blade in inventory slot {slot + 1}");
			for (int c = 0; c < SwordBuild.Categories; c++) {
				string count = $"({build.Get(c) + 1}/{SwordBuild.Count(c)})";
				valueTexts[c].SetText($"{SwordBuild.CategoryName(c)}: [c/FFE080:{build.ValueName(c)}] {count}");
				descriptionTexts[c].SetText(build.ValueDescription(c));
			}

			// The next steel you could earn.
			string hint = "All steels unlocked!";
			for (int s = 1; s < SwordParts.Steels.Length; s++) {
				if (!slayer.SteelUnlocked(s)) {
					hint = $"Next steel: {SwordParts.Steels[s].Name}\n(defeat {BossName(SwordParts.Steels[s].RequiredBoss)})";
					break;
				}
			}
			steelHint.SetText(hint);

			Item item = blade.Item;
			int damage = player.GetWeaponDamage(item);
			float crit = player.GetWeaponCrit(item);
			float useTime = SwordParts.Shapes[build.Shape].UseTime;
			string affinity = "none";
			foreach (BreathingStyle style in BreathingStyles.All) {
				if (style.AffinityColour == build.Colour) {
					affinity = affinity == "none" ? style.Name : affinity + ", " + style.Name;
				}
			}
			summaryText.SetText($"Damage {damage}   Use time {useTime}   Crit {crit:0}%   Reach {SwordParts.Shapes[build.Shape].Scale * 100:0}%   Knockback {SwordParts.Shapes[build.Shape].Knockback:0.#}\n"
				+ $"Colour affinity (+20% technique damage): {affinity}");
		}

		private static string BossName(string key) => key switch {
			"HandDemon" => "the Hand Demon",
			"GyutaroDaki" => "Gyutaro & Daki",
			null => "nobody",
			_ => key
		};
	}
}
