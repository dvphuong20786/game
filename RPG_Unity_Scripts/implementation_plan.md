# Kế hoạch Thêm NPC Huấn Luyện & Hệ thống Thuê Đệ Tử

Hệ thống này cho phép người chơi chiêu mộ đồng đội mới (Humans, Monsters) với các vai trò khác nhau (Cận chiến, Tầm xa, Tanker) thông qua NPC Huấn Luyện.

## User Review Required

> [!IMPORTANT]
> **Số lượng Đệ tử:** Đã thống nhất giới hạn tối đa **4 đệ tử** cùng lúc. Tôi sẽ tối ưu code để các đệ tử không đứng chồng lên nhau khi đi theo bạn.
>
> **Thuê vs Mua:** Đã thống nhất cơ chế "Mua đứt". Người chơi có thể sở hữu đội quân 4 người của riêng mình.

## Proposed Changes

### [Component] Companion System Expansion

#### [MODIFY] [CompanionAI.cs](file:///d:/work/1/RPG_Unity_Scripts/CompanionAI.cs)
- Cập nhật để hỗ trợ các loại tấn công khác nhau (Tầm xa - bắn cung, Cận chiến - chém).
- Thêm biến `companionType` để xác định hành vi riêng của từng loài.

#### [NEW] [TrainerNPC.cs](file:///d:/work/1/RPG_Unity_Scripts/TrainerNPC.cs)
- Script mới cho NPC Huấn Luyện.
- Quản lý danh sách các Prefab Đệ tử có thể thuê.
- Giao diện UI riêng để chọn Đệ tử:
  - **Kiếm Khách (Warrior):** Máu trâu, chém mạnh.
  - **Cung Thủ (Archer):** Máu giấy, bắn xa an toàn.
  - **Slime Đồng Minh:** Phản dame, làm chậm quái.

### [Component] User Interface (UI)

#### [MODIFY] [GameUI.cs](file:///d:/work/1/RPG_Unity_Scripts/GameUI.cs)
- Cập nhật HUD để hiện máu của Đệ tử hiện tại (để người chơi biết khi nào đệ tử sắp "tử trận").

## Open Questions

- Bạn đã có sẵn Sprite cho Cung thủ hay Quái vật đồng minh chưa? Nếu chưa tôi sẽ tạo hình bằng AI giúp bạn.

## Verification Plan

### Manual Verification
- Chạy lại gần NPC Huấn Luyện (phím E), chọn mua "Cung Thủ".
- Kiểm tra xem bạn có thể dẫn theo tối đa 4 đệ tử khác nhau không.
- Dẫn cả đội hình đi đánh quái xem hiệu quả chiến đấu nhóm.
