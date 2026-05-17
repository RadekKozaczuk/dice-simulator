using System.Collections.Generic;
using UnityEngine;

namespace Presentation.Editor
{
    public class D12AutoDetector : MonoBehaviour
    {
        [System.Serializable]
        public struct DieFace
        {
            public int Value;          // Numer ścianki (przypiszemy potem w inspektorze)
            public Vector3 LocalNormal; // Automatycznie wyliczony wektor lokalny
        }

        public List<DieFace> DetectedFaces = new();

        // Próg tolerancji dla ścianek (w radianach/stopniach), zapobiega dublowaniu
        const float AngleThreshold = 5f;

        void Awake()
        {
            GenerateFacesFromMesh();
        }

        [ContextMenu("Generate Faces From Mesh")] // Pozwala kliknąć prawym przyciskiem myszy na komponent w edytorze
        void GenerateFacesFromMesh()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogError("Brak MeshFilter lub przypisanego Mesha!");
                return;
            }

            Mesh mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            var uniqueNormals = new List<Vector3>();

            // Przechodzimy przez wszystkie trójkąty mesha (co 3 indeksy)
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v1 = vertices[triangles[i]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];

                // Obliczamy wektor normalny dla pojedynczego trójkąta (Local Space)
                Vector3 side1 = v2 - v1;
                Vector3 side2 = v3 - v1;
                Vector3 triangleNormal = Vector3.Cross(side1, side2).normalized;

                // Sprawdzamy, czy ten kierunek jest już na naszej liście unikalnych ścian
                bool isNewFace = true;
                foreach (Vector3 existingNormal in uniqueNormals)
                    if (Vector3.Angle(triangleNormal, existingNormal) < AngleThreshold)
                    {
                        isNewFace = false;
                        break;
                    }

                // Jeśli to nowa ściana, zapisujemy ją
                if (isNewFace)
                    uniqueNormals.Add(triangleNormal);
            }

            // Przepisujemy unikalne wektory do naszej ostatecznej listy
            DetectedFaces.Clear();
            for (int i = 0; i < uniqueNormals.Count; i++)
            {
                var newFace = new DieFace
                {
                    Value = i + 1, // Domyślnie przypisuje numery od 1 do 12
                    LocalNormal = uniqueNormals[i]
                };
                DetectedFaces.Add(newFace);
            }

            Debug.Log($"Pomyślnie wykryto {DetectedFaces.Count} ścian na meshu.");
        }
    }
}