using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using HeavenStudio;
using HeavenStudio.Editor;
using HeavenStudio.Editor.Track;
using TMPro;

public class TempoDialog : Dialog
{
    TempoTimelineObj tempoObj;

    [SerializeField] Button deleteButton;
    [SerializeField] TMP_InputField tempoInput;
    [SerializeField] TMP_InputField swingInput;
    [SerializeField] Slider swingSlider;
    [SerializeField] Toggle swingDivisionToggle;

    public void SwitchTempoDialog()
    {
        if (dialog.activeSelf)
        {
            tempoObj = null;
            dialog.SetActive(false);
            Editor.instance.inAuthorativeMenu = false;
        }
        else
        {
            Editor.instance.inAuthorativeMenu = true;
            ResetAllDialogs();
            dialog.SetActive(true);

            swingSlider.maxValue = 0.25f;
            swingSlider.minValue = 0;
        }
    }

    public void RefreshDialog()
    {
        if (tempoObj != null)
        {
            tempoInput.text = tempoObj.chartEntity["tempo"].ToString("F");
        }
    }

    public void SetTempoObj(TempoTimelineObj tempoObj)
    {
        this.tempoObj = tempoObj;
        deleteButton.gameObject.SetActive(!tempoObj.first);

        tempoInput.text = tempoObj.chartEntity["tempo"].ToString("F");
        swingInput.text = (tempoObj.chartEntity["swing"] * 400).ToString("F");
        swingSlider.value = tempoObj.chartEntity["swing"];

        swingDivisionToggle.isOn = tempoObj.chartEntity["swingDivision"] != 1f;
    }

    public void DeleteTempo()
    {
        if (tempoObj != null)
        {
            tempoObj.Remove();
        }
        if (dialog.activeSelf)
        {
            SwitchTempoDialog();
        }
    }

    public void SetTempo()
    {
        if (tempoObj != null)
        {
            float tempo = float.Parse(tempoInput.text);
            tempoObj.SetTempo(tempo);
            tempoInput.text = tempoObj.chartEntity["tempo"].ToString("F");
        }
    }

    public void DoubleTempo()
    {
        if (tempoObj != null)
        {
            tempoObj.SetTempo(tempoObj.chartEntity["tempo"] * 2);
            tempoInput.text = tempoObj.chartEntity["tempo"].ToString("F");
        }
    }

    public void HalveTempo()
    {
        if (tempoObj != null)
        {
            tempoObj.SetTempo(tempoObj.chartEntity["tempo"] * 0.5f);
            tempoInput.text = tempoObj.chartEntity["tempo"].ToString("F");
        }
    }

    public void SwingSliderUpdate()
    {
        if (tempoObj != null)
        {
            tempoObj.SetSwing(System.MathF.Round(swingSlider.value, 4));
            swingInput.text = (tempoObj.chartEntity["swing"] * 400).ToString("F");
            swingSlider.value = tempoObj.chartEntity["swing"];
        }
    }

    public void SetSwing()
    {
        if (tempoObj != null)
        {
            float swing = float.Parse(swingInput.text);
            tempoObj.SetSwing(swing * 0.25f / 100f);
            swingInput.text = (tempoObj.chartEntity["swing"] * 400).ToString("F");
            swingSlider.value = tempoObj.chartEntity["swing"];
        }
    }

    public void SwingDivisionToggle()
    {
        if (tempoObj != null)
        {
            tempoObj.SetSwingDivision(swingDivisionToggle.isOn);
        }
    }
}
