using UnityEngine;

public class DiggingController : MonoBehaviour
{
    public Camera playerCamera;  // FPS kameras�
    public float digRadius = 1f;  // Kazma �ap�
    public float digDepth = 0.5f; // Kazma derinli�i
    public LayerMask diggableLayer;  // Kaz�labilir y�zeyler (�rn. topra�� temsil eden layer)

    void Update()
    {
        // Mouse t�klama ile kazma i�lemi
        if (Input.GetMouseButtonDown(0))  // Mouse sol tu�una t�klama
        {
            Dig();
        }
    }

    void Dig()
    {
        // Kameradan bir ���n g�nder
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // I��n herhangi bir objeye �arpt�ysa
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, diggableLayer))
        {
            // T�klanan objeye do�ru bir kazma i�lemi ba�lat
            Vector3 hitPoint = hit.point;

            // Kazma i�lemi (delik a�ma) burada yap�lacak
            CreateHole(hitPoint);
        }
    }

    void CreateHole(Vector3 position)
    {
        // Burada delik a�ma i�lemi yap�lacak. �rne�in, mesh deformasyonu veya terrain deformasyonu.
        Debug.Log("Digging at: " + position);

        // �rnek: Kazma �ap� i�inde bir delik a�ma
        Collider[] colliders = Physics.OverlapSphere(position, digRadius, diggableLayer);

        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out MeshFilter meshFilter))
            {
                Mesh mesh = meshFilter.mesh;
                Vector3[] vertices = mesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 worldVertex = collider.transform.TransformPoint(vertices[i]);
                    float distance = Vector3.Distance(worldVertex, position);

                    // E�er vertex t�klanan nokta yak�nsa, onu a�a��ya do�ru �ek
                    if (distance < digRadius)
                    {
                        vertices[i] -= new Vector3(0, digDepth * (1 - distance / digRadius), 0);  // Delik a�ma
                    }
                }

                // Yeni vertexlerle mesh'i g�ncelle
                mesh.vertices = vertices;
                mesh.RecalculateNormals();  // Yeni normaller hesaplanmal�
            }
        }
    }
}
