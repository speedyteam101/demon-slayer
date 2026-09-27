using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace DemonSlayerMod.Content.Projectiles.Breath
{
	// A giant cut that swings around the player. ai[1]: total arc in radians (2*pi per full spin), ai[2]: reach in pixels.
	// Spawn velocity: X = duration in ticks, Y = the aim angle the arc is centred on.
	public class BreathSweep : BreathProjectile
	{
		public override string Texture => TexturePath + "Crescent";

		private float Arc => Projectile.ai[1];
		private float Reach => Projectile.ai[2];
		private int duration;
		private float centerAngle;
		private int direction;
		private float angle;
		private float lastAngle;

		public static void Spawn(Terraria.DataStructures.IEntitySource source, Player player, int style, float aimAngle, float arc, float reach, int duration, int hits, int damage, float knockback) {
			int index = Projectile.NewProjectile(source, player.Center, new Vector2(duration, aimAngle), Terraria.ModLoader.ModContent.ProjectileType<BreathSweep>(),
				damage, knockback, player.whoAmI, style, arc, reach);
			if (index < Main.maxProjectiles) {
				Main.projectile[index].localNPCHitCooldown = System.Math.Max(5, duration / System.Math.Max(1, hits));
			}
		}

		public override void SetDefaults() {
			base.SetDefaults();
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.timeLeft = 2;
			Projectile.ownerHitCheck = true;
		}

		protected override void Setup() {
			duration = System.Math.Max(4, (int)Projectile.velocity.X);
			centerAngle = Projectile.velocity.Y;
			Projectile.velocity = Vector2.Zero;
			Projectile.timeLeft = duration;
			direction = System.Math.Cos(centerAngle) >= 0 ? 1 : -1;
			angle = lastAngle = StartAngle;
			if (Projectile.localNPCHitCooldown < 0) {
				Projectile.localNPCHitCooldown = duration;
			}
		}

		// The swing goes top to bottom on the side you're facing.
		private float StartAngle => centerAngle - Arc / 2f * direction;

		protected override void Update() {
			Player owner = Owner;
			if (!owner.active || owner.dead) {
				Projectile.Kill();
				return;
			}
			float progress = 1f - (float)Projectile.timeLeft / duration;
			lastAngle = angle;
			angle = StartAngle + Arc * direction * progress;
			Projectile.Center = owner.Center;
			owner.ChangeDir(direction);

			Vector2 tip = owner.Center + angle.ToRotationVector2() * Reach;
			for (int i = 0; i < 2; i++) {
				Sparkle(tip - new Vector2(10f), 20, 20, 1.5f, 1f);
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Vector2 center = Owner.Center;
			float collisionPoint = 0f;
			// Check a few lines between last tick's angle and this one so fast spins don't skip enemies.
			for (int i = 0; i <= 3; i++) {
				float a = MathHelper.Lerp(lastAngle, angle, i / 3f);
				Vector2 end = center + a.ToRotationVector2() * Reach;
				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center, end, 30f, ref collisionPoint)) {
					return true;
				}
			}
			return false;
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 origin = texture.Size() / 2f;
			float scale = Reach / 60f;
			Color color = StyleColor with { A = 0 };
			// The crescent sits along the swing, with fading copies trailing behind.
			for (int i = 4; i >= 0; i--) {
				float a = angle - direction * i * 0.18f;
				Vector2 position = Owner.Center + a.ToRotationVector2() * Reach * 0.6f - Main.screenPosition;
				float fade = 1f - i / 5f;
				Main.EntitySpriteDraw(texture, position, null, color * fade, a, origin, scale, SpriteEffects.None);
			}
			Vector2 front = Owner.Center + angle.ToRotationVector2() * Reach * 0.6f - Main.screenPosition;
			Main.EntitySpriteDraw(texture, front, null, Color.White with { A = 0 } * 0.6f, angle, origin, scale * 0.75f, SpriteEffects.None);
			return false;
		}
	}
}
