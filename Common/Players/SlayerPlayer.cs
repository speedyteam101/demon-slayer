using DemonSlayerMod.Common.Breathing;
using DemonSlayerMod.Common.Config;
using DemonSlayerMod.Common.Swords;
using DemonSlayerMod.Common.Systems;
using DemonSlayerMod.Common.UI;
using DemonSlayerMod.Content.Buffs;
using DemonSlayerMod.Content.Items;
using DemonSlayerMod.Content.NPCs;
using DemonSlayerMod.Content.NPCs.Bosses;
using DemonSlayerMod.Content.Projectiles.Breath;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DemonSlayerMod.Common.Players
{
	// Level, rank, stats, Breath and breathing technique use for one character.
	public class SlayerPlayer : ModPlayer
	{
		public const int MaxLevel = 100;
		public const int PointsPerLevel = 2;
		public const int StatCap = 50;
		public const int MaxScrolls = 20;
		public const int BaseBreath = 100;
		public const float BaseBreathRegen = 8f; // per second

		// Saved
		public int Level = 1;
		public long XP;
		public int[] Stats = new int[SlayerStats.Count];
		public int ScrollsUsed;
		public int Style = BreathingStyles.None;
		public int Form;
		public readonly HashSet<string> Defeated = new(); // demon bosses this character has seen fall
		public bool IsDemon;                               // accepted Muzan's blood (MysteriousGentleman)

		// Not saved
		public float Breath;
		public int BreathMax = BaseBreath;
		public float BreathRegen = BaseBreathRegen;
		public int BreathDelay;          // ticks until Breath starts refilling after a technique
		public int TechniqueCooldown;
		public int TechniqueCooldownMax = 1;
		public int MarkCooldown;
		public SwordStats HeldBlade;     // bonuses of the Nichirin Blade in hand, or null

		// Set by accessories and buffs each tick (see Content/Items/Gear.cs).
		public int BonusMaxBreath;
		public float BonusBreathRegen;       // fraction
		public float BonusTechniqueDamage;   // fraction
		public float BonusSunTechniqueDamage; // fraction, Sun Breathing only
		public float DemonDamageTaken = 1f;  // multiplier on damage from demons
		public bool WisteriaPoison;          // blade and technique hits inflict Venom, +10% damage to demons
		private int lifeStealTimer;
		private HashSet<string> downedWhenEntered;
		private bool hintShown;

		public static SlayerPlayer Get(Player player) => player.GetModPlayer<SlayerPlayer>();

		public int Rank => SlayerRanks.RankOf(Level);
		public int PointsTotal => (Level - 1) * PointsPerLevel + ScrollsUsed;
		public int PointsFree => PointsTotal - Stats.Sum();
		public int Stat(SlayerStat stat) => Stats[(int)stat];
		public bool HoldingBlade => Player.HeldItem?.ModItem is NichirinBlade;
		public BreathingStyle CurrentStyle => BreathingStyles.Valid(Style) ? BreathingStyles.All[Style] : null;
		public Technique CurrentForm => CurrentStyle?.Forms[Math.Clamp(Form, 0, CurrentStyle.Forms.Length - 1)];

		public static long XPToNext(int level) => 40 + 4L * level * level;

		public bool HasDefeated(string key) => Defeated.Contains(key);

		// The best steel at or below `steel` that this character has earned.
		public int HighestUsableSteel(int steel) {
			for (int s = Math.Min(steel, SwordParts.Steels.Length - 1); s > 0; s--) {
				string boss = SwordParts.Steels[s].RequiredBoss;
				if (boss == null || HasDefeated(boss)) {
					return s;
				}
			}
			return 0;
		}

		public bool SteelUnlocked(int steel) => HighestUsableSteel(steel) == steel;

		public bool StyleUnlocked(int style) {
			BreathingStyle s = BreathingStyles.All[style];
			return Rank >= s.RequiredRank && BreathingStyles.BossRequirementMet(s, this);
		}

		public bool FormUnlocked(int form) {
			BreathingStyle s = CurrentStyle;
			return s != null && form >= 0 && form < s.Forms.Length && Rank >= BreathingStyles.FormRank(s, form);
		}

		public void SetStyle(int style) {
			if (BreathingStyles.Valid(style) && StyleUnlocked(style)) {
				Style = style;
				Form = 0;
			}
		}

		public void CycleForm(int step) {
			BreathingStyle s = CurrentStyle;
			if (s == null) {
				return;
			}
			for (int i = 1; i <= s.Forms.Length; i++) {
				int next = ((Form + step * i) % s.Forms.Length + s.Forms.Length) % s.Forms.Length;
				if (FormUnlocked(next)) {
					Form = next;
					return;
				}
			}
		}

		public void AddPoint(SlayerStat stat, int amount = 1) {
			int i = (int)stat;
			amount = Math.Min(amount, Math.Min(PointsFree, StatCap - Stats[i]));
			if (amount > 0) {
				Stats[i] += amount;
			}
		}

		public void ResetPoints() {
			Array.Clear(Stats);
		}

		// ---- XP -------------------------------------------------------------------------------------------------

		public void AddXP(long amount) {
			if (Level >= MaxLevel || amount <= 0) {
				return;
			}
			XP += amount;
			int oldRank = Rank;
			bool leveled = false;
			while (Level < MaxLevel && XP >= XPToNext(Level)) {
				XP -= XPToNext(Level);
				Level++;
				leveled = true;
			}
			if (Level >= MaxLevel) {
				XP = 0;
			}
			if (leveled && Player.whoAmI == Main.myPlayer) {
				SoundEngine.PlaySound(SoundID.Item4, Player.Center);
				CombatText.NewText(Player.Hitbox, new Color(255, 220, 90), $"Level {Level}!", true);
				if (Rank != oldRank) {
					Main.NewText($"You have been promoted to {SlayerRanks.Names[Rank]}! New forms and breathing styles may be available (open the Slayer menu).", 255, 200, 80);
				}
			}
		}

		public static long XPForKill(NPC npc) {
			if (npc.friendly || npc.townNPC || npc.lifeMax <= 5 || npc.SpawnedFromStatue || npc.CountsAsACritter || npc.immortal || npc.type == NPCID.TargetDummy) {
				return 0;
			}
			float xp = DemonTraits.IsDemon(npc) ? npc.lifeMax / 3f : npc.lifeMax / 15f;
			return Math.Max(1, (long)(xp * DemonSlayerConfig.Instance.XPMultiplier));
		}

		private void CheckKill(NPC target) {
			if (target.life <= 0 && Player.whoAmI == Main.myPlayer) {
				AddXP(XPForKill(target));
			}
		}

		// Boss credit: a boss that falls while you're in the world gives a big XP reward once; bosses already beaten
		// when you arrive still count for unlocking steels and styles, but give no XP.
		private void CheckBossCredit() {
			foreach (string key in DownedDemonSystem.Downed) {
				if (Defeated.Add(key) && downedWhenEntered != null && !downedWhenEntered.Contains(key)) {
					long reward = (long)(DemonBossRewards.XPFor(key) * DemonSlayerConfig.Instance.XPMultiplier);
					AddXP(reward);
					Main.NewText($"Demon slain! +{reward} XP. New Nichirin steel may be available at the Swordsmith's Forge.", 255, 120, 120);
				}
			}
		}

		public override void OnEnterWorld() {
			downedWhenEntered = new HashSet<string>(DownedDemonSystem.Downed);
			Defeated.UnionWith(DownedDemonSystem.Downed);
			Breath = BreathMax;
			hintShown = false;
		}

		// ---- Stats ------------------------------------------------------------------------------------------------

		public override void ResetEffects() {
			HeldBlade = null;
			BonusMaxBreath = 0;
			BonusBreathRegen = 0f;
			BonusTechniqueDamage = 0f;
			BonusSunTechniqueDamage = 0f;
			DemonDamageTaken = 1f;
			WisteriaPoison = false;
		}

		public override void ModifyMaxStats(out StatModifier health, out StatModifier mana) {
			base.ModifyMaxStats(out health, out mana);
			health.Base += Stat(SlayerStat.Endurance) * 3;
		}

		public override void PostUpdateEquips() {
			float rankDamage = Rank * 0.01f + (Rank >= SlayerRanks.Hashira ? 0.1f : 0f);
			Player.GetDamage(DamageClass.Melee) += Stat(SlayerStat.Strength) * 0.015f + rankDamage;
			Player.moveSpeed += Stat(SlayerStat.Agility) * 0.01f;
			Player.GetAttackSpeed(DamageClass.Melee) += Stat(SlayerStat.Agility) * 0.008f;
			Player.statDefense += Stat(SlayerStat.Endurance) / 2 + (Rank >= SlayerRanks.Hashira ? 10 : 0);
			Player.GetCritChance(DamageClass.Melee) += Stat(SlayerStat.Focus) * 0.5f;

			BreathMax = BaseBreath + Stat(SlayerStat.Breath) * 4 + BonusMaxBreath;
			float regen = 1f + Stat(SlayerStat.Breath) * 0.02f + BonusBreathRegen;
			if (Rank >= SlayerRanks.TotalConcentrationRank) {
				regen += 0.5f; // Total Concentration Breathing: Constant
			}

			if (Player.HeldItem?.ModItem is NichirinBlade blade) {
				HeldBlade = SwordStats.From(blade.Build);
				Player.statDefense += HeldBlade.Defense;
				Player.moveSpeed += HeldBlade.MoveSpeed;
				Player.GetAttackSpeed(DamageClass.Melee) += HeldBlade.MeleeSpeed;
				Player.GetDamage(DamageClass.Melee) += HeldBlade.Damage;
				Player.GetArmorPenetration(DamageClass.Melee) += HeldBlade.ArmorPen;
				BreathMax += HeldBlade.MaxBreath;
				regen += HeldBlade.BreathRegen;
			}
			BreathRegen = BaseBreathRegen * regen;
		}

		// Multiplier for breathing technique damage on top of the blade's damage.
		public float TechniqueMultiplier() {
			float mult = 1f + Stat(SlayerStat.Focus) * 0.01f + BonusTechniqueDamage;
			if (Style == BreathingStyles.Sun) {
				mult += BonusSunTechniqueDamage;
			}
			if (HeldBlade != null) {
				mult += HeldBlade.TechniqueDamage;
			}
			if (Player.HeldItem?.ModItem is NichirinBlade blade && CurrentStyle != null && blade.Build.Colour == CurrentStyle.AffinityColour) {
				mult += BreathingStyles.AffinityBonus;
			}
			return mult;
		}

		public bool StyleMatchesBlade() {
			return Player.HeldItem?.ModItem is NichirinBlade blade && CurrentStyle != null && blade.Build.Colour == CurrentStyle.AffinityColour;
		}

		// ---- Hits -------------------------------------------------------------------------------------------------

		private void ModifyBladeHit(NPC target, ref NPC.HitModifiers modifiers) {
			if (HeldBlade == null) {
				return;
			}
			float bonus = 0f;
			if (DemonTraits.IsDemon(target)) {
				bonus += HeldBlade.DemonDamage + (WisteriaPoison ? 0.1f : 0f);
			}
			bonus += Main.dayTime ? HeldBlade.DayDamage : HeldBlade.NightDamage;
			modifiers.FinalDamage *= 1f + bonus;
		}

		public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers) {
			if (item.ModItem is NichirinBlade) {
				ModifyBladeHit(target, ref modifiers);
			}
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers) {
			if (proj.ModProjectile is BreathProjectile) {
				ModifyBladeHit(target, ref modifiers);
			}
		}

		public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone) {
			CheckKill(target);
		}

		public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone) {
			CheckKill(target);
		}

		// Called for every hit from the blade or a technique: colour debuff and life steal.
		public void OnBladeHit(NPC target) {
			if (HeldBlade == null) {
				return;
			}
			if (HeldBlade.Debuff >= 0) {
				target.AddBuff(HeldBlade.Debuff, 240);
			}
			if (WisteriaPoison) {
				target.AddBuff(BuffID.Venom, 240);
			}
			if (HeldBlade.LifeSteal > 0 && lifeStealTimer <= 0 && !target.immortal) {
				Player.Heal(HeldBlade.LifeSteal);
				lifeStealTimer = 30;
			}
		}

		// Wisteria Charm: demons and their attacks hurt less.
		public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers) {
			if (DemonTraits.IsDemon(npc)) {
				modifiers.FinalDamage *= DemonDamageTaken;
			}
		}

		public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers) {
			if (proj.ModProjectile is Content.Projectiles.Demon.DemonShot or Content.Projectiles.Demon.DemonBomb or Content.Projectiles.Demon.DemonSpike) {
				modifiers.FinalDamage *= DemonDamageTaken;
			}
		}

		public override bool FreeDodge(Player.HurtInfo info) {
			if (Player.HasBuff<AfterimageBuff>() && Main.rand.NextBool(4)) {
				Player.SetImmuneTimeForAllTypes(Player.longInvince ? 90 : 60);
				for (int i = 0; i < 20; i++) {
					Dust dust = Dust.NewDustDirect(Player.position, Player.width, Player.height, BreathingStyles.DustOf(Style), 0f, 0f, 100, default, 1.5f);
					dust.noGravity = true;
				}
				CombatText.NewText(Player.Hitbox, Color.White, "Afterimage!");
				return true;
			}
			return false;
		}

		// ---- Breath, techniques and keys ----------------------------------------------------------------------------

		public override void PostUpdate() {
			if (lifeStealTimer > 0) {
				lifeStealTimer--;
			}
			if (Player.whoAmI != Main.myPlayer) {
				return;
			}
			if (TechniqueCooldown > 0) {
				TechniqueCooldown--;
			}
			if (MarkCooldown > 0) {
				MarkCooldown--;
			}
			if (BreathDelay > 0) {
				BreathDelay--;
			}
			else {
				Breath = Math.Min(BreathMax, Breath + BreathRegen / 60f);
			}
			Breath = Math.Min(Breath, BreathMax);

			CheckBossCredit();

			if (!hintShown && HoldingBlade && Style == BreathingStyles.None) {
				hintShown = true;
				Main.NewText("Choose a breathing style in the Slayer menu (default key: K) to use techniques with your Nichirin Blade.", 150, 200, 255);
			}
		}

		public override void ProcessTriggers(TriggersSet triggersSet) {
			if (SlayerKeybinds.OpenMenu.JustPressed) {
				SlayerUISystem.ToggleStats();
			}
			if (SlayerKeybinds.NextForm.JustPressed) {
				CycleForm(1);
			}
			if (SlayerKeybinds.PreviousForm.JustPressed) {
				CycleForm(-1);
			}
			if (SlayerKeybinds.Technique.JustPressed) {
				TryUseTechnique();
			}
			if (SlayerKeybinds.Mark.JustPressed) {
				TryAwakenMark();
			}
			if (SlayerKeybinds.DemonForm.JustPressed) {
				ToggleDemonForm();
			}
		}

		private void Fail(string message) {
			CombatText.NewText(Player.Hitbox, new Color(200, 200, 200), message);
		}

		public bool InDemonForm => Player.HasBuff<DemonFormBuff>();

		public void ToggleDemonForm() {
			if (!IsDemon || Player.dead) {
				return;
			}
			if (InDemonForm) {
				Player.ClearBuff(ModContent.BuffType<DemonFormBuff>());
				CombatText.NewText(Player.Hitbox, new Color(220, 200, 200), "Human again");
				return;
			}
			if (Main.dayTime) {
				Fail("Demon form only works at night");
				return;
			}
			Player.AddBuff(ModContent.BuffType<DemonFormBuff>(), 2);
			SoundEngine.PlaySound(SoundID.NPCDeath10 with { Pitch = 0.3f }, Player.Center);
			for (int i = 0; i < 30; i++) {
				Dust.NewDust(Player.position, Player.width, Player.height, DustID.Blood, 0f, -2f, 0, default, 1.5f);
			}
			CombatText.NewText(Player.Hitbox, new Color(220, 30, 50), "Demon form!", true);
		}

		// In demon form, without a Nichirin Blade in hand, the technique key fires a Blood Demon Art.
		private void BloodDemonArt() {
			const int cost = 15;
			if (TechniqueCooldown > 0) {
				return;
			}
			if (Breath < cost) {
				Fail("Out of breath!");
				return;
			}
			Breath -= cost;
			BreathDelay = 30;
			TechniqueCooldown = TechniqueCooldownMax = 20;
			int damage = (int)(Player.GetTotalDamage(DamageClass.Melee).ApplyTo(20 + Level * 2));
			Vector2 aim = (Main.MouseWorld - Player.Center).SafeNormalize(Vector2.UnitX * Player.direction);
			for (int i = -1; i <= 1; i++) {
				Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, aim.RotatedBy(i * 0.15f) * 16f,
					ModContent.ProjectileType<Content.Projectiles.BloodArtShot>(), damage, 4f, Player.whoAmI);
			}
			SoundEngine.PlaySound(SoundID.Item17, Player.Center);
		}

		public void TryUseTechnique() {
			if (Player.dead || Player.CCed || Player.noItems) {
				return;
			}
			if (InDemonForm && !HoldingBlade) {
				BloodDemonArt();
				return;
			}
			if (!HoldingBlade) {
				Fail("Hold a Nichirin Blade");
				return;
			}
			if (CurrentStyle == null) {
				Fail("No breathing style (press K)");
				return;
			}
			if (!FormUnlocked(Form)) {
				CycleForm(-1);
				return;
			}
			if (TechniqueCooldown > 0) {
				return;
			}
			Technique form = CurrentForm;
			if (Breath < form.Cost) {
				Fail("Out of breath!");
				return;
			}

			Item blade = Player.HeldItem;
			int damage = (int)(Player.GetWeaponDamage(blade) * form.Damage * TechniqueMultiplier());
			float knockback = Player.GetWeaponKnockback(blade);
			Breath -= form.Cost;
			BreathDelay = 45;
			TechniqueCooldown = TechniqueCooldownMax = Math.Max(10, form.Cooldown);

			TechniqueRunner.Execute(Player, Style, form, damage, knockback);
			CombatText.NewText(Player.Hitbox, CurrentStyle.Color, form.Name);
		}

		public void TryAwakenMark() {
			if (Rank < SlayerRanks.Hashira) {
				Fail("Only a Hashira can awaken the Mark");
				return;
			}
			if (MarkCooldown > 0) {
				Fail($"The Mark returns in {MarkCooldown / 60}s");
				return;
			}
			Player.AddBuff(ModContent.BuffType<DemonSlayerMarkBuff>(), DemonSlayerMarkBuff.Duration);
			MarkCooldown = DemonSlayerMarkBuff.Cooldown;
			Breath = BreathMax;
			SoundEngine.PlaySound(SoundID.Item119, Player.Center);
			Main.NewText("Your Demon Slayer Mark awakens!", 255, 90, 90);
		}

		// ---- Saving -----------------------------------------------------------------------------------------------

		public override void SaveData(TagCompound tag) {
			tag["level"] = Level;
			tag["xp"] = XP;
			tag["stats"] = Stats.ToList();
			tag["scrolls"] = ScrollsUsed;
			tag["style"] = Style;
			tag["form"] = Form;
			tag["defeated"] = Defeated.ToList();
			tag["isDemon"] = IsDemon;
		}

		public override void LoadData(TagCompound tag) {
			Level = Math.Clamp(tag.ContainsKey("level") ? tag.GetInt("level") : 1, 1, MaxLevel);
			XP = tag.GetLong("xp");
			IList<int> stats = tag.GetList<int>("stats");
			for (int i = 0; i < Stats.Length; i++) {
				Stats[i] = i < stats.Count ? Math.Clamp(stats[i], 0, StatCap) : 0;
			}
			ScrollsUsed = Math.Clamp(tag.GetInt("scrolls"), 0, MaxScrolls);
			if (Stats.Sum() > PointsTotal) {
				ResetPoints();
			}
			Style = tag.ContainsKey("style") ? tag.GetInt("style") : BreathingStyles.None;
			if (!BreathingStyles.Valid(Style)) {
				Style = BreathingStyles.None;
			}
			Form = tag.GetInt("form");
			IsDemon = tag.GetBool("isDemon");
			Defeated.Clear();
			foreach (string key in tag.GetList<string>("defeated")) {
				Defeated.Add(key);
			}
		}
	}
}
