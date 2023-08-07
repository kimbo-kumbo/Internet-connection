using Photon.Pun;
using UnityEngine;

public class JoinCamera : MonoBehaviour
{
    [SerializeField] private CameraPoint _cameraPoint;
    private Camera _camera;
    private PhotonView _photonView;

    private void Start()
    {        
        _photonView = GetComponent<PhotonView>();
        if (!_photonView.IsMine) return;
        _camera = FindObjectOfType<Camera>();        
        _camera.transform.position = _cameraPoint.transform.position;
        _camera.transform.rotation = _cameraPoint.transform.rotation;

        var foloover = _camera.gameObject.GetComponent<Follower>();
        foloover.enabled = true;        
        foloover.SetTarget(gameObject.transform);
    }
}