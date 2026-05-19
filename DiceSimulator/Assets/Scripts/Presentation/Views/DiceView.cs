using System.Collections.Generic;
using Core.Dtos;
using Core.Views;
using UnityEngine;

namespace Presentation.Views
{
    class DiceView : MonoBehaviour
    {
        readonly List<DiceFaceView> _faces = new();

        [SerializeField]
        Transform _model;

        [SerializeField]
        DiceFaceView _diceFacePrefab;

        internal void SpawnFaces(List<DiceFace> faces)
        {
            foreach (DiceFaceView face in _faces)
                DestroyImmediate(face.gameObject);

            _faces.Clear();

            foreach (DiceFace face in faces)
            {
                Vector3 position = transform.position + face.Normal * face.Distance;
                var rotation = Quaternion.LookRotation(face.Normal, Vector3.up);
                DiceFaceView diceFace = Instantiate(_diceFacePrefab, position, rotation, transform);

                string number = face.Number.ToString();
                diceFace.name = number;
                diceFace.Label.text = number;
                _faces.Add(diceFace);
            }
        }
    }
}
