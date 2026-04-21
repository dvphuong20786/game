# 📜 NHẬT KÝ TIẾN ĐỘ PHÁT TRIỂN RPG 2D (TỔNG HỢP TOÀN DIỆN)

Đây là file ghi chép tất cả các cột mốc quan trọng của dự án. **Không xóa bất kỳ mục nào**, chỉ bổ sung thêm để bạn và tôi theo dõi toàn bộ quá trình.

---

## 🎮 DANH SÁCH VIỆC CẦN LÀM TRONG UNITY (CẬP NHẬT MỚI NHẤT)

> **Đây là việc của BẠN trong phần mềm Unity — Tôi không làm thay được!**
> Đánh dấu `[x]` khi làm xong từng ô để theo dõi.

---

### 🔴 NHÓM A — PHẢI LÀM TRƯỚC KHI CHƠI (Bắt buộc)

#### A1. Gắn Script vào Nhân Vật Chính (HeroKnight / Player)
Chọn nhân vật trên **Hierarchy** → kéo thả từng file `.cs` từ **Project** vào **Inspector**:
- [x] Kéo `PlayerStats.cs` → vào Player
- [x] Kéo `PlayerMovement.cs` → vào Player
- [x] Kéo `PlayerCombat.cs` → vào Player

#### A2. Gắn Camera Theo Chân Player
- [x] Chọn **Main Camera** trên Hierarchy
- [x] Kéo `CameraFollow.cs` thả vào Inspector của Main Camera
- [x] *(Tự động tìm Player, không cần kéo thêm gì)*

#### A3. Tạo GameObject Trống cho GameUI
- [x] Trên thanh Menu Unity: **GameObject → Create Empty** → đặt tên là `GameUI`
- [x] Kéo `GameUI.cs` thả vào GameObject vừa tạo
- [x] *(Script này vẽ thanh HP, EXP, Vàng, và bảng kho đồ)*

#### A4. Gắn Script vào Quái Vật (Slime / Monster)
Chọn con Slime **Prefab** trong bảng Project → mở Inspector:
- [x] Kéo `Monster.cs` → vào Slime Prefab
- [x] Trong ô **Item Drop Prefab** của script Monster → kéo cục **Vàng Prefab** vào

#### A5. Tạo Lò Đẻ Quái (MonsterSpawner)
- [x] **GameObject → Create Empty** → đặt tên là `MonsterSpawner`
- [x] Kéo `MonsterSpawner.cs` → vào GameObject vừa tạo
- [x] Trong Inspector: kéo **Slime Prefab** vào ô **Monster Prefab**

#### A6. Thiết lập Rừng Ngẫu Nhiên (Map 50x50)
- [x] Thực hiện theo hướng dẫn chi tiết tại: [HUONG_DAN_TAO_MAP_TUNG_BUOC.md](file:///f:/WORK/GAME/RPG_Unity_Scripts/HUONG_DAN_TAO_MAP_TUNG_BUOC.md)
- [x] Đặt tên Scene mới là `Wilderness`
- [x] Gán đủ 4 loại tài nguyên (Cỏ, Đất, Cây, Đá) vào MapManager.

#### A7. Xây dựng Scene Làng (Village)
- [ ] Thực hiện theo hướng dẫn: [HUONG_DAN_TAO_LANG_VILLAGE.md](file:///f:/WORK/GAME/RPG_Unity_Scripts/HUONG_DAN_TAO_LANG_VILLAGE.md)
- [ ] Biến Player, UI, Camera thành Prefab.
- [ ] Sắp xếp Nhà, Giếng, Hàng rào và NPC.
- [x] **Tạo Boss Prefab** — Làm theo các bước sau:
  - [x] Kéo Slime Prefab từ Project thả vào **Hierarchy** (tạo 1 bản trong scene)
  - [x] Đổi tên thành `Boss_Slime`
  - [x] Trong Inspector → **Transform → Scale** đổi thành `3, 3, 1` (Boss to hơn)
  - [x] Trong Inspector → **SpriteRenderer → Color** đổi sang màu **đỏ đậm** để phân biệt
  - [x] Kéo `SmartBoss.cs` thả vào `Boss_Slime` (kéo thêm cùng Monster.cs đã có)
  - [x] Trong ô **Summon Prefab** của SmartBoss → kéo Slime Prefab thường vào (Boss sẽ triệu hồi quái nhỏ!)
  - [x] Kéo `Boss_Slime` từ Hierarchy xuống bảng **Project** để tạo thành **Prefab màu xanh**
  - [x] Xóa `Boss_Slime` khỏi Hierarchy (đã có Prefab rồi)
- [x] Trong Inspector của **MonsterSpawner** → kéo **Boss_Slime Prefab** vào ô **Boss Prefab**

#### A6. Cài đặt Animator cho Nhân Vật (Quan trọng nhất!)
Nếu không làm bước này, nhân vật sẽ đứng im như tượng đá:
- [x] Chọn Player → mở cửa sổ **Animator** (Window → Animation → Animator)
- [x] Vào Tab **Parameters** ở góc trái → bấm dấu **[+]**
  - [x] Tạo 1 biến **Float** tên là: `Speed` *(viết hoa S)*
  - [x] Tạo 1 biến **Trigger** tên là: `Attack` *(viết hoa A)*
- [x] Nối dây: **Idle → Run** với điều kiện `Speed > 0.1` + **tắt Has Exit Time**
- [x] Nối dây: **Run → Idle** với điều kiện `Speed < 0.1` + **tắt Has Exit Time**
- [x] Nối dây: **AnyState → Attack** với điều kiện `Attack` Trigger

---

### 🟡 NHÓM B — CÀI ĐẶT KHI CÓ THÊM TÀI NGUYÊN

#### B1. NPC Bán Đồ (Cần làm sau khi tôi code xong NPCShop.cs)
- [x] Tạo 1 nhân vật 2D đơn giản (hình vuông/tròn màu xanh) đặt trên bản đồ → đặt tên `NPC_ShopKeeper`
- [x] Kéo `NPCShop.cs` → vào đối tượng NPC đó
- [x] *(Người chơi đứng gần + bấm E sẽ mở cửa hàng)*

#### B2. Cắt Ảnh Hoạt Ảnh (Sprite Animation)
Làm bước này nếu bạn có bộ ảnh nhân vật:
- [ ] Chọn ảnh trong Project → Inspector → **Sprite Mode: Multiple** → **Sprite Editor**
- [ ] Vào **Slice** → cắt theo số cột/hàng → bấm **Apply**
- [ ] Quét chọn các frame **đứng yên** → kéo vào Player → đặt tên `Player_Idle`
- [ ] Quét chọn các frame **chạy** → kéo vào Player → đặt tên `Player_Run`
- [ ] Quét chọn các frame **đánh** → kéo vào Player → đặt tên `Player_Attack`

#### B3. Bật Trong Suốt cho Ảnh Nhân Vật
- [ ] Chọn tất cả ảnh nhân vật/quái vật trong bảng Project
- [ ] Inspector → tích vào ô **Alpha Is Transparency** → bấm **Apply**

#### B4. Hệ thống Đệ Tử (Companion AI)
- [ ] Tạo 1 đối tượng mới (ví dụ: nhân vật tròn hoặc hiệp sĩ khác) đặt vào scene.
- [ ] Đổi tên thành `Companion_DeTu`.
- [ ] Thêm thành phần **Sprite Renderer** (nếu chưa có).
- [ ] Kéo file script **`CompanionAI.cs`** thả vào nhân vật này.
- [ ] *(Đệ tử sẽ tự đổi sang màu Xanh Dương khi chạy game, tự động bảo vệ bạn khi có quái).*

#### B5. NPC Huấn Luyện (Trainer) - [CẦN BẠN KÉO THẢ]
- [x] Tạo GameObject đặt tên là `NPC_Trainer`.
- [x] Gán script `TrainerNPC.cs`.
- [ ] **Việc bạn phải làm:** Mở Inspector của `NPC_Trainer`:
    - [ ] Kéo **Warrior Prefab** vào ô tương ứng.
    - [ ] Kéo **Archer Prefab** vào ô tương ứng.
    - [ ] Kéo **Slime Prefab** vào ô tương ứng.
    - [ ] Nắm 1 file âm thanh (AudioClip) thả vào ô **Hire Sound**.
- [ ] **Thiết lập di chuyển:** Tích vào **Can Patrol**, chỉnh **Move Speed** = 1.5, **Patrol Distance** = 4.
- [ ] *(Xem hướng dẫn chi tiết tại HUONG_DAN_CHI_TIET_GAME.md mục 7.2 & 9)*.

---

### 🟢 NHÓM C — CÁC VIỆC ĐÃ LÀM XONG (DO BẠN THỰC HIỆN)

1. ✅ Đã đưa Map vào game (Tilemap).
2. ✅ Đã đưa mẫu Hero Knight vào Hierarchy.
3. ✅ Đã tạo ra các cục Prefab cho quái (Slime).

---

## ✅ LỊCH SỬ CODE ĐÃ XONG (DO TÔI VIẾT)

### 🎨 Phiên bản 21/04/2026 — Hoàn thành tích hợp 100% Asset Sliced
- **Hệ thống Asset Slicer:** Tự động cắt 45 layer từ file ảnh sheet gốc thành các assets riêng lẻ trong chuyên mục `Resources/Sprites`.
- **Background 4 Layer:** Dung hợp Dungeon → Light Overlay → Vignette → Particles tạo không gian 3D.
- **Frame 9-Slice:** Dùng code tính toán vẽ 4 góc, 4 cạnh và lòng khung để đảm bảo giao diện không bị méo khi co giãn.
- **Left Panel Pro:** 
    - Silhouette nhân vật hiện mờ sau lưới trang bị 3x3.
    - Avatar Circle bọc quanh chân dung nhân vật.
    - 9 Slot trang bị dùng đúng icon placeholder (Mũ, Áo, Găng...) từ assets đã cắt.
- **Inventory Pro:** Grid 4 cột, sử dụng `grid_bg.png` và hệ thống slot 3 trạng thái (Normal, Hover, Selected).
- **Tooltip Pro:** Header bar, Divider bar và Text area nền đá cổ cực đẹp.
- **Hiệu ứng Runes & Ornaments:** Trang trí Rune cổ ở 4 góc và họa tiết đầu/cuối của Master Panel.
- **Bút Close & Tabs:** Tích hợp bộ nút bấm theo phong cách gothic.

### 🎨 Phiên bản 21/04/2026 — Sửa lỗi Giao diện & Thêm ô Vòng cổ
- **Tái cấu trúc 3x3:** Sắp xếp lại 9 ô trang bị theo lưới 3x3 cân xứng, giúp các ô Mũ, Giáp, Ủng nằm đúng vị trí đầu, thân, chân của nhân vật.
- **Thêm ô Vòng cổ:** Bổ sung slot `AncientGold` (Vòng cổ) bị thiếu trong giao diện Pro (B).
- **Hình nền Master Panel v3:** Tạo và tích hợp ảnh nền mới có hình bóng nhân vật được căn giữa chuẩn xác, khớp 100% với các ô trang bị.
- **Cải thiện hiển thị:** Tăng độ tương phản cho các chữ gợi ý trong slot và cập nhật nhãn "Dây chuyền", "Vòng cổ" rõ ràng.
- **Auto-Equip:** Hệ thống tự động nhận diện vật phẩm có tên "vòng cổ" để mặc vào đúng vị trí mới.

### 🐾 Giai đoạn Đệ Tử - RPG Companion (Cao cấp)
- **Đệ tử như Người chơi:** Có đầy đủ 9 ô trang bị, túi đồ riêng, bảng kỹ năng và thăng cấp bằng EXP.
- **Trí tuệ nhân tạo:** Tự động đi theo, tự tìm quái và **tự dùng tuyệt chiêu AOE** khi cần.
- **Quản lý linh hoạt:** Trong bảng Túi đồ (phím B) có nút chuyển đổi giữa Người chơi và Đệ tử.
- **Tính năng chuyển đồ:** Nút "Giao cho đệ tử" giúp nhanh chóng đưa trang bị tốt cho đồng đội.
- **Kinh tế:** Đệ tử nhặt vàng sẽ tự động cộng dồn vào túi tiền chung của Người chơi.
- **Trang bị trực tiếp:** Nút "Trang bị cho đệ" giúp mặc đồ thẳng từ túi người chơi cho đệ tử, cực kỳ tiện lợi.
- **Hệ thống Hào quang & Hồi máu:** Đệ tử có vòng sáng dưới chân và tự hồi 2% HP mỗi 3s khi rảnh rỗi.

### 🕳️ Giai đoạn Lỗ Quái Vật & AI Nâng cao (Cập nhật)
- **Quái vật Đi tuần:** Quái vật sinh ra từ lỗ sẽ đi lại tuần tra xung quanh lỗ thay vì đứng im.
- **Tầm đánh Kiếm sĩ:** Điều chỉnh chỉ còn **1.2f** để tăng độ khó và thực tế.
- **Fix lỗi hiển thị:** Sửa lỗi đệ tử bị tàng hình khi vừa thuê (Z-index & Sorting Layer).
- **AI Đệ tử:** Tầm quét quái tăng lên 12m, thông minh hơn trong việc chọn mục tiêu.

### 🌐 Hệ thống Thế giới Ngẫu nhiên (Mới)
- **Sinh bản đồ 50x50:** Mỗi lần vào map "Wilderness", bản đồ sẽ được lắp ghép ngẫu nhiên từ 2500 ô gạch.
- **Tự động đặt vật cản:** Cây cối và đá được rải ngẫu nhiên, tạo ra các lối đi khác nhau mỗi lần chơi.
- **Lỗ quái vật ngẫu nhiên:** Sinh ra 8 "Monster Pit" tại các vị trí bất kỳ trong rừng.
- **AI Generated Sprites:** Các tài nguyên Cỏ, Đất, Cây, Đá được tạo bằng AI để sử dụng ngay.

### 🕳️ Giai đoạn Lỗ Quái Vật - Monster Pit (Mới)
- **Hệ thống Lỗ Quái:** Tự động sinh quái với số lượng cấu hình được (mặc định 4-5 con).
- **Trí tuệ Nhân tạo (AI Range):** Quái từ lỗ sẽ nhận diện mục tiêu ở tầm 10-12m. Nếu bị quái đuổi quá 20-24m (gấp đôi tầm nhìn), quái sẽ tự động quay về lỗ.
- **Tấn công Đệ tử:** Quái vật hiện đã biết tấn công cả Đệ tử nếu chúng ở gần hơn Người chơi.
- **Hệ thống Vật phẩm:** Toàn bộ item rớt ra đã được bổ sung đầy đủ chỉ số và mô tả trong kho đồ.

### 🔥 Phiên bản 15/04/2026 — Cập nhật 11 yêu cầu lớng RPG Cao cấp
- **9 Ô Trang bị (9 slots):** Đầu, Áo, Giày, Dây chuyền, Nhẫn 1, Nhẫn 2, Vàng cổ, Vũ khí chính, Khiên.
- **Logic 1 Tay & 2 Tay (1H/2H):** Cầm vũ khí Đại/2 Tay → tự tháo Khiên. Cầm 1 Tay → được dùng Khiên.
- **Hệ thống Khảm Ngọc (Socket System):** Ngọc Đỏ: +ATK, Ngọc Xanh: +DEF, Ngọc Tím: +HP.
- **Tuyệt Chiêu Theo Cấp Độ:** Cứ 3 Level học được 1 kỹ năng. Phím **1** = Chém Gió (x2 dame).
- **Giao diện Tab:** Tab TRANG BỊ và Tab KỸ NĂNG.
- **Hệ thống "An toàn & Chống lỗi":** Fix lỗi đứng game khi mặc/bán đồ quá nhanh.

### 🛡️ Giai đoạn Trước — Giao diện & Hiệu ứng
- **Chữ nhảy số (Damage Text):** Số đỏ khi bị đánh, số trắng khi chém quái.
- **Thanh cuộn (Scroll View):** Túi đồ không giới hạn.
- **Nút tắt [X]:** Đóng UI bằng chuột.
- **Camera Follow:** Script camera bám đuổi nhân vật.

### ⚔️ Giai đoạn Khởi đầu — Gameplay Cơ bản
- **Di chuyển & Chiến đấu:** Chạy 8 hướng, Chém thường (Space).
- **Lò đẻ quái vô tận:** Wave 10 quái → 1 Boss. Tăng độ khó theo thời gian.
- **Rơi đồ & Vàng:** Quái chết → rớt vàng (30%) hoặc đồ (10%). Tự hút khi đứng gần.

---

## 🔧 LỊCH SỬ SỬA LỖI

### Fix ngày 15/04/2026 — Sửa lỗi GameUI & API
- **Fix Game UI biến mất:** Thêm `TryFindPlayer()` tìm lại mỗi frame thay vì chỉ tìm 1 lần ở `Start()`.
- **Fix Damage Text không hiện:** Di chuyển `DrawFloatingDamage()` lên đầu `OnGUI()`, trước lệnh `return`.
- **Fix Camera null crash:** Thêm kiểm tra `Camera.main != null`.
- **Fix deprecated API Unity 6:** `FindObjectOfType` → `FindAnyObjectByType`, `FindObjectsOfType` → `FindObjectsByType`.
- **Cải thiện HUD:** Hiện "HP: X / MaxHP" thay vì chỉ "HP: X".
- **Files đã sửa:** `GameUI.cs`, `Monster.cs`, `PlayerCombat.cs`, `CameraFollow.cs`.

---

## 🚀 DANH SÁCH YÊU CẦU ĐANG LÀM (NGÀY 15/04/2026)

Đây là 11 yêu cầu từ file YÊU CẦU.md — đang được triển khai:

| # | Yêu cầu | Trạng thái | File |
|---|---------|-----------|------|
| 1 | Idle animation khi đứng yên | ⏳ Đang làm | `PlayerMovement.cs` |
| 2 | Camera di chuyển theo map | ⏳ Đang làm | `CameraFollow.cs` |
| 3 | Giảm tỉ lệ rớt đồ 10%, vàng 30% | ⏳ Đang làm | `ItemDrop.cs` |
| 4 | Item rớt ra sáng/nhấp nháy rõ hơn | ⏳ Đang làm | `ItemDrop.cs` |
| 5 | UI kho đồ có nút Sử dụng/Gỡ/Giá | ✅ Đã có | `GameUI.cs` |
| 6 | Chữ Vàng dễ đọc hơn | ⏳ Đang làm | `GameUI.cs` |
| 7 | 9 ô trang bị + Ngọc chỉ số | ✅ Đã có | `PlayerStats.cs` |
| 8 | Bảng kỹ năng cứ 3 Lv | ✅ Đã có | `GameUI.cs` |
| 9 | Đánh thường chỉ trúng 1 quái gần nhất | ⏳ Đang làm | `PlayerCombat.cs` |
| 10 | Giảm dame & chỉ số đồ cho cân bằng | ⏳ Đang làm | `PlayerStats.cs` |
| 11 | NPC bán đồ/bình máu/trang bị | ✅ Đã có | `NPCShop.cs` |
| 12 | HUD Đệ tử & Chỉ số chi tiết | ✅ Đã có | `GameUI.cs` |
| 13 | Hệ thống cộng điểm STR/VIT/AGI | ✅ Đã có | `PlayerStats.cs` |
| 14 | Chia sẻ 50/50 EXP & Focus Camera | ✅ Đã có | `Monster.cs` / `CameraFollow.cs` |
| 15 | Hệ thống Lỗ Quái Vật (Monster Pit) | ✅ Đã có | `MonsterPit.cs` |
| 16 | Quái tấn công cả đệ tử | ✅ Đã có | `Monster.cs` |
| 17 | Trang bị cho đệ từ túi người chơi | ✅ Đã có | `GameUI.cs` |
| 18 | Hào quang & Hồi máu đệ tử | ✅ Đã có | `CompanionAI.cs` |
| 19 | Quái vật đi tuần quanh lỗ | ✅ Đã có | `Monster.cs` |
| 20 | Fix lỗi đệ tử không hiện | ✅ Đã có | `NPCTrainer.cs` |
| 21 | Giảm tầm đánh kiếm sĩ 1.2f | ✅ Đã có | `PlayerCombat.cs` |
| 22 | Hệ thống sinh map ngẫu nhiên 50x50 | ✅ Đã có | `MapGenerator.cs` |
| 23 | Sprites Cỏ, Đất, Cây, Đá | ✅ Đã có | `AI_Generated_Sprites/` |
| 24 | Sprites Nhà, Giếng, Hàng rào | ✅ Đã có | `AI_Generated_Sprites/` |
| 25 | Hệ thống chuyển cảnh Portal | ✅ Đã có | `Portal.cs` |

---

## 🛡️ TIẾN ĐỘ NGOẠI LỆ: BOT TRADE VÀNG MT5 (21/04/2026)
- ⏳ **Đang kiểm tra**: Lỗi tín hiệu giả (Fakeout) và lỗi kẹt trạng thái `isBreakUp_H4` khiến Bot sai lệnh.
- 🔎 **Phân tích nguyên nhân**: 
  1. Trạng thái thủng S/R (ví dụ `IsPhaHoTro_H4`) được nhớ dưới dạng biến Global nhưng điều kiện Reset quá khắt khe, dẫn đến tình trạng nhớ mãi mãi nếu giá không bật hẳn qua EMA. Khi Restart MT5, biến Global reset về `false` nên lỗi tạm thời biến mất.
  2. Việc chỉ dùng 1 nến đóng cửa để xác nhận phá dải Hỗ trợ/Kháng cự gây ra các tín hiệu giả (Fakeout) khi rút râu.
- 🚧 **Chưa code**: Đã tìm ra lỗi, đang báo cáo User trước khi chỉnh sửa. Không có file nào cần kéo vào Unity.

---

> [!IMPORTANT]
> **Hướng dẫn sử dụng Kỹ năng:** Cày lên **Cấp 3**, mở bảng **Kỹ năng** (phím B), bấm nút **Học**, sau đó nhấn phím **số 1** để chém Chém Gió!

> [!TIP]
> **Mẹo nhanh:** Nếu thấy lỗi "Missing Script" màu vàng ở Inspector → xóa component đó → kéo lại script từ bảng Project vào là xong!