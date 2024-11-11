using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // Hareket, z�plama ve boyut k���ltme i�in gerekli parametreler
    [Header("Player Settings")]
    public float moveSpeed = 5f;           // Hareket h�z�
    public float jumpForce = 7f;           // Z�plama g�c�
    public float sizeChangeFactor = 0.5f;  // Boyut k���ltme fakt�r�

    // Kamera d�nd�rme i�in parametreler
    [Header("Mouse Look Settings")]
    public float lookSpeedX = 2.0f;       // Yatay d�n�� h�z� (sa�a/sola)
    public float lookSpeedY = 2.0f;       // Dikey d�n�� h�z� (yukar�/a�a��)
    public float upperLookLimit = -60f;   // Kameran�n yukar�ya bakma s�n�r�
    public float lowerLookLimit = 60f;    // Kameran�n a�a��ya bakma s�n�r�

    private Rigidbody rb;
    private Camera playerCamera;
    private Vector3 originalScale;
    private bool isGrounded;
    private float rotationX = 0f;          // Dikey d�n�� miktar� (kamera)

    void Start()
    {
        // Rigidbody bile�enini al
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main; // Ana kamera
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Hareket ve z�plama i�lemleri
        HandleMovement();
        HandleJumping();
        HandleSizeChange();

        // Kamera d�nd�rme i�lemleri
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        // Kullan�c� giri�ini al (W, A, S, D tu�lar�)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Hareket y�n�n� olu�tur
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Yeni pozisyonu hesapla
            Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
            transform.Translate(move, Space.World); // Hareketi d�nya uzay�nda yap
        }
    }

    private void HandleJumping()
    {
        // Z�plama i�lemi: Sadece yerle temas halindeyken
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleSizeChange()
    {
        // Boyut k���ltme i�lemi
        if (Input.GetKey(KeyCode.LeftControl))
        {
            transform.localScale = new Vector3(originalScale.x, originalScale.y * sizeChangeFactor, originalScale.z);
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    // Yerle temas kontrol�
    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    // Mouse hareketine g�re kameray� d�nd�rme
    private void HandleMouseLook()
    {
        // Yatay d�n�� (sa�a/sola) hareketi
        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        transform.Rotate(0, mouseX, 0);

        // Dikey d�n�� (yukar�/a�a��) hareketi
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, upperLookLimit, lowerLookLimit); // Dikey hareket s�n�rlar�
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Kameray� dikeyde d�nd�r
    }
}
