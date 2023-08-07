using UnityEngine;

public class Obstackle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ProjectileController>())
            Destroy(other.gameObject);
    }
}
