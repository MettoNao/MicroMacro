using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private bool isFirstSelectButton;
    private void Start()
    {
        if (isFirstSelectButton)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void LoadScene()
    {
        if(sceneName == string.Empty)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }

        SceneManager.LoadScene(sceneName);
    }
}
