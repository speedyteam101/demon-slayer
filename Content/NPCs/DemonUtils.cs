using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace DemonSlayerMod.Content.NPCs
{
	public static class DemonUtils
	{
		public static int CountActive(int type) {
			int count = 0;
			foreach (NPC other in Main.ActiveNPCs) {
				if (other.type == type) {
					count++;
				}
			}
			return count;
		}

		// Smoothly steers a flying NPC toward a point.
		public static void FlyToward(NPC npc, Vector2 target, float speed, float inertia) {
			Vector2 desired = (target - npc.Center).SafeNormalize(Vector2.Zero) * speed;
			npc.velocity = (npc.velocity * (inertia - 1f) + desired) / inertia;
		}

		// Spawns a minion NPC from the server and syncs it. Does nothing on multiplayer clients.
		public static void SpawnMinion(NPC parent, int type, Vector2 position) {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}
			NPC minion = NPC.NewNPCDirect(parent.GetSource_FromAI(), (int)position.X, (int)position.Y, type);
			if (minion.whoAmI < Main.maxNPCs && Main.netMode == NetmodeID.Server) {
				NetMessage.SendData(MessageID.SyncNPC, number: minion.whoAmI);
			}
		}

		// Fires a hostile projectile from the server (or in single player).
		public static void Shoot(NPC npc, Vector2 position, Vector2 velocity, int type, int damage, float ai0 = 0f, float ai1 = 0f) {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}
			Projectile.NewProjectile(npc.GetSource_FromAI(), position, velocity, type, damage, 0f, Main.myPlayer, ai0, ai1);
		}

		// The top of the ground at or below a point (searching 30 tiles down), or the point itself.
		public static Vector2 GroundBelow(Vector2 point) {
			int x = (int)(point.X / 16f);
			int y = (int)(point.Y / 16f);
			for (int j = y; j < y + 30; j++) {
				if (WorldGen.InWorld(x, j) && WorldGen.SolidTile(x, j)) {
					return new Vector2(point.X, j * 16f);
				}
			}
			return point;
		}

		public static void Bleed(NPC npc, NPC.HitInfo hit, int dustType) {
			int count = npc.life <= 0 ? (npc.boss ? 80 : 20) : 4;
			for (int i = 0; i < count; i++) {
				Dust.NewDust(npc.position, npc.width, npc.height, dustType, hit.HitDirection * 2f, -2f);
			}
		}
	}
}
