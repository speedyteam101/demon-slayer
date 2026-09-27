using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Buffs
{
	// Water's Dead Calm and Stone's Stone Skin: a wall of defense.
	public class DeadCalmBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex) {
			player.statDefense += 25;
			player.endurance += 0.15f;
			player.noKnockback = true;
		}
	}

	// Flower's Equinoctial Vermilion Eye and Beast's Spatial Awareness: see everything, strike where it hurts.
	public class SensesBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex) {
			player.GetCritChance(DamageClass.Generic) += 20f;
			player.detectCreature = true;
			player.dangerSense = true;
			player.nightVision = true;
		}
	}

	// Mist's Obscuring Clouds and Sun's Fake Rainbow: 25% chance to dodge (see SlayerPlayer.FreeDodge).
	public class AfterimageBuff : ModBuff
	{
		public override void Update(Player player, ref int buffIndex) {
			player.moveSpeed += 0.2f;
		}
	}

	// The Demon Slayer Mark, for Hashira only.
	public class DemonSlayerMarkBuff : ModBuff
	{
		public const int Duration = 20 * 60;
		public const int Cooldown = 120 * 60;

		public override void Update(Player player, ref int buffIndex) {
			player.GetDamage(DamageClass.Generic) += 0.25f;
			player.GetAttackSpeed(DamageClass.Melee) += 0.2f;
			player.moveSpeed += 0.25f;
			player.endurance += 0.1f;
			player.lifeRegen += 10;
			if (Main.rand.NextBool(3)) {
				Dust dust = Dust.NewDustDirect(player.position, player.width, 12, Terraria.ID.DustID.RedTorch, 0f, -1f, 100, default, 1.2f);
				dust.noGravity = true;
			}
		}
	}

	// Wisteria poison on your blade: hits inflict Venom and deal +10% damage to demons.
	public class WisteriaPoisonBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.meleeBuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			Common.Players.SlayerPlayer.Get(player).WisteriaPoison = true;
		}
	}

	// Demon form, granted by Muzan's blood. Night only: it ends by itself at dawn.
	// Bonuses grow with your Slayer rank.
	public class DemonFormBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoTimeDisplay[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			if (Main.dayTime) {
				player.DelBuff(buffIndex);
				buffIndex--;
				if (player.whoAmI == Main.myPlayer) {
					Main.NewText("The sun rises. Your demon form fades.", 220, 40, 60);
				}
				return;
			}
			player.buffTime[buffIndex] = 2;

			int rank = Common.Players.SlayerPlayer.Get(player).Rank;
			player.GetDamage(DamageClass.Generic) += 0.2f + rank * 0.02f;
			player.GetAttackSpeed(DamageClass.Melee) += 0.15f;
			player.moveSpeed += 0.25f;
			player.statDefense += 10 + rank;
			player.lifeRegen += 12 + rank;
			player.jumpSpeedBoost += 2.5f;
			player.noFallDmg = true;
			player.nightVision = true;
			player.buffImmune[BuffID.Poisoned] = true;
			player.buffImmune[BuffID.Bleeding] = true;

			if (Main.rand.NextBool(4)) {
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Blood, 0f, -1f, 0, default, 1.1f);
				dust.noGravity = true;
			}
			Lighting.AddLight(player.Center, 0.4f, 0.02f, 0.05f);
		}
	}
}
