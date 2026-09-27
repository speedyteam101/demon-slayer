using System;
using System.IO;
using Terraria.ModLoader.IO;

namespace DemonSlayerMod.Common.Swords
{
	// The forge choices stored on one Nichirin Blade (all indices into SwordParts tables).
	public class SwordBuild
	{
		public const int Categories = 8;

		public int Shape, Steel, Colour, Guard, GuardFinish, Wrap, Engraving, Trail;

		public SwordBuild Clone() => (SwordBuild)MemberwiseClone();

		public static int Count(int category) => category switch {
			0 => SwordParts.Steels.Length,
			1 => SwordParts.Shapes.Length,
			2 => SwordParts.Colours.Length,
			3 => SwordParts.Guards.Length,
			4 => SwordParts.GuardFinishes.Length,
			5 => SwordParts.Wraps.Length,
			6 => SwordParts.Engravings.Length,
			7 => SwordParts.Trails.Length,
			_ => 1
		};

		public static string CategoryName(int category) => category switch {
			0 => "Steel",
			1 => "Blade Shape",
			2 => "Nichirin Colour",
			3 => "Guard",
			4 => "Guard Finish",
			5 => "Hilt Wrap",
			6 => "Engraving",
			7 => "Swing Trail",
			_ => ""
		};

		public int Get(int category) => category switch {
			0 => Steel,
			1 => Shape,
			2 => Colour,
			3 => Guard,
			4 => GuardFinish,
			5 => Wrap,
			6 => Engraving,
			7 => Trail,
			_ => 0
		};

		public void Set(int category, int value) {
			value = Math.Clamp(value, 0, Count(category) - 1);
			switch (category) {
				case 0: Steel = value; break;
				case 1: Shape = value; break;
				case 2: Colour = value; break;
				case 3: Guard = value; break;
				case 4: GuardFinish = value; break;
				case 5: Wrap = value; break;
				case 6: Engraving = value; break;
				case 7: Trail = value; break;
			}
		}

		public string ValueName(int category) {
			int v = Get(category);
			return category switch {
				0 => SwordParts.Steels[v].Name,
				1 => SwordParts.Shapes[v].Name,
				2 => SwordParts.Colours[v].Name,
				3 => SwordParts.Guards[v].Name,
				4 => SwordParts.GuardFinishes[v].Name,
				5 => SwordParts.Wraps[v].Name,
				6 => SwordParts.Engravings[v].Name,
				7 => SwordParts.Trails[v].Name,
				_ => ""
			};
		}

		public string ValueDescription(int category) {
			int v = Get(category);
			return category switch {
				0 => $"{SwordParts.Steels[v].Damage} base damage. {SwordParts.Steels[v].Description}",
				1 => SwordParts.Shapes[v].Description,
				2 => SwordParts.Colours[v].Description,
				3 => SwordParts.Guards[v].Description,
				4 => "Cosmetic",
				5 => "Cosmetic",
				6 => SwordParts.Engravings[v].Description,
				7 => "Cosmetic",
				_ => ""
			};
		}

		public void Clamp() {
			for (int c = 0; c < Categories; c++) {
				Set(c, Get(c));
			}
		}

		public void Save(TagCompound tag) {
			tag["shape"] = Shape;
			tag["steel"] = Steel;
			tag["colour"] = Colour;
			tag["guard"] = Guard;
			tag["guardFinish"] = GuardFinish;
			tag["wrap"] = Wrap;
			tag["engraving"] = Engraving;
			tag["trail"] = Trail;
		}

		public void Load(TagCompound tag) {
			Shape = tag.GetInt("shape");
			Steel = tag.GetInt("steel");
			Colour = tag.GetInt("colour");
			Guard = tag.GetInt("guard");
			GuardFinish = tag.GetInt("guardFinish");
			Wrap = tag.GetInt("wrap");
			Engraving = tag.GetInt("engraving");
			Trail = tag.GetInt("trail");
			Clamp();
		}

		public void Write(BinaryWriter writer) {
			for (int c = 0; c < Categories; c++) {
				writer.Write((byte)Get(c));
			}
		}

		public void Read(BinaryReader reader) {
			for (int c = 0; c < Categories; c++) {
				Set(c, reader.ReadByte());
			}
		}
	}

	// All the bonuses a build gives, added up.
	public class SwordStats
	{
		public int Crit, Defense, ArmorPen, MaxBreath, LifeSteal;
		public float MeleeSpeed, MoveSpeed, Damage, TechniqueDamage, BreathRegen, DemonDamage, DayDamage, NightDamage; // fractions
		public int Debuff = -1;

		public static SwordStats From(SwordBuild build) {
			var stats = new SwordStats();
			BladeShape shape = SwordParts.Shapes[build.Shape];
			stats.Crit += shape.Crit;
			stats.ArmorPen += shape.ArmorPen;
			stats.Add(SwordParts.Colours[build.Colour].Bonus);
			stats.Add(SwordParts.Guards[build.Guard].Bonus);
			stats.Add(SwordParts.Guards[build.Guard].Drawback);
			stats.Add(SwordParts.Engravings[build.Engraving].Bonus);
			return stats;
		}

		private void Add(PartBonus bonus) {
			if (bonus == null) {
				return;
			}
			float pct = bonus.Amount / 100f;
			switch (bonus.Kind) {
				case BonusKind.Crit: Crit += bonus.Amount; break;
				case BonusKind.MeleeSpeed: MeleeSpeed += pct; break;
				case BonusKind.MoveSpeed: MoveSpeed += pct; break;
				case BonusKind.Defense: Defense += bonus.Amount; break;
				case BonusKind.Damage: Damage += pct; break;
				case BonusKind.TechniqueDamage: TechniqueDamage += pct; break;
				case BonusKind.BreathRegen: BreathRegen += pct; break;
				case BonusKind.MaxBreath: MaxBreath += bonus.Amount; break;
				case BonusKind.ArmorPen: ArmorPen += bonus.Amount; break;
				case BonusKind.LifeSteal: LifeSteal += bonus.Amount; break;
				case BonusKind.DemonDamage: DemonDamage += pct; break;
				case BonusKind.DayDamage: DayDamage += pct; break;
				case BonusKind.NightDamage: NightDamage += pct; break;
				case BonusKind.Debuff: Debuff = bonus.Buff; break;
			}
		}
	}
}
