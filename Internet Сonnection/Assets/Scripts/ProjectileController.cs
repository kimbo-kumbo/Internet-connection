using System.Collections;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField, Range(1f, 10f)]
    private float _moveSpeed = 3f;
    [SerializeField, Range(1f, 10f)]
    private float _damage = 1f;
    [SerializeField, Range(1f, 15f)]
    private float _lifeTime = 7f;

    public float GetDamage => _damage;

    public string Parent { get; set; }

    void Start()
    {
        StartCoroutine(OnDie());
    }

    
    void Update()
    {
        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private IEnumerator OnDie()
    {
        yield return new WaitForSeconds(_lifeTime);
        Destroy(gameObject);
    }
}
