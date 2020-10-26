using UnityEngine;
using System.Collections;

public class PauseMenuCanvasTransition : MonoBehaviour
{
    public CanvasGroup canvas_Pause;
    public CanvasGroup canvas_Options;
    public GameObject background;
    const float transitionTime = 0.2f;


    private void Awake()
    {
        CanvasGroupHelper.InstantHide(canvas_Pause);
        CanvasGroupHelper.InstantHide(canvas_Options);
        background.SetActive(false);
    }

    public void Pause()
    {
        CanvasGroupHelper.InstantReveal(canvas_Pause);
        background.SetActive(true);
    }

    public void UnPause()
    {
        CanvasGroupHelper.InstantHide(canvas_Pause);
        background.SetActive(false);
    }

    public void PauseMainToOptions()
    {
        CanvasGroupHelper.InstantReveal(canvas_Options);
        CanvasGroupHelper.InstantHide(canvas_Pause);
    }

    public void OptionsToPauseMain()
    {
        CanvasGroupHelper.InstantReveal(canvas_Pause);
        CanvasGroupHelper.InstantHide(canvas_Options);
    }
}