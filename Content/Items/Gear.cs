using DemonSlayerMod.Common.Players;
using DemonSlayerMod.Content.Buffs;
using DemonSlayerMod.Content.Pets;
using DemonSlayerMod.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Items
{
	// Demon Slayer Corps gear: accessories, a pet and consumables.

	public abstract class SlayerAccessory : ModItem
	{
		protected abstract int Rarity { get; }

		public override void SetDefaults() {
			Item.width = 28;
			Item.height = 28;
			Item.accessory = true;
			Item.rare = Rarity;
			Item.value = Item.sellPrice(gold: 1);
		}
	}

	// The black gakuran every Slayer wears. It's tougher than it looks.
	public class CorpsUniform : SlayerAccessory
	{
		protected override int Rarity => ItemRarityID.Blue;

		public override void SetDefaults() {
			base.SetDefaults();
			Item.defense = 6;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			SlayerPlayer.Get(player).BonusMaxBreath += 20;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.Silk, 12).AddIngredient<DemonBlood>(5).AddTile(TileID.Loom).Register();
		}
	}

	// A haori of two halves, calm as still water.
	public class WaterHaori : SlayerAccessory
	{
		protected override int Rarity => ItemRarityID.Green;

		public override void UpdateAccessory(Player player, bool hideVisual) {
			SlayerPlayer.Get(player).BonusBreathRegen += 0.25f;
			player.GetAttackSpeed(DamageClass.Melee) += 0.06f;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.Silk, 10).AddIngredient<DemonBlood>(10).AddIngredient(ItemID.WaterCandle).AddTile(TileID.Loom).Register();
		}
	}

	// A white haori with flames licking at the hem.
	public class FlameHaori : SlayerAccessory
	{
		protected override int Rarity => ItemRarityID.Orange;

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetDamage(DamageClass.Melee) += 0.08f;
			player.buffImmune[BuffID.OnFire] = true;
			player.buffImmune[BuffID.Burning] = true;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.Silk, 10).AddIngredient<DemonBlood>(10).AddIngredient(ItemID.HellstoneBar, 5).AddTile(TileID.Loom).Register();
		}
	}

	// A haori patterned like butterfly wings.
	public class ButterflyHaori : SlayerAccessory
	{
		protected override int Rarity => ItemRarityID.Green;

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.moveSpeed += 0.1f;
			player.GetCritChance(DamageClass.Melee) += 8f;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.Silk, 10).AddIngredient<DemonBlood>(10).AddIngredient(ItemID.JungleSpores, 8).AddTile(TileID.Loom).Register();
		}
	}

	// A green and black checkered haori.
	public class CheckeredHaori : SlayerAccessory
	{
		protected override int Rarity => ItemRarityID.Green;

		public override void UpdateAccessory(Player player, bool hideVisual) {
			SlayerPlayer.Get(player).BonusTechniqueDamage += 0.1f;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.Silk, 10).AddIngredient<DemonBlood>(10).AddIngredient(ItemID.Emerald, 3).AddTile(TileID.Loom).Register();
		}
	}

	// Hanafuda earrings, passed down with the Dance of the Fire God.
	public class HanafudaEarrings : SlayerAccessory
	{
		protected override int Rarity => ItemRarityID.LightRed;

		public override void UpdateAccessory(Player player, bool hideVisual) {
			SlayerPlayer slayer = SlayerPlayer.Get(player);
			slayer.BonusBreathRegen += 0.25f;
			slayer.BonusSunTechniqueDamage += 0.2f;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(20).AddIngredient(ItemID.SoulofLight, 5).AddIngredient(ItemID.Sunflower).AddTile<SwordsmithForgeTile>().Register();
		}
	}

	// Demons hate wisteria. They deal 20% less damage to you.
	public class WisteriaCharm : SlayerAccessory
	{
		protected override int Rarity => ItemRarityID.Green;

		public override void UpdateAccessory(Player player, bool hideVisual) {
			SlayerPlayer.Get(player).DemonDamageTaken *= 0.8f;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(8).AddIngredient(ItemID.Daybloom, 3).AddIngredient(ItemID.Amethyst, 3).AddTile(TileID.Anvils).Register();
		}
	}

	// Wisteria poison to coat your blade.
	public class WisteriaPoison : ModItem
	{
		public override void SetDefaults() {
			Item.width = 16;
			Item.height = 24;
			Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.useAnimation = 17;
			Item.useTime = 17;
			Item.useTurn = true;
			Item.UseSound = SoundID.Item3;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.sellPrice(silver: 20);
			Item.buffType = ModContent.BuffType<WisteriaPoisonBuff>();
			Item.buffTime = 10 * 60 * 60;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient<DemonBlood>(2).AddIngredient(ItemID.VileMushroom).AddTile(TileID.Bottles).Register();
			CreateRecipe().AddIngredient(ItemID.BottledWater).AddIngredient<DemonBlood>(2).AddIngredient(ItemID.ViciousMushroom).AddTile(TileID.Bottles).Register();
		}
	}

	// Medicine from the Butterfly Mansion.
	public class ButterflyMedicine : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToHealingPotion(20, 26, 120);
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(silver: 5);
		}

		public override void AddRecipes() {
			CreateRecipe(2).AddIngredient(ItemID.LesserHealingPotion, 2).AddIngredient<DemonBlood>(1).AddIngredient(ItemID.Daybloom).AddTile(TileID.Bottles).Register();
		}
	}

	// Summons a Kasugai Crow, the Corps' messenger.
	public class CrowWhistle : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToVanitypet(ModContent.ProjectileType<KasugaiCrow>(), ModContent.BuffType<KasugaiCrowBuff>());
			Item.width = 22;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.sellPrice(silver: 50);
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				player.AddBuff(Item.buffType, 3600);
			}
			return true;
		}

		public override void AddRecipes() {
			CreateRecipe().AddIngredient(ItemID.Feather, 5).AddIngredient<DemonBlood>(3).AddTile<SwordsmithForgeTile>().Register();
		}
	}
}
