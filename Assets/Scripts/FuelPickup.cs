using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    [SerializeField] private float fuelAmount = 30f;
     private float rotationSpeed = 100f;     
     private float bounceHeight = 0.07f;    
     private float bounceSpeed = 1f;        

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Rotate around Z axis
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // Bounce using sine wave
        float newY = Mathf.Sin(Time.time * bounceSpeed * Mathf.PI * 2f) * bounceHeight;
        transform.position = startPos + new Vector3(0f, newY, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FuelManager fuel = other.GetComponentInParent<FuelManager>();
            if (fuel != null)
            {
                fuel.RefillFuel(fuelAmount);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("FuelManager not found!");
            }
        }
    }
}
