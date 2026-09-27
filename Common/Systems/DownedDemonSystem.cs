using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DemonSlayerMod.Common.Systems
{
	// Remembers which demon bosses have been defeated in this world, by NPC class name.
	// The game syncs world data (NetSend) to clients after a boss dies.
	public class DownedDemonSystem : ModSystem
	{
		public static readonly HashSet<string> Downed = new();

		public static bool IsDowned(string key) => Downed.Contains(key);

		public static void MarkDowned(string key) {
			Downed.Add(key);
		}

		public override void ClearWorld() {
			Downed.Clear();
		}

		public override void SaveWorldData(TagCompound tag) {
			if (Downed.Count > 0) {
				tag["downedDemons"] = Downed.ToList();
			}
		}

		public override void LoadWorldData(TagCompound tag) {
			Downed.Clear();
			foreach (string key in tag.GetList<string>("downedDemons")) {
				Downed.Add(key);
			}
		}

		public override void NetSend(BinaryWriter writer) {
			writer.Write(Downed.Count);
			foreach (string key in Downed) {
				writer.Write(key);
			}
		}

		public override void NetReceive(BinaryReader reader) {
			Downed.Clear();
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++) {
				Downed.Add(reader.ReadString());
			}
		}
	}
}
