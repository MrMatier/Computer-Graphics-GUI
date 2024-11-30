using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public float speed = 2.0f;
    public float turnSpeed = 1.0f;
    public Vector3 minBounds = new Vector3(-10, 1, -10); // Dolne ograniczenia
    public Vector3 maxBounds = new Vector3(10, 5, 10);   // Górne ograniczenia

    private Vector3 targetPosition;

    // Korekta rotacji o 90 stopni wokó³ osi X i 180 stopni wokó³ osi Z
    private Quaternion rotationOffset = Quaternion.Euler(-90, 180, 0);

    void Start()
    {
        SetNewTargetPosition();
    }

    void Update()
    {
        // Oblicz kierunek do celu na p³aszczyŸnie XZ
        Vector3 directionXZ = new Vector3(
            targetPosition.x - transform.position.x,
            0,
            targetPosition.z - transform.position.z
        ).normalized;

        // Jeœli mamy jakiœ kierunek
        if (directionXZ != Vector3.zero)
        {
            // Obracamy siê tylko wokó³ osi Y i dodajemy korektê rotacji
            Quaternion targetRotation = Quaternion.LookRotation(directionXZ) * rotationOffset;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        // Obliczamy ró¿nicê w osi Y
        float yDifference = targetPosition.y - transform.position.y;
        // Ograniczamy prêdkoœæ ruchu w osi Y
        float yDirection = Mathf.Clamp(yDifference, -0.5f, 0.5f);

        // Ruch do przodu z uwzglêdnieniem korekty rotacji
        Vector3 moveDirection = new Vector3(directionXZ.x, yDirection, directionXZ.z).normalized;
        transform.position += moveDirection * speed * Time.deltaTime;

        // Sprawdzenie, czy ryba zbli¿y³a siê wystarczaj¹co do celu
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            SetNewTargetPosition();
        }

        // Upewniamy siê, ¿e ryba pozostaje w granicach
        ClampPosition();
    }

    void SetNewTargetPosition()
    {
        float x = Random.Range(minBounds.x, maxBounds.x);
        float y = Random.Range(minBounds.y, maxBounds.y);
        float z = Random.Range(minBounds.z, maxBounds.z);

        targetPosition = new Vector3(x, y, z);
    }

    void ClampPosition()
    {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.y = Mathf.Clamp(position.y, minBounds.y, maxBounds.y);
        position.z = Mathf.Clamp(position.z, minBounds.z, maxBounds.z);
        transform.position = position;
    }
}
