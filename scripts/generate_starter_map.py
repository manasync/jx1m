#!/usr/bin/env python3
import os
import struct
from PIL import Image, ImageDraw, ImageFont

def create_wav(filepath, duration_sec=3, sample_rate=44100):
    num_samples = sample_rate * duration_sec
    raw_data = bytearray()
    import math
    for i in range(num_samples):
        # Soft gentle 220Hz harmonic hum
        val = int(1500 * math.sin(2 * math.pi * 220 * i / sample_rate))
        raw_data += struct.pack('<h', val)

    header = bytearray(b'RIFF')
    header += struct.pack('<I', 36 + len(raw_data))
    header += b'WAVEfmt '
    header += struct.pack('<I', 16)
    header += struct.pack('<H', 1) # PCM
    header += struct.pack('<H', 1) # Mono
    header += struct.pack('<I', sample_rate)
    header += struct.pack('<I', sample_rate * 2)
    header += struct.pack('<H', 2)
    header += struct.pack('<H', 16)
    header += b'data'
    header += struct.pack('<I', len(raw_data))
    header += raw_data

    with open(filepath, 'wb') as f:
        f.write(header)

def create_map_tile(filepath, col, row, layer_id):
    width, height = 512, 512
    # Base earthy grass green
    base_color = (62, 98, 54) if (col + row) % 2 == 0 else (55, 90, 48)
    img = Image.new('RGB', (width, height), base_color)
    draw = ImageDraw.Draw(img)

    # Draw grid texture / stone paths
    border_color = (78, 118, 68)
    draw.rectangle([0, 0, width - 1, height - 1], outline=border_color, width=4)

    # Subtle internal stone tiles / path grid
    step = 64
    for x in range(step, width, step):
        draw.line([(x, 0), (x, height)], fill=(50, 80, 44), width=1)
    for y in range(step, height, step):
        draw.line([(0, y), (width, y)], fill=(50, 80, 44), width=1)

    # Central path if row == 1 (center horizontal street of Ba Lang Huyen)
    if row == 1:
        path_color = (130, 115, 95) # Earthy dirt path
        draw.rectangle([0, 180, width, 332], fill=path_color)
        draw.line([(0, 180), (width, 180)], fill=(110, 95, 75), width=3)
        draw.line([(0, 332), (width, 332)], fill=(110, 95, 75), width=3)

    # Center vertical path if col == 2 (center crossroad)
    if col == 2:
        path_color = (130, 115, 95)
        draw.rectangle([180, 0, 332, height], fill=path_color)
        draw.line([(180, 0), (180, height)], fill=(110, 95, 75), width=3)
        draw.line([(332, 0), (332, height)], fill=(110, 95, 75), width=3)

    # Water pond / river corner for col == 0, row == 0
    if col == 0 and row == 0:
        water_color = (45, 95, 125)
        draw.rectangle([0, 0, 200, 200], fill=water_color)

    # Draw Tile Info Label in center
    label = f"Ba Lăng Huyện [{col},{row}]\nTile #{layer_id}"
    draw.text((width // 2 - 60, height // 2 - 15), label, fill=(240, 240, 220))

    img.save(filepath, 'PNG')

def main():
    target_dir = "client/src/Assets/Editor/TempMap"
    image_dir = os.path.join(target_dir, "Image")
    os.makedirs(image_dir, exist_ok=True)

    # 1. Create 15 map tiles
    layer_id = 1
    total_horizontal = 5
    total_vertical = 3

    for col in range(total_horizontal):
        for row in range(total_vertical):
            tile_path = os.path.join(image_dir, f"{layer_id}.png")
            create_map_tile(tile_path, col, row, layer_id)
            layer_id += 1

    # 2. Create Music.wav
    wav_path = os.path.join(target_dir, "Music.wav")
    create_wav(wav_path)
    print("Generated 15 Map tiles and Music.wav in Assets/Editor/TempMap")

if __name__ == "__main__":
    main()
