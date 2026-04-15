# HƯỚNG DẪN CHI TIẾT CÁC BƯỚC ĐÃ LÀM (WALKTHROUGH)
*(Dùng để đồng bộ và xem lại trên mọi máy tính)*

## 1. CÁCH LẮP RÁP CƠ BẢN TRONG UNITY
- **Gắn Scripts:** Kéo thả các file `.cs` (PlayerStats, Monster, v.v.) vào mục **Inspector** của đối tượng tương ứng trên cột **Hierarchy**.
- **Cơ chế Prefab (Bản mẫu):** Nắm kéo đối tượng từ Hierarchy xuống bảng **Project** để biến nó thành cục màu xanh dương. Sau này dùng nó để đẻ ra vô tận (như Vàng, Quái vật).

## 2. HƯỚNG DẪN ĐỒ HỌA (ÁO MỚI)
- **Texture Type:** Chọn ảnh trong Project -> Inspector -> Texture Type -> Đổi thành **Sprite (2D and UI)** -> Bấm **Apply**.
- **Gán hình:** Kéo ảnh đã đổi Sprite vào ô **Sprite** của thành phần `Sprite Renderer` trên nhân vật.

## 3. BÍ QUYẾT LÀM ANIMATION (ẢNH ĐỘNG)
- **Không dùng GIF:** Game dùng **Sprite Sheet** (Băng phim dài).
- **Cắt ảnh:** Trong Inspector, chọn Sprite Mode: **Multiple** -> Bấm **Sprite Editor** -> Slice -> Gõ số lượng khung hình -> Apply.
- **Tạo Animation:** Quét chọn các miếng ảnh nhỏ đã cắt -> Kéo thả vào nhân vật -> Lưu tên File (vd: `HiepSi_Walk`).
- **Nối dây (Animator):** Mở cửa sổ **Animator** -> Nối dây giữa các trạng thái (Idle -> Walk) -> Cài đặt tham số (Parameter) là `Speed` hoặc `Attack` để gọi lệnh từ C# code.

### 3.1. HƯỚNG DẪN HOẠT ẢNH ĐỨNG YÊN (IDLE) - CHI TIẾT
Để nhân vật đứng lại ngay lập tức khi buông phím, bạn làm như sau:
- **Tạo File:** Quét chọn 4 khung hình đứng yên -> Thả vào Player -> Đặt tên `HiepSi_Idle`.
- **Cài đặt dây nối (Transitions):**
    - **Từ Idle sang Run:** Chọn dây nối -> Bảng Inspector -> Conditions: Bấm dấu [+] chọn **Speed > 0.1**.
    - **Từ Run sang Idle:** Chọn dây nối -> Bảng Inspector -> Conditions: Bấm dấu [+] chọn **Speed < 0.1**.
- **MẸO QUAN TRỌNG:** Ở cả 2 sợi dây trên, bạn phải **BỎ TÍCH** ô **Has Exit Time**. Nếu để tích ô này, nhân vật sẽ bị "trượt" một đoạn rồi mới đứng lại hoặc chạy.

## 4. HƯỚNG DẪN TÚI ĐỒ & TRANG BỊ
- **Mở túi:** Nhấn phím **B** khi đang chơi để mở bảng ma thuật màu đen.
- **Trang bị:** Nhấp chuột vào nút "Trang bị: [Tên Kiếm]" để tăng Lực Chiến (Attack Damage) ngay lập tức.
- **Quy tắc rớt đồ:** Quái chết rớt đồ hên xui. Kẹo vàng (70%) cộng EXP, Kiếm (30%) chui vào túi đồ.

## 5. HƯỚNG DẪN CÀI ĐẶT CAMERA (THEO CHÂN NGƯỜI CHƠI)
Để bản đồ tự di chuyển theo nhân vật, bạn cần làm đúng các bước sau:
- **Bước 1:** Trong cột **Hierarchy** (bên trái), hãy nhấn chọn vào đối tượng **Main Camera**.
- **Bước 2:** Trong bảng **Project** (dưới cùng), tìm file script tên là **`CameraFollow`**.
- **Bước 3:** Nắm kéo file script `CameraFollow` đó thả trực tiếp vào ô **Main Camera** ở bước 1 (hoặc thả vào bảng Inspector của nó).
- **Bước 4 (Tinh chỉnh):** Sau khi gắn xong, bạn nhìn sang bảng Inspector của Camera sẽ thấy các ô:
    - **Smooth Speed:** Độ mượt. (Để 0.125 là chuẩn mượt, số càng nhỏ càng trễ).
    - **Offset:** Vị trí tương đối. (Mặc định là Z = -10 để camera nhìn từ xa vào).

## 6. DANH SÁCH KIỂM TRA NHANH (CHECKLIST "KÉO THẢ")
Để game chạy 100% sức mạnh mà không bị lỗi, bạn hãy dành 2 phút để kiểm tra 5 mục "quốc hồn quốc túy" này nhé:

- [ ] **1. Tab Animator (Quan trọng nhất):** Phải có 2 Parameter là **`Speed`** (Float) và **`Attack`** (Trigger). Nếu thiếu, Hiệp sĩ sẽ đứng im như tượng đá.
- [ ] **2. Script Lò Đẻ (MonsterSpawner):** Chọn lò đẻ trên Hierarchy -> Nhìn bảng Inspector -> Kéo con Quái mẫu (Prefab) vào ô **Monster Prefab**.
- [ ] **3. Script Rơi Đồ (Monster):** Chọn con Quái (Prefab) -> Inspector -> Tìm ô **Item Drop Prefab** -> Kéo cục Vàng mẫu vào đó để khi chết nó có cái mà rớt ra.
- [ ] **4. Script Bám Đuôi (Main Camera):** Như đã nói ở trên, phải kéo script **`CameraFollow`** vào Camera.
- [ ] **5. Độ Trong Suốt (Sprites):** Chọn tất cả ảnh nhân vật/quái vật trong Project -> Tích vào ô **Alpha Is Transparency** -> Bấm **Apply** để xóa nền trắng.

---
> [!IMPORTANT]
> **Mẹo nhỏ:** 
> Nếu bạn thấy lỗi "Missing Script" màu vàng ở Inspector, hãy xóa component đó đi và kéo lại script từ bảng Project vào là xong!

## 7. HƯỚNG DẪN CHI TIẾT NPC & ĐỆ TỬ (MỚI)

### 7.1. NPC BÁN ĐỒ (`NPC_ShopKeeper`)
- **Gắn Script:** `NPCShop.cs`.
- **Nhiệm vụ:** Quản lý kho hàng 14 vật phẩm (Máu, Kiếm, Áo giáp, Ngọc).
- **Lưu ý:** Tầm tương tác (`Interact Range`) nên để 2.5. Bạn chỉ cần đứng gần và bấm phím **E** để mở quầy hàng.

### 7.2. NPC HUẤN LUYỆN (`NPC_Trainer`)
- **Gắn Script:** `TrainerNPC.cs` và `NPCAnimation.cs`.
- **Nhiệm vụ:** Hiện menu mua đệ tử. 
- **Bước quan trọng:** Bạn phải mở Inspector của NPC này, tìm 3 ô: `Warrior Prefab`, `Archer Prefab`, `Slime Prefab` và kéo 3 bản mẫu đệ tử tương ứng vào đó.

### 7.3. CÁCH LÀM ANIMATOR CHO ĐỆ TỬ
Để đệ tử biết đánh, biết chạy đúng lúc, bạn mở cửa sổ **Animator** của chúng:
1. **Parameters:** Tạo một **Bool** tên là `IsWalking` và một **Trigger** tên là `Attack`. (Phải viết đúng chính tả!).
2. **Nối dây từ Idle sang Run:** Điều kiện: `IsWalking = true`.
3. **Nối dây từ Run về Idle:** Điều kiện: `IsWalking = false`.
4. **Hành động Đánh:** Nối từ **Any State** tới clip `Attack`. Điều kiện: `Attack` (Trigger). 
5. **Dây nối ngược:** Nối từ `Attack` về `Idle`. Nhớ **TÍCH** vào **Has Exit Time** để nó chém xong mới thôi.

## 8. XƯỞNG CHẾ TẠO ĐỆ TỬ (COMPANION WORKSHOP) - TỪNG BƯỚC MỘT

Bạn hãy làm theo đúng bảng danh sách này để tạo ra một đệ tử hoàn hảo:

### 📦 PHẦN 1: THIẾT LẬP THÂN XÁC (OUTSIDE)
- [ ] **Bước 1:** Kéo ảnh (Sprite) của đệ tử vào **Hierarchy**. Đặt tên (vd: `Companion_Archer`).
- [ ] **Bước 2:** Chỉnh **Sorting Layer** thành cùng layer với Player để không bị che khuất.

### 🧠 PHẦN 2: THIẾT LẬP BỘ NÃO (SCRIPTS)
- [ ] **Bước 3: Gắn script `PlayerStats.cs`**
  - [ ] **CỰC KỲ QUAN TRỌNG:** Bỏ tích ô **`Is Player`**.
- [ ] **Bước 4: Gắn script `CompanionAI.cs`**
  - [ ] Ở ô **Type**, chọn đúng: `Warrior`, `Archer`, hoặc `Slime`.

### 🎬 PHẦN 3: THIẾT LẬP ĐỘNG TÁC (ANIMATOR)
- [ ] **Bước 5:** Trong cửa sổ **Animator**, tạo 2 Parameters:
   1.  **Bool**: `IsWalking`
   2.  **Trigger**: `Attack`
- [ ] **Bước 6:** Nối dây:
   - [ ] Idle <-> Run (Dùng `IsWalking`).
   - [ ] Any State -> Attack (Dùng `Attack`).

### 🚀 PHẦN 4: XUẤT XƯỞNG (PREFAB)
- [ ] **Bước 7:** Kéo đệ tử từ Hierarchy xuống Project để tạo **Prefab**.
- [ ] **Bước 8:** Chọn **`NPC_Trainer`** -> Kéo Prefab vào ô tương ứng trong Inspector.

---

### 7.4. PHÂN LOẠI ĐỆ TỬ (COMPANION TYPE)
Trên mỗi đệ tử, tìm script **`CompanionAI`**:
- **Warrior:** Tự động áp sát quái và chém AOE.
- **Archer:** Tự động đứng từ xa bắn cung (tầm bắn 6m).
- **Slime:** Đi chậm, phản dame và làm tanker cho bạn.