using Terraria;
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
}
