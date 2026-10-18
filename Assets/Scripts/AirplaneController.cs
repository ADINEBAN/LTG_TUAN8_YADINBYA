using UnityEngine;

/// <summary>
/// Airplane movement and takeoff controller.
/// Moves the airplane along the runway, accelerates, takes off, and collides with the Enter Wall.
/// Supports both automatic flight down the runway and optional manual controls (WASD / Arrows).
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AirplaneController : MonoBehaviour
{
    [Header("Flight Parameters")]
    [Tooltip("Forward cruise speed down runway")]
    public float forwardSpeed = 14f;

    [Tooltip("Climb rate during takeoff")]
    public float climbSpeed = 2.5f;

    [Tooltip("Distance traveled along Z before initiating takeoff")]
    public float takeoffDistance = 14f;

    [Tooltip("Maximum flight altitude before reaching wall")]
    public float maxAltitude = 4.2f;

    [Tooltip("Enable automatic takeoff and flight")]
    public bool autoFly = true;

    [Header("Physics State")]
    private Rigidbody rb;
    private Vector3 startPosition;
    private bool isFlying = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isFlying) return;

        // Manual controls (WASD or Arrows)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Forward motion
        Vector3 movement = Vector3.forward * (forwardSpeed * Time.deltaTime);

        // Auto takeoff logic: lift off smoothly after running down runway
        float traveled = transform.position.z - startPosition.z;
        if (autoFly && traveled > takeoffDistance && transform.position.y < maxAltitude)
        {
            movement.y += climbSpeed * Time.deltaTime;
        }

        // Apply manual input overrides if detected
        if (Mathf.Abs(vertical) > 0.01f)
        {
            movement.y += vertical * climbSpeed * Time.deltaTime;
        }

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            movement.x += horizontal * (forwardSpeed * 0.4f) * Time.deltaTime;
        }

        // Apply translation
        transform.position += movement;

        // Visual banking / pitch
        float targetPitch = (movement.y > 0.01f) ? -8f : 0f;
        Quaternion targetRotation = Quaternion.Euler(targetPitch, 0f, -horizontal * 12f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // When touching the Wall trigger, gently halt flight so the plane remains in place
        if (other.name.Contains("Wall"))
        {
            isFlying = false;
        }
    }

    /// <summary>
    /// Explicitly pause or halt airplane flight.
    /// </summary>
    public void StopFlight()
    {
        isFlying = false;
    }

    /// <summary>
    /// Resume airplane flight.
    /// </summary>
    public void ResumeFlight()
    {
        isFlying = true;
    }
}
