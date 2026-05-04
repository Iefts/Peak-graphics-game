using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterClassData classData;

    public float currentHealth;
    public float currentStamina;
    public bool isDead;

    void Start()
    {
        if (classData == null) classData = CharacterClassData.Get(ClassType.Warrior);
        currentHealth  = classData.maxHealth;
        currentStamina = classData.maxStamina;
        GameManager.Instance?.RegisterPlayer(gameObject);
    }

    void OnDestroy() => GameManager.Instance?.UnregisterPlayer(gameObject);

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (currentHealth == 0) Die();
    }

    public void Heal(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, classData.maxHealth);
    }

    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Max(0, currentStamina - amount);
    }

    public void RecoverStamina(float amount)
    {
        currentStamina = Mathf.Min(currentStamina + amount, classData.maxStamina);
    }

    void Die()
    {
        isDead = true;
        Debug.Log($"[CharacterStats] {classData.displayName} has died.");
        GameManager.Instance?.UnregisterPlayer(gameObject);
    }
}
