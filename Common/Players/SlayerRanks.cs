using System;

namespace DemonSlayerMod.Common.Players
{
	// Demon Slayer Corps ranks, lowest first, then Hashira.
	public static class SlayerRanks
	{
		public static readonly string[] Names = {
			"Mizunoto", "Mizunoe", "Kanoto", "Kanoe", "Tsuchinoto",
			"Tsuchinoe", "Hinoto", "Hinoe", "Kinoto", "Kinoe", "Hashira"
		};

		public const int Hashira = 10;
		public const int LevelsPerRank = 6;
		public const int TotalConcentrationRank = 4; // Total Concentration Breathing: Constant

		public static int RankOf(int level) => Math.Min(Hashira, (level - 1) / LevelsPerRank);

		public static int FirstLevelOf(int rank) => rank * LevelsPerRank + 1;
	}

	public enum SlayerStat
	{
		Strength,  // melee damage
		Agility,   // movement and melee speed
		Endurance, // max life and defense
		Breath,    // max Breath and Breath regeneration
		Focus      // crit and technique damage
	}

	public static class SlayerStats
	{
		public const int Count = 5;

		public static readonly string[] Names = { "Strength", "Agility", "Endurance", "Breath", "Focus" };

		public static readonly string[] PerPoint = {
			"+1.5% melee damage",
			"+1% movement speed, +0.8% melee speed",
			"+3 max life, +1 defense per 2 points",
			"+4 max Breath, +2% Breath regeneration",
			"+0.5% melee crit, +1% technique damage"
		};
	}
}
