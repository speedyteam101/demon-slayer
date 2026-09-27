using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Projectiles.Breath
{
	// Leap up, then crash down; landing sets off an explosion. ai[2]: explosion radius.
	public class BreathSlam : BreathProjectile
	{
		public override string Texture => TexturePath + "Crescent";

		private const int RiseTicks = 14;
		private const int MaxTicks = 90;
		private int timer;

		public static void Spawn(IEntitySource source, Player player, int style, float radius, int damage, float knockback) {
			Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<BreathSlam>(),
				damage, knockback, player.whoAmI, style, 0f, radius);
			if (player.whoAmI == Main.myPlayer) {
				player.velocity = new Vector2(player.direction * 4f, -13f);
			}
		}

		public override void SetDefaults() {
			base.SetDefaults();
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.timeLeft = MaxTicks + 5;
			Projectile.friendly = false; // only the landing explosion hurts
		}

		protected override void Update() {
			Player owner = Owner;
			if (!owner.active || owner.dead) {
				Projectile.Kill();
				return;
			}
			timer++;
			Projectile.Center = owner.Center;
			Sparkle(owner.position, owner.width, owner.height, 1.5f, 0.3f);

			if (Projectile.owner != Main.myPlayer) {
				return;
			}
			owner.fallStart = (int)(owner.position.Y / 16f);
			owner.immune = true;
			owner.immuneNoBlink = true;
			owner.immuneTime = System.Math.Max(owner.immuneTime, 10);
			if (timer > RiseTicks) {
				owner.velocity.Y = 18f;
				owner.velocity.X *= 0.9f;
				bool landed = owner.velocity.Y == 0f || Collision.SolidCollision(owner.BottomLeft, owner.width, 4);
				if (landed || timer >= MaxTicks) {
					BreathField.Spawn(Projectile.GetSource_FromThis(), owner.Bottom - new Vector2(0f, 16f), Style, BreathField.Explosion,
						Projectile.ai[2], 10, 1, Projectile.damage, Projectile.knockBack, Projectile.owner);
					Projectile.Kill();
				}
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			return false;
		}
	}
}
