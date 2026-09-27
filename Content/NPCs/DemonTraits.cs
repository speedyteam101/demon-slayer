using DemonSlayerMod.Common.Config;
using DemonSlayerMod.Content.Items;
using DemonSlayerMod.Content.Projectiles.Breath;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace DemonSlayerMod.Content.NPCs
{
	// Marks an NPC as a demon: it burns in sunlight, regenerates when left alone, and resists anything that isn't
	// Nichirin steel or a breathing technique. Enemy and boss bases call the helpers below.
	public interface IDemon
	{
		int SinceHit { get; set; }
	}

	public static class DemonTraits
	{
		public const float OtherWeaponMultiplier = 0.6f;
		private const int RegenDelay = 180;

		public static bool IsDemon(NPC npc) => npc.ModNPC is IDemon;

		public static bool IsNichirin(Item item) => item?.ModItem is NichirinBlade;

		public static bool IsNichirin(Projectile projectile) => projectile.ModProjectile is BreathProjectile;

		// Out in the open during the day, with open sky above.
		public static bool InSunlight(NPC npc) {
			if (!Main.dayTime || npc.Center.Y > Main.worldSurface * 16.0) {
				return false;
			}
			Vector2 sky = npc.Center - new Vector2(0f, 480f);
			return Collision.CanHitLine(npc.Center, 1, 1, sky, 1, 1);
		}

		// Call every tick from AI (runs everywhere; healing only on the server).
		public static void Update(NPC npc, IDemon demon, float regenPerSecond) {
			demon.SinceHit++;
			if (!DemonSlayerConfig.Instance.DemonsRegenerate || Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}
			if (demon.SinceHit > RegenDelay && npc.life < npc.lifeMax && demon.SinceHit % 30 == 0) {
				int heal = Math.Max(1, (int)(npc.lifeMax * regenPerSecond / 2f));
				npc.life = Math.Min(npc.lifeMax, npc.life + heal);
				npc.HealEffect(heal, true);
				npc.netUpdate = true;
			}
		}

		// Call from UpdateLifeRegen: sunlight burns.
		public static void Sunburn(NPC npc, ref int damage) {
			if (!DemonSlayerConfig.Instance.DemonsBurnInSunlight || !InSunlight(npc)) {
				return;
			}
			if (npc.lifeRegen > 0) {
				npc.lifeRegen = 0;
			}
			int burn = Math.Max(20, npc.lifeMax / 5);
			npc.lifeRegen -= burn;
			damage = Math.Max(damage, burn / 8);
			if (Main.rand.NextBool(2)) {
				Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Torch, 0f, -2f, 100, default, 1.8f);
				dust.noGravity = true;
			}
		}

		public static void ModifyHitByItem(Item item, ref NPC.HitModifiers modifiers) {
			if (DemonSlayerConfig.Instance.DemonsResistOtherWeapons && !IsNichirin(item)) {
				modifiers.FinalDamage *= OtherWeaponMultiplier;
			}
		}

		public static void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) {
			if (DemonSlayerConfig.Instance.DemonsResistOtherWeapons && !IsNichirin(projectile)) {
				modifiers.FinalDamage *= OtherWeaponMultiplier;
			}
		}
	}
}
