from pathlib import Path

from PIL import Image, ImageDraw, ImageFilter


ROOT = Path(__file__).resolve().parent
SCALE = 2
SIZE = 1024
CANVAS = SIZE * SCALE


def lerp(a: int, b: int, t: float) -> int:
    return round(a + (b - a) * t)


def render() -> Image.Image:
    image = Image.new("RGBA", (CANVAS, CANVAS), (0, 0, 0, 0))
    background = ImageDraw.Draw(image)
    top = (27, 73, 96)
    bottom = (17, 24, 39)
    for y in range(CANVAS):
        t = y / (CANVAS - 1)
        background.line((0, y, CANVAS, y), fill=tuple(lerp(top[i], bottom[i], t) for i in range(3)) + (255,))

    glow = Image.new("RGBA", image.size, (0, 0, 0, 0))
    ImageDraw.Draw(glow).ellipse((1200, -180, 2140, 760), fill=(50, 198, 212, 55))
    glow = glow.filter(ImageFilter.GaussianBlur(260 * SCALE))
    image = Image.alpha_composite(image, glow)

    scale = SCALE
    rounded_mask = Image.new("L", (CANVAS, CANVAS), 0)
    mask_draw = ImageDraw.Draw(rounded_mask)
    margin = 24 * scale
    mask_draw.rounded_rectangle((margin, margin, CANVAS - margin, CANVAS - margin), radius=232 * scale, fill=255)
    image.putalpha(rounded_mask)

    border = Image.new("RGBA", image.size, (0, 0, 0, 0))
    bd = ImageDraw.Draw(border)
    bd.rounded_rectangle((36 * scale, 36 * scale, CANVAS - 36 * scale, CANVAS - 36 * scale), radius=220 * scale,
                         outline=(181, 234, 250, 42), width=10 * scale)
    image = Image.alpha_composite(image, border)

    sigma = [(718, 244), (314, 244), (510, 500), (314, 756), (718, 756),
             (718, 646), (528, 646), (652, 500), (528, 354), (718, 354)]
    scaled_sigma = [(x * scale, y * scale) for x, y in sigma]
    shadow_mask = Image.new("L", image.size, 0)
    sd = ImageDraw.Draw(shadow_mask)
    sd.polygon([(x, y + 18 * scale) for x, y in scaled_sigma], fill=125)
    shadow_mask = shadow_mask.filter(ImageFilter.GaussianBlur(18 * scale))
    shadow = Image.new("RGBA", image.size, (3, 9, 20, 0))
    shadow.putalpha(shadow_mask)
    image = Image.alpha_composite(image, shadow)

    mark_mask = Image.new("L", image.size, 0)
    ImageDraw.Draw(mark_mask).polygon(scaled_sigma, fill=255)
    mark_gradient = Image.new("RGBA", image.size, (239, 249, 255, 0))
    mark_gradient.putalpha(mark_mask)
    image = Image.alpha_composite(image, mark_gradient)

    accent = Image.new("RGBA", image.size, (0, 0, 0, 0))
    ad = ImageDraw.Draw(accent)
    ad.ellipse(((804 - 26) * scale, (270 - 26) * scale, (804 + 26) * scale, (270 + 26) * scale), fill="#67e8df")
    ad.ellipse(((796 - 7) * scale, (262 - 7) * scale, (796 + 7) * scale, (262 + 7) * scale), fill="#e6fffc")
    return Image.alpha_composite(image, accent).resize((SIZE, SIZE), Image.Resampling.LANCZOS)


if __name__ == "__main__":
    icon = render()
    icon.save(ROOT / "MathKey.ico", format="ICO", sizes=[(16, 16), (20, 20), (24, 24), (32, 32), (40, 40), (48, 48), (64, 64), (128, 128), (256, 256)])
    icon.save(ROOT / "MathKey-preview.png")
