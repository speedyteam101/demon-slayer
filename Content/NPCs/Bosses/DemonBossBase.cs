using DemonSlayerMod.Common.Systems;
using DemonSlayerMod.Content.Items;
using DemonSlayerMod.Content.Projectiles.Demon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.NPCs.Bosses
{
	public enum BossMove
	{
		Hover,  // floats above you
		Circle, // orbits you
		Stalk,  // keeps level with you at sword's length
		Walk    // walks and leaps (gravity, tile collision)
	}

	public enum BossAttack
	{
		Spread,    // aimed fans of shots
		Ring,      // rings of shots in every direction
		Spiral,    // a rotating spiral
		Charge,    // dashes at you (walkers leap)
		Summon,    // calls minions
		Rain,      // shots fall from above you
		Spikes,    // spikes burst from the ground under you after a warning
		Teleport,  // vanishes and reappears beside you
		Barrage,   // a fast aimed stream
		Bombs,     // warning circles around you that explode
		Crescents, // huge crescents that burst into shards
		Wall       // a sweeping wall of shots from one side
	}

	// Shared template for the demon bosses. It moves for a while, then runs the next attack in its list, and repeats.
	// Below half health everything gets faster and denser; bosses with Enrage get faster again below a quarter.
	// Demons flee at dawn. Sprites are 4 vertical frames facing LEFT like vanilla. Subclasses need [AutoloadBossHead].
	public abstract class DemonBossBase : ModNPC, IDemon
	{
		private const int FrameCount = 4;
		private const int MaxMinions = 6;
		private const int StateMove = 0; // attack states are (int)BossAttack + 1

		public int SinceHit { get; set; }

		protected abstract BossMove Movement { get; }
		protected abstract BossAttack[] Attacks { get; }
		protected abstract int ShotType { get; }
		protected abstract int Life { get; }
		protected abstract int Damage { get; }
		protected abstract int Defense { get; }
		protected abstract int Width { get; }
		protected abstract int Height { get; }
		protected virtual int MinionType => -1;
		protected virtual int CrescentType => ModContent.ProjectileType<MoonCrescent>();
		protected virtual float SpikeTint => 0f;   // DemonSpike ai[0]
		protected virtual float BombTint => 0f;    // DemonBomb ai[0]
		protected virtual int HitDust => DustID.Blood;
		protected virtual float ShotSpeed => 10f;
		protected virtual float MoveSpeed => 11f;
		protected virtual int MoveTicks => 90;
		protected virtual bool Enrage => false;
		protected virtual int PotionType => ItemID.GreaterHealingPotion;
		protected virtual int MusicTrack => MusicID.Boss2;
		protected virtual int ValueGold => 10;
		protected virtual int BloodMin => 15;
		protected virtual int BloodMax => 30;
		protected virtual int Scrolls => 1;
		protected virtual int YotoChance => 25; // 1 in N chance to drop a Yōtō

		private ref float Timer => ref NPC.ai[0];
		private ref float State => ref NPC.ai[1];
		private ref float NextAttack => ref NPC.ai[2];

		protected bool PhaseTwo => NPC.life < NPC.lifeMax / 2;
		protected bool Enraged => Enrage && NPC.life < NPC.lifeMax / 4;
		private bool Walker => Movement == BossMove.Walk;
		private float Tempo => Enraged ? 1.6f : PhaseTwo ? 1.3f : 1f; // how much faster attacks run

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = FrameCount;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
			NPCID.Sets.BossBestiaryPriority.Add(Type);
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
		}

		public override void SetDefaults() {
			NPC.width = Width;
			NPC.height = Height;
			NPC.damage = Damage;
			NPC.defense = Defense;
			NPC.lifeMax = Life;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath10;
			NPC.knockBackResist = 0f;
			NPC.noGravity = !Walker;
			NPC.noTileCollide = !Walker;
			NPC.value = Item.buyPrice(gold: ValueGold);
			NPC.SpawnWithHigherTime(30);
			NPC.boss = true;
			NPC.npcSlots = 10f;
			NPC.aiStyle = -1;
			NPC.lavaImmune = true;
			if (!Main.dedServ) {
				Music = MusicTrack;
			}
		}

		public override void AI() {
			DemonTraits.Update(NPC, this, 0.004f);
			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active) {
				NPC.TargetClosest();
			}
			Player player = Main.player[NPC.target];

			// Demons flee at dawn, or when everyone is dead.
			if (player.dead || Main.dayTime) {
				if (Walker) {
					NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, -NPC.direction * 6f, 0.05f);
					NPC.noTileCollide = true;
				}
				NPC.velocity.Y -= 0.3f;
				NPC.alpha = Math.Min(255, NPC.alpha + 3);
				NPC.EncourageDespawn(10);
				return;
			}

			NPC.direction = NPC.spriteDirection = player.Center.X > NPC.Center.X ? 1 : -1;
			Timer += Tempo;

			// Anything left far behind catches up.
			if (State == StateMove && Vector2.Distance(NPC.Center, player.Center) > 1500f) {
				SetState((int)BossAttack.Teleport + 1);
			}

			if (State == StateMove) {
				Move(player);
				if (Timer >= MoveTicks) {
					SetState((int)Attacks[(int)NextAttack % Attacks.Length] + 1);
					NextAttack++;
				}
				return;
			}

			bool done = (BossAttack)((int)State - 1) switch {
				BossAttack.Spread => Spread(player),
				BossAttack.Ring => Ring(),
				BossAttack.Spiral => Spiral(),
				BossAttack.Charge => Charge(player),
				BossAttack.Summon => Summon(),
				BossAttack.Rain => Rain(player),
				BossAttack.Spikes => Spikes(player),
				BossAttack.Teleport => Teleport(player),
				BossAttack.Barrage => Barrage(player),
				BossAttack.Bombs => Bombs(player),
				BossAttack.Crescents => Crescents(player),
				BossAttack.Wall => Wall(player),
				_ => true
			};
			if (done) {
				SetState(StateMove);
			}
		}

		// Timer advances by Tempo per tick, so "every N ticks" checks use this.
		private bool Every(float interval, float offset = 0f) {
			float before = Timer - Tempo - offset;
			float now = Timer - offset;
			return now >= 0f && Math.Floor(now / interval) != Math.Floor(before / interval);
		}

		private bool At(float tick) => Timer >= tick && Timer - Tempo < tick;

		private void Move(Player player) {
			NPC.alpha = Math.Max(0, NPC.alpha - 15);
			float speed = MoveSpeed * Tempo;
			switch (Movement) {
				case BossMove.Hover:
					DemonUtils.FlyToward(NPC, player.Center + new Vector2((float)Math.Sin(Timer / 40f) * 260f, -250f), speed, 20f);
					break;
				case BossMove.Circle:
					DemonUtils.FlyToward(NPC, player.Center + (Timer / 40f).ToRotationVector2() * 360f, speed, 15f);
					break;
				case BossMove.Stalk:
					float side = NPC.Center.X < player.Center.X ? -1f : 1f;
					DemonUtils.FlyToward(NPC, player.Center + new Vector2(side * 260f, -40f), speed, 12f);
					break;
				case BossMove.Walk:
					NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * speed * 0.35f, 0.1f);
					if (NPC.velocity.Y == 0f && (NPC.collideX || player.Bottom.Y < NPC.Top.Y - 48f)) {
						NPC.velocity.Y = -11f;
					}
					break;
			}
		}

		// ---- Attacks. Each returns true when finished. ----

		private bool Spread(Player player) {
			Brake();
			int volleys = PhaseTwo ? 4 : 3;
			if (Every(25f, 10f) && Timer < 25 * volleys) {
				int shots = PhaseTwo ? 7 : 5;
				Vector2 aim = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				for (int i = 0; i < shots; i++) {
					Fire(NPC.Center, aim.RotatedBy(MathHelper.ToRadians(11f * (i - (shots - 1) / 2f))) * ShotSpeed);
				}
				SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
			}
			return Timer >= 25 * volleys + 15;
		}

		private bool Ring() {
			Brake();
			int rings = PhaseTwo ? 3 : 2;
			if (Every(30f, 15f) && Timer < 30 * rings) {
				int shots = Enraged ? 24 : PhaseTwo ? 18 : 12;
				float offset = Timer * 0.2f;
				for (int i = 0; i < shots; i++) {
					Fire(NPC.Center, (offset + MathHelper.TwoPi * i / shots).ToRotationVector2() * ShotSpeed * 0.8f);
				}
				SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
			}
			return Timer >= 30 * rings + 20;
		}

		private bool Spiral() {
			Brake();
			if (Every(6f)) {
				int arms = Enraged ? 4 : PhaseTwo ? 3 : 2;
				for (int i = 0; i < arms; i++) {
					Fire(NPC.Center, (Timer * 0.11f + MathHelper.TwoPi * i / arms).ToRotationVector2() * ShotSpeed * 0.8f);
				}
			}
			return Timer >= 150;
		}

		private bool Charge(Player player) {
			int charges = Enraged ? 4 : PhaseTwo ? 3 : 2;
			float t = Timer % 55f;
			if (Every(55f, 18f) && Timer < 55 * charges) {
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
				if (Walker) {
					NPC.velocity = new Vector2(MathHelper.Clamp((player.Center.X - NPC.Center.X) / 30f, -14f, 14f), -13f);
				}
				else {
					NPC.velocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * (PhaseTwo ? 26f : 21f);
				}
				NPC.netUpdate = true;
			}
			else if (t < 18f && !Walker) {
				NPC.velocity *= 0.85f;
			}
			if (!Walker && Every(4f) && NPC.velocity.Length() > 15f) {
				Fire(NPC.Center, Vector2.Zero); // leaves a trail of lingering shots
			}
			return Timer >= 55 * charges;
		}

		private bool Summon() {
			Brake();
			if (At(20f) && MinionType >= 0 && DemonUtils.CountActive(MinionType) < MaxMinions) {
				int count = PhaseTwo ? 3 : 2;
				for (int i = 0; i < count; i++) {
					DemonUtils.SpawnMinion(NPC, MinionType, NPC.Center + new Vector2(Main.rand.NextFloat(-120f, 120f), -40f));
				}
				SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
			}
			return Timer >= 50;
		}

		private bool Rain(Player player) {
			Brake();
			if (Every(PhaseTwo ? 5f : 8f)) {
				Vector2 start = player.Center + new Vector2(Main.rand.NextFloat(-550f, 550f), -600f);
				Fire(start, new Vector2(Main.rand.NextFloat(-1f, 1f), ShotSpeed * 0.7f));
			}
			return Timer >= 150;
		}

		private bool Spikes(Player player) {
			Brake();
			int waves = PhaseTwo ? 3 : 2;
			if (Every(35f, 10f) && Timer < 35 * waves) {
				int count = Enraged ? 9 : PhaseTwo ? 7 : 5;
				for (int i = 0; i < count; i++) {
					float x = (i - (count - 1) / 2f) * 64f + player.velocity.X * 20f;
					Vector2 ground = DemonUtils.GroundBelow(player.Center + new Vector2(x, 0f));
					DemonUtils.Shoot(NPC, ground - new Vector2(0f, 45f), Vector2.Zero, ModContent.ProjectileType<DemonSpike>(), ShotDamage, SpikeTint);
				}
			}
			return Timer >= 35 * waves + 50;
		}

		private bool Teleport(Player player) {
			Brake();
			if (Timer < 30) {
				NPC.alpha = Math.Min(255, NPC.alpha + 9);
			}
			if (At(30f) && Main.netMode != NetmodeID.MultiplayerClient) {
				float side = Main.rand.NextBool() ? 1f : -1f;
				NPC.Center = Walker
					? new Vector2(player.Center.X + side * 320f, player.Bottom.Y - NPC.height / 2f)
					: player.Center + new Vector2(side * 340f, -140f);
				NPC.velocity = Vector2.Zero;
				NPC.netUpdate = true;
				// Arrive with a ring of shots.
				for (int i = 0; i < 10; i++) {
					Fire(NPC.Center, (MathHelper.TwoPi * i / 10f).ToRotationVector2() * ShotSpeed * 0.6f);
				}
			}
			if (Timer > 30) {
				NPC.alpha = Math.Max(0, NPC.alpha - 20);
			}
			return Timer >= 50;
		}

		private bool Barrage(Player player) {
			Brake();
			if (Every(PhaseTwo ? 4f : 6f, 20f)) {
				Vector2 aim = (player.Center + player.velocity * 12f - NPC.Center).SafeNormalize(Vector2.UnitY);
				Fire(NPC.Center, aim.RotatedByRandom(0.12f) * ShotSpeed * 1.3f);
			}
			return Timer >= 110;
		}

		private bool Bombs(Player player) {
			Brake();
			int waves = PhaseTwo ? 4 : 3;
			if (Every(28f, 5f) && Timer < 28 * waves) {
				int count = Enraged ? 5 : 3;
				for (int i = 0; i < count; i++) {
					Vector2 spot = player.Center + player.velocity * 20f + Main.rand.NextVector2Circular(200f, 140f);
					DemonUtils.Shoot(NPC, spot, Vector2.Zero, ModContent.ProjectileType<DemonBomb>(), ShotDamage, BombTint, 80f);
				}
			}
			return Timer >= 28 * waves + 60;
		}

		private bool Crescents(Player player) {
			Brake();
			int count = Enraged ? 4 : PhaseTwo ? 3 : 2;
			if (Every(30f, 10f) && Timer < 30 * count) {
				Vector2 aim = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
				DemonUtils.Shoot(NPC, NPC.Center, aim * ShotSpeed * 0.8f, CrescentType, ShotDamage * 3 / 2);
				SoundEngine.PlaySound(SoundID.Item71, NPC.Center);
			}
			return Timer >= 30 * count + 40;
		}

		private bool Wall(Player player) {
			Brake();
			// Shots stream in from one side with a gap to slip through.
			if (Every(20f, 10f) && Timer < 100) {
				float side = ((int)(Timer / 100f) % 2 == 0) == (NPC.whoAmI % 2 == 0) ? -1f : 1f;
				int gap = Main.rand.Next(2, 9);
				for (int i = 0; i < 11; i++) {
					if (i == gap || i == gap + 1) {
						continue;
					}
					Vector2 start = player.Center + new Vector2(side * 700f, (i - 5) * 60f);
					Fire(start, new Vector2(-side * ShotSpeed * 0.9f, 0f));
				}
			}
			return Timer >= 130;
		}

		private int ShotDamage => NPC.damage / 4;

		private void Fire(Vector2 position, Vector2 velocity) {
			DemonUtils.Shoot(NPC, position, velocity, ShotType, ShotDamage);
		}

		private void Brake() {
			if (Walker) {
				NPC.velocity.X *= 0.85f;
			}
			else {
				NPC.velocity *= 0.92f;
			}
		}

		private void SetState(int state) {
			State = state;
			Timer = 0f;
			NPC.netUpdate = true;
		}

		public override void FindFrame(int frameHeight) {
			NPC.frameCounter += State == (int)BossAttack.Charge + 1 ? 2 : 1;
			if (NPC.frameCounter >= 7) {
				NPC.frameCounter = 0;
				NPC.frame.Y = (NPC.frame.Y + frameHeight) % (FrameCount * frameHeight);
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			cooldownSlot = ImmunityCooldownID.Bosses;
			return NPC.alpha < 150; // not while faded out
		}

		public override void UpdateLifeRegen(ref int damage) {
			DemonTraits.Sunburn(NPC, ref damage);
		}

		public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers) {
			DemonTraits.ModifyHitByItem(item, ref modifiers);
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) {
			DemonTraits.ModifyHitByProjectile(projectile, ref modifiers);
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonBlood>(), 1, BloodMin, BloodMax));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TrainingScroll>(), 1, Scrolls, Scrolls));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Yoto>(), YotoChance));
		}

		public override void BossLoot(ref int potionType) {
			potionType = PotionType;
		}

		public override void OnKill() {
			DownedDemonSystem.MarkDowned(Name);
			if (Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.WorldData);
			}
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange([
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.DemonSlayerMod.Bestiary." + Name)
			]);
		}

		public override void HitEffect(NPC.HitInfo hit) {
			SinceHit = 0;
			DemonUtils.Bleed(NPC, hit, HitDust);
		}
	}
}
