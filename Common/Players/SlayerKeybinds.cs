using Terraria.ModLoader;

namespace DemonSlayerMod.Common.Players
{
	public class SlayerKeybinds : ModSystem
	{
		public static ModKeybind Technique { get; private set; }
		public static ModKeybind NextForm { get; private set; }
		public static ModKeybind PreviousForm { get; private set; }
		public static ModKeybind OpenMenu { get; private set; }
		public static ModKeybind Mark { get; private set; }
		public static ModKeybind DemonForm { get; private set; }

		public override void Load() {
			Technique = KeybindLoader.RegisterKeybind(Mod, "Technique", "F");
			NextForm = KeybindLoader.RegisterKeybind(Mod, "NextForm", "G");
			PreviousForm = KeybindLoader.RegisterKeybind(Mod, "PreviousForm", "V");
			OpenMenu = KeybindLoader.RegisterKeybind(Mod, "OpenMenu", "K");
			Mark = KeybindLoader.RegisterKeybind(Mod, "Mark", "X");
			DemonForm = KeybindLoader.RegisterKeybind(Mod, "DemonForm", "Z");
		}

		public override void Unload() {
			Technique = null;
			NextForm = null;
			PreviousForm = null;
			OpenMenu = null;
			Mark = null;
			DemonForm = null;
		}
	}
}
