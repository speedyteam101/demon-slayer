using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Projectiles.Breath
{
	// Carries the player through enemies, damaging everything on the way.
	// ai[1]: extra dashes left (each aimed at the nearest enemy), ai[2]: dash speed.
	// Spawn velocity: X = ticks per dash, Y = first dash angle.
	public class BreathDash : BreathProjectile
	{
		public override string Texture => TexturePath + "Crescent";

		private int dashTicks;
		private int timer;
		private Vector2 direction;

		public static void Spawn(IEntitySource source, Player player, int style, float angle, float speed, int ticks, int extraDashes, int damage, float knockback) {
			Projectile.NewProjectile(source, player.Center, new Vector2(ticks, angle), ModContent.ProjectileType<BreathDash>(),
				damage, knockback, player.whoAmI, style, extraDashes, speed);
		}

		public override void SetDefaults() {
			base.SetDefaults();
			Projectile.width = 70;
			Projectile.height = 70;
			Projectile.timeLeft = 600;
		}

		protected override void Setup() {
			dashTicks = System.Math.Max(4, (int)Projectile.velocity.X);
			direction = Projectile.velocity.Y.ToRotationVector2();
			Projectile.velocity = Vector2.Zero;
			Projectile.localNPCHitCooldown = dashTicks;
		}

		protected override void Update() {
			Player owner = Owner;
			if (!owner.active || owner.dead) {
				Projectile.Kill();
				return;
			}
			timer++;
			Projectile.Center = owner.Center;
			Projectile.rotation = direction.ToRotation();

			if (Projectile.owner == Main.myPlayer) {
				owner.velocity = direction * Projectile.ai[2];
				owner.immune = true;
				owner.immuneNoBlink = true;
				owner.immuneTime = System.Math.Max(owner.immuneTime, 10);
				owner.fallStart = (int)(owner.position.Y / 16f);
				owner.ChangeDir(direction.X >= 0 ? 1 : -1);
			}

			for (int i = 0; i < 3; i++) {
				Sparkle(owner.position, owner.width, owner.height, 1.6f, 0.2f);
			}

			if (timer >= dashTicks) {
				NPC next = Projectile.ai[1] > 0 ? FindNext(owner) : null;
				if (next != null) {
					Projectile.ai[1]--;
					timer = 0;
					direction = (next.Center - owner.Center).SafeNormalize(direction);
					Projectile.ResetLocalNPCHitImmunity();
					Projectile.netUpdate = true;
				}
				else {
					if (Projectile.owner == Main.myPlayer) {
						owner.velocity *= 0.25f;
					}
					Projectile.Kill();
				}
			}
		}

		private NPC FindNext(Player owner) {
			NPC best = null;
			float bestDistance = 520f;
			foreach (NPC npc in Main.ActiveNPCs) {
				float distance = Vector2.Distance(npc.Center, owner.Center);
				if (npc.CanBeChasedBy(Projectile) && distance < bestDistance) {
					best = npc;
					bestDistance = distance;
				}
			}
			return best; // null ends the chain
		}

		public override bool PreDraw(ref Color lightColor) {
			return false; // only dust; the player is the "blade"
		}
	}
}
