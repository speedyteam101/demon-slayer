using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace DemonSlayerMod.Content.Projectiles.Breath
{
	// An area that stays put and hits whatever is inside it several times.
	// ai[1]: mode (Barrage, Vortex, Pillar, Explosion), ai[2]: radius in pixels.
	// Spawn velocity: X = duration in ticks, Y = how many times it can hit each enemy.
	// A Pillar is anchored at its base (the ground), everything else at its centre.
	public class BreathField : BreathProjectile
	{
		public const int Barrage = 0;
		public const int Vortex = 1;
		public const int Pillar = 2;
		public const int Explosion = 3;

		public override string Texture => TexturePath + "Slash";

		private int Mode => (int)Projectile.ai[1];
		private float Radius => Projectile.ai[2];
		private int duration;

		public static void Spawn(IEntitySource source, Vector2 position, int style, int mode, float radius, int duration, int hits, int damage, float knockback, int owner) {
			Projectile.NewProjectile(source, position, new Vector2(duration, hits), ModContent.ProjectileType<BreathField>(),
				damage, knockback, owner, style, mode, radius);
		}

		public override void SetDefaults() {
			base.SetDefaults();
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.timeLeft = 2;
		}

		protected override void Setup() {
			duration = System.Math.Max(2, (int)Projectile.velocity.X);
			int hits = System.Math.Max(1, (int)Projectile.velocity.Y);
			Projectile.velocity = Vector2.Zero;
			Projectile.timeLeft = duration;
			Projectile.localNPCHitCooldown = hits <= 1 ? -1 : System.Math.Max(4, duration / hits);

			if (Mode == Explosion) {
				Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
				for (int i = 0; i < 30; i++) {
					Vector2 speed = Main.rand.NextVector2Circular(1f, 1f) * Radius / 8f;
					Dust dust = Dust.NewDustPerfect(Projectile.Center, StyleDust, speed, 100, default, 1.8f);
					dust.noGravity = true;
				}
			}
			else if (Mode == Pillar) {
				Terraria.Audio.SoundEngine.PlaySound(SoundID.Item69, Projectile.Center);
			}
		}

		private Rectangle PillarBox => new((int)(Projectile.Center.X - Radius * 0.3f), (int)(Projectile.Center.Y - Radius * 2.2f), (int)(Radius * 0.6f), (int)(Radius * 2.2f));

		protected override void Update() {
			float progress = 1f - (float)Projectile.timeLeft / duration;
			switch (Mode) {
				case Barrage:
					if (Main.rand.NextBool(2)) {
						Vector2 spot = Projectile.Center + Main.rand.NextVector2Circular(Radius, Radius);
						Sparkle(spot - new Vector2(8f), 16, 16, 1.4f, 2f);
					}
					if (Projectile.timeLeft % 6 == 0) {
						Terraria.Audio.SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.5f, Pitch = 0.4f }, Projectile.Center);
					}
					break;
				case Vortex:
					for (int i = 0; i < 2; i++) {
						float angle = Main.GlobalTimeWrappedHourly * 12f + i * MathHelper.Pi;
						Vector2 spot = Projectile.Center + angle.ToRotationVector2() * Radius * (1f - progress * 0.5f);
						Dust dust = Dust.NewDustPerfect(spot, StyleDust, (Projectile.Center - spot) * 0.05f, 100, default, 1.5f);
						dust.noGravity = true;
					}
					// Pull enemies in (the server moves NPCs; in single player that is us).
					if (Main.netMode != NetmodeID.MultiplayerClient) {
						foreach (NPC npc in Main.ActiveNPCs) {
							if (!npc.friendly && !npc.boss && npc.knockBackResist > 0f && Vector2.Distance(npc.Center, Projectile.Center) < Radius * 2f) {
								npc.velocity = Vector2.Lerp(npc.velocity, (Projectile.Center - npc.Center).SafeNormalize(Vector2.Zero) * 6f, 0.15f);
								if (Projectile.timeLeft % 10 == 0) {
									npc.netUpdate = true;
								}
							}
						}
					}
					break;
				case Pillar:
					Rectangle box = PillarBox;
					Sparkle(new Vector2(box.X, box.Y + box.Height * progress * 0.5f), box.Width, box.Height / 2, 1.6f, 1.5f);
					break;
			}
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			if (Mode == Pillar) {
				return PillarBox.Intersects(targetHitbox);
			}
			Vector2 closest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
			return Vector2.Distance(closest, Projectile.Center) <= Radius;
		}

		public override bool PreDraw(ref Color lightColor) {
			float fade = System.Math.Min(1f, Projectile.timeLeft / 10f);
			Color color = StyleColor with { A = 0 } * fade;
			Vector2 center = Projectile.Center - Main.screenPosition;

			if (Mode == Barrage || Mode == Vortex) {
				// Random slash marks that change every few ticks.
				Texture2D slash = ModContent.Request<Texture2D>(Texture).Value;
				var rand = new UnifiedRandom(Projectile.identity * 97 + Projectile.timeLeft / 3);
				for (int i = 0; i < 4; i++) {
					Vector2 offset = new Vector2(rand.NextFloat(-1f, 1f), rand.NextFloat(-1f, 1f)) * Radius * 0.7f;
					float rotation = rand.NextFloat(MathHelper.TwoPi);
					float scale = Radius / 64f * rand.NextFloat(0.7f, 1.3f);
					Main.EntitySpriteDraw(slash, center + offset, null, color, rotation, slash.Size() / 2f, scale, SpriteEffects.None);
					Main.EntitySpriteDraw(slash, center + offset, null, Color.White with { A = 0 } * 0.5f * fade, rotation, slash.Size() / 2f, scale * 0.6f, SpriteEffects.None);
				}
			}
			else if (Mode == Pillar) {
				Texture2D pillar = ModContent.Request<Texture2D>(TexturePath + "Pillar").Value;
				Rectangle box = PillarBox;
				var scale = new Vector2(box.Width / (float)pillar.Width, box.Height / (float)pillar.Height);
				var origin = new Vector2(pillar.Width / 2f, pillar.Height);
				Main.EntitySpriteDraw(pillar, center, null, color, 0f, origin, scale, SpriteEffects.None);
				Main.EntitySpriteDraw(pillar, center, null, Color.White with { A = 0 } * 0.4f * fade, 0f, origin, scale * new Vector2(0.5f, 1f), SpriteEffects.None);
			}
			return false;
		}
	}
}
