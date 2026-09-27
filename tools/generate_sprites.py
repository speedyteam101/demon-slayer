"""Generates the placeholder pixel-art sprites for the Demon Slayer mod.

Run from the repository root:  python3 tools/generate_sprites.py
Requires Pillow (pip install pillow). Most sprites are drawn at half size and
scaled 2x with nearest-neighbour, which matches Terraria's pixel style.
Replace any PNG with hand-made art at the same size whenever you like.
"""
import math
import os

from PIL import Image, ImageDraw

CLEAR = (0, 0, 0, 0)
BLACK = (25, 20, 25, 255)
WHITE = (250, 250, 250, 255)


def canvas(w, h):
    img = Image.new("RGBA", (w, h), CLEAR)
    return img, ImageDraw.Draw(img)


def save(img, path, scale=2):
    if os.path.dirname(path):
        os.makedirs(os.path.dirname(path), exist_ok=True)
    if scale != 1:
        img = img.resize((img.width * scale, img.height * scale), Image.NEAREST)
    img.save(path)
    print("wrote", path, img.width, "x", img.height)


def grey(v, a=255):
    return (v, v, v, a)


def shade(c, f):
    return tuple(max(0, min(255, int(x * f))) for x in c[:3]) + (c[3] if len(c) > 3 else 255,)


# ---------------------------------------------------------------------------------------------------------------------
# Nichirin Blade parts. All share a 32x32 half-size canvas; the sword runs from the pommel (bottom-left) to the
# tip (top-right). Positions are given as (t, w): t along the sword from the pommel, w across it (+w is the edge side).
# ---------------------------------------------------------------------------------------------------------------------

ORIGIN = (1.6, 30.4)
U = (1 / math.sqrt(2), -1 / math.sqrt(2))   # toward the tip
V = (1 / math.sqrt(2), 1 / math.sqrt(2))    # toward the cutting edge
GUARD_T = 11.5
PARTS = "Common/Swords/Parts/"


def P(t, w):
    return (ORIGIN[0] + U[0] * t + V[0] * w, ORIGIN[1] + U[1] * t + V[1] * w)


def poly(d, pts, fill):
    d.polygon([P(t, w) for t, w in pts], fill=fill)


# Each shape: list of (t, spine_w, edge_w) samples from the guard to the tip.
def profile(shape):
    s = []
    if shape == "Katana":
        for t in range(13, 39):
            s.append((t, -1.1, 1.2 + 0.2 * math.sin((t - 13) / 26 * math.pi)))
        s.append((41, -0.2, 0.2))
    elif shape == "Nodachi":
        for t in range(13, 41):
            s.append((t, -0.9, 1.0 + 0.2 * math.sin((t - 13) / 28 * math.pi)))
        s.append((42.5, -0.1, 0.1))
    elif shape == "Wakizashi":
        for t in range(13, 30):
            s.append((t, -1.2, 1.3))
        s.append((32.5, -0.2, 0.2))
    elif shape == "Cleaver":
        for t in range(13, 36):
            s.append((t, -1.4, 2.0 + (t - 13) * 0.08))
        s.append((37, -1.2, 3.5))
        s.append((38.5, -0.5, 1.0))
    elif shape == "Stinger":
        for t in range(13, 38):
            s.append((t, -0.6, 0.6))
        s.append((42, 0, 0.1))
    elif shape == "Ribbon":
        for t in range(13, 41):
            wob = 0.8 * math.sin(t * 0.7)
            s.append((t, -0.7 + wob, 0.7 + wob))
        s.append((42.5, 0, 0.1))
    elif shape == "Serpent":
        for t in range(13, 39):
            wob = 1.2 * math.sin(t * 0.45)
            s.append((t, -1.0 + wob, 1.1 + wob))
        s.append((41, 0.5, 0.7))
    elif shape == "Jagged":
        for t in range(13, 38):
            tooth = 0.9 if t % 3 == 0 else 0.0
            s.append((t, -1.2, 1.3 + tooth))
        s.append((40.5, -0.2, 0.3))
    elif shape == "Greatblade":
        for t in range(13, 37):
            s.append((t, -2.2, 2.4))
        s.append((38.5, -1.6, 1.8))
        s.append((40.5, 0, 0.2))
    elif shape == "Tachi":
        for t in range(13, 40):
            bend = 0.035 * (t - 13) ** 1.5 / 5    # curves toward the spine
            s.append((t, -1.0 - bend, 1.1 - bend))
        s.append((42, -1.6, -1.2))
    elif shape == "Crescent":
        for t in range(13, 37):
            bend = 2.2 * math.sin((t - 13) / 24 * math.pi)
            s.append((t, -1.1 + bend, 1.2 + bend))
        s.append((39.5, -0.2, 0.3))
    elif shape == "Chisel":
        for t in range(13, 35):
            s.append((t, -1.7, 1.8))
        s.append((36, -1.7, 1.8))
        s.append((36.5, -1.7, -0.5))
    return s


SHAPES = ["Katana", "Nodachi", "Wakizashi", "Cleaver", "Stinger", "Ribbon", "Serpent", "Jagged", "Greatblade",
          "Tachi", "Crescent", "Chisel"]


def blade_layers(shape):
    s = profile(shape)
    # Blade body (greyscale, tinted in game), lighter along the spine.
    img, d = canvas(32, 32)
    outline = [(t, sw) for t, sw, ew in s] + [(t, ew) for t, sw, ew in reversed(s)]
    poly(d, outline, grey(175))
    spine = [(t, sw) for t, sw, ew in s] + [(t, (sw + ew) / 2) for t, sw, ew in reversed(s)]
    poly(d, spine, grey(235))
    save(img, PARTS + "Blade_" + shape + ".png")

    # Silver cutting edge (drawn untinted) plus the shinogi line.
    img, d = canvas(32, 32)
    edge = [P(t, ew - 0.2) for t, sw, ew in s]
    d.line(edge, fill=(225, 230, 240, 255), width=1)
    tip = s[-1]
    d.point(P(tip[0], (tip[1] + tip[2]) / 2), fill=WHITE)
    save(img, PARTS + "Edge_" + shape + ".png")


def hilt():
    img, d = canvas(32, 32)
    poly(d, [(1, -1.6), (GUARD_T, -1.6), (GUARD_T, 1.6), (1, 1.6)], grey(215))
    poly(d, [(1, 0.4), (GUARD_T, 0.4), (GUARD_T, 1.6), (1, 1.6)], grey(160))
    save(img, PARTS + "Hilt.png")

    img, d = canvas(32, 32)
    for t in (2.6, 5.0, 7.4, 9.8):
        d.point(P(t, 0), fill=(240, 235, 220, 255))
    poly(d, [(-0.4, -1.8), (1.2, -1.8), (1.2, 1.8), (-0.4, 1.8)], (70, 65, 75, 255))  # kashira (pommel cap)
    save(img, PARTS + "HiltDiamonds.png")


def guard(name):
    img, d = canvas(32, 32)
    g = GUARD_T + 0.6
    light, mid, dark = grey(235), grey(190), grey(120)
    if name == "Round":
        pts = [(g + 1.6 * math.cos(a), 4.2 * math.sin(a)) for a in [i * math.pi / 8 for i in range(16)]]
        poly(d, pts, mid)
        poly(d, [(g - 0.3, -3), (g + 0.5, -3), (g + 0.5, 3), (g - 0.3, 3)], light)
    elif name == "Square":
        poly(d, [(g - 1.6, -3.8), (g + 1.6, -3.8), (g + 1.6, 3.8), (g - 1.6, 3.8)], mid)
        poly(d, [(g - 1.6, -3.8), (g + 1.6, -3.8), (g + 1.6, -2.6), (g - 1.6, -2.6)], light)
        poly(d, [(g - 0.4, -0.6), (g + 0.4, -0.6), (g + 0.4, 0.6), (g - 0.4, 0.6)], dark)
    elif name == "Flame":
        pts = []
        for i in range(16):
            a = i * math.pi / 8
            r = 4.6 if i % 2 == 0 else 3.0
            pts.append((g + 0.4 * r * math.cos(a), r * math.sin(a)))
        poly(d, pts, mid)
        poly(d, [(g - 0.4, -2), (g + 0.4, -2), (g + 0.4, 2), (g - 0.4, 2)], light)
    elif name == "Hexagon":
        pts = [(g + 1.8 * math.cos(a), 4.0 * math.sin(a)) for a in [i * math.pi / 3 for i in range(6)]]
        poly(d, pts, mid)
        poly(d, [(g, -2.5), (g + 0.8, -2.5), (g + 0.8, 2.5), (g, 2.5)], light)
    elif name == "Flower":
        for c in (-2.6, 0, 2.6):
            pts = [(g + 1.3 * math.cos(a), c + 1.5 * math.sin(a)) for a in [i * math.pi / 6 for i in range(12)]]
            poly(d, pts, mid)
        poly(d, [(g - 0.5, -0.5), (g + 0.5, -0.5), (g + 0.5, 0.5), (g - 0.5, 0.5)], light)
    elif name == "Butterfly":
        poly(d, [(g, 0), (g - 2.2, -4.4), (g + 2.2, -3.6)], mid)
        poly(d, [(g, 0), (g - 2.2, 4.4), (g + 2.2, 3.6)], mid)
        poly(d, [(g - 0.3, -1), (g + 0.3, -1), (g + 0.3, 1), (g - 0.3, 1)], dark)
    elif name == "Wheel":
        pts = [(g + 1.6 * math.cos(a), 4.2 * math.sin(a)) for a in [i * math.pi / 8 for i in range(16)]]
        poly(d, pts, mid)
        inner = [(g + 0.9 * math.cos(a), 2.6 * math.sin(a)) for a in [i * math.pi / 8 for i in range(16)]]
        poly(d, inner, dark)
        poly(d, [(g - 0.3, -3.6), (g + 0.3, -3.6), (g + 0.3, 3.6), (g - 0.3, 3.6)], light)
    elif name == "Clover":
        for c, dt in ((-2.4, 0), (2.4, 0), (0, 1.4)):
            pts = [(g + dt + 1.2 * math.cos(a), c + 1.6 * math.sin(a)) for a in [i * math.pi / 6 for i in range(12)]]
            poly(d, pts, mid)
        poly(d, [(g - 0.4, -0.4), (g + 0.4, -0.4), (g + 0.4, 0.4), (g - 0.4, 0.4)], light)
    elif name == "Serpent":
        pts = [(g + 1.3 * math.sin(w * 0.9), w) for w in [x * 0.5 for x in range(-9, 10)]]
        d.line([P(t, w) for t, w in pts], fill=mid, width=2)
        d.point(P(g + 1.3 * math.sin(4.5 * 0.9), 4.5), fill=(200, 40, 40, 255))
    elif name == "Crescent":
        outer = [(g + 2.2 * math.cos(a), 4.4 * math.sin(a)) for a in [math.pi / 2 + i * math.pi / 10 for i in range(11)]]
        inner = [(g - 0.2 + 1.2 * math.cos(a), 3.0 * math.sin(a)) for a in [math.pi * 1.5 - i * math.pi / 10 for i in range(11)]]
        poly(d, outer + inner, mid)
        poly(d, [(g - 0.3, -1.2), (g + 0.3, -1.2), (g + 0.3, 1.2), (g - 0.3, 1.2)], light)
    elif name == "Star":
        pts = []
        for i in range(10):
            a = i * math.pi / 5
            r = 4.6 if i % 2 == 0 else 2.0
            pts.append((g + 0.45 * r * math.cos(a), r * math.sin(a)))
        poly(d, pts, mid)
        poly(d, [(g - 0.3, -0.8), (g + 0.3, -0.8), (g + 0.3, 0.8), (g - 0.3, 0.8)], light)
    elif name == "Sun":
        pts = [(g + 1.2 * math.cos(a), 3.0 * math.sin(a)) for a in [i * math.pi / 8 for i in range(16)]]
        poly(d, pts, mid)
        for i in range(8):
            a = i * math.pi / 4
            d.line([P(g + 1.2 * math.cos(a), 3.0 * math.sin(a)), P(g + 1.9 * math.cos(a), 4.6 * math.sin(a))], fill=light)
    elif name == "Cross":
        poly(d, [(g - 0.8, -4.4), (g + 0.8, -4.4), (g + 0.8, 4.4), (g - 0.8, 4.4)], mid)
        poly(d, [(g - 2.2, -1.0), (g + 2.2, -1.0), (g + 2.2, 1.0), (g - 2.2, 1.0)], mid)
        poly(d, [(g - 0.3, -4.2), (g + 0.3, -4.2), (g + 0.3, 4.2), (g - 0.3, 4.2)], light)
    elif name == "Wave":
        pts = [(g + 1.0 * math.sin(w * 1.6), w) for w in [x * 0.4 for x in range(-11, 12)]]
        d.line([P(t, w) for t, w in pts], fill=mid, width=2)
        pts2 = [(g + 0.9 + 1.0 * math.sin(w * 1.6), w) for w in [x * 0.4 for x in range(-9, 10)]]
        d.line([P(t, w) for t, w in pts2], fill=light, width=1)
    save(img, PARTS + "Guard_" + name + ".png")


GUARDS = ["Round", "Square", "Flame", "Hexagon", "Flower", "Butterfly", "Wheel", "Clover", "Serpent", "Crescent",
          "Star", "Sun", "Cross", "Wave"]


def tint_layer(path, color):
    img = Image.open(path).convert("RGBA")
    px = img.load()
    for y in range(img.height):
        for x in range(img.width):
            r, g, b, a = px[x, y]
            if a:
                px[x, y] = (r * color[0] // 255, g * color[1] // 255, b * color[2] // 255, a)
    return img


def nichirin_item():
    # The fallback item texture: a black katana with a square black guard and navy wrap (the default build is
    # Tamahagane katana, black blade, round tsuba, black iron, navy wrap).
    base = Image.new("RGBA", (64, 64), CLEAR)
    for path, color in [(PARTS + "Blade_Katana.png", (45, 45, 55)), (PARTS + "Edge_Katana.png", (255, 255, 255)),
                        (PARTS + "Hilt.png", (40, 55, 120)), (PARTS + "HiltDiamonds.png", (255, 255, 255)),
                        (PARTS + "Guard_Round.png", (60, 60, 70))]:
        base = Image.alpha_composite(base, tint_layer(path, color))
    base.save("Content/Items/NichirinBlade.png")
    print("wrote Content/Items/NichirinBlade.png 64 x 64")


# ---------------------------------------------------------------------------------------------------------------------
# Items
# ---------------------------------------------------------------------------------------------------------------------

def demon_blood():
    img, d = canvas(10, 12)
    d.rectangle([4, 0, 5, 1], fill=(150, 110, 70, 255))                 # cork
    d.rectangle([3, 2, 6, 3], fill=(200, 210, 220, 200))                # neck
    d.ellipse([1, 3, 8, 11], fill=(200, 210, 220, 200))
    d.ellipse([2, 5, 7, 10], fill=(170, 15, 35, 255))
    d.point((3, 6), fill=(250, 120, 120, 255))
    save(img, "Content/Items/DemonBlood.png")


def scroll(path, paper, ribbon, mark):
    img, d = canvas(12, 12)
    d.rectangle([1, 2, 10, 9], fill=paper)
    d.rectangle([0, 1, 1, 10], fill=(120, 80, 40, 255))
    d.rectangle([10, 1, 11, 10], fill=(120, 80, 40, 255))
    d.line([3, 4, 8, 4], fill=mark)
    d.line([3, 6, 7, 6], fill=mark)
    d.rectangle([5, 8, 6, 11], fill=ribbon)
    save(img, path)


def forge_item():
    img, d = canvas(16, 12)
    d.rectangle([1, 4, 14, 11], fill=(120, 115, 110, 255))
    d.rectangle([1, 4, 14, 5], fill=(160, 155, 150, 255))
    d.rectangle([4, 6, 11, 10], fill=(40, 25, 20, 255))
    d.rectangle([5, 7, 10, 10], fill=(250, 140, 30, 255))
    d.point((7, 7), fill=(255, 230, 120, 255))
    d.rectangle([10, 0, 12, 4], fill=(90, 85, 80, 255))  # chimney
    save(img, "Content/Items/SwordsmithForge.png")


def forge_tile():
    # 3x2 tiles of 16px with 2px padding: 54x36. Drawn at 24x16 half size, then cut into the padded sheet.
    img, d = canvas(24, 16)
    d.rectangle([0, 5, 23, 15], fill=(115, 110, 105, 255))
    d.rectangle([0, 5, 23, 6], fill=(155, 150, 145, 255))
    for x in range(0, 24, 4):
        d.line([x, 7, x, 15], fill=(95, 90, 85, 255))
    d.rectangle([7, 8, 16, 15], fill=(40, 25, 20, 255))
    d.rectangle([8, 10, 15, 15], fill=(240, 120, 25, 255))
    d.rectangle([10, 12, 13, 15], fill=(255, 220, 110, 255))
    d.rectangle([17, 0, 20, 5], fill=(90, 85, 80, 255))      # chimney
    d.rectangle([1, 2, 6, 4], fill=(70, 70, 80, 255))        # anvil
    d.rectangle([3, 4, 4, 5], fill=(70, 70, 80, 255))
    full = img.resize((48, 32), Image.NEAREST)
    sheet = Image.new("RGBA", (54, 36), CLEAR)
    for col in range(3):
        for row in range(2):
            tile = full.crop((col * 16, row * 16, col * 16 + 16, row * 16 + 16))
            sheet.paste(tile, (col * 18, row * 18))
    os.makedirs("Content/Tiles", exist_ok=True)
    sheet.save("Content/Tiles/SwordsmithForgeTile.png")
    print("wrote Content/Tiles/SwordsmithForgeTile.png 54 x 36")


def summon_items():
    out = "Content/Items/"
    # Final Selection Tag: a wooden tag on a cord.
    img, d = canvas(12, 14)
    d.line([6, 0, 6, 3], fill=(200, 60, 60, 255))
    d.rectangle([3, 3, 8, 13], fill=(190, 150, 100, 255))
    d.rectangle([3, 3, 8, 4], fill=(220, 190, 140, 255))
    d.line([5, 6, 6, 6], fill=BLACK)
    d.line([5, 8, 6, 9], fill=BLACK)
    d.line([5, 11, 6, 11], fill=BLACK)
    save(img, out + "SelectionTag.png")
    # Spider Silk Doll
    img, d = canvas(12, 14)
    d.ellipse([3, 0, 8, 5], fill=(240, 240, 245, 255))
    d.point((4, 2), fill=(200, 30, 30, 255)); d.point((7, 2), fill=(200, 30, 30, 255))
    d.polygon([(2, 13), (6, 5), (9, 13)], fill=(235, 235, 240, 255))
    for x in range(0, 12, 3):
        d.line([x, 13, 6, 6], fill=(200, 200, 210, 160))
    save(img, out + "SpiderSilkDoll.png")
    # Train Ticket
    img, d = canvas(14, 10)
    d.rectangle([0, 1, 13, 8], fill=(230, 210, 170, 255))
    d.rectangle([0, 1, 13, 2], fill=(150, 40, 60, 255))
    d.line([2, 5, 8, 5], fill=BLACK)
    d.ellipse([9, 4, 12, 7], fill=(200, 60, 200, 255))
    save(img, out + "TrainTicket.png")
    # Entertainment District Obi
    img, d = canvas(14, 14)
    for i in range(14):
        y = int(7 + 4 * math.sin(i * 0.6))
        d.rectangle([i, y - 2, i, y + 2], fill=(245, 110, 170, 255))
        if i % 3 == 0:
            d.point((i, y), fill=(60, 180, 90, 255))
    save(img, out + "DistrictObi.png")
    # Cracked Vase
    img, d = canvas(12, 14)
    d.ellipse([1, 4, 10, 13], fill=(220, 225, 240, 255))
    d.rectangle([4, 0, 7, 5], fill=(220, 225, 240, 255))
    d.rectangle([3, 0, 8, 1], fill=(80, 110, 200, 255))
    d.line([3, 7, 8, 7], fill=(80, 110, 200, 255))
    d.line([6, 5, 5, 8], fill=BLACK); d.line([5, 8, 7, 11], fill=BLACK)
    save(img, out + "CrackedVase.png")
    # Leaf Fan (yatsude)
    img, d = canvas(14, 14)
    d.line([2, 13, 6, 8], fill=(120, 80, 40, 255))
    for a in range(-60, 70, 20):
        r = math.radians(a - 45)
        d.line([6, 8, 6 + 7 * math.cos(r), 8 + 7 * math.sin(r)], fill=(80, 170, 80, 255), width=2)
    save(img, out + "LeafFan.png")
    # Martial Artist's Beads
    img, d = canvas(14, 14)
    for i in range(10):
        a = i * math.pi / 5
        x, y = 7 + 5 * math.cos(a), 7 + 5 * math.sin(a)
        d.ellipse([x - 1.4, y - 1.4, x + 1.4, y + 1.4], fill=(90, 200, 240, 255) if i % 2 else (240, 120, 170, 255))
    save(img, out + "MartialBeads.png")
    # Golden Fan
    img, d = canvas(14, 12)
    d.pieslice([0, 0, 14, 22], 200, 340, fill=(235, 195, 70, 255))
    d.pieslice([4, 6, 10, 16], 200, 340, fill=CLEAR)
    for a in (220, 250, 280, 310):
        r = math.radians(a)
        d.line([7, 11, 7 + 7 * math.cos(r), 11 + 11 * math.sin(r)], fill=(180, 130, 30, 255))
    save(img, out + "GoldenFan.png")
    # Broken Flute
    img, d = canvas(14, 14)
    d.line([1, 12, 6, 7], fill=(120, 60, 150, 255), width=2)
    d.line([8, 5, 12, 1], fill=(120, 60, 150, 255), width=2)
    for p in ((3, 10), (10, 3)):
        d.point(p, fill=BLACK)
    save(img, out + "BrokenFlute.png")
    # Blue Spider Lily
    img, d = canvas(14, 14)
    d.line([7, 13, 7, 6], fill=(60, 140, 70, 255))
    for i in range(6):
        a = i * math.pi / 3
        d.line([7, 5, 7 + 5 * math.cos(a), 5 + 4 * math.sin(a) - 1], fill=(70, 120, 255, 255))
        d.point((7 + 6 * math.cos(a), 5 + 5 * math.sin(a) - 2), fill=(170, 210, 255, 255))
    d.point((7, 5), fill=WHITE)
    save(img, out + "BlueSpiderLily.png")


def more_items():
    out = "Content/Items/"
    # Mukago's wooden charm
    img, d = canvas(12, 12)
    d.ellipse([1, 1, 10, 10], fill=(160, 110, 60, 255))
    d.ellipse([3, 3, 8, 8], fill=(200, 150, 90, 255))
    d.line([5, 3, 6, 8], fill=(200, 40, 40, 255))
    save(img, out + "WoodenCharm.png")
    # Tsuzumi drum
    img, d = canvas(12, 12)
    d.ellipse([0, 1, 5, 11], fill=(230, 220, 200, 255))
    d.ellipse([7, 1, 11, 11], fill=(230, 220, 200, 255))
    d.rectangle([3, 4, 9, 8], fill=(150, 40, 40, 255))
    d.line([2, 2, 9, 9], fill=(240, 170, 60, 255))
    d.line([2, 9, 9, 2], fill=(240, 170, 60, 255))
    save(img, out + "TsuzumiDrum.png")
    # Cracked magatama
    img, d = canvas(12, 12)
    d.ellipse([2, 1, 10, 9], fill=(255, 210, 60, 255))
    d.ellipse([5, 5, 11, 11], fill=CLEAR)
    d.polygon([(2, 5), (5, 11), (6, 8)], fill=(255, 210, 60, 255))
    d.line([6, 2, 8, 6], fill=BLACK)
    save(img, out + "CrackedMagatama.png")
    # Biwa string (a coiled string with a plectrum)
    img, d = canvas(12, 12)
    d.arc([1, 1, 10, 10], 0, 330, fill=(240, 230, 200, 255))
    d.arc([3, 3, 8, 8], 30, 360, fill=(240, 230, 200, 255))
    d.polygon([(7, 7), (11, 9), (8, 11)], fill=(140, 80, 40, 255))
    save(img, out + "BiwaString.png")

    def haori(name, left, right, trim=None, pattern=None):
        img, d = canvas(14, 14)
        d.polygon([(1, 2), (7, 1), (7, 13), (0, 13)], fill=left)
        d.polygon([(7, 1), (13, 2), (14, 13), (7, 13)], fill=right)
        d.rectangle([5, 1, 8, 13], fill=(30, 30, 35, 255))
        if pattern == "check":
            for y in range(2, 13, 2):
                for x in range(0, 14, 2):
                    if (x // 2 + y // 2) % 2 == 0 and not 5 <= x <= 8:
                        d.point((x, y), fill=BLACK)
        if pattern == "flame":
            for x in range(0, 14, 2):
                d.line([x, 13, x + 1, 10], fill=(250, 120, 30, 255))
        if pattern == "butterfly":
            for x, y in ((2, 6), (11, 6), (3, 10), (10, 10)):
                d.point((x, y), fill=(120, 60, 160, 255))
        if trim:
            d.line([0, 13, 14, 13], fill=trim)
        save(img, out + name + ".png")

    haori("CorpsUniform", (30, 30, 40, 255), (30, 30, 40, 255), trim=(230, 220, 190, 255))
    haori("WaterHaori", (150, 40, 50, 255), (80, 150, 90, 255), pattern="check")
    haori("FlameHaori", (240, 240, 235, 255), (240, 240, 235, 255), pattern="flame")
    haori("ButterflyHaori", (250, 200, 220, 255), (190, 230, 240, 255), pattern="butterfly")
    haori("CheckeredHaori", (40, 150, 100, 255), (40, 150, 100, 255), pattern="check")
    # Hanafuda earrings
    img, d = canvas(12, 12)
    for x in (1, 7):
        d.line([x + 2, 0, x + 2, 2], fill=(200, 200, 200, 255))
        d.rectangle([x, 2, x + 4, 10], fill=(245, 240, 225, 255))
        d.ellipse([x + 1, 3, x + 3, 6], fill=(230, 60, 40, 255))
        d.line([x, 8, x + 4, 8], fill=(40, 40, 40, 255))
    save(img, out + "HanafudaEarrings.png")
    # Wisteria charm
    img, d = canvas(12, 14)
    d.line([6, 0, 6, 3], fill=(120, 80, 40, 255))
    for i, y in enumerate(range(3, 13, 2)):
        wdt = 4 - i // 2
        d.ellipse([6 - wdt, y, 6 + wdt, y + 2], fill=(180, 140, 230, 255) if i % 2 else (150, 110, 210, 255))
    save(img, out + "WisteriaCharm.png")
    # Wisteria poison flask
    img, d = canvas(8, 12)
    d.rectangle([3, 0, 4, 2], fill=(150, 110, 70, 255))
    d.ellipse([0, 3, 7, 11], fill=(200, 210, 220, 200))
    d.ellipse([1, 5, 6, 10], fill=(170, 110, 230, 255))
    save(img, out + "WisteriaPoison.png")
    # Butterfly medicine
    img, d = canvas(10, 13)
    d.rectangle([4, 0, 5, 2], fill=(150, 110, 70, 255))
    d.ellipse([1, 3, 8, 12], fill=(200, 210, 220, 200))
    d.ellipse([2, 5, 7, 11], fill=(120, 200, 240, 255))
    d.point((4, 7), fill=(250, 150, 200, 255)); d.point((5, 7), fill=(250, 150, 200, 255))
    save(img, out + "ButterflyMedicine.png")
    # Crow whistle
    img, d = canvas(12, 12)
    d.rectangle([1, 5, 9, 7], fill=(140, 90, 50, 255))
    d.rectangle([9, 4, 11, 8], fill=(110, 70, 40, 255))
    d.point((4, 5), fill=BLACK)
    d.polygon([(2, 1), (5, 0), (7, 2), (4, 3)], fill=(30, 30, 35, 255))
    save(img, out + "CrowWhistle.png")

    # Kasugai crow pet: 4 frames of 12x10 (half size), facing LEFT.
    img, d = canvas(12, 40)
    for f in range(4):
        oy = f * 10
        wing = [0, -2, -3, -1][f]
        d.ellipse([3, oy + 3, 10, oy + 8], fill=(30, 30, 38, 255))
        d.ellipse([1, oy + 2, 5, oy + 6], fill=(30, 30, 38, 255))
        d.point((0, oy + 4), fill=(230, 180, 60, 255))
        d.point((2, oy + 3), fill=WHITE)
        d.polygon([(5, oy + 5), (9, oy + 5 + wing), (8, oy + 6)], fill=(55, 55, 65, 255))
        d.line([10, oy + 6, 11, oy + 7], fill=(30, 30, 38, 255))
    save(img, "Content/Pets/KasugaiCrow.png")


# ---------------------------------------------------------------------------------------------------------------------
# Projectiles (white shapes, tinted in game)
# ---------------------------------------------------------------------------------------------------------------------

def crescent(size, thickness, path, scale=2):
    img, d = canvas(size, size)
    c = size / 2
    for y in range(size):
        for x in range(size):
            dx, dy = x + 0.5 - c, y + 0.5 - c
            outer = dx * dx + dy * dy <= (c - 0.5) ** 2
            ix = dx + thickness
            inner = ix * ix + dy * dy <= (c - 1) ** 2
            if outer and not inner and dx > -c * 0.2:
                a = 255 if dx > c * 0.3 else 200
                img.putpixel((x, y), (255, 255, 255, a))
    save(img, path, scale)


def breath_shapes():
    base = "Content/Projectiles/Breath/"
    crescent(24, 6, base + "Crescent.png")
    img, d = canvas(48, 12)                        # thrust: long spike pointing right
    d.polygon([(0, 5), (40, 3), (47, 6), (40, 8), (0, 7)], fill=(255, 255, 255, 220))
    d.line([4, 6, 44, 6], fill=WHITE)
    save(img, base + "Thrust.png")
    img, d = canvas(32, 6)                         # slash mark
    d.polygon([(0, 3), (16, 0), (31, 3), (16, 5)], fill=(255, 255, 255, 230))
    d.line([2, 3, 29, 3], fill=WHITE)
    save(img, base + "Slash.png")
    img, d = canvas(16, 64)                        # pillar
    for y in range(64):
        half = 2 + 6 * (y / 63) ** 0.5
        a = int(120 + 135 * (y / 63))
        d.line([8 - half, y, 8 + half, y], fill=(255, 255, 255, a))
    save(img, base + "Pillar.png")


def demon_shapes():
    base = "Content/Projectiles/Demon/"
    img, d = canvas(8, 8)
    d.ellipse([0, 0, 7, 7], fill=(255, 255, 255, 230))
    d.ellipse([2, 2, 5, 5], fill=WHITE)
    save(img, base + "Orb.png")
    img, d = canvas(16, 5)
    d.polygon([(0, 2), (12, 0), (15, 2), (12, 4)], fill=(255, 255, 255, 240))
    save(img, base + "Needle.png")
    crescent(16, 4, base + "Crescent.png")
    img, d = canvas(10, 10)
    d.ellipse([0, 0, 9, 9], fill=(255, 255, 255, 255))
    d.line([0, 4, 9, 5], fill=(200, 200, 200, 255))
    d.line([4, 0, 5, 9], fill=(200, 200, 200, 255))
    save(img, base + "Ball.png")
    img, d = canvas(32, 32)
    d.ellipse([0, 0, 31, 31], outline=(255, 255, 255, 255), width=2)
    d.ellipse([6, 6, 25, 25], outline=(255, 255, 255, 120), width=1)
    save(img, base + "Ring.png")
    img, d = canvas(13, 45)
    d.polygon([(6, 0), (12, 44), (0, 44)], fill=(235, 235, 235, 255))
    d.polygon([(6, 0), (8, 44), (6, 44)], fill=WHITE)
    save(img, base + "Spike.png")


# ---------------------------------------------------------------------------------------------------------------------
# Demons. Drawn at half size, facing LEFT, 4 frames stacked vertically (walk/float cycle).
# ---------------------------------------------------------------------------------------------------------------------

def humanoid(d, w, h, f, spec, oy):
    """A demon standing in a (w x h) half-size frame whose top is at y=oy. f = frame 0..3."""
    skin, cloth, hair = spec["skin"], spec["cloth"], spec.get("hair", BLACK)
    eye = spec.get("eye", (230, 40, 40, 255))
    bob = [0, 1, 0, 1][f]
    floating = spec.get("float", False)
    cx = w // 2
    head = max(4, w // 3)
    top = oy + 1 + bob
    # legs
    if not floating:
        step = [-1, 0, 1, 0][f]
        leg = spec.get("legs", shade(cloth, 0.7))
        d.rectangle([cx - 2 + step, oy + h - 6, cx - 1 + step, oy + h - 1], fill=leg)
        d.rectangle([cx + 1 - step, oy + h - 6, cx + 2 - step, oy + h - 1], fill=leg)
    else:
        for i in range(3):
            d.point((cx - 1 + i, oy + h - 3 + (f + i) % 2), fill=shade(cloth, 0.8))
    # torso / clothing
    body_top = top + head
    body_bottom = oy + h - (6 if not floating else 3)
    d.rectangle([cx - 3, body_top, cx + 3, body_bottom], fill=cloth)
    d.line([cx - 3, body_top, cx - 3, body_bottom], fill=shade(cloth, 1.25))
    if spec.get("belt"):
        d.line([cx - 3, (body_top + body_bottom) // 2 + 1, cx + 3, (body_top + body_bottom) // 2 + 1], fill=spec["belt"])
    if spec.get("pattern"):
        for y in range(body_top + 1, body_bottom, 3):
            d.point((cx - 1 + (y % 2), y), fill=spec["pattern"])
    # arms, swinging a little
    swing = [0, 1, 0, -1][f]
    arm = spec.get("arm", skin)
    d.line([cx - 4, body_top + 1, cx - 5 - swing, body_top + 6], fill=cloth)
    d.point((cx - 5 - swing, body_top + 7), fill=arm)
    d.line([cx + 4, body_top + 1, cx + 5 + swing, body_top + 6], fill=shade(cloth, 0.8))
    d.point((cx + 5 + swing, body_top + 7), fill=arm)
    if spec.get("claws"):
        d.point((cx - 6 - swing, body_top + 8), fill=WHITE)
    # head
    d.ellipse([cx - head // 2 - 1, top, cx + head // 2, top + head], fill=skin)
    hs = spec.get("hairstyle", "short")
    if hs == "short":
        d.rectangle([cx - head // 2 - 1, top - 1, cx + head // 2, top + 1], fill=hair)
    elif hs == "long":
        d.rectangle([cx - head // 2 - 1, top - 1, cx + head // 2, top + 1], fill=hair)
        d.rectangle([cx + head // 2 - 1, top, cx + head // 2 + 1, body_top + 6], fill=hair)
    elif hs == "spiky":
        for i in range(-head // 2 - 1, head // 2 + 1, 2):
            d.line([cx + i, top + 1, cx + i + 1, top - 2], fill=hair)
    elif hs == "hat":
        d.rectangle([cx - head // 2 - 2, top, cx + head // 2 + 1, top], fill=hair)
        d.rectangle([cx - head // 2, top - 3, cx + head // 2 - 1, top], fill=hair)
    elif hs == "bald":
        pass
    # horns
    if spec.get("horns"):
        hc = spec["horns"]
        d.line([cx - 2, top, cx - 3, top - 3], fill=hc)
        d.line([cx + 1, top, cx + 2, top - 3], fill=hc)
    # eyes (facing left)
    ey = top + head // 2
    d.point((cx - head // 2 + 1, ey), fill=eye)
    if spec.get("six_eyes"):
        d.point((cx - head // 2 + 1, ey - 2), fill=eye)
        d.point((cx - head // 2 + 1, ey + 2), fill=eye)
        d.point((cx, ey - 1), fill=eye)
    else:
        d.point((cx - 1, ey), fill=eye)
    d.point((cx - head // 2 + 1, ey + 2), fill=(90, 10, 20, 255))  # mouth
    if spec.get("marks"):
        d.point((cx - 1, top + 1), fill=spec["marks"])
        d.point((cx - head // 2, ey + 1), fill=spec["marks"])
    # held extras
    held = spec.get("held")
    if held == "sword":
        d.line([cx - 5 - swing, body_top + 7, cx - 10 - swing, body_top - 2], fill=(200, 200, 215, 255))
    elif held == "fans":
        for sx in (cx - 7 - swing, cx + 5 + swing):
            d.pieslice([sx - 2, body_top + 4, sx + 3, body_top + 10], 180, 360, fill=(235, 195, 70, 255))
    elif held == "sickle":
        d.arc([cx - 10 - swing, body_top + 2, cx - 4 - swing, body_top + 9], 90, 270, fill=(220, 30, 50, 255))
    elif held == "obi":
        for i in range(6):
            d.point((cx + 5 + i, body_top + 3 + int(2 * math.sin(i + f))), fill=(245, 110, 170, 255))
            d.point((cx - 6 - i, body_top + 5 + int(2 * math.cos(i + f))), fill=(245, 110, 170, 255))
    elif held == "whips":
        for i in range(8):
            d.point((cx + 4 + i, body_top + 4 + int(3 * math.sin(i * 0.8 + f))), fill=(170, 20, 40, 255))
            d.point((cx - 5 - i, body_top + 6 + int(3 * math.cos(i * 0.8 + f))), fill=(170, 20, 40, 255))
    elif held == "drums":
        for dx, dy in ((-4, 2), (3, 3), (0, 6)):
            d.ellipse([cx + dx - 2, body_top + dy - 1, cx + dx + 1, body_top + dy + 2], fill=(200, 160, 110, 255), outline=(120, 70, 40, 255))
    elif held == "tattoo":
        for y in range(body_top + 1, body_bottom, 2):
            d.point((cx - 2, y), fill=(60, 120, 230, 255))
            d.point((cx + 2, y), fill=(60, 120, 230, 255))
    elif held == "biwa":
        d.ellipse([cx - 8, body_top + 3, cx - 3, body_top + 9], fill=(150, 90, 50, 255))
        d.line([cx - 5, body_top + 4, cx - 9, body_top - 3], fill=(110, 60, 30, 255))
        d.line([cx - 6, body_top + 5, cx - 6, body_top + 8], fill=(240, 230, 200, 255))
    elif held == "claws2":
        d.line([cx - 5 - swing, body_top + 6, cx - 9 - swing, body_top + 2], fill=(120, 200, 90, 255), width=1)
        d.line([cx + 5 + swing, body_top + 6, cx + 8 + swing, body_top + 2], fill=(90, 160, 70, 255), width=1)
    elif held == "eyehands":
        d.point((cx - 5 - swing, body_top + 7), fill=(230, 40, 40, 255))
        d.point((cx + 5 + swing, body_top + 7), fill=(230, 40, 40, 255))


def blob(d, w, h, f, spec, oy):
    """A shapeless burrowing / floating demon: body, eyes and teeth."""
    body = spec["skin"]
    wob = [0, 1, 0, -1][f]
    d.ellipse([1, oy + 2 + wob, w - 2, oy + h - 2 - wob], fill=body)
    d.ellipse([2, oy + 3 + wob, w - 3, oy + h // 2], fill=shade(body, 1.2))
    eye = spec.get("eye", (230, 40, 40, 255))
    d.point((3, oy + h // 2 - 1), fill=eye)
    d.point((6, oy + h // 2 - 2), fill=eye)
    for x in range(2, w // 2, 2):
        d.point((x, oy + h // 2 + 2), fill=WHITE)
    if spec.get("tongue"):
        for i in range(6):
            d.point((1 - i + 6 - 6, oy + h // 2 + 2 + int(math.sin(i + f))), fill=(220, 90, 110, 255))
        d.line([0, oy + h // 2 + 2, 3, oy + h // 2 + 2], fill=(220, 90, 110, 255))


def spider(d, w, h, f, spec, oy):
    body = spec["skin"]
    step = [0, 1, 0, -1][f]
    d.ellipse([w // 2 - 1, oy + h // 3, w - 2, oy + h - 3], fill=body)
    for i in range(4):
        x = 3 + i * (w - 6) // 3
        d.line([x, oy + h // 2 + 1, x - 2 + (step if i % 2 else -step), oy + h - 1], fill=shade(body, 0.7))
    d.ellipse([1, oy + h // 3 - 1, w // 2 + 1, oy + h // 2 + 3], fill=spec.get("face", (235, 225, 215, 255)))
    d.rectangle([1, oy + h // 3 - 2, w // 2 + 1, oy + h // 3], fill=spec.get("hair", WHITE))
    d.point((3, oy + h // 3 + 2), fill=(220, 30, 30, 255))
    d.point((5, oy + h // 3 + 2), fill=(220, 30, 30, 255))


def fish(d, w, h, f, spec, oy):
    body = spec["skin"]
    d.ellipse([2, oy + 3, w - 5, oy + h - 3], fill=body)
    d.polygon([(w - 6, oy + h // 2), (w - 1, oy + 2 + f % 2), (w - 1, oy + h - 3 - f % 2)], fill=shade(body, 0.8))
    d.point((4, oy + h // 2 - 1), fill=BLACK)
    for i in range(3):
        d.line([5 + i * 3, oy + h - 4, 4 + i * 3 + (f % 2), oy + h - 1], fill=(240, 200, 180, 255))


def hand_demon(d, w, h, f, spec, oy):
    body = spec["skin"]
    wob = [0, 1, 0, -1][f]
    d.ellipse([4, oy + 8, w - 4, oy + h - 4], fill=body)
    # arms wrapping the body and reaching out
    for i in range(9):
        a = i * math.pi * 2 / 9 + f * 0.2
        x0, y0 = w / 2 + (w / 2 - 6) * math.cos(a), oy + h / 2 + (h / 2 - 8) * math.sin(a)
        x1, y1 = w / 2 + (w / 2 - 1) * math.cos(a + 0.3), oy + h / 2 + (h / 2 - 2) * math.sin(a + 0.3)
        d.line([x0, y0, x1, y1], fill=shade(body, 0.8), width=2)
        d.point((x1, y1), fill=shade(body, 1.3))
    d.ellipse([w // 2 - 7, oy + 4 + wob, w // 2 + 3, oy + 16 + wob], fill=shade(body, 1.15))
    d.point((w // 2 - 5, oy + 9 + wob), fill=(250, 220, 60, 255))
    d.point((w // 2 - 2, oy + 9 + wob), fill=(250, 220, 60, 255))
    d.line([w // 2 - 5, oy + 13 + wob, w // 2 - 1, oy + 13 + wob], fill=(80, 10, 20, 255))
    # legs
    d.rectangle([w // 2 - 8, oy + h - 5, w // 2 - 5, oy + h - 1], fill=shade(body, 0.7))
    d.rectangle([w // 2 + 4, oy + h - 5, w // 2 + 7, oy + h - 1], fill=shade(body, 0.7))


def vase_demon(d, w, h, f, spec, oy):
    d.ellipse([3, oy + h // 2, w - 4, oy + h - 1], fill=(220, 225, 240, 255))
    d.rectangle([w // 2 - 3, oy + h // 2 - 3, w // 2 + 2, oy + h // 2 + 2], fill=(220, 225, 240, 255))
    d.line([4, oy + h * 3 // 4, w - 5, oy + h * 3 // 4], fill=(80, 110, 200, 255))
    bob = [0, 1, 2, 1][f]
    body = spec["skin"]
    d.rectangle([w // 2 - 2, oy + 6 - bob, w // 2 + 1, oy + h // 2 - 2], fill=body)
    d.ellipse([w // 2 - 3, oy + 2 - bob, w // 2 + 2, oy + 8 - bob], fill=body)
    d.point((w // 2 - 2, oy + 4 - bob), fill=(250, 200, 40, 255))
    d.point((w // 2, oy + 6 - bob), fill=(250, 200, 40, 255))
    for i in range(3):
        d.line([w // 2 - 3, oy + 10 + i * 3 - bob, w // 2 - 6, oy + 9 + i * 3 - bob + (f + i) % 2], fill=shade(body, 0.8))


def npc_sheet(name, w, h, drawer, spec, folder):
    """w, h: the full-size frame (matches the NPC hitbox). Drawn at half size."""
    hw, hh = w // 2, h // 2
    img, d = canvas(hw, hh * 4)
    for f in range(4):
        drawer(d, hw, hh, f, spec, f * hh)
    save(img, folder + name + ".png")
    return img


def boss_head(name, sheet, hw, hh):
    # Map icon: the top part of the first frame, cropped square.
    size = min(hw, 16)
    left = max(0, hw // 2 - size // 2)
    crop = sheet.crop((left, 0, left + size, size))
    save(crop, "Content/NPCs/Bosses/" + name + "_Head_Boss.png")


SKIN = (235, 205, 180, 255)
PALE = (230, 225, 225, 255)
GREEN_SKIN = (120, 160, 90, 255)


def demons():
    e = "Content/NPCs/Enemies/"
    npc_sheet("LesserDemon", 28, 44, humanoid, dict(skin=(200, 170, 160, 255), cloth=(90, 70, 60, 255), hairstyle="spiky", claws=True), e)
    npc_sheet("HornedDemon", 32, 50, humanoid, dict(skin=(190, 140, 130, 255), cloth=(70, 50, 60, 255), horns=(230, 220, 200, 255), claws=True, hairstyle="bald"), e)
    npc_sheet("SwampDemon", 30, 30, blob, dict(skin=(70, 90, 70, 255)), e)
    npc_sheet("TemariDemon", 28, 44, humanoid, dict(skin=SKIN, cloth=(230, 120, 50, 255), pattern=(250, 220, 90, 255), hairstyle="long", belt=(200, 40, 40, 255)), e)
    npc_sheet("ArrowDemon", 28, 44, humanoid, dict(skin=SKIN, cloth=(120, 60, 60, 255), hairstyle="short", float=True, held="eyehands"), e)
    npc_sheet("DrumDemon", 36, 54, humanoid, dict(skin=(200, 170, 150, 255), cloth=(140, 110, 80, 255), hair=(60, 40, 30, 255), held="drums"), e)
    npc_sheet("SpiderDemon", 34, 34, spider, dict(skin=(200, 195, 190, 255)), e)
    npc_sheet("TongueDemon", 30, 30, blob, dict(skin=(160, 120, 140, 255), tongue=True), e)
    npc_sheet("BloodBrute", 40, 60, humanoid, dict(skin=(170, 60, 60, 255), cloth=(80, 20, 30, 255), horns=(60, 20, 20, 255), claws=True, hairstyle="bald"), e)
    npc_sheet("BiwaDemon", 28, 44, humanoid, dict(skin=PALE, cloth=(60, 30, 70, 255), hairstyle="long", float=True, eye=(250, 220, 60, 255)), e)
    npc_sheet("IceDemon", 28, 44, humanoid, dict(skin=(200, 230, 245, 255), cloth=(120, 170, 220, 255), hair=(230, 240, 250, 255), float=True, eye=(80, 200, 255, 255)), e)
    npc_sheet("SickleDemon", 28, 44, humanoid, dict(skin=(150, 130, 120, 255), cloth=(60, 90, 60, 255), hair=(40, 60, 40, 255), hairstyle="spiky", held="sickle"), e)
    npc_sheet("FleshCrawler", 40, 40, blob, dict(skin=(150, 40, 60, 255)), e)
    npc_sheet("ThunderDemon", 28, 44, humanoid, dict(skin=(200, 180, 170, 255), cloth=(30, 30, 40, 255), hair=(40, 40, 40, 255), held="sword", eye=(255, 220, 60, 255), marks=(255, 220, 60, 255)), e)
    npc_sheet("VaseFish", 26, 22, fish, dict(skin=(240, 150, 150, 255)), e)
    npc_sheet("EmotionClone", 28, 44, humanoid, dict(skin=(200, 170, 140, 255), cloth=(180, 60, 50, 255), horns=(230, 220, 200, 255), float=True, hairstyle="bald", held="fans"), e)
    npc_sheet("SandDemon", 30, 30, blob, dict(skin=(205, 175, 110, 255)), e)
    npc_sheet("MantisDemon", 28, 44, humanoid, dict(skin=(120, 180, 90, 255), cloth=(60, 110, 50, 255), hairstyle="bald", held="claws2", eye=(250, 220, 60, 255)), e)
    npc_sheet("DrownedDemon", 28, 44, humanoid, dict(skin=(150, 190, 200, 255), cloth=(40, 70, 110, 255), hair=(30, 60, 70, 255), hairstyle="long", float=True), e)
    npc_sheet("HellfireDemon", 28, 44, humanoid, dict(skin=(220, 90, 50, 255), cloth=(90, 20, 10, 255), horns=(40, 20, 20, 255), hairstyle="bald", float=True, eye=(255, 230, 80, 255)), e)
    npc_sheet("HollowDemon", 28, 44, humanoid, dict(skin=(150, 130, 160, 255), cloth=(70, 50, 90, 255), hairstyle="spiky", hair=(40, 30, 60, 255), claws=True, eye=(20, 10, 20, 255)), e)
    npc_sheet("FrostbittenDemon", 28, 44, humanoid, dict(skin=(190, 210, 230, 255), cloth=(90, 110, 150, 255), hair=(230, 240, 250, 255), claws=True), e)
    npc_sheet("MirrorDemon", 28, 44, humanoid, dict(skin=(245, 220, 245, 255), cloth=(200, 170, 240, 255), hair=(255, 190, 240, 255), float=True, pattern=(255, 255, 255, 255), eye=(120, 220, 255, 255)), e)
    npc_sheet("IceDoll", 28, 44, humanoid, dict(skin=(210, 235, 250, 255), cloth=(170, 210, 240, 255), hair=(240, 245, 255, 255), float=True, held="fans", eye=(120, 220, 255, 255)), e)

    b = "Content/NPCs/Bosses/"
    bosses = [
        ("HandDemon", 80, 110, hand_demon, dict(skin=GREEN_SKIN)),
        ("Rui", 36, 56, humanoid, dict(skin=PALE, cloth=(240, 240, 245, 255), hair=(245, 245, 250, 255), pattern=(200, 40, 40, 255), float=True, marks=(200, 40, 40, 255))),
        ("Enmu", 36, 60, humanoid, dict(skin=PALE, cloth=(40, 50, 60, 255), hair=(40, 60, 70, 255), float=True, held="eyehands", eye=(200, 120, 220, 255))),
        ("GyutaroDaki", 40, 64, humanoid, dict(skin=PALE, cloth=(60, 60, 70, 255), hair=(30, 30, 30, 255), hairstyle="long", held="obi", belt=(245, 110, 170, 255), marks=(245, 110, 170, 255))),
        ("Gyokko", 40, 56, vase_demon, dict(skin=(220, 225, 240, 255))),
        ("Hantengu", 40, 70, humanoid, dict(skin=(190, 170, 150, 255), cloth=(200, 180, 140, 255), horns=(230, 220, 200, 255), hairstyle="bald", held="fans")),
        ("Akaza", 36, 60, humanoid, dict(skin=(235, 205, 190, 255), cloth=(235, 235, 240, 255), hair=(245, 140, 180, 255), hairstyle="spiky", held="tattoo", legs=(235, 235, 240, 255), eye=(250, 220, 60, 255))),
        ("Doma", 36, 62, humanoid, dict(skin=PALE, cloth=(40, 40, 50, 255), hair=(240, 230, 210, 255), belt=(200, 40, 40, 255), held="fans", eye=(170, 120, 230, 255))),
        ("Kokushibo", 40, 68, humanoid, dict(skin=(200, 150, 150, 255), cloth=(110, 50, 110, 255), hair=(20, 20, 25, 255), hairstyle="long", held="sword", six_eyes=True, eye=(250, 220, 60, 255), marks=(200, 40, 40, 255))),
        ("Muzan", 40, 70, humanoid, dict(skin=PALE, cloth=(30, 30, 35, 255), hair=(20, 20, 25, 255), hairstyle="hat", belt=(240, 240, 240, 255), held="whips", eye=(230, 30, 40, 255))),
    ]
    bosses += [
        ("Mukago", 32, 54, humanoid, dict(skin=PALE, cloth=(120, 90, 140, 255), hair=(40, 30, 50, 255), hairstyle="long", claws=True, marks=(200, 40, 40, 255))),
        ("Kyogai", 40, 60, humanoid, dict(skin=(210, 180, 160, 255), cloth=(150, 110, 70, 255), hair=(60, 40, 30, 255), hairstyle="long", held="drums", horns=(230, 220, 200, 255))),
        ("Kaigaku", 34, 60, humanoid, dict(skin=(215, 195, 180, 255), cloth=(25, 25, 30, 255), hair=(30, 30, 30, 255), hairstyle="spiky", held="sword", eye=(255, 220, 60, 255), marks=(255, 220, 60, 255), pattern=(255, 220, 60, 255))),
        ("Nakime", 36, 60, humanoid, dict(skin=PALE, cloth=(120, 40, 70, 255), hair=(15, 15, 20, 255), hairstyle="long", float=True, held="biwa", eye=(250, 220, 60, 255))),
    ]
    for name, w, h, drawer, spec in bosses:
        sheet = npc_sheet(name, w, h, drawer, spec, b)
        boss_head(name, sheet, w // 2, h // 2)


# ---------------------------------------------------------------------------------------------------------------------
# Buffs and icon
# ---------------------------------------------------------------------------------------------------------------------

def buff_icon(path, color, symbol):
    img, d = canvas(16, 16)
    d.rectangle([0, 0, 15, 15], fill=(30, 30, 40, 255))
    d.rectangle([1, 1, 14, 14], fill=shade(color, 0.5))
    if symbol == "wave":
        for x in range(2, 14):
            d.point((x, 8 + int(2 * math.sin(x * 0.9))), fill=color)
            d.point((x, 11 + int(2 * math.sin(x * 0.9 + 1))), fill=color)
    elif symbol == "eye":
        d.ellipse([3, 5, 12, 10], fill=WHITE)
        d.ellipse([6, 5, 9, 10], fill=color)
    elif symbol == "ghost":
        d.ellipse([4, 3, 11, 12], fill=(255, 255, 255, 140))
        d.ellipse([2, 4, 9, 13], fill=(255, 255, 255, 200))
    elif symbol == "mark":
        d.polygon([(8, 2), (11, 8), (8, 14), (5, 8)], fill=color)
        d.line([4, 5, 12, 11], fill=WHITE)
    save(img, path)


def icon():
    img, d = canvas(40, 40)
    d.rectangle([0, 0, 39, 39], fill=(25, 25, 40, 255))
    for i in range(40):                                   # checkered haori pattern in the corner
        for j in range(40):
            if i + j > 52 and (i // 4 + j // 4) % 2 == 0:
                img.putpixel((i, j), (40, 120, 90, 255))
    d.line([6, 34, 34, 6], fill=(45, 45, 55, 255), width=3)
    d.line([7, 35, 35, 7], fill=(220, 225, 235, 255), width=1)
    d.rectangle([4, 30, 9, 35], fill=(40, 55, 120, 255))
    d.ellipse([8, 25, 15, 32], fill=(60, 60, 70, 255))
    d.ellipse([22, 4, 36, 18], outline=(250, 140, 40, 255), width=2)  # rising sun
    save(img, "icon.png")


def main():
    for shape in SHAPES:
        blade_layers(shape)
    hilt()
    for g in GUARDS:
        guard(g)
    nichirin_item()
    demon_blood()
    scroll("Content/Items/TrainingScroll.png", (240, 230, 200, 255), (200, 40, 40, 255), (60, 50, 40, 255))
    scroll("Content/Items/ScrollOfForgetting.png", (200, 210, 230, 255), (80, 110, 200, 255), (120, 130, 160, 255))
    forge_item()
    forge_tile()
    summon_items()
    breath_shapes()
    demon_shapes()
    demons()
    more_items()
    buff_icon("Content/Buffs/WisteriaPoisonBuff.png", (180, 130, 240, 255), "wave")
    buff_icon("Content/Pets/KasugaiCrowBuff.png", (60, 60, 80, 255), "eye")
    buff_icon("Content/Buffs/DeadCalmBuff.png", (90, 160, 255, 255), "wave")
    buff_icon("Content/Buffs/SensesBuff.png", (250, 120, 180, 255), "eye")
    buff_icon("Content/Buffs/AfterimageBuff.png", (200, 200, 230, 255), "ghost")
    buff_icon("Content/Buffs/DemonSlayerMarkBuff.png", (230, 40, 50, 255), "mark")
    icon()


if __name__ == "__main__":
    main()
