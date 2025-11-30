using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTransition : MonoBehaviour
{
    public string levelToLoad = "MainMenu";
    public GameObject loadingScreen;

    private bool transitioning = false;

    //public void Start()
    //{
    //    loadingScreen = GameObject.Find("LoadingScreen");
    //}

    public void TransitionTo(string level)
    {
        if (transitioning) return;
        transitioning = true;
        loadingScreen.SetActive(true);
        loadingScreen.GetComponentInChildren<Image>().DOColor(new Color(0, 0, 0, 1), 0.2f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                var promise = SceneManager.LoadSceneAsync(level, new LoadSceneParameters(LoadSceneMode.Single));
                promise.completed += (asyncOperation) =>
                {
                    loadingScreen.GetComponentInChildren<Image>().DOColor(new Color(0, 0, 0, 0), 0.5f).SetEase(Ease.InOutQuad)
                    .OnComplete(() =>
                        {
                            loadingScreen.SetActive(false);
                        });
                };
            });
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            TransitionTo(levelToLoad);
        }
    }

}
