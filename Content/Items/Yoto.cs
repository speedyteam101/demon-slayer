using DemonSlayerMod.Common.Swords;
using Terraria;
using Terraria.ID;

namespace DemonSlayerMod.Content.Items
{
	// Yōtō: a demon-forged katana, a rare boss drop. It works exactly like a Nichirin Blade (fully customisable at the
	// Swordsmith's Forge, breathing techniques scale with it, demons take full damage from it), but its base damage is a
	// fixed 2000 whatever steel it's set to. The blade shape still multiplies it.
	public class Yoto : NichirinBlade
	{
		public override int? FixedBaseDamage => 2000;
		protected override int FixedRarity => ItemRarityID.Purple;

		public Yoto() {
			// Its default look: a black blade with crimson wrap, crescent guard and moonlight trail.
			Build = new SwordBuild { Shape = 0, Colour = 19, Guard = 9, GuardFinish = 5, Wrap = 3, Engraving = 1, Trail = 13 };
		}

		public override void SetDefaults() {
			base.SetDefaults();
			Item.rare = ItemRarityID.Purple;
			Item.value = Item.sellPrice(platinum: 1);
		}

		public override void AddRecipes() {
			// Boss drop only.
		}
	}
}
