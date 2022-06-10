using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Scene
{
    public class GameSceneManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string showLoadPage = "showTransition";
        [SerializeField] private string hideLoadPage = "exitTransition";

        private const string MainMenuScene = "MainMenuScene";
        private const string GameScene = "GameScene";

        public void StartGame() => TransitionScene(GameScene, 1f);
        public void HideLoading() => animator.SetTrigger(hideLoadPage);
        public void ReturnHome() => TransitionScene(MainMenuScene, 1f);

        private void TransitionScene(string sceneName, float duration) => StartCoroutine(DelaySceneChange(sceneName, duration));
        private void ChangeScene(string sceneName) => SceneManager.LoadSceneAsync(sceneName);

        private IEnumerator DelaySceneChange(string sceneName, float duration)
        {
            animator.SetTrigger(showLoadPage);
            yield return new WaitForSeconds(duration);
            ChangeScene(sceneName);
        }
    }
}