using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Pets
{
	public class KasugaiCrowBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<KasugaiCrow>());
		}
	}

	// A crow that flies over your shoulder. It squawks when demons come near.
	public class KasugaiCrow : ModProjectile
	{
		private const int Frames = 4;

		public override void SetStaticDefaults() {
			Main.projFrames[Type] = Frames;
			Main.projPet[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 24;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 2;
			Projectile.aiStyle = -1;
		}

		public override void AI() {
			Player owner = Main.player[Projectile.owner];
			if (!owner.active || owner.dead || !owner.HasBuff<KasugaiCrowBuff>()) {
				Projectile.Kill();
				return;
			}
			Projectile.timeLeft = 2;

			Vector2 target = owner.Center + new Vector2(-owner.direction * 40f, -50f + (float)System.Math.Sin(Main.GameUpdateCount / 20f) * 6f);
			Vector2 toTarget = target - Projectile.Center;
			if (toTarget.Length() > 1200f) {
				Projectile.Center = target;
			}
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, toTarget * 0.08f, 0.2f);
			Projectile.spriteDirection = owner.direction;

			if (++Projectile.frameCounter >= 6) {
				Projectile.frameCounter = 0;
				Projectile.frame = (Projectile.frame + 1) % Frames;
			}

			// Caw when a demon is close.
			if (Projectile.owner == Main.myPlayer && Main.GameUpdateCount % 300 == 0) {
				foreach (NPC npc in Main.ActiveNPCs) {
					if (NPCs.DemonTraits.IsDemon(npc) && Vector2.Distance(npc.Center, owner.Center) < 900f) {
						CombatText.NewText(Projectile.Hitbox, new Color(220, 220, 220), "Caw! Demon nearby!");
						Terraria.Audio.SoundEngine.PlaySound(SoundID.Zombie15 with { Pitch = 0.5f }, Projectile.Center);
						break;
					}
				}
			}
		}
	}
}
