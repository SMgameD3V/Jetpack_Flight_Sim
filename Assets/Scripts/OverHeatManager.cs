using UnityEngine;

public class OverheatManager : MonoBehaviour
{
    public float maxHeat = 100f;
    public float currentHeat = 0f;
    [SerializeField] private float heatIncreaseRate = 20f;
    [SerializeField] private float coolDownRate = 30f;
    private bool overheated = false;

    public bool IsOverheated() => overheated;

    public void IncreaseHeat(float deltaTime)
    {
        currentHeat += heatIncreaseRate * deltaTime;
        if (currentHeat >= maxHeat)
        {
            currentHeat = maxHeat;
            overheated = true;
        }
    }

    public void CoolDown(float deltaTime)
    {
        if (currentHeat > 0f)
        {
            currentHeat -= coolDownRate * deltaTime;
            if (currentHeat < 0f) currentHeat = 0f;
        }

        // Recover from overheat once you cool below 50%
        if (currentHeat < maxHeat * 0.5f)
            overheated = false;
    }
}
