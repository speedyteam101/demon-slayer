using DemonSlayerMod.Common.Players;
using DemonSlayerMod.Content.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Items
{
	// Dropped by every demon. Used to craft boss summons.
	public class DemonBlood : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 25;
		}

		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = Item.sellPrice(silver: 5);
			Item.rare = ItemRarityID.Blue;
		}
	}

	// Grants one extra stat point, up to SlayerPlayer.MaxScrolls in total. Dropped by demon bosses.
	public class TrainingScroll : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 5;
		}

		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.consumable = true;
			Item.UseSound = SoundID.Item4;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ItemRarityID.Orange;
		}

		public override bool CanUseItem(Player player) => SlayerPlayer.Get(player).ScrollsUsed < SlayerPlayer.MaxScrolls;

		public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				SlayerPlayer.Get(player).ScrollsUsed++;
				CombatText.NewText(player.Hitbox, Microsoft.Xna.Framework.Color.Gold, "+1 stat point");
			}
			return true;
		}
	}

	// Refunds every stat point so you can spend them again.
	public class ScrollOfForgetting : ModItem
	{
		public override void SetDefaults() {
			Item.width = 24;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.consumable = true;
			Item.UseSound = SoundID.Item29;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.Green;
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				SlayerPlayer.Get(player).ResetPoints();
				CombatText.NewText(player.Hitbox, Microsoft.Xna.Framework.Color.LightBlue, "Stat points refunded");
			}
			return true;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient<DemonBlood>(5)
				.AddIngredient(ItemID.Book)
				.AddTile(TileID.Bookcases)
				.Register();
		}
	}

	// Places the Swordsmith's Forge, where Nichirin Blades are made and customised.
	public class SwordsmithForge : ModItem
	{
		public override void SetDefaults() {
			Item.DefaultToPlaceableTile(ModContent.TileType<SwordsmithForgeTile>());
			Item.width = 32;
			Item.height = 24;
			Item.value = Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Blue;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddIngredient(ItemID.StoneBlock, 30)
				.AddRecipeGroup(RecipeGroupID.IronBar, 8)
				.AddIngredient(ItemID.Torch, 3)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}
