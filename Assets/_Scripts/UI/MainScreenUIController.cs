using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MainScreenUIController : MonoBehaviour
    {
        [SerializeField] private float transitionSpeed = 1f;
        [SerializeField] private LeanTweenType easeIn = LeanTweenType.easeOutSine;
        [SerializeField] private LeanTweenType easeOut = LeanTweenType.easeInSine;
        [SerializeField] private Transform mainScreenPage;

        public void ShowScreen()
        {
            mainScreenPage.localPosition = new Vector2(-Screen.width, 0f);
            mainScreenPage.LeanMoveLocalX(0, transitionSpeed).setEase(easeIn);
        }

        public void HideScreen()
        {
            mainScreenPage.LeanMoveLocalX(-Screen.width, transitionSpeed).setEase(easeOut);
        }
    }
}