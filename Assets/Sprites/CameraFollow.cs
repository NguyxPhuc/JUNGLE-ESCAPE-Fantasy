using UnityEngine;

/// <summary>
/// Camera theo nhân vật với hiệu ứng smooth + giới hạn biên map.
/// Gắn vào Main Camera.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1f, -10f);

    [Header("Smooth Settings")]
    [SerializeField] private float _smoothSpeed = 5f;

    [Header("Camera Bounds (tuỳ chọn)")]
    [SerializeField] private bool _useBounds = false;
    [SerializeField] private float _minX = -50f;
    [SerializeField] private float _maxX = 50f;
    [SerializeField] private float _minY = -10f;
    [SerializeField] private float _maxY = 20f;

    private void LateUpdate()
    {
        if (_target == null) return;

        Vector3 desiredPosition = _target.position + _offset;

        if (_useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, _minX, _maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, _minY, _maxY);
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
    }

    /// <summary>Gán target từ code</summary>
    public void SetTarget(Transform target) => _target = target;
}