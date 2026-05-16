#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
using Presentation.Config;
using UnityEngine;

namespace Presentation.Views
{
    class BallView : MonoBehaviour
    {
        static readonly BallConfig _config;

        [SerializeField]
        Transform _model;

        float _rotationSpeed;

        void Start() => _rotationSpeed = Random.Range(_config.MinRotationSpeed, _config.MaxRotationSpeed);

        internal void CustomUpdate() => _model.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime, Space.Self);
    }
}
