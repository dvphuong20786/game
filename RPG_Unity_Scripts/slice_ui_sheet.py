"""
UI ASSET SHEET SLICER - Calibrated for 1536x1024 source image
Crops each labeled asset from the sheet into individual PNG files.
"""

from PIL import Image
import os

SOURCE = r'f:\WORK\GAME\RPG_Unity_Scripts\AI_Generated_Sprites\ui_sheet_source.png'
OUT    = r'f:\WORK\GAME\RPG_Unity_Scripts\MyRPG\Assets\Resources\Sprites'
BACKUP = r'f:\WORK\GAME\RPG_Unity_Scripts\AI_Generated_Sprites\UI_Sliced'

img = Image.open(SOURCE).convert("RGBA")
W, H = img.size  # 1536 x 1024
print(f"Source: {W}x{H}px\n")

# ─────────────────────────────────────────────────────────────────────────────
# CROP MAP: (left, top, right, bottom) in PIXELS for 1536x1024
# Measured carefully from the labeled source sheet
# ─────────────────────────────────────────────────────────────────────────────
CROPS = {

    # ── 1. BACKGROUND ────────────────────────────────────────────────────────
    "background/bg_dungeon.png":          (507,  28,  985, 300),
    "background/bg_light_overlay.png":    (507, 305,  700, 385),
    "background/bg_vignette.png":         (720, 305,  998, 385),
    "background/bg_particles.png":        (507, 360,  998, 405),

    # ── 2. MAIN FRAME (9-Slice) ───────────────────────────────────────────────
    "frame/corner_top_left.png":          (1022,  50, 1132, 185),
    "frame/edge_top.png":                 (1150,  50, 1343, 185),
    "frame/corner_top_right.png":         (1365,  50, 1530, 185),
    "frame/edge_left.png":                (1022, 200, 1112, 340),
    "frame/center_fill.png":              (1135, 200, 1343, 340),
    "frame/edge_right.png":               (1365, 200, 1530, 340),
    "frame/corner_bottom_left.png":       (1022, 345, 1132, 406),
    "frame/edge_bottom.png":             (1150, 345, 1343, 406),
    "frame/corner_bottom_right.png":      (1365, 345, 1530, 406),

    # ── 3. LEFT PANEL (Character & Equip) ────────────────────────────────────
    "left_panel/panel_bg.png":            (  14, 448,  200, 730),
    "left_panel/avatar_circle.png":       ( 218, 462,  415, 625),
    "left_panel/silhouette.png":          ( 228, 628,  415, 748),

    # Slot icons (individual equipment type icons)
    "left_panel/slot_head.png":           ( 432, 452,  510, 530),
    "left_panel/slot_chest.png":          ( 432, 538,  510, 614),
    "left_panel/slot_glove.png":          ( 432, 620,  510, 698),
    "left_panel/slot_boot.png":           ( 432, 704,  510, 748),  # cut at boundary if needed
    "left_panel/slot_weapon.png":         ( 432, 636,  505, 716),  # adjust if overlapping
    "left_panel/slot_empty.png":          ( 432, 722,  505, 748),

    # ── 4. INVENTORY GRID (Center) ────────────────────────────────────────────
    "inventory/grid_bg.png":              ( 525, 438,  865, 748),
    "inventory/slot.png":                 ( 876, 430,  990, 535),
    "inventory/slot_hover.png":           ( 876, 548,  990, 650),
    "inventory/slot_selected.png":        ( 876, 660,  990, 748),

    # ── 5. RIGHT PANEL (Info / Tooltip) ──────────────────────────────────────
    "right_panel/panel_bg.png":           (1022, 440, 1200, 748),
    "right_panel/header_bar.png":         (1215, 440, 1530, 510),
    "right_panel/divider_line.png":       (1215, 528, 1530, 570),
    "right_panel/text_area.png":          (1215, 584, 1530, 748),

    # ── 6. BUTTONS & UI ELEMENTS ──────────────────────────────────────────────
    "ui/btn_close.png":                   (  15, 762,  110, 850),
    "ui/btn_close_hover.png":             ( 120, 762,  215, 850),
    "ui/btn_close_pressed.png":           ( 228, 762,  325, 850),
    "ui/tab_button.png":                  (  15, 872,  170, 945),
    "ui/tab_active.png":                  ( 185, 872,  340, 945),

    # ── 7. DECORATIONS & EFFECTS ──────────────────────────────────────────────
    "effects/rune_top_left.png":          ( 350, 752,  480, 808),
    "effects/rune_top_right.png":         ( 510, 752,  640, 808),
    "effects/ornament_center_top.png":    ( 650, 748,  930, 808),
    "effects/rune_bottom_left.png":       ( 350, 820,  480, 878),
    "effects/rune_bottom_right.png":      ( 510, 820,  640, 878),
    "effects/ornament_center_bottom.png": ( 650, 820,  930, 878),
    "effects/glow_border.png":            ( 950, 752, 1080, 1010),
    "effects/light_overlay.png":          (1095, 752, 1235, 1010),
    "effects/dust_particles.png":         (1250, 752, 1395, 1010),
    "effects/magic_glow.png":             (1405, 752, 1530, 1010),
}

# ─────────────────────────────────────────────────────────────────────────────
# CROP & SAVE
# ─────────────────────────────────────────────────────────────────────────────
ok = 0
fail = 0

for rel_path, box in CROPS.items():
    l, t, r, b = box
    # Clamp to image bounds
    l, t = max(0, l), max(0, t)
    r, b = min(W, r), min(H, b)
    
    if r - l < 4 or b - t < 4:
        print(f"  [SKIP] {rel_path}: vùng quá nhỏ {r-l}x{b-t}")
        fail += 1
        continue

    cropped = img.crop((l, t, r, b))

    # Save to Unity Resources/Sprites
    unity_path = os.path.join(OUT, rel_path.replace("/", os.sep))
    os.makedirs(os.path.dirname(unity_path), exist_ok=True)
    cropped.save(unity_path, "PNG")

    # Also save to AI_Generated_Sprites/UI_Sliced (backup)
    backup_path = os.path.join(BACKUP, rel_path.replace("/", os.sep))
    os.makedirs(os.path.dirname(backup_path), exist_ok=True)
    cropped.save(backup_path, "PNG")

    print(f"  [OK] {rel_path:50s}  {r-l}x{b-t}px")
    ok += 1

# Clean up test files
import glob
for f in glob.glob(r'f:\WORK\GAME\RPG_Unity_Scripts\AI_Generated_Sprites\_test_*.png'):
    os.remove(f)

print(f"\n{'='*60}")
print(f"  XONG: {ok} assets da cat | {fail} bi bo qua")
print(f"  Unity: {OUT}")
print(f"  Backup: {BACKUP}")
print(f"{'='*60}")
print("\nLuu y: Vao Unity -> Project -> chuot phai -> Refresh de nhan dien files moi!")
