using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour
{
    public string LevelName = "YourNextSceneName";
    public TextMeshProUGUI progressText;
    public Image fillImage;
    public float MinLoadTime = 2f;
    public GameObject LoadingUI;
    public GameObject MenuUI;

    private float timer;

    void Start()
    {
        if (MenuUI != null) MenuUI.SetActive(true);
        if (LoadingUI != null) LoadingUI.SetActive(false);
        timer = 0f;
    }

    void Update()
    {
        
    }

    public void PlayGame()
    {
        if (MenuUI != null) MenuUI.SetActive(false);
        if (LoadingUI != null) LoadingUI.SetActive(true);
        timer = 0f;
        StartCoroutine(StartLoad());
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator StartLoad()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(LevelName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (fillImage != null)
            {
                fillImage.fillAmount = progress;
            }

            if (progressText != null)
            {
                progressText.text = Mathf.RoundToInt(progress * 100f).ToString() + "%";
            }

            if (operation.progress >= 0.9f && timer >= MinLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}