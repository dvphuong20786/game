# 🗺️ Hướng dẫn: Tự tạo Bản đồ Ngẫu nhiên từ A-Z

Chào mừng bạn đến với hướng dẫn thiết lập hệ thống bản đồ 50x50. Hãy tích vào từng mục sau khi bạn hoàn thành để theo dõi tiến độ.

---

## 📦 Phần 1: Chuẩn bị Tài nguyên (Assets)

- [x] **Bước 1.1: Chuyển ảnh thành Sprite**
    - Mở thư mục `AI_Generated_Sprites` trong Unity.
    - Chọn tất cả các file ảnh (`Grass`, `Dirt`, `Tree`, `Rock`).
    - Ở bảng **Inspector**, mục **Texture Type**, chọn: `Sprite (2D and UI)`.
    - Nhấn **Apply**.

- [x] **Bước 1.2: Tạo Prefab (Vật mẫu)**
    - Kéo ảnh `Grass` từ cửa sổ Project vào cửa sổ Hierarchy.
    - Kéo ngược đối tượng `Grass` từ Hierarchy vào một thư mục bất kỳ trong Project (ví dụ thư mục `Prefabs`).
    - Làm tương tự với `Dirt`, `Tree`, và `Rock`.

- [x] **Bước 1.3: Thêm Vật cản (Va chạm)**
    - Click đúp vào Prefab `Tree` trong cửa sổ Project để mở nó.
    - Nhấn **Add Component** -> Gõ **Box Collider 2D**.
    - Làm tương tự với Prefab `Rock`. (Điều này giúp nhân vật không đi xuyên qua cây/đá).

---

## 🏗️ Phần 2: Thiết lập Map (Scene) mới

- [x] **Bước 2.1: Tạo Scene mang tên Wilderness**
    - Vào menu **File** -> **New Scene** -> Chọn **Basic 2D** -> Nhấn **Create**.
    - Nhấn **Ctrl + S**, lưu vào thư mục `Scenes` với tên: `Wilderness`.

- [x] **Bước 2.2: Tạo Bộ quản lý Map**
    - Trong Scene `Wilderness`, chuột phải vào Hierarchy -> **Create Empty**.
    - Đổi tên thành: `MapManager`.
    - Kéo file `MapGenerator.cs` thả vào đối tượng `MapManager`.

- [x] **Bước 2.3: Gán Prefab vào Code**
    - Chọn `MapManager` trong Hierarchy.
    - Trong Inspector, kéo các Prefab bạn đã tạo ở Phần 1 vào đúng các ô:
        - `Floor Prefabs`: Kéo Grass/Dirt vào.
        - `Obstacle Prefabs`: Kéo Tree/Rock vào.
        - `Monster Pit Prefab`: Kéo lỗ quái vật của bạn vào.
        - `Exit Portal Prefab`: Kéo cổng dịch chuyển vào.

---

## 🔗 Phần 3: Kết nối Thế giới

- [ ] **Bước 3.1: Mở Scene Làng (Village)**
    - Mở Scene chính của bạn.
    - Tìm đối tượng **Portal** (Cổng dịch chuyển).
    - Trong mục `Ten Map Tiep Theo`, gõ chính xác: `Wilderness`.

- [ ] **Bước 3.2: Đăng ký Scene với hệ thống**
    - Vào menu **File** -> **Build Settings**.
    - Nhấn nút **Add Open Scenes**. Hãy đảm bảo cả map Làng và map `Wilderness` đều có tên trong danh sách này.

---

## 🏚️ CHƯƠNG 3: TẠO NGÔI LÀNG "Ổ CHUỘT KINH KỲ" (TRASHY VILLAGE)

Để tạo ra một ngôi làng vừa **Đông đúc** vừa **Nghèo khổ**, hãy áp dụng các kỹ thuật sau:

### 1. Kỹ thuật "Mê Cung Ngõ Ngách"
- **Đường đi**: Tránh làm đường thẳng tắp. Hãy đặt các ngôi nhà lệch nhau để tạo ra các lối đi uốn lượn, thắt nút cổ chai.
- **Mật độ**: Đặt các ngôi nhà sát nhau (khoảng cách 1-2 ô). Dùng các "mái hiên vá víu" để kết nối các mái nhà lại với nhau.

### 2. Thiết kế Props (Chi tiết nhỏ làm nên cái hồn)
- **Layer Details**: Rải rác các xe đẩy gãy bánh, thùng gỗ mục ở các góc đường.
- **Vệ sinh**: Đặt những đống rác hoặc bao tải cũ rách ở các góc khuất. Điều này tạo cảm giác chật hẹp và tiêu điều.

### 3. Vị trí NPC "Chiến thần nghèo"
- **Thợ Rèn**: Nên nằm ở một hẻm cụt, dưới một mái nhà lợp bằng tôn gỉ. Xung quanh hãy đặt những đống sắt gỉ.
- **Dân làng**: Đặt NPC ở các vị trí ngồi bệt xuống đất hoặc đứng tụ tập thành nhóm 2-3 người ở các ngã rẽ hẹp.

---

## 🏗️ CHƯƠNG 4: SỬ DỤNG CÔNG CỤ XÂY LÀNG TỰ ĐỘNG (VILLAGEBUILDER)

Nếu bạn không muốn kéo thả từng ngôi nhà, hãy sử dụng script **VillageBuilder** để tạo map cực nhanh:

### 1. Thiết lập đối tượng Builder
- Tạo một đối tượng Empty trong Scene của bạn, tên là `VillageManager`.
- Kéo script `VillageBuilder.cs` thả vào đối tượng này.

### 2. Gán Prefabs
- Trong Inspector của `VillageManager`, bạn sẽ thấy 2 danh sách:
    - **House Prefabs**: Kéo các mẫu nhà tranh/lều rách của bạn vào đây.
    - **Junk Prefabs**: Kéo các vật trang trí rác (thùng gỗ, bao tải...) vào đây.

### 3. Kích hoạt xây dựng
- Thiết lập **Village Size** (ví dụ 30) và **House Count** (số nhà bạn muốn, ví dụ 20).
- Chuột phải vào tiêu đề Component `Village Builder` (hoặc nhìn vào menu Context phía trên script).
- Nhấn **🏗️ GENERATE VILLAGE**: Làng sẽ tự động được lắp ghép!
- Nhấn **🧹 CLEAR VILLAGE**: Để xóa sạch và làm lại từ đầu với một sơ đồ mới.

---

### 💡 Lưu ý cho lập trình viên:
- Nếu bạn muốn map rộng hơn hoặc hẹp hơn, hãy chỉnh chỉ số **Width** và **Height** trong `MapManager`.
- Nếu muốn cây cối dày đặc hơn, hãy tăng chỉ số **Obstacle Density** (ví dụ 0.2 là dày, 0.05 là thưa).

---
*Hướng dẫn này được tạo tự động bởi Antigravity dành cho người mới bắt đầu làm game.*
