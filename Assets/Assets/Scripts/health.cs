using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health = 5;
    public TextMeshProUGUI healthText;

    private void Update()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + health.ToString();
        }
    }
}