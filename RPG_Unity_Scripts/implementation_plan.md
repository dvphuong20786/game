# Kế hoạch sửa lỗi: Đệ tử & Tầm đánh & Chống đánh đồng đội

Bản kế hoạch này tập trung vào việc khắc phục các lỗi vận hành của đệ tử và điều chỉnh cân bằng tầm đánh cho người chơi, đồng thời đảm bảo người chơi và đệ tử không thể làm hại nhau.

## User Review Required

> [!IMPORTANT]
> - **Chống đánh đồng đội**: Tôi sẽ thêm cơ chế kiểm tra (Check Team) để đảm bảo các kỹ năng AOE hoặc đòn đánh thường của bạn không gây sát thương lên đệ tử và ngược lại.
> - **Tầm đánh nhân vật**: Sẽ giảm từ 1.8 xuống 1.2 để sát với thực tế của một kiếm sĩ (cần đứng gần mới chém được).

## Proposed Changes

---

### [Component] AI & Combat (Chiến đấu và Trí tuệ nhân tạo)

#### [MODIFY] [PlayerCombat.cs](file:///f:/WORK/GAME/RPG_Unity_Scripts/PlayerCombat.cs)
- Giảm `attackRange` từ 1.8f xuống 1.2f.
- Đảm bảo các hàm `FindNearestMonster` và `HitAllMonstersInRange` chỉ nhắm mục tiêu là quái vật (đã có nhưng sẽ làm chặt chẽ hơn).

#### [MODIFY] [CompanionAI.cs](file:///f:/WORK/GAME/RPG_Unity_Scripts/CompanionAI.cs)
- Cải thiện logic tìm quái: Tăng dải quét mục tiêu và đảm bảo đệ tử không bị khựng khi quái ở xa.
- Thêm `isPlayer = false` khi bắt đầu để tránh nhầm lẫn hệ thống.

#### [MODIFY] [NPCTrainer.cs](file:///f:/WORK/GAME/RPG_Unity_Scripts/NPCTrainer.cs)
- Sửa lỗi đệ tử không hiện: Đảm bảo khi tạo ra (`Instantiate`), đệ tử sẽ ở đúng lớp hiển thị (Sorting Layer) và vị trí Z = 0.
- Thêm `Debug.Log` để bạn kiểm tra trong bảng Console xem đệ tử đã được sinh ra thực sự chưa.

#### [MODIFY] [Monster.cs](file:///f:/WORK/GAME/RPG_Unity_Scripts/Monster.cs)
- Duy trì việc tấn công cả người chơi và đệ tử (như yêu cầu trước).

---

### [Component] Documentation (Tiến độ)

#### [MODIFY] [TIEN_DO_GAME.md](file:///f:/WORK/GAME/RPG_Unity_Scripts/TIEN_DO_GAME.md)
- Ghi nhận việc sửa lỗi tầm đánh và AI đệ tử.

---

## Open Questions

- Bạn có muốn đệ tử tự động hồi máu khi không chiến đấu không?
- Bạn có muốn thêm hiệu ứng vòng tròn dưới chân đệ tử để dễ nhìn thấy chúng hơn không?

## Verification Plan

### Automated Tests
- Kiểm tra tầm đánh mới 1.2f trong code.
- Kiểm tra log sinh đệ tử trong Console.

### Manual Verification
- Bạn thử thuê đệ tử và xem chúng có xuất hiện ngay cạnh NPC Trainer không.
- Thử chém vào đệ tử xem có mất máu không (kết quả mong đợi: không mất máu).
