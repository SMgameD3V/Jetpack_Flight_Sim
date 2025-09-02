using UnityEngine;

public class FuelManager : MonoBehaviour
{
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    public float fuelConsumptionRate = 10f;
    public JetpackUIManager uiManager;

    public bool HasFuel() => currentFuel > 0f;
    void Start()
    {
        currentFuel = maxFuel;       
    }

    public void ConsumeFuel(float deltaTime)
    {
        currentFuel = Mathf.Max(0f, currentFuel - fuelConsumptionRate * deltaTime);
        UpdateUI();
    }

    public void RefillFuel(float amount)
    {
        currentFuel = Mathf.Min(maxFuel, currentFuel + amount);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (uiManager != null)
            uiManager.RefreshUI();
    }
}
