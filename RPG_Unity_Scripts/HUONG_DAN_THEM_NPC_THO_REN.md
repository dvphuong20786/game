# Hướng dẫn thêm NPC Thợ Rèn vào Game

Chào bạn! Đây là các bước để bạn đưa Lão Thợ Rèn mới vào game cùng với bộ hoạt ảnh gõ búa sinh động.

## 1. Chuẩn bị Hình ảnh (Sprites)
Các hình ảnh đã được tôi lưu vào thư mục: `AI_Generated_Sprites\`
- `blacksmith_npc_sprite_standing.png`: Đứng yên.
- `blacksmith_npc_animation_hammer_up.png`: Giơ búa.
- `blacksmith_npc_animation_hammer_hit.png`: Gõ búa (có tia lửa).

### Cài đặt Sprite trong Unity:
1. Chọn cả 3 ảnh trong cửa sổ Project.
2. Tại bảng **Inspector**, chỉnh các thông số sau:
   - **Texture Type**: Sprite (2D and UI)
   - **Pixels Per Unit**: 100
   - **Filter Mode**: Point (no filter)
3. Nhấn **Apply**.

## 2. Tạo GameObject NPC
1. Trong cửa sổ **Hierarchy**, chuột phải chọn `Create Empty`, đặt tên là **NPC_Blacksmith**.
2. Thêm Component **Sprite Renderer**:
   - Kéo ảnh `blacksmith_npc_sprite_standing` vào ô **Sprite**.
3. Thêm Component **Box Collider 2D**:
   - Tích chọn **Is Trigger**.
4. Kéo script `NPCBlacksmith.cs` vào GameObject này.

## 3. Tạo Hoạt ảnh (Animation)
1. Mở cửa sổ **Window > Animation > Animation**.
2. Chọn `NPC_Blacksmith`, nhấn **Create** (đặt tên là `Blacksmith_Working`).
3. Kéo thả 3 khung hình vào timeline: `standing` -> `hammer_up` -> `hammer_hit`.
4. Chỉnh tốc độ (Samples) khoảng 8 để hành động mượt mà.

---
![Thợ Rèn Đứng](file:///f:/WORK/GAME/RPG_Unity_Scripts/AI_Generated_Sprites/blacksmith_npc_sprite_standing_1776595001874.png)
![Thợ Rèn Gõ Búa](file:///f:/WORK/GAME/RPG_Unity_Scripts/AI_Generated_Sprites/blacksmith_npc_animation_hammer_hit_1776595046082.png)
