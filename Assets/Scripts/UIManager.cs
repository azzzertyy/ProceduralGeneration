using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StatManager statManager;
    [SerializeField] private GameObject healthBar; // GameObject reference
    [SerializeField] private float lerpSpeed;

    private void Update()
    {
        ManageHealthBar();
    }

    private void ManageHealthBar()
    {
        float health = statManager.GetStat("Health");
        float maxHealth = statManager.GetMaxStat("Health");
        float normalizedHealth = Mathf.Clamp01(health / maxHealth);

        float healthBarLength = normalizedHealth * 100;
        Vector3 targetScale = new Vector3(healthBarLength, healthBar.transform.localScale.y, healthBar.transform.localScale.z);

        healthBar.transform.localScale = Vector3.Lerp(healthBar.transform.localScale, targetScale, Time.deltaTime * lerpSpeed);
    }
}
