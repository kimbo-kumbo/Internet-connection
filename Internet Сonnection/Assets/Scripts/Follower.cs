using UnityEngine;

public class Follower : MonoBehaviour
{
    protected Transform _targetTransform;
    [SerializeField] private float _smoothing;
    private Vector3 _offset;
    private bool _isTarget = false;

    public void SetTarget(Transform target)
    {
        _targetTransform = target;
        _offset = transform.position - _targetTransform.position;
        _isTarget = true;
    }
    private void FixedUpdate()
    {
        Move(Time.deltaTime);
    }

    protected void Move(float deltaTime)
    {
        if (!_isTarget) return;
        var nextPosition = Vector3.Lerp(transform.position, _targetTransform.position + _offset, deltaTime * _smoothing);
        transform.position = nextPosition;
    }
}
