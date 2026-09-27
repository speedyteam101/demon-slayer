using DemonSlayerMod.Common.Players;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DemonSlayerMod.Common.Breathing
{
	public class BreathingStyle
	{
		public string Name, Description;
		public Color Color;
		public int Dust;
		public int AffinityColour;   // SwordParts.Colours index that gives +20% technique damage with this style
		public int RequiredRank;     // SlayerRanks index
		public string RequiredBoss;  // DownedDemonSystem key, or null
		public int Debuff = -1;      // extra debuff every technique of this style inflicts
		public Technique[] Forms;
	}

	public static class BreathingStyles
	{
		public const int None = -1;
		public const int Sound = 10;
		public const float AffinityBonus = 0.2f;

		public static readonly BreathingStyle[] All = {
			new() {
				Name = "Water Breathing", Description = "Flowing, adaptable forms. The easiest style to master.",
				Color = new Color(70, 145, 255), Dust = DustID.Water, AffinityColour = 1, RequiredRank = 0, Forms = Techniques.Water
			},
			new() {
				Name = "Flame Breathing", Description = "Straightforward, overwhelming strength. Burns enemies.",
				Color = new Color(255, 110, 30), Dust = DustID.Torch, AffinityColour = 2, RequiredRank = 0, Debuff = BuffID.OnFire, Forms = Techniques.Flame
			},
			new() {
				Name = "Thunder Breathing", Description = "Blinding speed focused on the legs.",
				Color = new Color(255, 230, 60), Dust = DustID.Electric, AffinityColour = 3, RequiredRank = 0, Forms = Techniques.Thunder
			},
			new() {
				Name = "Wind Breathing", Description = "Wild, slicing gales that tear through crowds.",
				Color = new Color(110, 230, 140), Dust = DustID.GreenTorch, AffinityColour = 4, RequiredRank = 0, Forms = Techniques.Wind
			},
			new() {
				Name = "Stone Breathing", Description = "Crushing power and an unbreakable body.",
				Color = new Color(170, 160, 150), Dust = DustID.Stone, AffinityColour = 5, RequiredRank = 0, Forms = Techniques.Stone
			},
			new() {
				Name = "Mist Breathing", Description = "Derived from Wind. Confusing, elusive strikes.",
				Color = new Color(215, 235, 245), Dust = DustID.Smoke, AffinityColour = 10, RequiredRank = 3, Debuff = BuffID.Confused, Forms = Techniques.Mist
			},
			new() {
				Name = "Serpent Breathing", Description = "Derived from Water. Winding strikes that hunt their prey.",
				Color = new Color(150, 90, 205), Dust = DustID.PurpleTorch, AffinityColour = 8, RequiredRank = 3, Debuff = BuffID.Venom, Forms = Techniques.Serpent
			},
			new() {
				Name = "Love Breathing", Description = "Derived from Flame. Sweeping, flexible slashes.",
				Color = new Color(255, 120, 190), Dust = DustID.PinkTorch, AffinityColour = 6, RequiredRank = 3, Forms = Techniques.Love
			},
			new() {
				Name = "Insect Breathing", Description = "Derived from Flower. Weak cuts, deadly poison.",
				Color = new Color(190, 145, 255), Dust = DustID.PurpleTorch, AffinityColour = 9, RequiredRank = 4, Debuff = BuffID.Venom, Forms = Techniques.Insect
			},
			new() {
				Name = "Flower Breathing", Description = "Derived from Water. Graceful forms and keen eyes.",
				Color = new Color(255, 170, 210), Dust = DustID.PinkFairy, AffinityColour = 6, RequiredRank = 2, Forms = Techniques.Flower
			},
			new() {
				Name = "Sound Breathing", Description = "Derived from Thunder. Every strike explodes.",
				Color = new Color(255, 200, 80), Dust = DustID.GoldFlame, AffinityColour = 12, RequiredRank = 4, Forms = Techniques.Sound
			},
			new() {
				Name = "Beast Breathing", Description = "Self-taught and savage. Tears enemies apart.",
				Color = new Color(145, 155, 195), Dust = DustID.BlueCrystalShard, AffinityColour = 8, RequiredRank = 2, Debuff = BuffID.Ichor, Forms = Techniques.Beast
			},
			new() {
				Name = "Moon Breathing", Description = "A demon's style: every slash sheds crescent moons. Requires Kokushibo's defeat.",
				Color = new Color(185, 90, 255), Dust = DustID.Shadowflame, AffinityColour = 7, RequiredRank = 6, RequiredBoss = "Kokushibo", Debuff = BuffID.ShadowFlame, Forms = Techniques.Moon
			},
			new() {
				Name = "Sun Breathing", Description = "The first breathing style, the Dance of the Fire God. Requires an Upper Moon's defeat.",
				Color = new Color(255, 150, 40), Dust = DustID.SolarFlare, AffinityColour = 0, RequiredRank = 5, RequiredBoss = "GyutaroDaki", Debuff = BuffID.OnFire3, Forms = Techniques.Sun
			},
		};

		public static bool Valid(int style) => style >= 0 && style < All.Length;

		public static Color ColorOf(int style) => Valid(style) ? All[style].Color : Color.White;

		public static int DustOf(int style) => Valid(style) ? All[style].Dust : DustID.WhiteTorch;

		public static bool BossRequirementMet(BreathingStyle style, SlayerPlayer slayer) {
			return style.RequiredBoss == null || slayer.HasDefeated(style.RequiredBoss);
		}

		// Forms unlock as your rank rises, spread evenly from Mizunoto (rank 0) to Kinoe (rank 9).
		public static int FormRank(BreathingStyle style, int form) {
			return form * 10 / style.Forms.Length;
		}
	}
}
