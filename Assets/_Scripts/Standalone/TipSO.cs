using UnityEngine;

namespace Standalone
{
    [CreateAssetMenu(fileName = "LoadingTip", menuName = "ScriptableObjects/New Loading Tip")]
    public class TipSO : ScriptableObject 
    {
        [TextArea(5, 10)]
        [SerializeField] private string[] tips;

        public string[] Tips => tips;
    }
}