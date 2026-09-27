using Microsoft.Xna.Framework;
using Terraria.ID;

namespace DemonSlayerMod.Common.Swords
{
	// Everything the Swordsmith's Forge can change on a Nichirin Blade. Each table is indexed by the number saved on the item,
	// so only ever add new entries at the end of a table.

	public class BladeShape
	{
		public string Name, Description, Texture;
		public float DamageMult = 1f, Scale = 1f, Knockback = 5f;
		public int UseTime = 20, Crit, ArmorPen;
	}

	public class Steel
	{
		public string Name, Description;
		public int Damage, Rarity;
		public string RequiredBoss; // DownedDemonSystem key, or null if always available
	}

	public enum BonusKind
	{
		None,
		Crit,          // +Amount% melee crit
		MeleeSpeed,    // +Amount% melee speed
		MoveSpeed,     // +Amount% move speed
		Defense,       // +Amount defense
		Damage,        // +Amount% melee damage
		TechniqueDamage, // +Amount% breathing technique damage
		BreathRegen,   // +Amount% Breath regeneration
		MaxBreath,     // +Amount max Breath
		ArmorPen,      // +Amount armor penetration
		LifeSteal,     // heal Amount HP on hit (at most once every half second)
		DemonDamage,   // +Amount% damage against demons
		DayDamage,     // +Amount% damage during the day
		NightDamage,   // +Amount% damage at night
		Debuff         // inflicts the buff in Buff on hit
	}

	public class PartBonus
	{
		public BonusKind Kind;
		public int Amount;
		public int Buff = -1;
		public PartBonus(BonusKind kind, int amount = 0, int buff = -1) {
			Kind = kind;
			Amount = amount;
			Buff = buff;
		}
	}

	public class BladeColour
	{
		public string Name, Description;
		public Color Color;
		public PartBonus Bonus;
	}

	public class Guard
	{
		public string Name, Description, Texture;
		public PartBonus Bonus, Drawback;
	}

	public class Tint
	{
		public string Name;
		public Color Color;
	}

	public class Engraving
	{
		public string Name, Description;
		public PartBonus Bonus;
	}

	public class Trail
	{
		public string Name;
		public int Dust = -1;         // -1: no trail
		public bool UseBladeColour;   // tint dust with the blade colour
		public bool UseStyleColour;   // tint dust with the breathing style's colour
		public bool Rainbow;
	}

	public static class SwordParts
	{
		public static readonly BladeShape[] Shapes = {
			new() { Name = "Katana", Texture = "Katana", Description = "Balanced in every way", DamageMult = 1f, UseTime = 20, Scale = 1.1f, Crit = 4, Knockback = 5f },
			new() { Name = "Nodachi", Texture = "Nodachi", Description = "A long field sword: more reach and damage, slower", DamageMult = 1.15f, UseTime = 26, Scale = 1.45f, Crit = 4, Knockback = 6.5f },
			new() { Name = "Wakizashi", Texture = "Wakizashi", Description = "A short blade: very fast, less reach", DamageMult = 0.8f, UseTime = 13, Scale = 0.85f, Crit = 8, Knockback = 3.5f },
			new() { Name = "Cleaver", Texture = "Cleaver", Description = "A broad chopping blade: heavy hits, pierces armor", DamageMult = 1.3f, UseTime = 30, Scale = 1.25f, Crit = 0, Knockback = 9f, ArmorPen = 5 },
			new() { Name = "Stinger", Texture = "Stinger", Description = "A needle-thin blade: weak cuts, deadly crits and armor piercing", DamageMult = 0.7f, UseTime = 12, Scale = 1f, Crit = 20, Knockback = 2f, ArmorPen = 15 },
			new() { Name = "Ribbon Blade", Texture = "Ribbon", Description = "A thin, flexible blade with enormous reach", DamageMult = 0.85f, UseTime = 18, Scale = 1.7f, Crit = 6, Knockback = 3f },
			new() { Name = "Serpent Blade", Texture = "Serpent", Description = "A winding blade that slips past guards", DamageMult = 1f, UseTime = 18, Scale = 1.15f, Crit = 10, Knockback = 4.5f, ArmorPen = 8 },
			new() { Name = "Jagged Blade", Texture = "Jagged", Description = "A chipped, serrated blade that tears flesh", DamageMult = 1.1f, UseTime = 20, Scale = 1.1f, Crit = 6, Knockback = 6f, ArmorPen = 10 },
			new() { Name = "Greatblade", Texture = "Greatblade", Description = "A massive slab of steel: the slowest and strongest", DamageMult = 1.55f, UseTime = 36, Scale = 1.35f, Crit = 0, Knockback = 11f, ArmorPen = 10 },
		};

		// Damage is the blade's base damage before its shape and your bonuses.
		public static readonly Steel[] Steels = {
			new() { Name = "Tamahagane", Description = "Plain folded steel", Damage = 16, Rarity = ItemRarityID.White },
			new() { Name = "Scarlet Iron Sand", Description = "Nichirin steel from Mount Yoko", Damage = 24, Rarity = ItemRarityID.Blue, RequiredBoss = "HandDemon" },
			new() { Name = "Scarlet Ore", Description = "Steel that drinks in the sun", Damage = 33, Rarity = ItemRarityID.Green, RequiredBoss = "Rui" },
			new() { Name = "Mount Yoko Steel", Description = "The finest pre-Hardmode steel", Damage = 44, Rarity = ItemRarityID.Orange, RequiredBoss = "Enmu" },
			new() { Name = "Sunlight Steel", Description = "Folded under the midday sun", Damage = 64, Rarity = ItemRarityID.LightRed, RequiredBoss = "GyutaroDaki" },
			new() { Name = "Village Masterwork", Description = "Forged at the Swordsmith Village", Damage = 82, Rarity = ItemRarityID.Pink, RequiredBoss = "Gyokko" },
			new() { Name = "Haganezuka's Steel", Description = "Sharpened with obsessive care", Damage = 100, Rarity = ItemRarityID.LightPurple, RequiredBoss = "Hantengu" },
			new() { Name = "Crimson Red Steel", Description = "Glows red-hot when gripped hard", Damage = 124, Rarity = ItemRarityID.Lime, RequiredBoss = "Akaza" },
			new() { Name = "Frostproof Steel", Description = "Steel that ice cannot grip", Damage = 146, Rarity = ItemRarityID.Yellow, RequiredBoss = "Doma" },
			new() { Name = "Yoriichi's Steel", Description = "Steel of the first Sun Breathing swordsman", Damage = 176, Rarity = ItemRarityID.Cyan, RequiredBoss = "Kokushibo" },
			new() { Name = "Dawnbreaker Steel", Description = "Steel that holds the light of sunrise", Damage = 225, Rarity = ItemRarityID.Red, RequiredBoss = "Muzan" },
		};

		public static readonly BladeColour[] Colours = {
			new() { Name = "Black", Color = new Color(45, 45, 55), Description = "+8% crit", Bonus = new(BonusKind.Crit, 8) },
			new() { Name = "Blue", Color = new Color(60, 120, 235), Description = "Frostburns enemies", Bonus = new(BonusKind.Debuff, 0, BuffID.Frostburn) },
			new() { Name = "Red", Color = new Color(225, 45, 30), Description = "Sets enemies ablaze with Hellfire", Bonus = new(BonusKind.Debuff, 0, BuffID.OnFire3) },
			new() { Name = "Yellow", Color = new Color(245, 210, 40), Description = "+8% melee speed", Bonus = new(BonusKind.MeleeSpeed, 8) },
			new() { Name = "Green", Color = new Color(60, 195, 95), Description = "+8% movement speed", Bonus = new(BonusKind.MoveSpeed, 8) },
			new() { Name = "Grey", Color = new Color(140, 140, 150), Description = "+4 defense", Bonus = new(BonusKind.Defense, 4) },
			new() { Name = "Pink", Color = new Color(250, 130, 185), Description = "Heals 2 HP on hit", Bonus = new(BonusKind.LifeSteal, 2) },
			new() { Name = "Purple", Color = new Color(145, 70, 205), Description = "Inflicts Shadowflame", Bonus = new(BonusKind.Debuff, 0, BuffID.ShadowFlame) },
			new() { Name = "Indigo", Color = new Color(75, 60, 175), Description = "Inflicts Venom", Bonus = new(BonusKind.Debuff, 0, BuffID.Venom) },
			new() { Name = "Lavender", Color = new Color(195, 155, 235), Description = "+8 armor penetration", Bonus = new(BonusKind.ArmorPen, 8) },
			new() { Name = "White", Color = new Color(235, 240, 248), Description = "Inflicts Frostbite", Bonus = new(BonusKind.Debuff, 0, BuffID.Frostburn2) },
			new() { Name = "Orange", Color = new Color(245, 140, 40), Description = "Inflicts Ichor (lower defense)", Bonus = new(BonusKind.Debuff, 0, BuffID.Ichor) },
			new() { Name = "Gold", Color = new Color(235, 195, 60), Description = "Inflicts Midas (more coins)", Bonus = new(BonusKind.Debuff, 0, BuffID.Midas) },
			new() { Name = "Crimson", Color = new Color(155, 20, 45), Description = "+5% melee damage", Bonus = new(BonusKind.Damage, 5) },
		};

		public static readonly Guard[] Guards = {
			new() { Name = "Round Tsuba", Texture = "Round", Description = "+2 defense", Bonus = new(BonusKind.Defense, 2) },
			new() { Name = "Square Tsuba", Texture = "Square", Description = "+4 defense", Bonus = new(BonusKind.Defense, 4) },
			new() { Name = "Flame Tsuba", Texture = "Flame", Description = "+6% technique damage", Bonus = new(BonusKind.TechniqueDamage, 6) },
			new() { Name = "Hexagonal Tsuba", Texture = "Hexagon", Description = "+5% crit", Bonus = new(BonusKind.Crit, 5) },
			new() { Name = "Flower Tsuba", Texture = "Flower", Description = "+20% Breath regeneration", Bonus = new(BonusKind.BreathRegen, 20) },
			new() { Name = "Butterfly Tsuba", Texture = "Butterfly", Description = "+8% movement speed", Bonus = new(BonusKind.MoveSpeed, 8) },
			new() { Name = "Wheel Tsuba", Texture = "Wheel", Description = "+6% melee speed", Bonus = new(BonusKind.MeleeSpeed, 6) },
			new() { Name = "Clover Tsuba", Texture = "Clover", Description = "+5% melee damage", Bonus = new(BonusKind.Damage, 5) },
			new() { Name = "Serpent Tsuba", Texture = "Serpent", Description = "+6 armor penetration", Bonus = new(BonusKind.ArmorPen, 6) },
			new() { Name = "Crescent Tsuba", Texture = "Crescent", Description = "+15 max Breath", Bonus = new(BonusKind.MaxBreath, 15) },
			new() { Name = "No Guard", Texture = "None", Description = "+10% melee speed, -2 defense", Bonus = new(BonusKind.MeleeSpeed, 10), Drawback = new(BonusKind.Defense, -2) },
		};

		public static readonly Tint[] GuardFinishes = {
			new() { Name = "Black Iron", Color = new Color(60, 60, 70) },
			new() { Name = "Gold", Color = new Color(235, 195, 70) },
			new() { Name = "Silver", Color = new Color(200, 205, 215) },
			new() { Name = "Bronze", Color = new Color(185, 120, 60) },
			new() { Name = "Copper", Color = new Color(210, 110, 70) },
			new() { Name = "Crimson Lacquer", Color = new Color(170, 30, 40) },
			new() { Name = "Jade", Color = new Color(80, 180, 130) },
			new() { Name = "Violet Lacquer", Color = new Color(120, 70, 170) },
		};

		public static readonly Tint[] Wraps = {
			new() { Name = "Navy", Color = new Color(40, 55, 120) },
			new() { Name = "Black", Color = new Color(35, 35, 40) },
			new() { Name = "White", Color = new Color(225, 225, 230) },
			new() { Name = "Crimson", Color = new Color(170, 30, 40) },
			new() { Name = "Gold", Color = new Color(220, 180, 60) },
			new() { Name = "Purple", Color = new Color(110, 60, 160) },
			new() { Name = "Green", Color = new Color(50, 140, 70) },
			new() { Name = "Pink", Color = new Color(240, 140, 180) },
			new() { Name = "Grey", Color = new Color(120, 120, 130) },
			new() { Name = "Brown", Color = new Color(110, 70, 40) },
			new() { Name = "Teal", Color = new Color(40, 150, 150) },
			new() { Name = "Orange", Color = new Color(230, 130, 40) },
		};

		public static readonly Engraving[] Engravings = {
			new() { Name = "None", Description = "A plain blade", Bonus = new(BonusKind.None) },
			new() { Name = "Destroy All Demons", Description = "+15% damage against demons", Bonus = new(BonusKind.DemonDamage, 15) },
			new() { Name = "Swift", Description = "+6% melee speed", Bonus = new(BonusKind.MeleeSpeed, 6) },
			new() { Name = "Guardian", Description = "+3 defense", Bonus = new(BonusKind.Defense, 3) },
			new() { Name = "Keen", Description = "+5% crit", Bonus = new(BonusKind.Crit, 5) },
			new() { Name = "Vampire's Bane", Description = "Heals 3 HP on hit", Bonus = new(BonusKind.LifeSteal, 3) },
			new() { Name = "Deep Breath", Description = "+20 max Breath", Bonus = new(BonusKind.MaxBreath, 20) },
			new() { Name = "Sunrise", Description = "+12% damage during the day", Bonus = new(BonusKind.DayDamage, 12) },
			new() { Name = "Moonlit", Description = "+12% damage at night", Bonus = new(BonusKind.NightDamage, 12) },
			new() { Name = "Unbroken", Description = "+10% technique damage", Bonus = new(BonusKind.TechniqueDamage, 10) },
		};

		public static readonly Trail[] Trails = {
			new() { Name = "None" },
			new() { Name = "Blade Colour", Dust = DustID.WhiteTorch, UseBladeColour = true },
			new() { Name = "Breathing Style", Dust = DustID.WhiteTorch, UseStyleColour = true },
			new() { Name = "Flames", Dust = DustID.Torch },
			new() { Name = "Water", Dust = DustID.Water },
			new() { Name = "Petals", Dust = DustID.PinkTorch },
			new() { Name = "Lightning", Dust = DustID.Electric },
			new() { Name = "Shadow", Dust = DustID.Shadowflame },
			new() { Name = "Frost", Dust = DustID.IceTorch },
			new() { Name = "Rainbow", Dust = DustID.WhiteTorch, Rainbow = true },
		};
	}
}
