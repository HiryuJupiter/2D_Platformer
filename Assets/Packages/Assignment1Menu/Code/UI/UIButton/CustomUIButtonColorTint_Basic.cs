using UnityEngine;
using UnityEngine.UI;

public class CustomUIButtonColorTint_Basic : CustomUIButtonColorTint
{
    [SerializeField] Color defaultColor = new Color(0.082f, 0.102f, 0.118f, 1.000f);
    [SerializeField] Color highlightColor = Color.white;
    [SerializeField] Color disabledColor = Color.grey;

    protected override void SetDefaultColor()
    {
        background.color = defaultColor;
        label.color = highlightColor;
    }

    protected override void SetHighlightColor()
    {
        background.color = highlightColor;
        label.color = defaultColor;
    }

    protected override void SetDisabledColor()
    {
        background.color = defaultColor;
        label.color = disabledColor;
    }
}