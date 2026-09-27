using DemonSlayerMod.Content.Items;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.NPCs.Enemies
{
	public enum DemonKind
	{
		Walker,   // walks and jumps like a zombie
		Leaper,   // walks, and pounces at you when close
		Flyer,    // hovers near you
		Burrower  // swims through the ground, circling and lunging
	}

	// Shared template for the smaller demons. Sprites are 4 vertical frames facing LEFT like vanilla enemies.
	public abstract class DemonEnemyBase : ModNPC, IDemon
	{
		private const int FrameCount = 4;

		public int SinceHit { get; set; }

		protected abstract DemonKind Kind { get; }
		protected abstract int Life { get; }
		protected abstract int Damage { get; }
		protected abstract int Defense { get; }
		protected virtual int Width => 28;
		protected virtual int Height => 44;
		protected virtual float Speed => 5f;
		protected virtual float KnockbackResist => 0.4f;
		protected virtual bool ThroughWalls => false;
		protected virtual int DustType => DustID.Blood;
		protected virtual int ContactDebuff => -1;
		protected virtual int ShotType => -1;
		protected virtual int ShotRate => 120;
		protected virtual float ShotSpeed => 9f;
		protected virtual int ShotCount => 1;
		protected virtual float ShotSpread => 10f;  // degrees between shots; 360/ShotCount makes a ring
		protected virtual bool ShotGravity => false;
		protected virtual int BloodMin => 1;
		protected virtual int BloodMax => 2;
		protected virtual bool LavaImmune => false;

		private ref float ShotTimer => ref NPC.localAI[0];
		private ref float MoveTimer => ref NPC.localAI[1];

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = FrameCount;
		}

		public override void SetDefaults() {
			NPC.width = Width;
			NPC.height = Height;
			NPC.damage = Damage;
			NPC.defense = Defense;
			NPC.lifeMax = Life;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.value = Life * 3f;
			NPC.knockBackResist = KnockbackResist;
			NPC.lavaImmune = LavaImmune;

			if (Kind == DemonKind.Walker || Kind == DemonKind.Leaper) {
				NPC.aiStyle = NPCAIStyleID.Fighter;
				AIType = NPCID.Zombie;
			}
			else {
				NPC.aiStyle = -1;
				NPC.noGravity = true;
				NPC.noTileCollide = Kind == DemonKind.Burrower || ThroughWalls;
			}
		}

		public override void AI() {
			DemonTraits.Update(NPC, this, 0.02f);
			if (Kind != DemonKind.Walker && Kind != DemonKind.Leaper) {
				NPC.TargetClosest();
			}
			Player player = Main.player[NPC.target];
			MoveTimer++;

			switch (Kind) {
				case DemonKind.Leaper:
					// Fighter AI does the walking; pounce when close and on the ground.
					if (NPC.velocity.Y == 0f && MoveTimer % 90 == 0 && Math.Abs(player.Center.X - NPC.Center.X) < 260f) {
						NPC.velocity = new Vector2(NPC.direction * 8f, -8f);
						NPC.netUpdate = true;
					}
					break;
				case DemonKind.Flyer:
					Vector2 hover = player.Center + new Vector2((float)Math.Sin(MoveTimer / 50f) * 180f, -150f);
					DemonUtils.FlyToward(NPC, hover, Speed, 25f);
					if (!ThroughWalls) {
						if (NPC.collideX) {
							NPC.velocity.X = -NPC.oldVelocity.X * 0.6f;
						}
						if (NPC.collideY) {
							NPC.velocity.Y = -NPC.oldVelocity.Y * 0.6f;
						}
					}
					NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
					break;
				case DemonKind.Burrower:
					if (MoveTimer % 120 == 90) {
						NPC.velocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * Speed * 2.2f;
					}
					else if (MoveTimer % 120 < 90) {
						Vector2 circle = player.Center + new Vector2((float)Math.Cos(MoveTimer / 20f) * 170f, 90f);
						DemonUtils.FlyToward(NPC, circle, Speed, 25f);
					}
					NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
					if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && Main.rand.NextBool(3)) {
						Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Dirt);
					}
					break;
			}

			if (ShotType >= 0) {
				ShotTimer++;
				if (ShotTimer >= ShotRate && Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1) && Vector2.Distance(NPC.Center, player.Center) < 700f) {
					ShotTimer = 0f;
					Vector2 aim = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
					if (ShotGravity) {
						aim = (aim + new Vector2(0f, -0.5f)).SafeNormalize(Vector2.UnitY);
					}
					for (int i = 0; i < ShotCount; i++) {
						float angle = MathHelper.ToRadians(ShotSpread * (i - (ShotCount - 1) / 2f));
						DemonUtils.Shoot(NPC, NPC.Center, aim.RotatedBy(angle) * ShotSpeed, ShotType, NPC.damage / 3, ShotGravity ? 1f : 0f);
					}
				}
			}

			NPC.spriteDirection = NPC.direction;
		}

		public override void FindFrame(int frameHeight) {
			if (Kind == DemonKind.Walker || Kind == DemonKind.Leaper) {
				if (NPC.velocity.Y != 0f) {
					NPC.frame.Y = frameHeight;
					return;
				}
				NPC.frameCounter += Math.Abs(NPC.velocity.X) + 0.15f;
			}
			else {
				NPC.frameCounter += 1;
			}
			if (NPC.frameCounter >= 6) {
				NPC.frameCounter = 0;
				NPC.frame.Y = (NPC.frame.Y + frameHeight) % (FrameCount * frameHeight);
			}
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

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
			if (ContactDebuff >= 0) {
				target.AddBuff(ContactDebuff, 240);
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DemonBlood>(), 2, BloodMin, BloodMax));
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange([
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.DemonSlayerMod.Bestiary." + Name)
			]);
		}

		public override void HitEffect(NPC.HitInfo hit) {
			SinceHit = 0;
			DemonUtils.Bleed(NPC, hit, DustType);
		}
	}
}
