using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroUIManager : MonoBehaviour {
    [SerializeField] private Button singlePlayButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    [Space(10f)]

    [SerializeField] private GameObject loadingScene;


    private void Init() {
        this.singlePlayButton.onClick.AddListener(SinglePlay);
        this.optionButton.onClick.AddListener(Option);
        this.quitButton.onClick.AddListener(GameQuit);

        this.loadingScene.SetActive(false);
    }

    private void Start() {
        Init();
    }
    
    private void SinglePlay() {
        StartCoroutine(LoadSceneAsync(1));
    }

    private void Option() {
        // TODO
    }

    private void GameQuit() {
        Application.Quit();
    }

    IEnumerator LoadSceneAsync(int sceneID) {
        this.loadingScene.SetActive(true);

        yield return StartCoroutine(LoadingFade());
        
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        operation.allowSceneActivation = false;
        
        while (!operation.isDone) {
            if (operation.progress >= 0.9f) {
                yield return new WaitForSeconds(2); 
                
                operation.allowSceneActivation = true;

                // if (GameManager.instance != null) {
                //     GameManager.instance.gameObject.SetActive(false);
                //     GameManager.instance.gameObject.SetActive(true);
                // }
                
                yield return null;
            }
        }
    }

    IEnumerator LoadingFade() {
        float timer = 0;

        while (timer <= 1) {
            yield return null;

            timer += Time.unscaledDeltaTime;
            this.loadingScene.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, timer);
        }
    }
}