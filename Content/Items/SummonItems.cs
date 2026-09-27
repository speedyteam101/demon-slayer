using DemonSlayerMod.Content.NPCs.Bosses;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace DemonSlayerMod.Content.Items
{
	// Shared boss-summoning behaviour. Demons only answer at night.
	public abstract class DemonSummonItem : ModItem
	{
		protected abstract int BossType { get; }
		protected virtual bool Unlocked => true;
		protected abstract int Rarity { get; }

		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 3;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
		}

		public override void SetDefaults() {
			Item.width = 28;
			Item.height = 28;
			Item.maxStack = 20;
			Item.useAnimation = 30;
			Item.useTime = 30;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.consumable = true;
			Item.rare = Rarity;
			Item.value = Item.sellPrice(gold: 1);
		}

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup) {
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossSpawners;
		}

		public override bool CanUseItem(Player player) {
			return !Main.dayTime && Unlocked && !NPC.AnyNPCs(BossType);
		}

		public override bool? UseItem(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				SoundEngine.PlaySound(SoundID.Roar, player.position);
				if (Main.netMode != NetmodeID.MultiplayerClient) {
					NPC.SpawnOnPlayer(player.whoAmI, BossType);
				}
				else {
					NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: BossType);
				}
			}
			return true;
		}
	}

	public class SelectionTag : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<HandDemon>();
		protected override int Rarity => ItemRarityID.Blue;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(5).AddIngredient(ItemID.Wood, 10).AddTile(TileID.WorkBenches).Register();
		}
	}

	public class SpiderSilkDoll : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Rui>();
		protected override bool Unlocked => NPC.downedBoss1;
		protected override int Rarity => ItemRarityID.Green;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(10).AddIngredient(ItemID.Cobweb, 20).AddTile(TileID.Anvils).Register();
		}
	}

	public class TrainTicket : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Enmu>();
		protected override bool Unlocked => NPC.downedBoss3;
		protected override int Rarity => ItemRarityID.Orange;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(15).AddIngredient(ItemID.Silk, 5).AddTile(TileID.Anvils).Register();
		}
	}

	public class DistrictObi : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<GyutaroDaki>();
		protected override bool Unlocked => Main.hardMode;
		protected override int Rarity => ItemRarityID.LightRed;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(20).AddIngredient(ItemID.Silk, 10).AddIngredient(ItemID.SoulofNight, 5).AddTile(TileID.MythrilAnvil).Register();
		}
	}

	public class CrackedVase : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Gyokko>();
		protected override bool Unlocked => NPC.downedMechBossAny;
		protected override int Rarity => ItemRarityID.Pink;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(25).AddIngredient(ItemID.ClayBlock, 20).AddIngredient(ItemID.HallowedBar, 5).AddTile(TileID.MythrilAnvil).Register();
		}
	}

	public class LeafFan : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Hantengu>();
		protected override bool Unlocked => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
		protected override int Rarity => ItemRarityID.Pink;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(25).AddIngredient(ItemID.SoulofMight, 5).AddIngredient(ItemID.SoulofSight, 5).AddIngredient(ItemID.SoulofFright, 5)
				.AddTile(TileID.MythrilAnvil).Register();
		}
	}

	public class MartialBeads : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Akaza>();
		protected override bool Unlocked => NPC.downedPlantBoss;
		protected override int Rarity => ItemRarityID.Lime;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(30).AddIngredient(ItemID.ChlorophyteBar, 10).AddTile(TileID.MythrilAnvil).Register();
		}
	}

	public class GoldenFan : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Doma>();
		protected override bool Unlocked => NPC.downedGolemBoss;
		protected override int Rarity => ItemRarityID.Yellow;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(30).AddIngredient(ItemID.BeetleHusk, 5).AddIngredient(ItemID.IceBlock, 20).AddTile(TileID.MythrilAnvil).Register();
		}
	}

	public class BrokenFlute : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Kokushibo>();
		protected override bool Unlocked => NPC.downedAncientCultist;
		protected override int Rarity => ItemRarityID.Cyan;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(35).AddIngredient(ItemID.FragmentSolar, 3).AddIngredient(ItemID.FragmentVortex, 3)
				.AddIngredient(ItemID.FragmentNebula, 3).AddIngredient(ItemID.FragmentStardust, 3).AddTile(TileID.LunarCraftingStation).Register();
		}
	}

	public class BlueSpiderLily : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Muzan>();
		protected override bool Unlocked => NPC.downedMoonlord;
		protected override int Rarity => ItemRarityID.Red;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(50).AddIngredient(ItemID.LunarBar, 10).AddTile(TileID.LunarCraftingStation).Register();
		}
	}

	public class WoodenCharm : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Mukago>();
		protected override bool Unlocked => NPC.downedBoss1;
		protected override int Rarity => ItemRarityID.Blue;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(8).AddIngredient(ItemID.Lens, 2).AddTile(TileID.Anvils).Register();
		}
	}

	public class TsuzumiDrum : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Kyogai>();
		protected override bool Unlocked => NPC.downedBoss2;
		protected override int Rarity => ItemRarityID.Green;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(12).AddIngredient(ItemID.Leather, 5).AddTile(TileID.Anvils).Register();
		}
	}

	public class CrackedMagatama : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Kaigaku>();
		protected override bool Unlocked => NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
		protected override int Rarity => ItemRarityID.LightPurple;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(25).AddIngredient(ItemID.SoulofLight, 8).AddIngredient(ItemID.SoulofNight, 8).AddTile(TileID.MythrilAnvil).Register();
		}
	}

	public class BiwaString : DemonSummonItem
	{
		protected override int BossType => ModContent.NPCType<Nakime>();
		protected override bool Unlocked => NPC.downedGolemBoss;
		protected override int Rarity => ItemRarityID.Yellow;

		public override void AddRecipes() {
			CreateRecipe().AddIngredient<DemonBlood>(30).AddIngredient(ItemID.Silk, 10).AddIngredient(ItemID.BeetleHusk, 5).AddTile(TileID.MythrilAnvil).Register();
		}
	}
}
