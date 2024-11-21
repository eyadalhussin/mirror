using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HealthUIComponent : MonoBehaviour
{
    public TextMeshProUGUI healthbar;

    private float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = GetComponent<LifeComponent>().GetCurrentHealth();
        healthbar.text = "" + currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        currentHealth = GetComponent<LifeComponent>().GetCurrentHealth();
        healthbar.text = "" + currentHealth;
    }
}
