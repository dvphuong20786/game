# 🏠 Hướng dẫn: Xây dựng Scene Làng (Village) Từng Bước

Làng là "hậu phương" quan trọng. Tại đây chúng ta sẽ thực hiện cơ chế **Prefab**, giúp bạn chỉ cần thiết lập nhân vật 1 lần và dùng được cho mọi bản đồ.

---

## 📦 Phần 1: Biến đối tượng thành Prefab (Vật mẫu)

Đây là bước cực kỳ quan trọng để nhân vật của bạn có thể "di chuyển" từ map này sang map khác mà không bị mất chỉ số.

- [x] **Bước 1.1: Tạo Prefab cho Nhân vật (HiepSi)**
    - Trong Scene map Rừng (Wilderness), chọn đối tượng **HiepSi** đang có đủ các script (`PlayerStats`, `PlayerMovement`...).
    - Kéo đối tượng **HiepSi** từ Hierarchy vào thư mục `Prefabs` trong cửa sổ Project. 
    - *Bây giờ, HiepSi đã trở thành một "khuôn mẫu". Bạn có thể kéo nó vào bất kỳ Map nào.*

- [x] **Bước 1.2: Tạo Prefab cho GameUI và Camera**
    - Làm tương tự: Kéo đối tượng **GameUI** vào thư mục Prefabs.
    - Làm tương tự: Kéo **Main Camera** (đã gắn `CameraFollow.cs`) vào thư mục Prefabs.

---

## 🏗️ Phần 2: Thiết lập Scene Village (Làng)

- [x] **Bước 2.1: Tạo Scene mới**
    - File -> New Scene -> Basic 2D. Lưu tên là `Village`.
    - **Quan trọng**: Vào **File -> Build Settings**, nhấn **Add Open Scenes**. Đảm bảo cả `Village` và `Wilderness` đều có trong danh sách.

- [ ] **Bước 2.2: Xây nền Làng tự động (Nhanh nhất)**
    1. Chuẩn bị: Trên Hierarchy, chuột phải chọn **Create Empty**, đặt tên là `Village_Base`.
    2. Gắn Script: Kéo file `VillageBuilder.cs` thả vào `Village_Base`.
    3. Gán ảnh: Trong Inspector của `Village_Base`, hãy kéo các file sau vào ô tương ứng:
        - Ô **Floor Prefab**: Kéo ảnh **Cỏ** (hoặc gạch) vào.
        - Ô **Fence Prefab**: Kéo ảnh **Fence** vào.
        - Ô **Tree Prefab**: Kéo ảnh **Tree** vào.
    4. Nhấn **Play**: Làng sẽ tự động lót nền 20x20 và xây hàng rào bao quanh cho bạn!

- [x] **Bước 2.3: Đưa các "diễn viên" vào làng**
    - Kéo Prefab **HiepSi** vào Hierarchy.
    - Kéo Prefab **GameUI** vào Hierarchy.
    - Kéo Prefab **Main Camera** vào Hierarchy.
    - *Lúc này bạn nhấn Play, bạn đã có thể điều khiển nhân vật chạy trong màn hình xanh!*

- [x] **Bước 2.4: Trang trí & Tương tác**
    - Kéo ảnh `House`, `Well` vào giữa làng.
    - Đừng quên thêm **Box Collider 2D** cho nhà và giếng.
    - Kéo NPC và Cổng Portal vào (như hướng dẫn phần 3).

- [x] **Bước 2.5: Tạo Cổng Dịch Chuyển (Portal)**
    1. Chuẩn bị ảnh: Sử dụng ảnh `MagicPortal` trong thư mục `AI_Generated_Sprites`.
    2. Tạo đối tượng: Kéo ảnh vào Scene, đặt tên là `Portal_To_Wilderness`.
    3. Thêm va chạm: Bấm **Add Component** -> **Box Collider 2D**.
    4. **Quan trọng**: Tích chọn ô **Is Trigger** trong Box Collider 2D.
    5. Gắn Script: Kéo file `Portal.cs` vào đối tượng này.
    6. Nhập tên Map: Trong ô **Ten Map Tiep Theo**, gõ chính xác: `Wilderness`.
    *(Làm tương tự ở Map Wilderness để quay về Làng, nhưng gõ tên là `Village`)*.

---

## 🤝 Phần 3: Bố trí NPC và Cổng truyền tống

- [x] **Bước 3.1: Đặt NPC Shop và Trainer**
    - Kéo ảnh NPC của bạn vào làng.
    - Gắn script `NPCShop.cs` cho NPC bán đồ.
    - Gắn script `NPCTrainer.cs` cho NPC huấn luyện đệ tử.

- [x] **Bước 3.2: Đặt Cổng đi Rừng**
    - Kéo Prefab **Portal** vào một vị trí ở bìa làng.
    - Trong script Portal, ô `Ten Map Tiep Theo`, hãy gõ: `Wilderness`.

- [x] **Bước 3.3: Kiểm tra cổng quay về**
    - Mở lại Scene `Wilderness`.
    - Chọn đối tượng `MapManager`.
    - Đảm bảo trong code hoặc Inspector, Cổng thoát (Exit Portal) được thiết lập tên map tới là: `Village`.

---

### 💡 Gợi ý thiết kế:
- Hãy dùng các tấm `Fence` (Hàng rào) để bao quanh làng, tạo cảm giác an toàn.
- Đặt cổng `Portal` ở một lối mòn nhỏ dẫn ra ngoài làng.

---
*Cảm ơn bạn đã tin tưởng Antigravity! Ngôi làng của bạn đang dần hình thành rồi đấy.*
