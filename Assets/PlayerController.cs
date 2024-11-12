using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float moveSpeed = 5f;           // Hareket h�z�
    public float sprintMultiplier = 2f;    // Ko�ma h�z� �arpan�
    public float jumpForce = 7f;           // Z�plama g�c�
    public float sizeChangeFactor = 0.5f;  // Boyut k���ltme fakt�r�

    [Header("Mouse Look Settings")]
    public float lookSpeedX = 2.0f;        // Yatay d�n�� h�z�
    public float lookSpeedY = 2.0f;        // Dikey d�n�� h�z�
    public float upperLookLimit = -60f;    // Kamera �st s�n�r�
    public float lowerLookLimit = 60f;     // Kamera alt s�n�r�

    [Header("Camera Bobbing Settings")]
    public float walkBobAmount = 0.05f;    // Yava� y�r�rken ba� sallanma miktar�
    public float sprintBobAmount = 0.1f;   // H�zl� ko�arken ba� sallanma miktar�
    public float bobSpeed = 10f;           // Sallanma h�z�n� kontrol etme

    private Rigidbody rb;
    private Camera playerCamera;
    private Vector3 originalScale;
    private bool isGrounded;
    private float rotationX = 0f;          // Dikey d�n�� miktar�
    private Vector3 lastPosition;          // Son pozisyon
    private float bobTimer = 0f;           // Sallanma zamanlay�c�s�

    void Start()
    {
        // Rigidbody ve kamera bile�enlerini al
        rb = GetComponent<Rigidbody>();
        playerCamera = Camera.main;
        originalScale = transform.localScale;
        lastPosition = transform.position;
    }

    void Update()
    {
        // Hareket, z�plama, boyut k���ltme i�lemleri
        HandleMovement();
        HandleJumping();
        HandleSizeChange();

        // Kamera d�nd�rme
        HandleMouseLook();

        // Kamera sallanma (head bobbing) i�lemleri
        HandleHeadBobbing();
    }

    private void HandleMovement()
    {
        // Kullan�c� giri�ini al (W, A, S, D tu�lar�)
        float horizontal = Input.GetAxis("Horizontal");  // A/D tu�lar�
        float vertical = Input.GetAxis("Vertical");      // W/S tu�lar�

        // Ko�ma durumu: Shift tu�u ile h�z art���
        float currentMoveSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed *= sprintMultiplier; // Ko�ma h�z�n� art�r
        }

        // Hareket y�n�n� olu�tur
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // Yeni pozisyonu hesapla
            Vector3 move = moveDirection * currentMoveSpeed * Time.deltaTime;

            // Rigidbody ile hareket etmek, fiziksel hesaplamalar i�in daha do�ru sonu� verir
            rb.MovePosition(transform.position + move);
        }
    }

    private void HandleJumping()
    {
        // Z�plama i�lemi
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

    void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    private void HandleMouseLook()
    {
        // Yatay d�n�� (sa�a/sola)
        float mouseX = Input.GetAxis("Mouse X") * lookSpeedX;
        transform.Rotate(0, mouseX, 0);

        // Dikey d�n�� (yukar�/a�a��)
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, upperLookLimit, lowerLookLimit); // Dikey hareket s�n�rlar�
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // Kameray� dikeyde d�nd�r
    }

    private void HandleHeadBobbing()
    {
        // Ko�arken ba� hareketi (head bobbing)
        if (isGrounded && (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0))
        {
            // Y�r�rken ba� hareketini hesapla
            float speed = (Input.GetKey(KeyCode.LeftShift)) ? sprintBobAmount : walkBobAmount;
            bobTimer += Time.deltaTime * bobSpeed;

            // X ve Y ekseninde hareket
            float bobbingX = Mathf.Sin(bobTimer) * speed;
            float bobbingY = Mathf.Cos(bobTimer) * speed;

            // Kameray� ba� sallanmas�yla hareket ettir
            playerCamera.transform.localPosition = new Vector3(0, bobbingY, bobbingX);
        }
        else
        {
            // Hareket yoksa ba� sallanmas� s�f�r
            playerCamera.transform.localPosition = Vector3.zero;
        }
    }

    private void HandleMovementWithMouseDirection()
    {
        // Mouse pozisyonunu kullanarak karakteri y�nlendir
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition); // Kamera ile mouse pozisyonu aras�nda ���n g�nder

        // I��n�n �arpma noktas�n� bul
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Mouse'un g�sterdi�i yere do�ru y�nelme
            Vector3 targetDirection = (hit.point - transform.position).normalized;

            // Karakteri bu y�ne d�nd�r
            float step = moveSpeed * Time.deltaTime; // Yava� yava� d�nd�rme
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            // Hareketi karakterin �n y�n�nde yap
            Vector3 move = transform.forward * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + move);
        }
    }
}
