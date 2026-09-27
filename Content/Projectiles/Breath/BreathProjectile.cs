using DemonSlayerMod.Common.Breathing;
using DemonSlayerMod.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Projectiles.Breath
{
	// Shared behaviour for every breathing technique projectile. ai[0] is the breathing style (for colour and dust).
	// Several techniques need more numbers than the three ai slots hold, so they pass extra settings in the spawn
	// velocity and read them on the first tick (see Setup); the velocity is synced when the projectile spawns.
	public abstract class BreathProjectile : ModProjectile
	{
		public const string TexturePath = "DemonSlayerMod/Content/Projectiles/Breath/";

		protected int Style => (int)Projectile.ai[0];
		protected Color StyleColor => BreathingStyles.ColorOf(Style);
		protected int StyleDust => BreathingStyles.DustOf(Style);
		protected Player Owner => Main.player[Projectile.owner];

		private bool setUp;

		public override void SetDefaults() {
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.penetrate = -1;
			Projectile.aiStyle = -1;
		}

		public sealed override void AI() {
			if (!setUp) {
				setUp = true;
				Setup();
			}
			Lighting.AddLight(Projectile.Center, StyleColor.ToVector3() * 0.6f);
			Update();
		}

		protected virtual void Setup() { }

		protected abstract void Update();

		protected void Sparkle(Vector2 position, int width, int height, float scale = 1.2f, float speed = 0.4f) {
			Dust dust = Dust.NewDustDirect(position, width, height, StyleDust, 0f, 0f, 100, default, scale);
			dust.noGravity = true;
			dust.velocity *= speed;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			SlayerPlayer slayer = SlayerPlayer.Get(Owner);
			slayer.OnBladeHit(target);
			if (BreathingStyles.Valid(Style) && BreathingStyles.All[Style].Debuff >= 0) {
				target.AddBuff(BreathingStyles.All[Style].Debuff, 240);
			}
			// Sound Breathing: strikes explode.
			if (Style == BreathingStyles.Sound && this is not BreathField && Projectile.owner == Main.myPlayer && Main.rand.NextBool(3)) {
				BreathField.Spawn(Projectile.GetSource_FromThis(), target.Center, Style, BreathField.Explosion, 60f, 10, 1,
					Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
			}
		}

		public override Color? GetAlpha(Color lightColor) {
			return (StyleColor * Projectile.Opacity) with { A = 0 };
		}
	}
}
