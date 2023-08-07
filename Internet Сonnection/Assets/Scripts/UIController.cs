using Net.Managers;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private PanelInfo _panelInfo;
    [SerializeField] private Text _info;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    public void ShowPanelInfo(bool photonView)
    {
        _panelInfo.gameObject.SetActive(true);
        if (photonView)        
            _info.text = "Вы проиграли";        
        else        
            _info.text = "Вы выиграли";        
    }
}
