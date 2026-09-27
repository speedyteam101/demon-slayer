using DemonSlayerMod.Common.Players;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DemonSlayerMod.Content.NPCs.Town
{
	// Muzan Kibutsuji in disguise. He rarely appears on the surface at night and vanishes at dawn.
	// Talk to him and you can accept his blood, which lets you take a demon form at night (see DemonFormBuff).
	// He can't be hurt; the real fight is the Muzan boss.
	public class MysteriousGentleman : ModNPC
	{
		private const int FrameCount = 4;

		public override void SetStaticDefaults() {
			Main.npcFrameCount[Type] = FrameCount;
		}

		public override void SetDefaults() {
			NPC.width = 28;
			NPC.height = 48;
			NPC.friendly = true;
			NPC.dontTakeDamage = true;
			NPC.damage = 0;
			NPC.defense = 999;
			NPC.lifeMax = 1000;
			NPC.knockBackResist = 0f;
			NPC.aiStyle = -1;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.rarity = 3; // shows on the Lifeform Analyzer
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			if (NPC.AnyNPCs(Type) || spawnInfo.PlayerInTown) {
				return 0f;
			}
			return SpawnCondition.OverworldNightMonster.Chance * 0.015f;
		}

		public override bool CheckActive() => Main.dayTime; // stays until dawn, even off-screen

		public override void AI() {
			// Stands still, watching you. Leaves at dawn in a swirl of blood.
			NPC.velocity.X = 0f;
			NPC.TargetClosest();
			NPC.spriteDirection = NPC.direction;
			if (Main.dayTime) {
				for (int i = 0; i < 20; i++) {
					Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood);
				}
				NPC.active = false;
				if (Main.netMode == NetmodeID.Server) {
					NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
				}
			}
		}

		public override void FindFrame(int frameHeight) {
			if (++NPC.frameCounter >= 20) {
				NPC.frameCounter = 0;
				NPC.frame.Y = (NPC.frame.Y + frameHeight) % (FrameCount * frameHeight);
			}
		}

		public override bool CanChat() => true;

		public override string GetChat() {
			if (SlayerPlayer.Get(Main.LocalPlayer).IsDemon) {
				return Main.rand.Next(3) switch {
					0 => "My blood suits you. Hunt well, and stay out of the sun.",
					1 => "You feel it, don't you? The night belongs to us.",
					_ => "Don't disappoint me."
				};
			}
			return Main.rand.Next(3) switch {
				0 => "What an interesting human. You carry the scent of those wretched Slayers... and yet you came to me.",
				1 => "Do you want to be stronger? To never grow old, never tire? A single drop of my blood is all it takes.",
				_ => "Such a pale face. Are you afraid of me? You should be."
			};
		}

		public override void SetChatButtons(ref string button, ref string button2) {
			if (!SlayerPlayer.Get(Main.LocalPlayer).IsDemon) {
				button = "Accept his blood";
			}
			button2 = "Walk away";
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName) {
			if (!firstButton) {
				Main.npcChatText = "Run along, then. We will meet again.";
				return;
			}
			Player player = Main.LocalPlayer;
			SlayerPlayer slayer = SlayerPlayer.Get(player);
			if (slayer.IsDemon) {
				return;
			}
			slayer.IsDemon = true;
			SoundEngine.PlaySound(SoundID.NPCDeath10, player.Center);
			for (int i = 0; i < 40; i++) {
				Dust.NewDust(player.position, player.width, player.height, DustID.Blood, 0f, -2f, 0, default, 1.5f);
			}
			Main.npcChatText = "Good. Now you are mine. Only the night will welcome you in that form... the sun will not.";
			Main.NewText("Muzan's blood flows through you. At night, press the Demon Form key (default Z) to transform.", 220, 40, 60);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange([
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.DemonSlayerMod.Bestiary." + Name)
			]);
		}
	}
}
