using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroUIManager : MonoBehaviour {
    [Header("Main Menu")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private Button singlePlayButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton;

    [Header("Option Menu")]
    [SerializeField] private GameObject optionMenuCanvas;
    [SerializeField] private Button graphicTabButton;
    [SerializeField] private Button soundTabButton;
    [SerializeField] private Button exitTabButton;
    
    [Space(10f)]
    
    [SerializeField] private GameObject graphicTab;
    [SerializeField] private GameObject soundTab;

    [Space(10f)]

    [SerializeField] private GameObject loadingSceneCanvas;

    private GameObject prevTab;


    private void Init() {
        this.singlePlayButton.onClick.AddListener(SinglePlay);
        this.optionButton.onClick.AddListener(Option);
        this.quitButton.onClick.AddListener(GameQuit);

        this.graphicTabButton.onClick.AddListener(() => OptionTab(graphicTab));
        this.soundTabButton.onClick.AddListener(() => OptionTab(soundTab));
        this.exitTabButton.onClick.AddListener(OptionExit);

        this.loadingSceneCanvas.SetActive(false);

        this.prevTab = this.graphicTab;
    }

    private void Start() {
        Init();
    }
    
    private void SinglePlay() {
        StartCoroutine(LoadSceneAsync(1));
    }

    private void Option() {
        this.mainMenuCanvas.SetActive(false);
        this.optionMenuCanvas.SetActive(true);
    }

    private void GameQuit() {
        Application.Quit();
    }

    private void OptionTab(GameObject tab) {
        if (this.prevTab.activeInHierarchy) {
            this.prevTab.SetActive(false);
        }
        
        this.prevTab = tab;

        tab.SetActive(true);
    }

    private void OptionExit() {
        this.optionMenuCanvas.SetActive(false);
        this.mainMenuCanvas.SetActive(true);
    }

    IEnumerator LoadSceneAsync(int sceneID) {
        this.loadingSceneCanvas.SetActive(true);

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
            this.loadingSceneCanvas.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, timer);
        }
    }
}