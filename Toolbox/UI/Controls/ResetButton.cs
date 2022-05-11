
using UnityEngine;
using UnityEngine.EventSystems;

public class ResetButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string control;
    [SerializeField] private string actionName;
    [SerializeField] Canvas parentCanvas;
    private ControlManager manager;

    private void Start()
    {
        manager = ModulManager.get<ControlManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!parentCanvas.enabled) return;
        if (control.Equals("-")) manager.resetAllKeys();
        else manager.resetKey(control, actionName);
    }
}
