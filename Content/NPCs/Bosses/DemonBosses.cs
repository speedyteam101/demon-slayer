using DemonSlayerMod.Content.NPCs.Enemies;
using DemonSlayerMod.Content.Projectiles.Demon;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.NPCs.Bosses
{
	// XP each character gets the first time it sees a demon boss fall (on top of the kill's own XP).
	public static class DemonBossRewards
	{
		private static readonly Dictionary<string, long> FirstKillXP = new() {
			["HandDemon"] = 800,
			["Rui"] = 1500,
			["Enmu"] = 2500,
			["GyutaroDaki"] = 6000,
			["Gyokko"] = 8000,
			["Hantengu"] = 10000,
			["Akaza"] = 14000,
			["Doma"] = 18000,
			["Kokushibo"] = 25000,
			["Muzan"] = 50000,
		};

		public static long XPFor(string key) => FirstKillXP.TryGetValue(key, out long xp) ? xp : 1000;
	}

	// The Hand Demon of the Final Selection: a mass of grasping arms.
	[AutoloadBossHead]
	public class HandDemon : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Walk;
		protected override BossAttack[] Attacks => [BossAttack.Charge, BossAttack.Spikes, BossAttack.Spread, BossAttack.Summon];
		protected override int ShotType => ModContent.ProjectileType<HandShot>();
		protected override int Life => 3200;
		protected override int Damage => 24;
		protected override int Defense => 8;
		protected override int Width => 80;
		protected override int Height => 110;
		protected override int MinionType => ModContent.NPCType<LesserDemon>();
		protected override float SpikeTint => 3f;
		protected override int HitDust => DustID.GreenBlood;
		protected override float ShotSpeed => 8f;
		protected override int MusicTrack => MusicID.Boss1;
		protected override int PotionType => ItemID.LesserHealingPotion;
		protected override int ValueGold => 3;
		protected override int BloodMin => 8;
		protected override int BloodMax => 15;
	}

	// Rui, Lower Moon Five, master of the spider threads.
	[AutoloadBossHead]
	public class Rui : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Hover;
		protected override BossAttack[] Attacks => [BossAttack.Spread, BossAttack.Wall, BossAttack.Summon, BossAttack.Spiral, BossAttack.Spikes];
		protected override int ShotType => ModContent.ProjectileType<ThreadShot>();
		protected override int Life => 6000;
		protected override int Damage => 30;
		protected override int Defense => 12;
		protected override int Width => 36;
		protected override int Height => 56;
		protected override int MinionType => ModContent.NPCType<SpiderDemon>();
		protected override float SpikeTint => 1f;
		protected override float ShotSpeed => 11f;
		protected override int MusicTrack => MusicID.Boss3;
		protected override int PotionType => ItemID.LesserHealingPotion;
		protected override int ValueGold => 5;
	}

	// Enmu, Lower Moon One, who fights through dreams.
	[AutoloadBossHead]
	public class Enmu : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Hover;
		protected override BossAttack[] Attacks => [BossAttack.Teleport, BossAttack.Ring, BossAttack.Bombs, BossAttack.Spiral, BossAttack.Summon, BossAttack.Barrage];
		protected override int ShotType => ModContent.ProjectileType<DreamShot>();
		protected override int Life => 9000;
		protected override int Damage => 34;
		protected override int Defense => 16;
		protected override int Width => 36;
		protected override int Height => 60;
		protected override int MinionType => ModContent.NPCType<TongueDemon>();
		protected override float BombTint => 3f;
		protected override int MusicTrack => MusicID.Boss5;
		protected override int PotionType => ItemID.HealingPotion;
		protected override int ValueGold => 8;
	}

	// Gyutaro and Daki, Upper Moon Six: blood sickles and flying obi sashes.
	[AutoloadBossHead]
	public class GyutaroDaki : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Stalk;
		protected override BossAttack[] Attacks => [BossAttack.Barrage, BossAttack.Wall, BossAttack.Charge, BossAttack.Spiral, BossAttack.Ring, BossAttack.Teleport];
		protected override int ShotType => ModContent.ProjectileType<BloodSickle>();
		protected override int Life => 34000;
		protected override int Damage => 72;
		protected override int Defense => 28;
		protected override int Width => 40;
		protected override int Height => 64;
		protected override float ShotSpeed => 11f;
		protected override float MoveSpeed => 13f;
		protected override bool Enrage => true;
		protected override int MusicTrack => MusicID.Boss2;
		protected override int ValueGold => 15;
		protected override int BloodMin => 20;
		protected override int BloodMax => 35;
	}

	// Gyokko, Upper Moon Five, who hides in his vases.
	[AutoloadBossHead]
	public class Gyokko : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Hover;
		protected override BossAttack[] Attacks => [BossAttack.Teleport, BossAttack.Rain, BossAttack.Summon, BossAttack.Bombs, BossAttack.Spread];
		protected override int ShotType => ModContent.ProjectileType<VaseWater>();
		protected override int Life => 44000;
		protected override int Damage => 76;
		protected override int Defense => 30;
		protected override int Width => 40;
		protected override int Height => 56;
		protected override int MinionType => ModContent.NPCType<VaseFish>();
		protected override float BombTint => 1f;
		protected override int HitDust => DustID.Water;
		protected override int MusicTrack => MusicID.Boss3;
		protected override int ValueGold => 18;
		protected override int BloodMin => 20;
		protected override int BloodMax => 35;
	}

	// Hantengu, Upper Moon Four, who splits into his emotions.
	[AutoloadBossHead]
	public class Hantengu : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Walk;
		protected override BossAttack[] Attacks => [BossAttack.Summon, BossAttack.Ring, BossAttack.Charge, BossAttack.Spikes, BossAttack.Rain];
		protected override int ShotType => ModContent.ProjectileType<LeafWind>();
		protected override int Life => 52000;
		protected override int Damage => 80;
		protected override int Defense => 34;
		protected override int Width => 40;
		protected override int Height => 70;
		protected override int MinionType => ModContent.NPCType<EmotionClone>();
		protected override float SpikeTint => 2f;
		protected override bool Enrage => true;
		protected override int MusicTrack => MusicID.Boss1;
		protected override int ValueGold => 20;
		protected override int BloodMin => 25;
		protected override int BloodMax => 40;
	}

	// Akaza, Upper Moon Three: relentless martial arts and shockwaves.
	[AutoloadBossHead]
	public class Akaza : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Stalk;
		protected override BossAttack[] Attacks => [BossAttack.Charge, BossAttack.Ring, BossAttack.Barrage, BossAttack.Charge, BossAttack.Spread, BossAttack.Teleport];
		protected override int ShotType => ModContent.ProjectileType<ShockwaveShot>();
		protected override int Life => 72000;
		protected override int Damage => 110;
		protected override int Defense => 50;
		protected override int Width => 36;
		protected override int Height => 60;
		protected override float ShotSpeed => 13f;
		protected override float MoveSpeed => 15f;
		protected override int MoveTicks => 60;
		protected override bool Enrage => true;
		protected override int HitDust => DustID.BlueTorch;
		protected override int MusicTrack => MusicID.Plantera;
		protected override int ValueGold => 25;
		protected override int BloodMin => 25;
		protected override int BloodMax => 45;
		protected override int Scrolls => 2;
	}

	// Doma, Upper Moon Two: freezing lotus blooms and ice dolls.
	[AutoloadBossHead]
	public class Doma : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Hover;
		protected override BossAttack[] Attacks => [BossAttack.Bombs, BossAttack.Spiral, BossAttack.Rain, BossAttack.Summon, BossAttack.Ring, BossAttack.Wall];
		protected override int ShotType => ModContent.ProjectileType<IceShard>();
		protected override int Life => 86000;
		protected override int Damage => 115;
		protected override int Defense => 54;
		protected override int Width => 36;
		protected override int Height => 62;
		protected override int MinionType => ModContent.NPCType<IceDoll>();
		protected override float BombTint => 1f;
		protected override float ShotSpeed => 12f;
		protected override bool Enrage => true;
		protected override int HitDust => DustID.IceTorch;
		protected override int MusicTrack => MusicID.Boss4;
		protected override int ValueGold => 30;
		protected override int BloodMin => 30;
		protected override int BloodMax => 50;
		protected override int Scrolls => 2;
	}

	// Kokushibo, Upper Moon One: Moon Breathing, blades that shed crescent moons.
	[AutoloadBossHead]
	public class Kokushibo : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Stalk;
		protected override BossAttack[] Attacks => [BossAttack.Crescents, BossAttack.Wall, BossAttack.Spread, BossAttack.Charge, BossAttack.Ring, BossAttack.Teleport, BossAttack.Spiral];
		protected override int ShotType => ModContent.ProjectileType<MoonBlade>();
		protected override int Life => 118000;
		protected override int Damage => 140;
		protected override int Defense => 70;
		protected override int Width => 40;
		protected override int Height => 68;
		protected override float BombTint => 2f;
		protected override float ShotSpeed => 13f;
		protected override float MoveSpeed => 14f;
		protected override bool Enrage => true;
		protected override int HitDust => DustID.Shadowflame;
		protected override int MusicTrack => MusicID.Boss5;
		protected override int PotionType => ItemID.SuperHealingPotion;
		protected override int ValueGold => 40;
		protected override int BloodMin => 35;
		protected override int BloodMax => 60;
		protected override int Scrolls => 2;
	}

	// Muzan Kibutsuji, the Demon King. Every attack, and far more of them.
	[AutoloadBossHead]
	public class Muzan : DemonBossBase
	{
		protected override BossMove Movement => BossMove.Hover;
		protected override BossAttack[] Attacks => [BossAttack.Barrage, BossAttack.Wall, BossAttack.Spikes, BossAttack.Bombs, BossAttack.Summon,
			BossAttack.Spiral, BossAttack.Charge, BossAttack.Ring, BossAttack.Crescents, BossAttack.Rain];
		protected override int ShotType => ModContent.ProjectileType<FleshWhip>();
		protected override int Life => 260000;
		protected override int Damage => 170;
		protected override int Defense => 90;
		protected override int Width => 40;
		protected override int Height => 70;
		protected override int MinionType => ModContent.NPCType<BloodBrute>();
		protected override float SpikeTint => 3f;
		protected override float ShotSpeed => 14f;
		protected override float MoveSpeed => 15f;
		protected override int MoveTicks => 60;
		protected override bool Enrage => true;
		protected override int MusicTrack => MusicID.LunarBoss;
		protected override int PotionType => ItemID.SuperHealingPotion;
		protected override int ValueGold => 80;
		protected override int BloodMin => 60;
		protected override int BloodMax => 100;
		protected override int Scrolls => 3;
	}
}
