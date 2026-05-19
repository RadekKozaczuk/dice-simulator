using System.Collections.Generic;
using Core.Dtos;
using Core.Views;
using Sirenix.OdinInspector;
using Unity.Entities;
using UnityEngine;
using System.Linq;
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

        internal List<DiceFaceView> FaceViews = new();

        [SerializeField]
        DiceFaceView _diceFacePrefab;

        // prevent duplicates
        const float AngleThreshold = 5f;

        [Button("Generate Faces")]
        void GenerateFacesFromMesh()
        {
            DetectedFaces.Clear();

            List<Vector3> uniqueNormals = UniqueNormals(out float smallestDistance);

            for (int i = 0; i < uniqueNormals.Count; i++)
            {
                var face = new DiceFace
                {
                    Number = i + 1,
                    Normal = uniqueNormals[i]
                };

                DetectedFaces.Add(face);
            }

            // choose minimum
            foreach (DiceFace face in DetectedFaces)
            {
                Vector3 position = transform.position + face.Normal * smallestDistance;
                var rotation = Quaternion.LookRotation(face.Normal, Vector3.up);
                DiceFaceView view = Instantiate(_diceFacePrefab, position, rotation, transform);

                string number = face.Number.ToString();
                view.name = number;
                view.Label.text = number;
                FaceViews.Add(view);
            }
        }

        // todo: we need smallest distance in case mesh was not even
        List<Vector3> UniqueNormals(out float smallestDistance)
        {
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

            smallestDistance = distances.Min();

            return uniqueNormals;
        }
    }
}