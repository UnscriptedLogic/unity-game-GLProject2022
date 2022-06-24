using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainScreenUIController : MonoBehaviour
    {
        [SerializeField] private float transitionSpeed = 1f;
        [SerializeField] private LeanTweenType easeIn = LeanTweenType.easeOutSine;
        [SerializeField] private LeanTweenType easeOut = LeanTweenType.easeInSine;
        [SerializeField] private Transform mainScreenPage;
        [SerializeField] private Transform lbPage;

        [SerializeField] private Transform logIn;
        [SerializeField] private Transform signUp;

        [SerializeField] private Transform usernamePrompt;
        [SerializeField] private CanvasGroup backgroundPrompt;
        [SerializeField] private LeanTweenType promptEaseIn = LeanTweenType.easeOutBack;
        [SerializeField] private LeanTweenType promptEaseOut = LeanTweenType.easeInBack;

        [SerializeField] private Button aboutButton;

        public Button AboutButton => aboutButton;

        private void Start()
        {
            logIn.gameObject.SetActive(false);
            signUp.gameObject.SetActive(false);
        }

        public void ParseSeed(string text)
        {
            if (text != "")
            {
                GameManager.seed = int.Parse(text);
                GameManager.setSeed = true;
            }
            else
            {
                GameManager.seed = 0;
            }
        }

        //Main Screen
        public void ShowMainScreen()
        {
            mainScreenPage.localPosition = new Vector2(-Screen.width, 0f);
            mainScreenPage.LeanMoveLocalX(0, transitionSpeed).setEase(easeIn);
        }

        public void HideMainScreen()
        {
            mainScreenPage.LeanMoveLocalX(-Screen.width, transitionSpeed).setEase(easeOut);
        }

        //Leaderboard
        public void ShowLeaderBoards()
        {
            lbPage.gameObject.SetActive(true);
            lbPage.localPosition = new Vector2(Screen.width, 0f);
            lbPage.LeanMoveLocalX(0, transitionSpeed).setEase(easeIn);
        }

        public void HideLeaderBoards()
        {
            lbPage.LeanMoveLocalX(Screen.width, transitionSpeed).setEase(easeOut).setOnComplete(() => { lbPage.gameObject.SetActive(false); });
        }

        //Account
        public void ShowSignIn()
        {
            if (!signUp.gameObject.activeInHierarchy)
            {
                signUp.gameObject.SetActive(true);
                signUp.LeanMoveLocalX(-275, transitionSpeed).setEase(easeIn);
            }
            else
            {
                HideSignIn();
            }
        }

        public void HideSignIn()
        {
            if (signUp.gameObject.activeInHierarchy)
                signUp.LeanMoveLocalX(-672, transitionSpeed).setEase(easeIn).setOnComplete(() => { signUp.gameObject.SetActive(false); });
        }

        public void ShowLogIn()
        {
            if (!logIn.gameObject.activeInHierarchy)
            {
                logIn.gameObject.SetActive(true);
                logIn.LeanMoveLocalX(-275, transitionSpeed).setEase(easeIn);
            }
            else
            {
                HideLogIn();
            }
        }

        public void HideLogIn()
        {
            if (logIn.gameObject.activeInHierarchy)
                logIn.LeanMoveLocalX(-672, transitionSpeed).setEase(easeIn).setOnComplete(() => { logIn.gameObject.SetActive(false); });
        }

        public void ShowUsernamePrompt()
        {
            if (!usernamePrompt.gameObject.activeSelf)
            {
                backgroundPrompt.gameObject.SetActive(true);
                usernamePrompt.gameObject.SetActive(true);

                backgroundPrompt.alpha = 0f;
                usernamePrompt.localPosition = new Vector2(0f, -Screen.height);

                backgroundPrompt.LeanAlpha(1, transitionSpeed).setDelay(1);
                usernamePrompt.LeanMoveLocalY(0, transitionSpeed).setEase(promptEaseIn).setDelay(2); 
            }
        }

        public void HideUsernamePrompt()
        {
            if (usernamePrompt.gameObject.activeSelf)
            {
                backgroundPrompt.LeanAlpha(0, transitionSpeed).setOnComplete(() => { backgroundPrompt.gameObject.SetActive(false); });
                usernamePrompt.LeanMoveLocalY(-Screen.height, transitionSpeed).setEase(promptEaseOut).setOnComplete(() => { usernamePrompt.gameObject.SetActive(false); }); 
            }
        }
    }
}