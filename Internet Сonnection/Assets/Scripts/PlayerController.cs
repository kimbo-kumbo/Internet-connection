using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Net
{
    public class PlayerController : MonoBehaviour
    {
        private Controls _controls;
        private Transform _bulletPool;
        [SerializeField] private Transform _target;
        [SerializeField] private ProjectileController _bulletPrefab;
        [SerializeField] private Rigidbody _rigidbody;
        
        [Space, SerializeField, Range(1f, 10f)] private float _moveSpeed = 2f;
        [SerializeField, Range(0.5f, 5f)] private float _maxSpeed = 2f;
        [SerializeField, Range(0.1f, 1f)] private float _attackDelay = 0.4f;
        [SerializeField, Range(0.1f, 1f)] private float _rotateDelay = 0.25f;
        [Range(1f, 50f)] public float Health = 5f;
        [SerializeField] private Vector3 _firePoint;

        public bool Player1;


        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bulletPool = FindObjectOfType<EventSystem>().transform;
            _controls = new Controls();
            if(Player1)_controls.Player1.Enable();
            else _controls.Player2.Enable();

            StartCoroutine(Fire());
            StartCoroutine(Focus());
        }

        private IEnumerator Fire()
        {
            while(true)
            {
                var bullet = Instantiate(_bulletPrefab, _bulletPool);
                bullet.transform.position = transform.TransformPoint(_firePoint);
                bullet.transform.rotation = transform.rotation;
                bullet.Parent = name;
                yield return new WaitForSeconds(_attackDelay);
            }
        }

        private IEnumerator Focus()
        {
            while(true)
            {
                transform.LookAt(_target);
                transform.eulerAngles = new Vector3(0f,transform.eulerAngles.y,0f);
                yield return new WaitForSeconds(_rotateDelay);
            }
        }
        
        void FixedUpdate()
        {
            var direction = Player1
                ? _controls.Player1.Movement.ReadValue<Vector2>()
                : _controls.Player2.Movement.ReadValue<Vector2>();
            if (direction.x == 0 && direction.y == 0) return;
            var velocity = _rigidbody.velocity;
            velocity += new Vector3(direction.x, 0f, direction.y) * _moveSpeed * Time.fixedDeltaTime;

            velocity.y = 0f;
            velocity = Vector3.ClampMagnitude(velocity, _maxSpeed);
            _rigidbody.velocity = velocity;

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_firePoint, 0.2f);
        }

        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<ProjectileController>();
            if (bullet == null || bullet.Parent == name) return;

            Health -= bullet.GetDamage;
            Destroy(other.gameObject);
            if (Health <= 0) Debug.Log($"Player with name {name} is dead");
        }

        private void OnDestroy()
        {
            _controls.Player1.Disable();
            _controls.Player2.Disable();
        }
    }
}
