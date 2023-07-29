using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Net.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputAction _quit;

        void Start()
        {
            _quit.performed += OnQuit;
        }

        private void OnQuit(InputAction.CallbackContext obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE && !UNITY_EDITOR
            Application.Quit();
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
