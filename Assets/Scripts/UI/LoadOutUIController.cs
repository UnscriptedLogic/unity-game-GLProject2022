using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class LoadOutUIController : MonoBehaviour
    {
        [SerializeField] private float transitionSpeed = 1f;
        [SerializeField] private LeanTweenType easeIn = LeanTweenType.easeOutExpo;
        [SerializeField] private LeanTweenType easeOut = LeanTweenType.easeInExpo;
        [SerializeField] private Transform loadOutPage;

        private void OnEnable()
        {
            loadOutPage.localPosition = new Vector2(Screen.width, 0f);
            loadOutPage.LeanMoveLocalX(0, transitionSpeed).setEase(easeIn);
        }

        public void CloseLoadOut()
        {
            loadOutPage.LeanMoveLocalX(Screen.width, transitionSpeed).setEase(easeOut).setOnComplete(() => { loadOutPage.gameObject.SetActive(false); });
        }
    }
}