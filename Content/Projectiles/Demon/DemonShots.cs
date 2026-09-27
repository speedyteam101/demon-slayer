using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Projectiles.Demon
{
	// Hostile shots fired by demons. All of them are drawn from a few white shapes tinted with Color.
	// ai[0] == 1 makes the shot fall with gravity.
	public abstract class DemonShot : ModProjectile
	{
		public const string ShapePath = "DemonSlayerMod/Content/Projectiles/Demon/";

		protected abstract string Shape { get; }   // Orb, Needle, Crescent or Ball
		protected abstract Color Color { get; }
		protected virtual int DustType => DustID.Blood;
		protected virtual int Size => 14;
		protected virtual float DrawScale => 1f;
		protected virtual int DebuffType => -1;
		protected virtual int DebuffTime => 180;
		protected virtual bool Spins => false;
		protected virtual bool Bounces => false;
		protected virtual int Life => 300;
		protected virtual int SplitInto => 0;       // crescents that burst into smaller shots when they expire

		public override string Texture => ShapePath + Shape;

		public override void SetDefaults() {
			Projectile.width = Size;
			Projectile.height = Size;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.penetrate = Bounces ? 4 : 1;
			Projectile.timeLeft = Life;
			Projectile.aiStyle = -1;
			CooldownSlot = ImmunityCooldownID.Bosses;
		}

		public override void AI() {
			if (Projectile.ai[0] == 1f) {
				Projectile.velocity.Y = System.Math.Min(Projectile.velocity.Y + 0.2f, 14f);
			}
			Projectile.rotation = Spins ? Projectile.rotation + 0.35f : Projectile.velocity.ToRotation();
			Lighting.AddLight(Projectile.Center, Color.ToVector3() * 0.5f);
			if (Main.rand.NextBool(3)) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, 0f, 100);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity) {
			if (!Bounces) {
				return true;
			}
			if (Projectile.velocity.X != oldVelocity.X) {
				Projectile.velocity.X = -oldVelocity.X * 0.9f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y) {
				Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
			}
			Projectile.penetrate--;
			return Projectile.penetrate <= 0;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info) {
			if (DebuffType >= 0) {
				target.AddBuff(DebuffType, DebuffTime);
			}
		}

		public override void OnKill(int timeLeft) {
			for (int i = 0; i < 6; i++) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType);
			}
			if (SplitInto > 0 && Main.netMode != NetmodeID.MultiplayerClient) {
				for (int i = 0; i < SplitInto; i++) {
					Vector2 velocity = (MathHelper.TwoPi * i / SplitInto + Projectile.rotation).ToRotationVector2() * 6f;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<MoonShard>(),
						Projectile.damage * 2 / 3, 0f, Main.myPlayer);
				}
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Vector2 position = Projectile.Center - Main.screenPosition;
			Vector2 origin = texture.Size() / 2f;
			Main.EntitySpriteDraw(texture, position, null, Color * Projectile.Opacity, Projectile.rotation, origin, DrawScale, SpriteEffects.None);
			Main.EntitySpriteDraw(texture, position, null, Color.White with { A = 0 } * 0.4f * Projectile.Opacity, Projectile.rotation, origin, DrawScale * 0.6f, SpriteEffects.None);
			return false;
		}
	}

	public class HandShot : DemonShot
	{
		protected override string Shape => "Orb";
		protected override Color Color => new(120, 170, 90);
		protected override int DustType => DustID.GreenBlood;
	}

	public class ThreadShot : DemonShot
	{
		protected override string Shape => "Needle";
		protected override Color Color => new(235, 235, 245);
		protected override int DustType => DustID.Silver;
		protected override int DebuffType => BuffID.Slow;
		protected override int DebuffTime => 90;
	}

	public class DreamShot : DemonShot
	{
		protected override string Shape => "Orb";
		protected override Color Color => new(230, 120, 220);
		protected override int DustType => DustID.PinkTorch;
		protected override int DebuffType => BuffID.Confused;
		protected override int DebuffTime => 90;
	}

	public class TemariBall : DemonShot
	{
		protected override string Shape => "Ball";
		protected override Color Color => new(250, 150, 60);
		protected override int DustType => DustID.OrangeTorch;
		protected override int Size => 20;
		protected override bool Spins => true;
		protected override bool Bounces => true;
	}

	public class DemonArrow : DemonShot
	{
		protected override string Shape => "Needle";
		protected override Color Color => new(230, 60, 60);
		protected override int DustType => DustID.RedTorch;
	}

	public class WebShot : DemonShot
	{
		protected override string Shape => "Orb";
		protected override Color Color => new(240, 240, 240);
		protected override int DustType => DustID.Web;
		protected override int DebuffType => BuffID.Webbed;
		protected override int DebuffTime => 40;
	}

	public class BloodShot : DemonShot
	{
		protected override string Shape => "Orb";
		protected override Color Color => new(200, 20, 40);
		protected override int DustType => DustID.Blood;
		protected override int DebuffType => BuffID.Bleeding;
	}

	public class BloodSickle : DemonShot
	{
		protected override string Shape => "Crescent";
		protected override Color Color => new(210, 30, 50);
		protected override int DustType => DustID.Blood;
		protected override int Size => 22;
		protected override bool Spins => true;
		protected override int DebuffType => BuffID.Poisoned;
	}

	public class ObiSash : DemonShot
	{
		protected override string Shape => "Needle";
		protected override Color Color => new(250, 110, 170);
		protected override int DustType => DustID.PinkTorch;
		protected override float DrawScale => 1.4f;
	}

	public class VaseWater : DemonShot
	{
		protected override string Shape => "Orb";
		protected override Color Color => new(80, 160, 255);
		protected override int DustType => DustID.Water;
		protected override int DebuffType => BuffID.Wet;
	}

	public class LeafWind : DemonShot
	{
		protected override string Shape => "Crescent";
		protected override Color Color => new(120, 230, 140);
		protected override int DustType => DustID.GreenTorch;
		protected override bool Spins => true;
	}

	public class ShockwaveShot : DemonShot
	{
		protected override string Shape => "Orb";
		protected override Color Color => new(90, 220, 255);
		protected override int DustType => DustID.BlueTorch;
		protected override int Size => 18;
		protected override float DrawScale => 1.3f;
		protected override bool Bounces => false;
	}

	public class IceShard : DemonShot
	{
		protected override string Shape => "Needle";
		protected override Color Color => new(170, 225, 255);
		protected override int DustType => DustID.IceTorch;
		protected override int DebuffType => BuffID.Frostburn;
	}

	public class MoonBlade : DemonShot
	{
		protected override string Shape => "Crescent";
		protected override Color Color => new(190, 90, 255);
		protected override int DustType => DustID.Shadowflame;
		protected override int Size => 24;
		protected override float DrawScale => 1.3f;
	}

	// A huge slow crescent that bursts into moon shards.
	public class MoonCrescent : DemonShot
	{
		protected override string Shape => "Crescent";
		protected override Color Color => new(210, 120, 255);
		protected override int DustType => DustID.Shadowflame;
		protected override int Size => 48;
		protected override float DrawScale => 2.6f;
		protected override int Life => 70;
		protected override int SplitInto => 8;

		public override void SetDefaults() {
			base.SetDefaults();
			Projectile.tileCollide = false;
		}
	}

	public class MoonShard : DemonShot
	{
		protected override string Shape => "Crescent";
		protected override Color Color => new(230, 170, 255);
		protected override int DustType => DustID.Shadowflame;
		protected override float DrawScale => 0.7f;
		protected override int Life => 120;
	}

	public class FleshWhip : DemonShot
	{
		protected override string Shape => "Needle";
		protected override Color Color => new(120, 10, 30);
		protected override int DustType => DustID.Blood;
		protected override int Size => 18;
		protected override float DrawScale => 1.6f;
		protected override int DebuffType => BuffID.Bleeding;
	}

	public class ThunderShot : DemonShot
	{
		protected override string Shape => "Needle";
		protected override Color Color => new(255, 200, 40);
		protected override int DustType => DustID.Electric;
		protected override int DebuffType => BuffID.Electrified;
	}

	// A warning circle that explodes after a moment. ai[1]: radius. Harmless until it goes off.
	public class DemonBomb : ModProjectile
	{
		public override string Texture => DemonShot.ShapePath + "Ring";

		private const int FuseTicks = 55;
		private float Radius => Projectile.ai[1] <= 0f ? 70f : Projectile.ai[1];
		private bool Armed => Projectile.timeLeft <= 8;
		private Color Tint => Projectile.ai[0] switch {
			1f => new Color(150, 220, 255), // ice
			2f => new Color(190, 90, 255),  // moon
			3f => new Color(230, 120, 220), // dream
			_ => new Color(220, 30, 50)     // blood
		};

		public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = FuseTicks + 8;
			Projectile.aiStyle = -1;
			CooldownSlot = ImmunityCooldownID.Bosses;
		}

		public override void AI() {
			if (Projectile.timeLeft == 8) {
				SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
				for (int i = 0; i < 30; i++) {
					Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.WhiteTorch, Main.rand.NextVector2Circular(1f, 1f) * Radius / 8f, 100, Tint, 1.8f);
					dust.noGravity = true;
				}
			}
		}

		public override bool CanHitPlayer(Player target) => Armed;

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Vector2 closest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
			return Vector2.Distance(closest, Projectile.Center) <= Radius;
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			float fuse = 1f - (Projectile.timeLeft - 8) / (float)FuseTicks;
			float scale = Radius * 2f / texture.Width;
			Color color = Tint with { A = 0 } * (Armed ? 1f : 0.3f + 0.4f * fuse);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color, 0f, texture.Size() / 2f, scale * (Armed ? 1f : fuse), SpriteEffects.None);
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, color * 0.5f, 0f, texture.Size() / 2f, scale, SpriteEffects.None);
			return false;
		}
	}

	// A spike that bursts out of the ground after a warning. Spawn it with its bottom on the ground.
	// ai[0]: tint (0 blood, 1 thread/white, 2 wood, 3 flesh).
	public class DemonSpike : ModProjectile
	{
		public override string Texture => DemonShot.ShapePath + "Spike";

		private const int WarnTicks = 40;
		private const int UpTicks = 30;
		private int Timer => WarnTicks + UpTicks - Projectile.timeLeft;
		private Color Tint => Projectile.ai[0] switch {
			1f => new Color(235, 235, 245),
			2f => new Color(150, 110, 60),
			3f => new Color(110, 10, 30),
			_ => new Color(210, 30, 50)
		};

		public override void SetDefaults() {
			Projectile.width = 26;
			Projectile.height = 90;
			Projectile.hostile = true;
			Projectile.friendly = false;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = WarnTicks + UpTicks;
			Projectile.aiStyle = -1;
			CooldownSlot = ImmunityCooldownID.Bosses;
		}

		public override void AI() {
			if (Timer < WarnTicks) {
				if (Main.rand.NextBool(2)) {
					Dust dust = Dust.NewDustDirect(Projectile.BottomLeft - new Vector2(0f, 6f), Projectile.width, 6, DustID.WhiteTorch, 0f, -2f, 100, Tint);
					dust.noGravity = true;
				}
			}
			else if (Timer == WarnTicks) {
				SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
			}
		}

		public override bool CanHitPlayer(Player target) => Timer >= WarnTicks;

		public override bool PreDraw(ref Color lightColor) {
			if (Timer < WarnTicks) {
				return false;
			}
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			float rise = System.Math.Min(1f, (Timer - WarnTicks) / 6f);
			var source = new Rectangle(0, 0, texture.Width, (int)(texture.Height * rise));
			Vector2 position = Projectile.Bottom - Main.screenPosition;
			Main.EntitySpriteDraw(texture, position, source, Tint.MultiplyRGB(lightColor), 0f, new Vector2(texture.Width / 2f, source.Height), 1f, SpriteEffects.None);
			return false;
		}
	}
}
