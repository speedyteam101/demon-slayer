using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DemonSlayerMod.Common.Config
{
	public class DemonSlayerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		public static DemonSlayerConfig Instance => ModContent.GetInstance<DemonSlayerConfig>();

		[DefaultValue(true)]
		public bool DemonsResistOtherWeapons;

		[DefaultValue(true)]
		public bool DemonsBurnInSunlight;

		[DefaultValue(true)]
		public bool DemonsRegenerate;

		[Range(0.25f, 10f)]
		[Increment(0.25f)]
		[DefaultValue(1f)]
		public float XPMultiplier;
	}
}
