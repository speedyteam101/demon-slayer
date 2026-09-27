namespace DemonSlayerMod.Common.Breathing
{
	public enum TechniqueKind
	{
		Wave,      // Count crescent slashes fly toward the cursor
		Fan,       // like Wave, spread wider
		Homing,    // crescents that seek enemies
		Sweep,     // a huge arc cut in front of you
		Spin,      // Count full turns of a whirling cut around you
		Dash,      // dash through enemies toward the cursor
		MultiDash, // Count dashes, each one aimed at the nearest enemy
		Barrage,   // a storm of cuts at the cursor, hitting Count times
		Vortex,    // like Barrage, and pulls enemies into it
		Rain,      // Count slashes fall on the cursor from above
		Pillar,    // Count eruptions along the ground toward the cursor
		Thrust,    // lunge and fire one fast, piercing thrust
		Slam,      // leap, then crash down with a shockwave
		Burst,     // Count slashes explode outward all around you
		Buff       // a timed buff (see BuffKind)
	}

	public enum BuffKind
	{
		None,
		DeadCalm,   // big defense, damage reduction, no knockback
		Senses,     // crit, and see enemies and danger
		Afterimage  // chance to dodge attacks
	}

	public class Technique
	{
		public string Name;
		public TechniqueKind Kind;
		public int Cost;          // Breath
		public float Damage;      // multiplier on the blade's damage
		public int Count = 1;
		public float Speed = 14f;
		public float Scale = 1f;
		public int Duration = 30; // ticks (Buff: ticks the buff lasts)
		public int Cooldown = 30; // ticks before any technique can be used again
		public BuffKind Buff;
	}

	// The forms of every breathing style. Names follow the English releases as closely as I could manage; some
	// translations differ. Balance: damage multiplier roughly tracks Breath cost.
	public static class Techniques
	{
		private static Technique T(string name, TechniqueKind kind, int cost, float damage, int count = 1, float speed = 14f,
			float scale = 1f, int duration = 30, int cooldown = 30, BuffKind buff = BuffKind.None) {
			return new Technique {
				Name = name, Kind = kind, Cost = cost, Damage = damage, Count = count, Speed = speed,
				Scale = scale, Duration = duration, Cooldown = cooldown, Buff = buff
			};
		}

		public static readonly Technique[] Water = {
			T("First Form: Water Surface Slash", TechniqueKind.Wave, 12, 1.3f, speed: 15f, scale: 1.2f),
			T("Second Form: Water Wheel", TechniqueKind.Spin, 18, 1.2f, count: 1, scale: 1.1f, duration: 20),
			T("Third Form: Flowing Dance", TechniqueKind.Dash, 20, 1.4f, speed: 17f, duration: 18),
			T("Fourth Form: Striking Tide", TechniqueKind.Barrage, 26, 0.7f, count: 6, scale: 1.1f, duration: 36),
			T("Fifth Form: Blessed Rain After the Drought", TechniqueKind.Rain, 24, 1.1f, count: 5, speed: 16f),
			T("Sixth Form: Whirlpool", TechniqueKind.Vortex, 32, 0.6f, count: 8, scale: 1.4f, duration: 60, cooldown: 45),
			T("Seventh Form: Drop Ripple Thrust", TechniqueKind.Thrust, 22, 2.2f, speed: 22f, scale: 1.2f),
			T("Eighth Form: Waterfall Basin", TechniqueKind.Slam, 34, 2.4f, scale: 1.4f, cooldown: 40),
			T("Ninth Form: Splashing Water Flow, Turbulent", TechniqueKind.MultiDash, 40, 1.1f, count: 4, speed: 20f, duration: 12, cooldown: 50),
			T("Tenth Form: Constant Flux", TechniqueKind.Barrage, 55, 0.8f, count: 14, scale: 2f, duration: 70, cooldown: 70),
			T("Eleventh Form: Dead Calm", TechniqueKind.Buff, 60, 0f, duration: 600, cooldown: 1200, buff: BuffKind.DeadCalm),
		};

		public static readonly Technique[] Flame = {
			T("First Form: Unknowing Fire", TechniqueKind.Dash, 16, 1.6f, speed: 20f, duration: 16),
			T("Second Form: Rising Scorching Sun", TechniqueKind.Sweep, 20, 1.7f, scale: 1.2f, duration: 16),
			T("Third Form: Blazing Universe", TechniqueKind.Slam, 28, 2.4f, scale: 1.3f, cooldown: 40),
			T("Fourth Form: Blooming Flame Undulation", TechniqueKind.Spin, 30, 1.5f, count: 2, scale: 1.3f, duration: 30),
			T("Fifth Form: Flame Tiger", TechniqueKind.Wave, 40, 3.2f, speed: 11f, scale: 2.4f, duration: 50, cooldown: 50),
			T("Ninth Form: Rengoku", TechniqueKind.MultiDash, 75, 2.2f, count: 3, speed: 26f, duration: 14, cooldown: 90),
		};

		public static readonly Technique[] Thunder = {
			T("First Form: Thunderclap and Flash", TechniqueKind.Dash, 14, 1.8f, speed: 30f, duration: 10),
			T("Second Form: Rice Spirit", TechniqueKind.Burst, 20, 1.1f, count: 5, speed: 13f),
			T("Third Form: Thunder Swarm", TechniqueKind.Barrage, 24, 0.7f, count: 7, scale: 1.2f, duration: 30),
			T("Fourth Form: Distant Thunder", TechniqueKind.Wave, 22, 1.8f, speed: 24f, scale: 1f),
			T("Fifth Form: Heat Lightning", TechniqueKind.Rain, 30, 1.2f, count: 6, speed: 22f),
			T("Sixth Form: Rumble and Flash", TechniqueKind.Pillar, 34, 1.5f, count: 5, scale: 1.2f, cooldown: 40),
			T("First Form: Thunderclap and Flash, Sixfold", TechniqueKind.MultiDash, 45, 1.3f, count: 6, speed: 32f, duration: 8, cooldown: 60),
			T("First Form: Thunderclap and Flash, Godspeed", TechniqueKind.MultiDash, 60, 1.5f, count: 8, speed: 36f, duration: 8, cooldown: 80),
			T("Seventh Form: Flaming Thunder God", TechniqueKind.Dash, 80, 6f, speed: 40f, duration: 12, cooldown: 120),
		};

		public static readonly Technique[] Wind = {
			T("First Form: Dust Whirlwind Cutter", TechniqueKind.Dash, 16, 1.4f, speed: 19f, duration: 18),
			T("Second Form: Claws-Purifying Wind", TechniqueKind.Fan, 18, 1f, count: 4, speed: 15f),
			T("Third Form: Clean Storm Wind Tree", TechniqueKind.Spin, 22, 1.3f, count: 2, scale: 1.3f, duration: 26),
			T("Fourth Form: Rising Dust Storm", TechniqueKind.Pillar, 26, 1.4f, count: 4, scale: 1.3f),
			T("Fifth Form: Cold Mountain Wind", TechniqueKind.Burst, 30, 1.2f, count: 8, speed: 14f),
			T("Sixth Form: Black Wind Mountain Mist", TechniqueKind.Barrage, 36, 0.8f, count: 9, scale: 1.5f, duration: 45),
			T("Seventh Form: Gale, Sudden Gusts", TechniqueKind.Wave, 36, 2.2f, count: 3, speed: 22f, scale: 1.2f),
			T("Eighth Form: Primary Gale Slash", TechniqueKind.Slam, 44, 2.8f, scale: 1.5f, cooldown: 50),
			T("Ninth Form: Idaten Typhoon", TechniqueKind.Vortex, 60, 0.9f, count: 12, scale: 2f, duration: 70, cooldown: 80),
		};

		public static readonly Technique[] Stone = {
			T("First Form: Serpentinite Bipolar", TechniqueKind.Fan, 20, 1.6f, count: 2, speed: 12f, scale: 1.4f),
			T("Second Form: Upper Smash", TechniqueKind.Slam, 28, 2.6f, scale: 1.5f, cooldown: 40),
			T("Third Form: Stone Skin", TechniqueKind.Buff, 40, 0f, duration: 600, cooldown: 1200, buff: BuffKind.DeadCalm),
			T("Fourth Form: Volcanic Rock, Rapid Conquest", TechniqueKind.Barrage, 45, 1f, count: 10, scale: 1.8f, duration: 55, cooldown: 60),
			T("Fifth Form: Arcs of Justice", TechniqueKind.Pillar, 60, 2.4f, count: 7, scale: 1.7f, cooldown: 80),
		};

		public static readonly Technique[] Mist = {
			T("First Form: Low Clouds, Distant Haze", TechniqueKind.Dash, 16, 1.5f, speed: 22f, duration: 14),
			T("Second Form: Eight-Layered Mist", TechniqueKind.Barrage, 22, 0.6f, count: 8, scale: 1.1f, duration: 32),
			T("Third Form: Scattering Mist Splash", TechniqueKind.Spin, 24, 1.3f, count: 2, scale: 1.2f, duration: 24),
			T("Fourth Form: Shifting Flow Slash", TechniqueKind.Sweep, 26, 1.9f, scale: 1.3f, duration: 14),
			T("Fifth Form: Sea of Clouds and Haze", TechniqueKind.MultiDash, 38, 1.2f, count: 5, speed: 24f, duration: 10, cooldown: 50),
			T("Sixth Form: Lunar Dispersing Mist", TechniqueKind.Fan, 34, 1.4f, count: 5, speed: 16f, scale: 1.2f),
			T("Seventh Form: Obscuring Clouds", TechniqueKind.Buff, 50, 0f, duration: 480, cooldown: 1200, buff: BuffKind.Afterimage),
		};

		public static readonly Technique[] Serpent = {
			T("First Form: Winding Serpent Slash", TechniqueKind.Homing, 16, 1.2f, count: 2, speed: 12f),
			T("Second Form: Venom Fangs of the Narrow Head", TechniqueKind.Thrust, 22, 2.2f, speed: 24f),
			T("Third Form: Coil Choke", TechniqueKind.Spin, 28, 1.4f, count: 3, scale: 1.2f, duration: 32),
			T("Fourth Form: Twin-Headed Reptile", TechniqueKind.Homing, 34, 1.4f, count: 4, speed: 13f, scale: 1.2f),
			T("Fifth Form: Slithering Serpent", TechniqueKind.Vortex, 50, 0.8f, count: 10, scale: 1.7f, duration: 60, cooldown: 70),
		};

		public static readonly Technique[] Love = {
			T("First Form: Shivers of First Love", TechniqueKind.Dash, 16, 1.5f, speed: 21f, duration: 16),
			T("Second Form: Love Pangs", TechniqueKind.Burst, 24, 1.2f, count: 8, speed: 13f),
			T("Third Form: Catlove Shower", TechniqueKind.Rain, 30, 1.1f, count: 8, speed: 18f),
			T("Fifth Form: Swaying Love, Wildclaw", TechniqueKind.Spin, 40, 1.4f, count: 3, scale: 1.8f, duration: 36, cooldown: 50),
			T("Sixth Form: Cat-Legged Winds of Love", TechniqueKind.Sweep, 50, 2.6f, scale: 2f, duration: 18, cooldown: 60),
		};

		public static readonly Technique[] Insect = {
			T("Butterfly Dance: Caprice", TechniqueKind.Thrust, 14, 1.8f, speed: 22f),
			T("Bee Sting: True Flutter", TechniqueKind.Thrust, 24, 3.2f, speed: 30f, scale: 1.3f, cooldown: 36),
			T("Dragonfly Dance: Compound Eye Hexagon", TechniqueKind.Barrage, 34, 0.9f, count: 6, scale: 1.2f, duration: 30),
			T("Centipede Dance: Hundred-Legged Zigzag", TechniqueKind.MultiDash, 45, 1.6f, count: 6, speed: 28f, duration: 8, cooldown: 60),
		};

		public static readonly Technique[] Flower = {
			T("Second Form: Honorable Shadow Plum", TechniqueKind.Spin, 18, 1.2f, count: 1, scale: 1.2f, duration: 20),
			T("Fourth Form: Crimson Hanagoromo", TechniqueKind.Sweep, 22, 1.8f, scale: 1.3f, duration: 16),
			T("Fifth Form: Peonies of Futility", TechniqueKind.Fan, 28, 1.1f, count: 9, speed: 15f, scale: 0.9f),
			T("Sixth Form: Whirling Peach", TechniqueKind.Vortex, 36, 0.7f, count: 8, scale: 1.4f, duration: 45),
			T("Final Form: Equinoctial Vermilion Eye", TechniqueKind.Buff, 60, 0f, duration: 600, cooldown: 1500, buff: BuffKind.Senses),
		};

		public static readonly Technique[] Sound = {
			T("First Form: Roar", TechniqueKind.Slam, 24, 2.4f, scale: 1.5f, cooldown: 36),
			T("Fourth Form: Constant Resounding Slashes", TechniqueKind.Barrage, 36, 1.1f, count: 8, scale: 1.6f, duration: 40),
			T("Fifth Form: String Performance", TechniqueKind.Burst, 50, 1.6f, count: 12, speed: 16f, scale: 1.3f, cooldown: 60),
		};

		public static readonly Technique[] Beast = {
			T("First Fang: Pierce", TechniqueKind.Thrust, 12, 1.6f, speed: 20f),
			T("Second Fang: Rip and Tear", TechniqueKind.Fan, 16, 1.2f, count: 2, speed: 14f),
			T("Third Fang: Devour", TechniqueKind.Sweep, 20, 1.6f, scale: 1.1f, duration: 14),
			T("Fourth Fang: Slice 'n' Dice", TechniqueKind.Barrage, 24, 0.6f, count: 8, duration: 30),
			T("Fifth Fang: Crazy Cutting", TechniqueKind.Spin, 26, 1.2f, count: 3, scale: 1.2f, duration: 30),
			T("Sixth Fang: Palisade Bite", TechniqueKind.Wave, 28, 2f, count: 2, speed: 14f, scale: 1.4f),
			T("Seventh Form: Spatial Awareness", TechniqueKind.Buff, 30, 0f, duration: 900, cooldown: 1200, buff: BuffKind.Senses),
			T("Eighth Fang: Explosive Rush", TechniqueKind.Dash, 34, 2.4f, speed: 24f, duration: 18, cooldown: 40),
			T("Ninth Fang: Extending Bendy Slash", TechniqueKind.Thrust, 38, 2.6f, speed: 26f, scale: 2f, cooldown: 40),
			T("Tenth Fang: Whirling Fangs", TechniqueKind.Spin, 50, 1.4f, count: 4, scale: 1.6f, duration: 40, cooldown: 60),
		};

		public static readonly Technique[] Moon = {
			T("First Form: Dark Moon, Evening Palace", TechniqueKind.Sweep, 20, 2f, scale: 1.4f, duration: 14),
			T("Second Form: Pearl Flower Moongazing", TechniqueKind.Burst, 26, 1.3f, count: 9, speed: 14f),
			T("Third Form: Loathsome Moon, Chains", TechniqueKind.Fan, 30, 1.4f, count: 5, speed: 15f, scale: 1.3f),
			T("Fifth Form: Moon Spirit Calamitous Eddy", TechniqueKind.Vortex, 38, 0.9f, count: 10, scale: 1.7f, duration: 50),
			T("Sixth Form: Perpetual Night, Lonely Moon - Incessant", TechniqueKind.Barrage, 46, 1f, count: 12, scale: 2f, duration: 55, cooldown: 60),
			T("Seventh Form: Mirror of Misfortune, Moonlit", TechniqueKind.Rain, 50, 1.4f, count: 10, speed: 20f, scale: 1.3f, cooldown: 60),
			T("Tenth Form: Drilling Slashes, Moon Through Bamboo Leaves", TechniqueKind.Pillar, 60, 2f, count: 7, scale: 1.8f, cooldown: 70),
			T("Sixteenth Form: Moonbow, Half Moon", TechniqueKind.Wave, 85, 5f, count: 3, speed: 12f, scale: 3f, duration: 60, cooldown: 120),
		};

		public static readonly Technique[] Sun = {
			T("Dance", TechniqueKind.Sweep, 18, 2f, scale: 1.3f, duration: 14),
			T("Clear Blue Sky", TechniqueKind.Spin, 22, 1.6f, count: 2, scale: 1.3f, duration: 24),
			T("Raging Sun", TechniqueKind.Fan, 26, 1.5f, count: 2, speed: 14f, scale: 1.4f),
			T("Burning Bones, Summer Sun", TechniqueKind.Slam, 30, 2.6f, scale: 1.5f, cooldown: 36),
			T("Setting Sun Transformation", TechniqueKind.Dash, 30, 2.2f, speed: 24f, duration: 16),
			T("Solar Heat Haze", TechniqueKind.Barrage, 36, 0.9f, count: 9, scale: 1.5f, duration: 40),
			T("Beneficent Radiance", TechniqueKind.Rain, 40, 1.4f, count: 8, speed: 20f, scale: 1.2f),
			T("Sunflower Thrust", TechniqueKind.Thrust, 34, 3f, speed: 30f, scale: 1.5f),
			T("Dragon Sun Halo Head Dance", TechniqueKind.Homing, 45, 1.6f, count: 6, speed: 14f, scale: 1.3f, cooldown: 50),
			T("Fake Rainbow", TechniqueKind.Buff, 50, 0f, duration: 480, cooldown: 1200, buff: BuffKind.Afterimage),
			T("Fire Wheel", TechniqueKind.Spin, 50, 1.9f, count: 3, scale: 1.8f, duration: 32, cooldown: 50),
			T("Flame Dance", TechniqueKind.Burst, 60, 2f, count: 12, speed: 16f, scale: 1.4f, cooldown: 60),
			T("Thirteenth Form", TechniqueKind.MultiDash, 100, 2.4f, count: 12, speed: 34f, duration: 8, cooldown: 150),
		};
	}
}
