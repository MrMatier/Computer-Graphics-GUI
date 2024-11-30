using UnityEngine;

public class CameraController : MonoBehaviour
{
    public EyeTrackingReceiver eyeTrackingReceiver;

    // Skala ruchu kamery
    public float movementScaleX = 5.0f;
    public float movementScaleY = 5.0f;
    public float movementScaleZ = 5.0f;

    // Wyg³adzanie
    public float smoothingSpeed = 10.0f;

    // Strefy martwe
    public float deadZoneX = 0.005f;
    public float deadZoneY = 0.005f;
    public float deadZoneZ = 0.005f;

    // Ograniczenia pozycji kamery
    public Vector3 minPosition = new Vector3(-10f, -10f, -20f); // Zmieniono minPosition.z
    public Vector3 maxPosition = new Vector3(10f, 10f, 0f);     // Zmieniono maxPosition.z

    private Vector3 initialPosition;
    private Vector3 smoothedPosition;

    void Start()
    {
        initialPosition = transform.position;
        smoothedPosition = initialPosition;

        // Zresetuj dane w odbiorniku
        if (eyeTrackingReceiver != null)
        {
            eyeTrackingReceiver.ResetData();
        }
    }

    void Update()
    {
        if (eyeTrackingReceiver != null)
        {
            Vector3 faceDeltas = eyeTrackingReceiver.GetFaceDeltas();

            if (faceDeltas == Vector3.zero)
            {
                // Jeœli nie ma danych, powoli powracaj do pozycji pocz¹tkowej
                smoothedPosition = Vector3.Lerp(smoothedPosition, initialPosition, Time.deltaTime * smoothingSpeed);
                transform.position = smoothedPosition;
                return;
            }

            // Zastosuj strefy martwe
            float deltaX = Mathf.Abs(faceDeltas.x) < deadZoneX ? 0 : faceDeltas.x;
            float deltaY = Mathf.Abs(faceDeltas.y) < deadZoneY ? 0 : faceDeltas.y;
            float deltaZ = Mathf.Abs(faceDeltas.z) < deadZoneZ ? 0 : faceDeltas.z;

            // Skalowanie ruchu
            deltaX *= movementScaleX;
            deltaY *= movementScaleY;
            deltaZ *= movementScaleZ;

            // Oblicz pozycjê docelow¹ kamery
            Vector3 targetPosition = initialPosition + new Vector3(-deltaX, -deltaY, deltaZ); // Upewnij siê co do znaku przy deltaZ

            // Ogranicz pozycjê
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
            targetPosition.z = Mathf.Clamp(targetPosition.z, minPosition.z, maxPosition.z);

            // Wyg³adzanie ruchu kamery
            smoothedPosition = Vector3.Lerp(smoothedPosition, targetPosition, Time.deltaTime * smoothingSpeed);

            // Aktualizuj pozycjê kamery
            transform.position = smoothedPosition;
        }
    }
}
