using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Skill", menuName = "RPG/Skill")]
public class SkillData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string skillName = "Kỹ năng mới";
    public Sprite icon;
    public float baseCooldown = 5f;

    [Header("Mô tả theo cấp độ")]
    [TextArea(2, 4)]
    public List<string> levelDescriptions = new List<string>(10);

    [Header("Thông số sức mạnh (Mỗi Level cộng thêm bao nhiêu)")]
    public float baseDamageMultiplier = 1.0f;
    public float damageIncreasePerLevel = 0.2f;
    public float rangeIncreasePerLevel = 0.1f;

    [Header("Thông số Buff/Hỗ trợ")]
    public int baseHealOrDef = 10;
    public int valueIncreasePerLevel = 2;

    public string GetDescription(int level) {
        int idx = Mathf.Clamp(level - 1, 0, levelDescriptions.Count - 1);
        if (idx < levelDescriptions.Count && !string.IsNullOrEmpty(levelDescriptions[idx]))
            return levelDescriptions[idx];
        return "Nâng cấp để tăng sức mạnh!";
    }
}
