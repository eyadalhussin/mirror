using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class UIHealthbar : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthBarSliderChanged))]
    private float _sliderValue;

    public Slider _slider;
    private float _currentHealth;
    LifeComponent lf;

    // Update is called once per frame

    void Start()
    {
        lf = GetComponent<LifeComponent>();
    }
    void Update()
    {
        if (!isServer) return;

        if (lf && _slider)
        {
            _currentHealth = lf.GetCurrentHealth();
            if (_sliderValue != _currentHealth)
            {
                UpdateSliderValue(_currentHealth);
            }
        }
    }

    private void UpdateSliderValue(float newValue)
    {
        _sliderValue = newValue;
    }

    private void OnHealthBarSliderChanged(float oldValue, float newValue)
    {
        if (_slider != null)
        {
            _slider.value = newValue;
        }
    }

}
