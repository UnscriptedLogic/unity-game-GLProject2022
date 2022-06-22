using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ThanksUIController : MonoBehaviour
    {
        [SerializeField] private float transitionSpeed = 1f;
        [SerializeField] private LeanTweenType easeIn = LeanTweenType.easeOutSine;
        [SerializeField] private LeanTweenType easeOut = LeanTweenType.easeInSine;
        [SerializeField] private Transform dialogue;

        public void ShowDialogue()
        {
            dialogue.gameObject.SetActive(true);
            dialogue.localPosition = new Vector2(dialogue.localPosition.x, -Screen.height);
            dialogue.LeanMoveLocalY(0, transitionSpeed).setEase(easeIn).setDelay(1);
        }

        public void HideDialogue()
        {
            dialogue.LeanMoveLocalY(-Screen.height, transitionSpeed).setEase(easeOut).setOnComplete(() => { dialogue.gameObject.SetActive(false); });
        }
    }
}