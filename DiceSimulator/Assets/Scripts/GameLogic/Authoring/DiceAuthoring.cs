using System.Collections.Generic;
using Core.Dtos;
using Core.Views;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using GameLogic.Components;

namespace GameLogic.Authoring
{
    class DiceAuthoring : MonoBehaviour
    {
        class Baker : Baker<DiceAuthoring>
        {
            public override void Bake(DiceAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<NewlySpawnedTag>(entity);

                var dice = new DiceComponent();

                foreach (DiceFace face in authoring.DetectedFaces)
                    dice.Faces.Add(face);

                AddComponent(entity, dice);
            }
        }

        [SerializeField]
        Mesh _collisionMesh;

        [SerializeField]
        internal List<DiceFace> DetectedFaces = new();

        readonly List<DiceFaceView> _faceViews = new();

        [SerializeField]
        DiceFaceView _diceFacePrefab;

        // prevent duplicates
        const float AngleThreshold = 5f;

        [Button("Generate Faces")]
        void GenerateFacesFromMesh()
        {
            DetectedFaces.Clear();

            foreach (DiceFaceView face in _faceViews)
                DestroyImmediate(face.gameObject);

            _faceViews.Clear();

            int counter = 1;

            Vector3[] vertices = _collisionMesh.vertices;
            int[] triangles = _collisionMesh.triangles;

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
                    distances.Add(distance);

                bool isNewFace = true;
                foreach (Vector3 existingNormal in uniqueNormals)
                    if (Vector3.Angle(normal, existingNormal) < AngleThreshold)
                    {
                        isNewFace = false;
                        break;
                    }

                // save only unique normals for later
                if (isNewFace)
                {
                    uniqueNormals.Add(normal);

                    var face = new DiceFace
                    {
                        Number = counter++,
                        Normal = normal,
                        Distance = distance
                    };

                    DetectedFaces.Add(face);
                }
            }

            // choose minimum
            foreach (DiceFace face in DetectedFaces)
            {
                Vector3 position = transform.position + face.Normal * face.Distance;
                var rotation = Quaternion.LookRotation(face.Normal, Vector3.up);
                DiceFaceView view = Instantiate(_diceFacePrefab, position, rotation, transform);

                string number = face.Number.ToString();
                view.name = number;
                view.Label.text = number;
                _faceViews.Add(view);
            }
        }
    }
}