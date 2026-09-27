using DemonSlayerMod.Content.Projectiles.Demon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Projectiles
{
	// Blood Demon Art: a lash of hardened blood fired while in demon form. Demons resist it like any non-Nichirin attack.
	public class BloodArtShot : ModProjectile
	{
		public override string Texture => DemonShot.ShapePath + "Needle";

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = 4;
			Projectile.timeLeft = 40;
			Projectile.tileCollide = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.aiStyle = -1;
		}

		public override void AI() {
			Projectile.rotation = Projectile.velocity.ToRotation();
			Lighting.AddLight(Projectile.Center, 0.6f, 0.05f, 0.1f);
			if (Main.rand.NextBool(2)) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 0, default, 1.3f);
				dust.noGravity = true;
				dust.velocity *= 0.3f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(BuffID.Bleeding, 180);
			Player owner = Main.player[Projectile.owner];
			if (Main.rand.NextBool(3)) {
				owner.Heal(2); // demons feed
			}
		}

		public override bool PreDraw(ref Color lightColor) {
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(190, 15, 35), Projectile.rotation, texture.Size() / 2f, 1.6f, SpriteEffects.None);
			return false;
		}
	}
}
