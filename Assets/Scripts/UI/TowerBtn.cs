using Standalone;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnscriptedEngine;
using DG.Tweening;

public class TowerBtn : UButtonComponent, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameTMP;

    public void Initalize(UCanvasController context, TowerSO towerSO)
    {
        InitializeUIComponent(context);
        nameTMP.GetComponent<UTextComponent>().InitializeUIComponent(context);
        nameTMP.transform.localScale = Vector3.zero;

        icon.sprite = towerSO.BaseIcon;
        nameTMP.text = $"{towerSO.TreeList[0].Name} (${towerSO.TowerCost})";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hello");
        nameTMP.transform.DOLocalMoveX(125, 0.15f).SetEase(Ease.OutSine);
        nameTMP.transform.DOScale(1, 0.15f).SetEase(Ease.OutSine);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        nameTMP.transform.DOLocalMoveX(0, 0.15f).SetEase(Ease.InSine);
        nameTMP.transform.DOScale(0, 0.15f).SetEase(Ease.InSine);
    }
}
