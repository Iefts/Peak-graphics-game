using UnityEngine;

public enum ClassType { Warrior, Rogue, Mage, Ranger }

[System.Serializable]
public class CharacterClassData
{
    public ClassType classType;
    public string displayName;
    public float maxHealth;
    public float moveSpeed;
    public float jumpHeight;
    public float maxStamina;
    public Color classColor;

    public static CharacterClassData Get(ClassType type)
    {
        return type switch
        {
            ClassType.Warrior => new CharacterClassData
            {
                classType = ClassType.Warrior, displayName = "Warrior",
                maxHealth = 150f, moveSpeed = 4.5f, jumpHeight = 1.2f,
                maxStamina = 80f,  classColor = new Color(0.85f, 0.15f, 0.1f)
            },
            ClassType.Rogue => new CharacterClassData
            {
                classType = ClassType.Rogue,   displayName = "Rogue",
                maxHealth = 90f,  moveSpeed = 7.5f, jumpHeight = 1.8f,
                maxStamina = 130f, classColor = new Color(0.1f, 0.75f, 0.3f)
            },
            ClassType.Mage => new CharacterClassData
            {
                classType = ClassType.Mage,    displayName = "Mage",
                maxHealth = 80f,  moveSpeed = 5f,   jumpHeight = 1.4f,
                maxStamina = 110f, classColor = new Color(0.2f, 0.35f, 0.9f)
            },
            ClassType.Ranger => new CharacterClassData
            {
                classType = ClassType.Ranger,  displayName = "Ranger",
                maxHealth = 110f, moveSpeed = 6f,   jumpHeight = 1.6f,
                maxStamina = 115f, classColor = new Color(0.85f, 0.72f, 0.1f)
            },
            _ => throw new System.ArgumentOutOfRangeException(nameof(type))
        };
    }
}
