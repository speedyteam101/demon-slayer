using DemonSlayerMod.Common.Breathing;
using DemonSlayerMod.Common.Players;
using DemonSlayerMod.Common.Swords;
using DemonSlayerMod.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DemonSlayerMod.Content.Items
{
	// The one sword of the mod. Everything about it (steel, shape, colour, guard...) is chosen at the Swordsmith's Forge
	// and saved on the item. Item.damage is a fixed 100 that is scaled to the real value in ModifyWeaponDamage, so
	// damage prefixes still work as a percentage.
	public class NichirinBlade : ModItem
	{
		public const int ReferenceDamage = 100;
		public const int ReferenceUseTime = 20;

		public SwordBuild Build = new();

		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults() {
			Item.width = 64;
			Item.height = 64;
			Item.damage = ReferenceDamage;
			Item.DamageType = DamageClass.Melee;
			Item.useTime = ReferenceUseTime;
			Item.useAnimation = ReferenceUseTime;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5f;
			Item.autoReuse = true;
			Item.UseSound = SoundID.Item1;
			Item.noUseGraphic = true; // drawn by NichirinBladeDrawLayer from its parts
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.White;
		}

		// The steel actually used for damage: one the holder has earned, never more.
		public int EffectiveSteel(Player player) {
			return SlayerPlayer.Get(player).HighestUsableSteel(Build.Steel);
		}

		public override void ModifyWeaponDamage(Player player, ref StatModifier damage) {
			Steel steel = SwordParts.Steels[EffectiveSteel(player)];
			damage *= steel.Damage * SwordParts.Shapes[Build.Shape].DamageMult / ReferenceDamage;
		}

		public override void ModifyWeaponCrit(Player player, ref float crit) {
			crit += SwordStats.From(Build).Crit;
		}

		public override void ModifyWeaponKnockback(Player player, ref StatModifier knockback) {
			knockback *= SwordParts.Shapes[Build.Shape].Knockback / 5f;
		}

		public override float UseSpeedMultiplier(Player player) {
			return (float)ReferenceUseTime / SwordParts.Shapes[Build.Shape].UseTime;
		}

		public override void ModifyItemScale(Player player, ref float scale) {
			scale *= SwordParts.Shapes[Build.Shape].Scale;
		}

		public override void UpdateInventory(Player player) {
			Item.rare = SwordParts.Steels[Build.Steel].Rarity;
		}

		public override void MeleeEffects(Player player, Rectangle hitbox) {
			Trail trail = SwordParts.Trails[Build.Trail];
			if (trail.Dust < 0 || !Main.rand.NextBool(2)) {
				return;
			}
			Dust dust = Dust.NewDustDirect(hitbox.TopLeft(), hitbox.Width, hitbox.Height, trail.Dust, 0f, 0f, 100, default, 1.3f);
			dust.noGravity = true;
			dust.velocity *= 0.4f;
			if (trail.UseBladeColour) {
				dust.color = SwordParts.Colours[Build.Colour].Color;
			}
			else if (trail.UseStyleColour) {
				dust.color = BreathingStyles.ColorOf(SlayerPlayer.Get(player).Style);
			}
			else if (trail.Rainbow) {
				dust.color = Main.hslToRgb(Main.GlobalTimeWrappedHourly * 0.5f % 1f, 1f, 0.6f);
			}
		}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			SlayerPlayer.Get(player).OnBladeHit(target);
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips) {
			Player player = Main.LocalPlayer;
			int effective = EffectiveSteel(player);
			var lines = new List<string> {
				$"{SwordParts.Steels[Build.Steel].Name} {SwordParts.Shapes[Build.Shape].Name} — {SwordParts.Colours[Build.Colour].Name} blade",
				$"{SwordParts.Guards[Build.Guard].Name} ({SwordParts.GuardFinishes[Build.GuardFinish].Name}), {SwordParts.Wraps[Build.Wrap].Name} wrap",
				$"Colour: {SwordParts.Colours[Build.Colour].Description}",
				$"Guard: {SwordParts.Guards[Build.Guard].Description}",
			};
			if (Build.Engraving != 0) {
				lines.Add($"Engraved \"{SwordParts.Engravings[Build.Engraving].Name}\": {SwordParts.Engravings[Build.Engraving].Description}");
			}
			if (effective != Build.Steel) {
				lines.Add($"[c/FF6060:You haven't earned {SwordParts.Steels[Build.Steel].Name} yet; it cuts like {SwordParts.Steels[effective].Name}]");
			}
			lines.Add("Customise it at a Swordsmith's Forge");
			for (int i = 0; i < lines.Count; i++) {
				tooltips.Add(new TooltipLine(Mod, "Build" + i, lines[i]));
			}
		}

		// Inventory and world drawing: the vanilla texture is only a fallback, the real look comes from the parts.
		public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
			SwordDrawer.DrawCentered(spriteBatch, Build, position, drawColor, scale);
			return false;
		}

		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
			Vector2 center = Item.Center - Main.screenPosition;
			SwordDrawer.DrawCentered(spriteBatch, Build, center, lightColor, scale, rotation);
			return false;
		}

		public override void SaveData(TagCompound tag) {
			Build.Save(tag);
		}

		public override void LoadData(TagCompound tag) {
			Build.Load(tag);
		}

		public override void NetSend(BinaryWriter writer) {
			Build.Write(writer);
		}

		public override void NetReceive(BinaryReader reader) {
			Build.Read(reader);
		}

		public override ModItem Clone(Item newEntity) {
			var clone = (NichirinBlade)base.Clone(newEntity);
			clone.Build = Build.Clone();
			return clone;
		}

		public override void AddRecipes() {
			CreateRecipe()
				.AddRecipeGroup(RecipeGroupID.IronBar, 12)
				.AddIngredient(ItemID.Wood, 5)
				.AddTile<SwordsmithForgeTile>()
				.Register();
		}
	}
}
