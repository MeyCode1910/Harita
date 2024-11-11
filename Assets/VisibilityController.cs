using System.Collections;
using UnityEngine;

public class VisibilityController : MonoBehaviour
{
    [Header("Visibility Settings")]
    [Tooltip("Fareyi bas�l� tutma s�resi (saniye)")]
    public float holdDuration = 5f;  // Fareyi bas�l� tutma s�resi (5 saniye)

    private bool isMouseDown = false;  // Mouse'un bas�l� tutulup tutulmad���n� kontrol etmek i�in
    private float mouseDownTime = 0f;  // Fare tu�unun ne kadar s�re bas�l� oldu�unu takip eder

    void Update()
    {
        // Sol fare tu�una bas�ld���nda, mouseDownTime'� s�f�rl�yoruz
        if (Input.GetMouseButtonDown(0))  // Fare tu�una bas�ld���nda
        {
            isMouseDown = true;
            mouseDownTime = Time.time;  // Bas�lmaya ba�lanan zaman� kaydediyoruz
        }

        // Sol fare tu�u b�rak�ld���nda, i�lemi sonland�r�yoruz
        if (Input.GetMouseButtonUp(0))  // Fare tu�u b�rak�ld���nda
        {
            isMouseDown = false;
            mouseDownTime = 0f;  // Zaman� s�f�rl�yoruz
        }

        // E�er fare bas�l� tutuluyorsa
        if (isMouseDown)
        {
            // Fareyi bas�l� tutma s�resi, belirlenen s�reyi ge�tiyse nesneyi gizle
            if (Time.time - mouseDownTime >= holdDuration)
            {
                // Raycast i�lemiyle t�klanan nesneyi buluyoruz
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    // E�er t�klanan nesne bu nesne ise, onu gizleriz
                    if (hit.transform == transform)
                    {
                        StartCoroutine(ToggleVisibility());  // G�r�n�rl��� ge�ici olarak kapat
                    }
                }

                isMouseDown = false;  // T�klama s�resi tamamland�ktan sonra tekrar ba�lat�lmas�n diye kapat�yoruz
            }
        }
    }

    // G�r�n�rl��� ge�ici olarak kapat ve sonra geri a�
    IEnumerator ToggleVisibility()
    {
        // G�r�n�rl��� kapat: Nesneyi aktif de�il yap�yoruz
        gameObject.SetActive(false);

        // 1 frame bekle ve ard�ndan g�r�n�rl��� geri a�
        yield return null;

        // G�r�n�rl��� geri a�: Nesneyi tekrar aktif yap�yoruz
        gameObject.SetActive(true);
    }
}
