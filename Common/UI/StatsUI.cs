using DemonSlayerMod.Common.Breathing;
using DemonSlayerMod.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace DemonSlayerMod.Common.UI
{
	// The Slayer menu: rank, XP, stat points and breathing styles.
	public class StatsUI : UIState
	{
		private const float PanelWidth = 640f;
		private const float PanelHeight = 640f;
		private const int MaxFormLines = 13;

		private UIPanel panel;
		private UIText rankText, pointsText, styleText, styleInfo, footer;
		private readonly UIText[] statTexts = new UIText[SlayerStats.Count];
		private readonly UIText[] formTexts = new UIText[MaxFormLines];
		private int viewStyle;

		private static SlayerPlayer Slayer => SlayerPlayer.Get(Main.LocalPlayer);

		public override void OnInitialize() {
			panel = new UIPanel();
			panel.Width.Set(PanelWidth, 0f);
			panel.Height.Set(PanelHeight, 0f);
			panel.HAlign = 0.5f;
			panel.VAlign = 0.5f;
			Append(panel);

			panel.Append(UIFactory.Text("Demon Slayer Corps", 0f, 0f, 0.6f, true));
			var close = new TextButton("X", SlayerUISystem.CloseStats, 30f);
			close.Left.Set(-30f, 1f);
			panel.Append(close);

			rankText = UIFactory.Text("", 0f, 36f);
			panel.Append(rankText);

			var xp = new Bar {
				Fill = () => Slayer.Level >= SlayerPlayer.MaxLevel ? 1f : (float)Slayer.XP / SlayerPlayer.XPToNext(Slayer.Level),
				Label = () => Slayer.Level >= SlayerPlayer.MaxLevel ? "Max level" : $"XP {Slayer.XP} / {SlayerPlayer.XPToNext(Slayer.Level)}"
			};
			xp.Width.Set(0f, 1f);
			xp.Height.Set(22f, 0f);
			panel.Append(UIFactory.Place(xp, 0f, 62f));

			pointsText = UIFactory.Text("", 0f, 92f);
			panel.Append(pointsText);

			for (int i = 0; i < SlayerStats.Count; i++) {
				var stat = (SlayerStat)i;
				float top = 120f + i * 30f;
				panel.Append(UIFactory.Place(new TextButton("+1", () => Slayer.AddPoint(stat), 36f, 24f, () => CanRaise(stat)), 0f, top));
				panel.Append(UIFactory.Place(new TextButton("+5", () => Slayer.AddPoint(stat, 5), 36f, 24f, () => CanRaise(stat)), 40f, top));
				statTexts[i] = UIFactory.Text("", 86f, top + 3f, 0.85f);
				panel.Append(statTexts[i]);
			}

			float styleTop = 280f;
			panel.Append(UIFactory.Place(new TextButton("<", () => ViewStyle(-1), 28f), 0f, styleTop));
			panel.Append(UIFactory.Place(new TextButton(">", () => ViewStyle(1), 28f), 32f, styleTop));
			styleText = UIFactory.Text("", 70f, styleTop + 3f, 0.95f);
			panel.Append(styleText);
			var learn = new TextButton("Use this style", () => Slayer.SetStyle(viewStyle), 150f, 26f,
				() => Slayer.Style != viewStyle && Slayer.StyleUnlocked(viewStyle));
			learn.Left.Set(-150f, 1f);
			learn.Top.Set(styleTop, 0f);
			panel.Append(learn);
			styleInfo = UIFactory.Text("", 0f, styleTop + 32f, 0.75f);
			panel.Append(styleInfo);

			for (int i = 0; i < MaxFormLines; i++) {
				int form = i;
				formTexts[i] = UIFactory.Text("", 10f, styleTop + 76f + i * 17f, 0.75f);
				formTexts[i].OnLeftClick += (_, _) => SelectForm(form);
				panel.Append(formTexts[i]);
			}

			footer = UIFactory.Text("", 0f, PanelHeight - 48f, 0.75f);
			panel.Append(footer);
		}

		public override void OnActivate() {
			viewStyle = BreathingStyles.Valid(Slayer.Style) ? Slayer.Style : 0;
		}

		private static bool CanRaise(SlayerStat stat) => Slayer.PointsFree > 0 && Slayer.Stat(stat) < SlayerPlayer.StatCap;

		private void ViewStyle(int step) {
			int count = BreathingStyles.All.Length;
			viewStyle = ((viewStyle + step) % count + count) % count;
		}

		private void SelectForm(int form) {
			if (viewStyle == Slayer.Style && Slayer.FormUnlocked(form)) {
				Slayer.Form = form;
			}
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			if (panel.ContainsPoint(Main.MouseScreen)) {
				Main.LocalPlayer.mouseInterface = true;
			}
			SlayerPlayer slayer = Slayer;

			string next = slayer.Rank >= SlayerRanks.Hashira ? "" : $"   (next rank at level {SlayerRanks.FirstLevelOf(slayer.Rank + 1)})";
			rankText.SetText($"Rank: [c/FFD060:{SlayerRanks.Names[slayer.Rank]}]   Level {slayer.Level}{next}");
			pointsText.SetText($"Stat points to spend: {slayer.PointsFree}   (Training Scrolls used: {slayer.ScrollsUsed}/{SlayerPlayer.MaxScrolls}, max {SlayerPlayer.StatCap} per stat)");
			for (int i = 0; i < SlayerStats.Count; i++) {
				statTexts[i].SetText($"{SlayerStats.Names[i]} [c/FFE080:{slayer.Stats[i]}]  -  {SlayerStats.PerPoint[i]} each");
			}

			BreathingStyle style = BreathingStyles.All[viewStyle];
			string state = slayer.Style == viewStyle ? "[c/80FF80:(your style)]"
				: slayer.StyleUnlocked(viewStyle) ? "[c/A0C0FF:(available)]" : "[c/FF8080:(locked)]";
			styleText.SetText($"{style.Name} {state}");

			string requirement = $"Needs rank {SlayerRanks.Names[style.RequiredRank]}";
			if (style.RequiredBoss != null) {
				requirement += $" and {style.RequiredBoss} defeated";
			}
			styleInfo.SetText($"{style.Description}\n{requirement}. Best with a {Swords.SwordParts.Colours[style.AffinityColour].Name} blade (+20% technique damage).");

			for (int i = 0; i < MaxFormLines; i++) {
				if (i >= style.Forms.Length) {
					formTexts[i].SetText("");
					continue;
				}
				Technique form = style.Forms[i];
				int rank = BreathingStyles.FormRank(style, i);
				bool unlocked = slayer.Rank >= rank;
				bool current = slayer.Style == viewStyle && slayer.Form == i;
				string text = $"{form.Name}  ({form.Cost} Breath)";
				formTexts[i].SetText(current ? $"[c/80FF80:> {text}]" : unlocked ? $"  {text}" : $"[c/808080:  {text} - rank {SlayerRanks.Names[rank]}]");
			}

			string mark = slayer.Rank < SlayerRanks.Hashira ? "Demon Slayer Mark: become a Hashira"
				: slayer.MarkCooldown > 0 ? $"Demon Slayer Mark: ready in {slayer.MarkCooldown / 60}s" : "Demon Slayer Mark: ready (X)";
			string concentration = slayer.Rank >= SlayerRanks.TotalConcentrationRank ? "Total Concentration: Constant (+50% Breath regen)" : $"Total Concentration: at rank {SlayerRanks.Names[SlayerRanks.TotalConcentrationRank]}";
			footer.SetText($"Breath {(int)slayer.Breath}/{slayer.BreathMax}   {concentration}\n{mark}.   Keys: F technique, G/V change form, click a form to pick it.");
		}
	}
}
