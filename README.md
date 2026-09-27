# Demon Slayer — a tModLoader mod for Terraria

An unofficial, fan-made Demon Slayer (Kimetsu no Yaiba) mod for **tModLoader 1.4.4** (built and load-tested against tModLoader v2025.08).

## Installing from source

1. Clone this repository into your ModSources folder **in a folder named `DemonSlayerMod`** (the folder name must match the mod's internal name):
   `Documents/My Games/Terraria/tModLoader/ModSources/DemonSlayerMod` (Windows) or `~/.local/share/Terraria/tModLoader/ModSources/DemonSlayerMod` (Linux).
2. In tModLoader: Workshop → Develop Mods → **Build + Reload** next to Demon Slayer.

## Getting started

1. Craft a **Swordsmith's Forge** (30 Stone Block, 8 Iron/Lead Bar, 3 Torch @ Anvil) and place it.
2. At the forge, craft a **Nichirin Blade** (12 Iron/Lead Bar, 5 Wood).
3. Press **K** to open the Slayer menu and choose a breathing style.
4. Press **F** to use the selected technique, and **G** / **V** to switch forms.
5. Right-click the forge to customise your blade.

All keys can be rebound in Settings → Controls → Mod Controls.

## Sword customisation (Swordsmith's Forge)

Right-click the forge with a Nichirin Blade in your hand (or your inventory). Every change shows live on a big preview, and the blade is drawn from its parts in your inventory, in the world and while swinging.

| Category | Options | Effect |
| --- | --- | --- |
| **Steel** | 11 (Tamahagane → Dawnbreaker Steel) | Base damage from 16 to 225. Each new steel unlocks by defeating a demon boss. |
| **Blade shape** | 12: Katana, Nodachi, Wakizashi, Cleaver, Stinger, Ribbon Blade, Serpent Blade, Jagged Blade, Greatblade, Tachi, Crescent Blade, Chisel Blade | Damage, speed, reach, crit, knockback, armor penetration |
| **Nichirin colour** | 20: Black, Blue, Red, Yellow, Green, Grey, Pink, Purple, Indigo, Lavender, White, Orange, Gold, Crimson, Teal, Silver, Rose, Amber, Jade, Obsidian | A bonus or on-hit debuff each, plus +20% technique damage when it matches your breathing style |
| **Guard (tsuba)** | 15: Round, Square, Flame, Hexagonal, Flower, Butterfly, Wheel, Clover, Serpent, Crescent, None, Star, Sun, Cross, Wave | Defense, crit, speed, Breath and more |
| **Guard finish** | 12 | Cosmetic |
| **Hilt wrap** | 16 | Cosmetic |
| **Engraving** | 15: e.g. "Destroy All Demons" (+15% vs demons), Sunrise, Moonlit, Deep Breath, Butterfly Poison, Iron Will | Bonus |
| **Swing trail** | 14: blade colour, style colour, flames, water, petals, lightning, shadow, frost, rainbow, sparks, blood, cursed flame, moonlight | Cosmetic |

There is also a "Randomise looks" button (cosmetics only) and a reset button.

## Stats and ranks

Earn XP by killing enemies (demons give 5x as much), plus a big bonus the first time each demon boss falls while you're in the world.

- **Ranks**: Mizunoto → Mizunoe → Kanoto → Kanoe → Tsuchinoto → Tsuchinoe → Hinoto → Hinoe → Kinoto → Kinoe → **Hashira** (every 6 levels, Hashira at level 61, max level 100).
- **2 stat points per level** (and +1 per Training Scroll, dropped by bosses, max 20). Up to 50 per stat:
  - Strength: +1.5% melee damage
  - Agility: +1% movement speed, +0.8% melee speed
  - Endurance: +3 max life, +1 defense per 2 points
  - Breath: +4 max Breath, +2% Breath regeneration
  - Focus: +0.5% melee crit, +1% technique damage
- Each rank gives +1% melee damage. At Tsuchinoto you gain **Total Concentration: Constant** (+50% Breath regen). Hashira get +10% damage, +10 defense and the **Demon Slayer Mark** (press X: +25% damage and more for 20s, 2 minute cooldown).
- A **Scroll of Forgetting** (5 Demon Blood + Book @ Bookcase) refunds your points.

## Breathing styles

Techniques cost **Breath** (the bar under your character) and need a Nichirin Blade in hand. Their damage scales with the blade. Forms unlock as your rank rises.

| Style | Forms | Unlock |
| --- | --- | --- |
| Water | 11 | Start |
| Flame | 6 | Start |
| Thunder | 9 | Start |
| Wind | 9 | Start |
| Stone | 5 | Start |
| Flower | 5 | Kanoto |
| Beast | 10 | Kanoto |
| Mist | 7 | Kanoe |
| Serpent | 5 | Kanoe |
| Love | 5 | Kanoe |
| Insect | 4 | Tsuchinoto |
| Sound | 3 | Tsuchinoto |
| Sun (Hinokami Kagura) | 13 | Tsuchinoe + Gyutaro & Daki defeated |
| Moon | 8 | Hinoto + Kokushibo defeated |

Techniques include flying slashes, homing crescents, huge sweeps and spins, dashes and chained multi-dashes, slash storms and whirlpools, falling rain, ground eruptions, thrusts, leaping slams, radial bursts, and buffs (Dead Calm, heightened senses, afterimage dodging). Some styles add their own debuff (Flame burns, Serpent and Insect poison, Mist confuses...), and Sound Breathing's strikes explode.

Form names follow the English releases as closely as I could manage; some translations differ.

## Demons

Demons come out at night. They **burn in sunlight**, **regenerate** when left alone for 3 seconds, and take **40% less damage from anything that isn't a Nichirin Blade or a breathing technique** (each of these can be turned off in the mod config). They drop **Demon Blood**.

- Pre-Hardmode: Lesser Demon, Horned Demon, Swamp Demon, Temari Demon, Arrow Demon, Drum Demon (caves), Spider Demon (forest), Tongue Demon (caves)
- Hardmode: Blood Brute, Biwa Demon, Ice Demon (snow), Sickle Demon, Flesh Crawler (caves), Thunder Demon
- Biome demons: Sand Demon (desert), Mantis Demon (jungle), Drowned Demon (ocean), Hellfire Demon (Underworld, any time), Hollow Demon (Corruption/Crimson), Frostbitten Demon (snow, pre-Hardmode), Mirror Demon (Hardmode Hallow)

### Bosses (summon at night; they flee at dawn)

| Boss | Summon item | Recipe | Available | Life |
| --- | --- | --- | --- | --- |
| Hand Demon | Final Selection Tag | 5 Demon Blood, 10 Wood @ Work Bench | Any time | 3,200 |
| Mukago (Lower Moon 4) | Wooden Charm | 8 Demon Blood, 2 Lens @ Anvil | After Eye of Cthulhu | 4,200 |
| Rui (Lower Moon 5) | Spider Silk Doll | 10 Demon Blood, 20 Cobweb @ Anvil | After Eye of Cthulhu | 6,000 |
| Kyogai (former Lower Moon 6) | Tsuzumi Drum | 12 Demon Blood, 5 Leather @ Anvil | After Eater of Worlds / Brain of Cthulhu | 7,000 |
| Enmu (Lower Moon 1) | Infinity Train Ticket | 15 Demon Blood, 5 Silk @ Anvil | After Skeletron | 9,000 |
| Gyutaro & Daki (Upper Moon 6) | Entertainment District Obi | 20 Demon Blood, 10 Silk, 5 Soul of Night @ Mythril Anvil | Hardmode | 34,000 |
| Gyokko (Upper Moon 5) | Cracked Vase | 25 Demon Blood, 20 Clay, 5 Hallowed Bar | After any mech boss | 44,000 |
| Hantengu (Upper Moon 4) | Leaf Fan | 25 Demon Blood, 5 each Soul of Might/Sight/Fright | After all mech bosses | 52,000 |
| Kaigaku (new Upper Moon 6) | Cracked Magatama | 25 Demon Blood, 8 Soul of Light, 8 Soul of Night @ Mythril Anvil | After all mech bosses | 58,000 |
| Akaza (Upper Moon 3) | Martial Artist's Beads | 30 Demon Blood, 10 Chlorophyte Bar | After Plantera | 72,000 |
| Doma (Upper Moon 2) | Golden Fan | 30 Demon Blood, 5 Beetle Husk, 20 Ice Block | After Golem | 86,000 |
| Nakime (new Upper Moon 4) | Biwa String | 30 Demon Blood, 10 Silk, 5 Beetle Husk @ Mythril Anvil | After Golem | 95,000 |
| Kokushibo (Upper Moon 1) | Broken Flute | 35 Demon Blood, 3 of each Lunar Fragment @ Ancient Manipulator | After Lunatic Cultist | 118,000 |
| Muzan Kibutsuji | Blue Spider Lily | 50 Demon Blood, 10 Luminite Bar @ Ancient Manipulator | After Moon Lord | 260,000 |

**Yōtō:** every demon boss has a 1 in 25 chance (Muzan: 1 in 8) to drop a **Yōtō**, a demon-forged katana with **2000 base damage**. It is customised at the Swordsmith's Forge exactly like a Nichirin Blade (the blade shape still multiplies its damage; the steel choice doesn't matter), breathing techniques scale from it, and demons take full damage from it.

Life values are for Classic mode (Expert and Master scale them up as usual). All 14 bosses mix 12 attack patterns (aimed fans, rings, spirals, charges, minions, rain, ground spikes, teleports, fast barrages, warning-circle bombs, splitting crescent moons and sweeping walls). Every boss speeds up below half health, and the Upper Moons and Muzan enrage again below a quarter.

## Becoming a demon

A **Mysterious Gentleman** often appears on the surface at night. He is Muzan Kibutsuji in disguise: he can't be hurt, and he vanishes at dawn. Talk to him and choose **Accept his blood** to become a demon (permanent for that character).

- Press **Z** (Toggle Demon Form) **at night** to transform, and again to turn back. You can't transform during the day, and at dawn you're turned back automatically.
- Demon form (it's meant to be overpowered): +60% damage (+4% per Slayer rank), +15% crit, +20 armor penetration, +40% melee speed, +50% movement speed, +30 defense (+3 per rank), 20% damage reduction, +100 max life (+10 per rank), very fast regeneration (+15 HP/s, more with rank), huge jumps, no fall damage or knockback, night vision, and immunity to most debuffs (poison, venom, bleeding, fire, frost, slow, confusion, weakness, darkness, webs, ichor). Every hit heals you for 5% of the damage dealt.
- With no Nichirin Blade in hand, the technique key (F) fires a **Blood Demon Art**: five lashes of blood that pierce 8 enemies and walls, make enemies bleed, and sometimes heal you (10 Breath).
- You can still use your Nichirin Blade and breathing techniques in demon form.

## Gear

| Item | Effect | Recipe |
| --- | --- | --- |
| Corps Uniform (accessory) | +6 defense, +20 max Breath | 12 Silk, 5 Demon Blood @ Loom |
| Two-Pattern Haori | +25% Breath regen, +6% melee speed | 10 Silk, 10 Demon Blood, Water Candle @ Loom |
| Flame Haori | +8% melee damage, immune to On Fire and Burning | 10 Silk, 10 Demon Blood, 5 Hellstone Bar @ Loom |
| Butterfly Haori | +10% move speed, +8% melee crit | 10 Silk, 10 Demon Blood, 8 Jungle Spores @ Loom |
| Checkered Haori | +10% technique damage | 10 Silk, 10 Demon Blood, 3 Emerald @ Loom |
| Hanafuda Earrings | +25% Breath regen, +20% Sun Breathing technique damage | 20 Demon Blood, 5 Soul of Light, Sunflower @ Swordsmith's Forge |
| Wisteria Charm | Demons and their attacks deal 20% less damage to you | 8 Demon Blood, 3 Daybloom, 3 Amethyst @ Anvil |
| Wisteria Poison (flask, 10 min) | Blade and technique hits inflict Venom, +10% damage to demons | Bottled Water, 2 Demon Blood, Vile or Vicious Mushroom @ Bottle |
| Butterfly Mansion Medicine | Heals 120 life | 2 Lesser Healing Potion, Demon Blood, Daybloom @ Bottle (makes 2) |
| Crow Whistle | Kasugai Crow pet; it caws when demons are near | 5 Feather, 3 Demon Blood @ Swordsmith's Forge |

## Art

All sprites are simple placeholders drawn by `tools/generate_sprites.py` (`pip install pillow`, then run `python3 tools/generate_sprites.py` from the repo root). The Nichirin Blade is drawn from layered parts in `Common/Swords/Parts/`: greyscale blades and guards are tinted in game, so every colour combination works automatically. Replace any PNG with hand-made art of the same size.

## Known limitations

- XP from a kill goes to the player who lands the killing blow.
- In multiplayer, Whirlpool-style techniques pull enemies on the server only, so the pull can look slightly delayed.

This is an unofficial fan project and is not affiliated with Koyoharu Gotouge, Shueisha or ufotable.
