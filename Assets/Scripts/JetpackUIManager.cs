using UnityEngine;
using UnityEngine.UI;

public class JetpackUIManager : MonoBehaviour
{
    public Slider fuelSlider;
    public Slider heatSlider;
    public FuelManager fuelManager;
    public OverheatManager overheatManager;

    void Update()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (fuelSlider != null && fuelManager != null)
            fuelSlider.value = fuelManager.currentFuel / fuelManager.maxFuel;

        if (heatSlider != null && overheatManager != null)
            heatSlider.value = overheatManager.currentHeat / overheatManager.maxHeat;
    }
}
