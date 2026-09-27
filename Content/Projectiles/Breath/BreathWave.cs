using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Projectiles.Breath
{
	// A flying crescent slash. ai[1]: 0 = straight, 1 = homing. ai[2]: size.
	public class BreathWave : BreathProjectile
	{
		public const int Straight = 0;
		public const int Seeking = 1;

		public override string Texture => TexturePath + "Crescent";

		protected virtual int Pierce => 6;
		protected virtual int Life => 45;
		private float Size => Projectile.ai[2] <= 0f ? 1f : Projectile.ai[2];

		public override void SetDefaults() {
			base.SetDefaults();
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.penetrate = Pierce;
			Projectile.timeLeft = Life;
		}

		protected override void Setup() {
			Projectile.scale = Size;
			Projectile.Resize((int)(40 * Size), (int)(40 * Size));
		}

		protected override void Update() {
			if (Projectile.ai[1] == Seeking) {
				NPC target = FindTarget(700f);
				if (target != null) {
					float speed = Projectile.velocity.Length();
					Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Projectile.velocity) * speed;
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.12f).SafeNormalize(desired) * speed;
				}
			}
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Projectile.timeLeft < 12) {
				Projectile.alpha = 255 - Projectile.timeLeft * 21;
			}
			if (Main.rand.NextBool(2)) {
				Sparkle(Projectile.position, Projectile.width, Projectile.height);
			}
		}

		private NPC FindTarget(float range) {
			NPC best = null;
			float bestDistance = range;
			foreach (NPC npc in Main.ActiveNPCs) {
				float distance = Vector2.Distance(npc.Center, Projectile.Center);
				if (npc.CanBeChasedBy(Projectile) && distance < bestDistance) {
					best = npc;
					bestDistance = distance;
				}
			}
			return best;
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 origin = texture.Size() / 2f;
			Color color = Projectile.GetAlpha(lightColor);
			Vector2 position = Projectile.Center - Main.screenPosition;
			// A few fading copies behind it for a motion trail.
			for (int i = 3; i >= 1; i--) {
				Vector2 back = -Projectile.velocity * i * 0.6f;
				Main.EntitySpriteDraw(texture, position + back, null, color * (0.5f / i), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
			}
			Main.EntitySpriteDraw(texture, position, null, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None);
			Main.EntitySpriteDraw(texture, position, null, Color.White with { A = 0 } * 0.5f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.7f, SpriteEffects.None);
			return false;
		}
	}

	// A long, fast, piercing thrust.
	public class BreathThrust : BreathWave
	{
		public override string Texture => TexturePath + "Thrust";
		protected override int Pierce => -1;
		protected override int Life => 18;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
			float half = 48f * Projectile.scale;
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
				Projectile.Center - direction * half, Projectile.Center + direction * half, 18f * Projectile.scale, ref collisionPoint);
		}
	}
}
