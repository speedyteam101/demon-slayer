using DemonSlayerMod.Content.Projectiles.Demon;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.NPCs.Enemies
{
	// Demons that haunt particular biomes.

	// Swims through desert sand and spits blinding grit.
	public class SandDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Burrower;
		protected override int Life => 130;
		protected override int Damage => 24;
		protected override int Defense => 8;
		protected override int Width => 30;
		protected override int Height => 30;
		protected override float KnockbackResist => 0f;
		protected override int DustType => DustID.Sand;
		protected override int ShotType => ModContent.ProjectileType<SandShot>();
		protected override int ShotRate => 140;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => !Main.dayTime && spawnInfo.Player.ZoneDesert && !spawnInfo.Player.ZoneBeach ? 0.12f : 0f;
	}

	// A mantis-armed demon stalking the jungle.
	public class MantisDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Leaper;
		protected override int Life => 220;
		protected override int Damage => 32;
		protected override int Defense => 12;
		protected override int ContactDebuff => BuffID.Bleeding;
		protected override int DustType => DustID.GreenBlood;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => NPC.downedBoss1 && spawnInfo.Player.ZoneJungle && (!Main.dayTime || !spawnInfo.Player.ZoneOverworldHeight) ? 0.1f : 0f;
	}

	// Rises from the sea at night, spitting brine.
	public class DrownedDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 170;
		protected override int Damage => 26;
		protected override int Defense => 10;
		protected override int DustType => DustID.Water;
		protected override int ShotType => ModContent.ProjectileType<VaseWater>();
		protected override int ShotRate => 100;
		protected override int ShotCount => 2;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => !Main.dayTime && spawnInfo.Player.ZoneBeach ? 0.15f : 0f;
	}

	// A demon that fled the sun all the way down to the Underworld.
	public class HellfireDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 320;
		protected override int Damage => 40;
		protected override int Defense => 18;
		protected override bool LavaImmune => true;
		protected override int DustType => DustID.Torch;
		protected override int ShotType => ModContent.ProjectileType<HellfireShot>();
		protected override int ShotRate => 90;
		protected override int ShotCount => 3;
		protected override float ShotSpread => 14f;
		protected override int BloodMax => 3;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => spawnInfo.Player.ZoneUnderworldHeight ? 0.1f : 0f;
	}

	// Hollow-eyed demons that feed on the Corruption and Crimson.
	public class HollowDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Walker;
		protected override int Life => 190;
		protected override int Damage => 30;
		protected override int Defense => 12;
		protected override float KnockbackResist => 0.3f;
		protected override int ContactDebuff => BuffID.Weak;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => !Main.dayTime && (spawnInfo.Player.ZoneCorrupt || spawnInfo.Player.ZoneCrimson) ? 0.12f : 0f;
	}

	// Numb with cold, it never stops walking.
	public class FrostbittenDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Walker;
		protected override int Life => 160;
		protected override int Damage => 26;
		protected override int Defense => 14;
		protected override int ContactDebuff => BuffID.Chilled;
		protected override int DustType => DustID.IceTorch;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => !Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneSnow ? 0.12f : 0f;
	}

	// Its body is a shattered mirror that scatters light into dizzying beams. Hardmode Hallow.
	public class MirrorDemon : DemonEnemyBase
	{
		protected override DemonKind Kind => DemonKind.Flyer;
		protected override int Life => 1100;
		protected override int Damage => 75;
		protected override int Defense => 30;
		protected override bool ThroughWalls => true;
		protected override int DustType => DustID.PinkFairy;
		protected override int ShotType => ModContent.ProjectileType<PrismShot>();
		protected override int ShotRate => 70;
		protected override int ShotCount => 5;
		protected override float ShotSpread => 12f;
		protected override int BloodMax => 4;

		public override float SpawnChance(NPCSpawnInfo spawnInfo) => Main.hardMode && !Main.dayTime && spawnInfo.Player.ZoneHallow ? 0.1f : 0f;
	}
}
