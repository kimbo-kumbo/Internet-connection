using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;
using Net.Managers;

namespace Net
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {        
        private Controls _controls;
        private Transform _bulletPool;
        private Transform _target;
        private GameManager _gameManager;
        private UIController _uIController;
        [SerializeField] private ProjectileController _bulletPrefab;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private PhotonView _photonView;
        
        [Space, SerializeField, Range(1f, 10f)] private float _moveSpeed = 2f;
        [SerializeField, Range(0.5f, 5f)] private float _maxSpeed = 2f;
        [SerializeField, Range(0.1f, 2f)] private float _attackDelay = 0.4f;               
        [SerializeField] private Vector3 _firePoint;        
        [Range(1f, 50f)] public float Health = 5f;

        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
            _uIController = FindObjectOfType<UIController>();
            _rigidbody = GetComponent<Rigidbody>();
            _bulletPool = FindObjectOfType<EventSystem>().transform;
            _controls = new Controls();

            FindObjectOfType<GameManager>().AddPlayer(this);
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            StartCoroutine(Fire());            
            if (!_photonView.IsMine) return;
            _controls.Player1.Enable();           
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(PlayerData.Create(this));
            }
            else
            {
                ((PlayerData)stream.ReceiveNext()).Set(this);
            }
        }

        private IEnumerator Fire()
        {
            while(true)
            {
                if (_target != null)
                {
                    var bullet = Instantiate(_bulletPrefab, _bulletPool);
                    bullet.transform.position = transform.TransformPoint(_firePoint);
                    bullet.transform.rotation = transform.rotation;
                    bullet.Parent = name;
                    yield return new WaitForSeconds(_attackDelay);
                }                
            }
        }      

        void FixedUpdate()
        {
            if(_target != null)
            {
                transform.LookAt(_target);
                transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
            }           

            if (!_photonView.IsMine) return;            

            var direction = _controls.Player1.Movement.ReadValue<Vector2>();
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
            if(other.GetComponent<WallOfFire>())
            {
                Debug.Log($"Player with name {name} is dead");
                OverGame();
            }
            var bullet = other.GetComponent<ProjectileController>();
            if (bullet == null || bullet.Parent == name) return;

            Health -= bullet.GetDamage;
            Destroy(other.gameObject);
            if (Health <= 0)
            {
                Debug.Log($"Player with name {name} is dead");
                OverGame();
            }
        }

        private void OverGame()
        {
            foreach (var player in _gameManager.Players)
            {
                var playerController = player.GetComponent<PlayerController>();
                playerController._uIController.ShowPanelInfo(_photonView.IsMine);
            }            
            Invoke(nameof(DisconectPlayer), 3.0f);
        }

        private void DisconectPlayer()
        {
            foreach (var player in _gameManager.Players)
            {
                var playerController = player.GetComponent<PlayerController>();
                if (_photonView.IsMine) continue;
                playerController.LeavePlayer();                
            }
            Invoke(nameof(LeavePlayer), 1.0f);            
        }

        public void LeavePlayer()
        {
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.LoadLevel("NetMenuScene");
        }        


        private void OnDestroy()
        {
            _controls.Player1.Disable();
            _controls.Player2.Disable();
        }       
    }
}
