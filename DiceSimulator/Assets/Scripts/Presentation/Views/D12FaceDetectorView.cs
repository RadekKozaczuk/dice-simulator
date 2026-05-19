#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Presentation.Views
{
    class D12FaceDetectorView : MonoBehaviour
    {
        [SerializeField]
        Mesh _mesh;

        [SerializeField]
        DiceFaceView _diceFacePrefab;

        [Serializable]
        public struct DiceFace
        {
            public int Number;
            public Vector3 Normal;
        }

        public List<DiceFace> DetectedFaces = new();

        // prevent duplicates
        const float AngleThreshold = 5f;

        [Button("Generate Faces")]
        void GenerateFacesFromMesh()
        {
            DetectedFaces.Clear();
            DiceView dice = GetComponent<DiceView>();

            foreach (DiceFaceView face in dice.Faces)
                DestroyImmediate(face.gameObject);

            dice.Faces.Clear();

            Vector3[] vertices = _mesh.vertices;
            int[] triangles = _mesh.triangles;

            var uniqueNormals = new List<Vector3>();

            List<float> distances = new();

            // iterate over every triangle
            for (int i = 0; i < triangles.Length; i += 3)
            {
                Vector3 v1 = vertices[triangles[i]];
                Vector3 v2 = vertices[triangles[i + 1]];
                Vector3 v3 = vertices[triangles[i + 2]];

                // normal
                Vector3 side1 = v2 - v1;
                Vector3 side2 = v3 - v1;
                Vector3 normal = Vector3.Cross(side1, side2).normalized;

                // calculate centroid
                Vector3 centroid = (v1 + v2 + v3) / 3f;
                float distance = Vector3.Distance(centroid, Vector3.zero);

                if (!distances.Contains(distance))
                {
                    distances.Add(distance);
                    Debug.LogError(distance);
                }

                bool isNewFace = true;
                foreach (Vector3 existingNormal in uniqueNormals)
                    if (Vector3.Angle(normal, existingNormal) < AngleThreshold)
                    {
                        isNewFace = false;
                        break;
                    }

                // save only unique normals for later
                if (isNewFace)
                    uniqueNormals.Add(normal);
            }

            for (int i = 0; i < uniqueNormals.Count; i++)
            {
                var newFace = new DiceFace
                {
                    Number = i + 1, // default numbers from 1 to 12
                    Normal = uniqueNormals[i]
                };
                DetectedFaces.Add(newFace);
            }

            // choose minimum
            float smallest = distances.Min();

            foreach (DiceFace face in DetectedFaces)
            {
                Vector3 position = Vector3.zero + face.Normal * smallest;
                var rotation = Quaternion.LookRotation(face.Normal, Vector3.up);
                DiceFaceView diceFace = Instantiate(_diceFacePrefab, position, rotation, dice.transform);

                string number = face.Number.ToString();
                diceFace.name = number;
                diceFace.Label.text = number;
                dice.Faces.Add(diceFace);
            }
        }
    }
}
#endif