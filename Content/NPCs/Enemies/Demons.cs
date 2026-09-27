using DemonSlayerMod.Content.Projectiles.Demon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DemonSlayerMod.Content.NPCs.Enemies
{
	// ---- Pre-Hardmode -----------------------------------------------------------------------------------------------

	// The most common demon: a starving, clawed wanderer of the night.
	public class LesserDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Walker;
		protected override int Life => 70;
		protected override int Damage => 18;
		protected override int Defense => 6;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.OverworldNightMonster.Chance * 0.35f;
	}

	// Bigger, horned, and pounces.
	public class HornedDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Leaper;
		protected override int Life => 150;
		protected override int Damage => 26;
		protected override int Defense => 10;
		protected override int Width => 32;
		protected override int Height => 50;
		protected override float KnockbackResist => 0.25f;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => NPC.downedBoss1 ? SpawnCondition.OverworldNightMonster.Chance * 0.15f : 0f;
	}

	// Swims through the ground and lunges out of it.
	public class SwampDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Burrower;
		protected override int Life => 110;
		protected override int Damage => 24;
		protected override int Defense => 8;
		protected override int Width => 30;
		protected override int Height => 30;
		protected override float Speed => 4.5f;
		protected override float KnockbackResist => 0f;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.OverworldNightMonster.Chance * 0.08f;
	}

	// Throws bouncing temari balls.
	public class TemariDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Walker;
		protected override int Life => 180;
		protected override int Damage => 22;
		protected override int Defense => 10;
		protected override int ShotType => ModContent.ProjectileType<TemariBall>();
		protected override int ShotRate => 100;
		protected override float ShotSpeed => 8f;
		protected override bool ShotGravity => true;
		protected override int DustType => DustID.OrangeTorch;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => NPC.downedBoss1 ? SpawnCondition.OverworldNightMonster.Chance * 0.07f : 0f;
	}

	// Flies above you, firing arrows of blood.
	public class ArrowDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 140;
		protected override int Damage => 20;
		protected override int Defense => 8;
		protected override int ShotType => ModContent.ProjectileType<DemonArrow>();
		protected override int ShotRate => 90;
		protected override int ShotCount => 3;
		protected override float ShotSpread => 12f;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => NPC.downedBoss1 ? SpawnCondition.OverworldNightMonster.Chance * 0.07f : 0f;
	}

	// Beats the drums in its body to fire shockwave rings. Lives in caves.
	public class DrumDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Walker;
		protected override int Life => 280;
		protected override int Damage => 30;
		protected override int Defense => 14;
		protected override int Width => 36;
		protected override int Height => 54;
		protected override float KnockbackResist => 0.1f;
		protected override int ShotType => ModContent.ProjectileType<ShockwaveShot>();
		protected override int ShotRate => 150;
		protected override float ShotSpeed => 6f;
		protected override int ShotCount => 8;
		protected override float ShotSpread => 45f;
		protected override int BloodMax => 4;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => NPC.downedBoss3 ? SpawnCondition.Cavern.Chance * 0.06f : 0f;
	}

	// One of Rui's spider "family": spits webs and pounces.
	public class SpiderDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Leaper;
		protected override int Life => 160;
		protected override int Damage => 28;
		protected override int Defense => 10;
		protected override int Width => 34;
		protected override int Height => 34;
		protected override int ShotType => ModContent.ProjectileType<WebShot>();
		protected override int ShotRate => 160;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneForest ? SpawnCondition.OverworldNightMonster.Chance * 0.08f : 0f;
	}

	// A floating head with a whip-like tongue that slides through walls. Lives in caves.
	public class TongueDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 120;
		protected override int Damage => 26;
		protected override int Defense => 6;
		protected override int Width => 30;
		protected override int Height => 30;
		protected override float Speed => 4f;
		protected override bool ThroughWalls => true;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => SpawnCondition.Cavern.Chance * 0.07f;
	}

	// ---- Hardmode ---------------------------------------------------------------------------------------------------

	// A hulking mass of muscle and blood.
	public class BloodBrute : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Leaper;
		protected override int Life => 1300;
		protected override int Damage => 80;
		protected override int Defense => 32;
		protected override int Width => 40;
		protected override int Height => 60;
		protected override float KnockbackResist => 0.05f;
		protected override int BloodMin => 2;
		protected override int BloodMax => 5;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.hardMode ? SpawnCondition.OverworldNightMonster.Chance * 0.18f : 0f;
	}

	// Summoned from the Infinity Castle by a biwa's note; drifts through walls firing blood.
	public class BiwaDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 900;
		protected override int Damage => 70;
		protected override int Defense => 24;
		protected override bool ThroughWalls => true;
		protected override int ShotType => ModContent.ProjectileType<BloodShot>();
		protected override int ShotRate => 80;
		protected override int ShotCount => 3;
		protected override float ShotSpeed => 11f;
		protected override int BloodMax => 4;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.hardMode ? (SpawnCondition.OverworldNightMonster.Chance + SpawnCondition.Cavern.Chance) * 0.07f : 0f;
	}

	// Doma's frozen servants, drifting through the snow.
	public class IceDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 1000;
		protected override int Damage => 75;
		protected override int Defense => 28;
		protected override int DustType => DustID.IceTorch;
		protected override int ShotType => ModContent.ProjectileType<IceShard>();
		protected override int ShotRate => 70;
		protected override int ShotCount => 2;
		protected override float ShotSpeed => 12f;
		protected override int BloodMax => 4;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneSnow ? 0.12f : 0f;
	}

	// Throws spinning blood sickles.
	public class SickleDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Walker;
		protected override int Life => 1600;
		protected override int Damage => 95;
		protected override int Defense => 36;
		protected override int ShotType => ModContent.ProjectileType<BloodSickle>();
		protected override int ShotRate => 75;
		protected override int ShotCount => 2;
		protected override float ShotSpeed => 12f;
		protected override int BloodMin => 2;
		protected override int BloodMax => 5;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => NPC.downedMechBossAny ? SpawnCondition.OverworldNightMonster.Chance * 0.1f : 0f;
	}

	// A burrowing horror of the underground.
	public class FleshCrawler : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Burrower;
		protected override int Life => 1400;
		protected override int Damage => 90;
		protected override int Defense => 30;
		protected override int Width => 40;
		protected override int Height => 40;
		protected override float Speed => 6f;
		protected override float KnockbackResist => 0f;
		protected override int ContactDebuff => BuffID.Bleeding;
		protected override int BloodMax => 5;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.hardMode ? SpawnCondition.Cavern.Chance * 0.08f : 0f;
	}

	// A would-be Upper Moon who wields crackling black thunder.
	public class ThunderDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Leaper;
		protected override int Life => 2600;
		protected override int Damage => 110;
		protected override int Defense => 45;
		protected override float KnockbackResist => 0.05f;
		protected override int DustType => DustID.Electric;
		protected override int ShotType => ModContent.ProjectileType<ThunderShot>();
		protected override int ShotRate => 60;
		protected override int ShotCount => 3;
		protected override float ShotSpeed => 14f;
		protected override int BloodMin => 3;
		protected override int BloodMax => 6;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => NPC.downedPlantBoss ? SpawnCondition.OverworldNightMonster.Chance * 0.1f : 0f;
	}

	// ---- Boss minions (never spawn naturally) -----------------------------------------------------------------------

	// Gyokko's fish, spat from his vases.
	public class VaseFish : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 400;
		protected override int Damage => 60;
		protected override int Defense => 20;
		protected override int Width => 26;
		protected override int Height => 22;
		protected override float Speed => 7f;
		protected override bool ThroughWalls => true;
		protected override int DustType => DustID.Water;
		protected override int ShotType => ModContent.ProjectileType<VaseWater>();
		protected override int ShotRate => 110;
	}

	// One of Hantengu's emotion clones.
	public class EmotionClone : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 900;
		protected override int Damage => 70;
		protected override int Defense => 26;
		protected override float Speed => 7f;
		protected override bool ThroughWalls => true;
		protected override int ShotType => ModContent.ProjectileType<LeafWind>();
		protected override int ShotRate => 90;
		protected override int ShotCount => 3;
		protected override float ShotSpread => 20f;
	}

	// Doma's ice doll: a small copy that fights on its own.
	public class IceDoll : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 1500;
		protected override int Damage => 90;
		protected override int Defense => 40;
		protected override float Speed => 6f;
		protected override bool ThroughWalls => true;
		protected override int DustType => DustID.IceTorch;
		protected override int ShotType => ModContent.ProjectileType<IceShard>();
		protected override int ShotRate => 60;
		protected override int ShotCount => 3;
		protected override float ShotSpeed => 12f;
	}
}
