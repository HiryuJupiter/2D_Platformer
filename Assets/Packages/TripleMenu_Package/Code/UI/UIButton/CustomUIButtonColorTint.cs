using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//OnPointer events work by implementing IPointerxHandler. You cannot extract them into your own custom interface.
public abstract class CustomUIButtonColorTint : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    protected Image background;
    protected Text label;

    public bool isDisabled { get; private set; } = false;

    protected void Awake()
    {
        background = GetComponent<Image>();
        label = GetComponentInChildren<Text>();

        SetDefaultColor();
    }

    #region Public
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDisabled)
        {
            SetHighlightColor();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDisabled)
        {
            SetDefaultColor();
        }
    }

    public void DisableButton()
    {
        isDisabled = true;
        SetDisabledColor();
    }

    public void EnableButton()
    {
        isDisabled = false;
        SetDefaultColor();
    }
    #endregion

    protected abstract void SetDefaultColor();

    protected abstract void SetHighlightColor();

    protected abstract void SetDisabledColor();
}