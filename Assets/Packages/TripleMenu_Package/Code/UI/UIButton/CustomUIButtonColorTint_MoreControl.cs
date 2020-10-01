using UnityEngine;
using UnityEngine.UI;

public class CustomUIButtonColorTint_MoreControl : CustomUIButtonColorTint
{
    [SerializeField] Color colorHighlight_bg = Color.black;
    [SerializeField] Color colorDefault_bg = new Color(0.082f, 0.102f, 0.118f, 1.000f);
    [SerializeField] Color colorDisabled_bg = Color.black;
    [SerializeField] Color colorHighlight_label = Color.white;
    [SerializeField] Color colorDefault_label = Color.white;
    [SerializeField] Color colorDisabled_label = Color.black;

    protected override void SetDefaultColor()
    {
        background.color = colorDefault_bg;
        label.color = colorDefault_label;
    }

    protected override void SetHighlightColor()
    {
        background.color = colorHighlight_bg;
        label.color = colorHighlight_label;
    }

    protected override void SetDisabledColor()
    {
        background.color = colorDisabled_bg;
        label.color = colorDisabled_label;
    }
}