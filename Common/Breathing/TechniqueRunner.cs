using DemonSlayerMod.Content.Buffs;
using DemonSlayerMod.Content.Projectiles.Breath;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Common.Breathing
{
	// Turns a Technique into projectiles. Runs on the client of the player using it.
	public static class TechniqueRunner
	{
		public static void Execute(Player player, int style, Technique t, int damage, float knockback) {
			IEntitySource source = player.GetSource_ItemUse(player.HeldItem);
			Vector2 aim = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX * player.direction);
			float aimAngle = aim.ToRotation();
			player.ChangeDir(aim.X >= 0 ? 1 : -1);
			int wave = ModContent.ProjectileType<BreathWave>();

			SoundEngine.PlaySound(SoundID.Item71 with { Pitch = 0.2f }, player.Center);

			switch (t.Kind) {
				case TechniqueKind.Wave:
				case TechniqueKind.Fan:
				case TechniqueKind.Homing: {
					float spread = t.Kind == TechniqueKind.Fan ? 16f : 8f;
					float mode = t.Kind == TechniqueKind.Homing ? BreathWave.Seeking : BreathWave.Straight;
					for (int i = 0; i < t.Count; i++) {
						float angle = MathHelper.ToRadians(spread * (i - (t.Count - 1) / 2f));
						Projectile.NewProjectile(source, player.Center, aim.RotatedBy(angle) * t.Speed, wave, damage, knockback, player.whoAmI, style, mode, t.Scale);
					}
					break;
				}
				case TechniqueKind.Thrust:
					player.velocity = aim * 9f;
					Projectile.NewProjectile(source, player.Center, aim * t.Speed, ModContent.ProjectileType<BreathThrust>(), damage, knockback, player.whoAmI, style, 0f, t.Scale);
					break;
				case TechniqueKind.Sweep:
					BreathSweep.Spawn(source, player, style, aimAngle, MathHelper.ToRadians(160f), 110f * t.Scale, t.Duration, 1, damage, knockback);
					break;
				case TechniqueKind.Spin:
					BreathSweep.Spawn(source, player, style, aimAngle, MathHelper.TwoPi * t.Count, 95f * t.Scale, t.Duration, t.Count, damage, knockback);
					break;
				case TechniqueKind.Dash:
					BreathDash.Spawn(source, player, style, aimAngle, t.Speed, t.Duration, 0, damage, knockback);
					break;
				case TechniqueKind.MultiDash:
					BreathDash.Spawn(source, player, style, aimAngle, t.Speed, t.Duration, t.Count - 1, damage, knockback);
					break;
				case TechniqueKind.Barrage:
				case TechniqueKind.Vortex: {
					Vector2 target = ClampReach(player, Main.MouseWorld, 500f);
					int mode = t.Kind == TechniqueKind.Vortex ? BreathField.Vortex : BreathField.Barrage;
					BreathField.Spawn(source, target, style, mode, 80f * t.Scale, t.Duration, t.Count, damage, knockback, player.whoAmI);
					break;
				}
				case TechniqueKind.Rain: {
					Vector2 target = ClampReach(player, Main.MouseWorld, 600f);
					for (int i = 0; i < t.Count; i++) {
						Vector2 start = target + new Vector2(Main.rand.NextFloat(-120f, 120f), -520f - i * 50f);
						Vector2 velocity = (target + new Vector2(Main.rand.NextFloat(-60f, 60f), 0f) - start).SafeNormalize(Vector2.UnitY) * t.Speed;
						Projectile.NewProjectile(source, start, velocity, wave, damage, knockback, player.whoAmI, style, BreathWave.Straight, t.Scale);
					}
					break;
				}
				case TechniqueKind.Pillar: {
					// Eruptions march along the ground from you toward the cursor.
					float step = 70f * t.Scale;
					int dir = aim.X >= 0 ? 1 : -1;
					for (int i = 1; i <= t.Count; i++) {
						Vector2 spot = FindGround(player.Center + new Vector2(dir * step * i, 0f));
						BreathField.Spawn(source, spot, style, BreathField.Pillar, 80f * t.Scale, 24 + i * 4, 1, damage, knockback, player.whoAmI);
					}
					break;
				}
				case TechniqueKind.Slam:
					BreathSlam.Spawn(source, player, style, 110f * t.Scale, damage, knockback);
					break;
				case TechniqueKind.Burst:
					for (int i = 0; i < t.Count; i++) {
						Vector2 dir = (aimAngle + MathHelper.TwoPi * i / t.Count).ToRotationVector2();
						Projectile.NewProjectile(source, player.Center, dir * t.Speed, wave, damage, knockback, player.whoAmI, style, BreathWave.Straight, t.Scale);
					}
					break;
				case TechniqueKind.Buff:
					int buff = t.Buff switch {
						BuffKind.DeadCalm => ModContent.BuffType<DeadCalmBuff>(),
						BuffKind.Senses => ModContent.BuffType<SensesBuff>(),
						_ => ModContent.BuffType<AfterimageBuff>()
					};
					player.AddBuff(buff, t.Duration);
					SoundEngine.PlaySound(SoundID.Item4, player.Center);
					for (int i = 0; i < 30; i++) {
						Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, BreathingStyles.DustOf(style), 0f, -2f, 100, default, 1.6f);
						dust.noGravity = true;
					}
					break;
			}
		}

		private static Vector2 ClampReach(Player player, Vector2 point, float reach) {
			Vector2 offset = point - player.Center;
			return offset.Length() > reach ? player.Center + offset.SafeNormalize(Vector2.Zero) * reach : point;
		}

		// The top of the first solid tile at or below `point` (searching 25 tiles down), or the point itself.
		private static Vector2 FindGround(Vector2 point) {
			int x = (int)(point.X / 16f);
			int y = (int)(point.Y / 16f);
			for (int j = y; j < y + 25; j++) {
				if (WorldGen.InWorld(x, j) && WorldGen.SolidTile(x, j)) {
					return new Vector2(point.X, j * 16f);
				}
			}
			return point + new Vector2(0f, 40f);
		}
	}
}
