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
        swingInput.text = (tempoObj.chartEntity["swing"] * 100 + 50).ToString("F");
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

    public void RefreshSwingUI()
    {
        swingInput.text = (tempoObj.chartEntity["swing"] * 100 + 50).ToString("F");
        swingSlider.value = tempoObj.chartEntity["swing"];
    }

    public void SwingSliderUpdate()
    {
        if (tempoObj != null)
        {
            tempoObj.SetSwing(System.MathF.Round(swingSlider.value, 4));
            RefreshSwingUI();
        }
    }

    public void SwingManualUpdate()
    {
        if (tempoObj != null)
        {
            float swing = float.Parse(swingInput.text);
            tempoObj.SetSwing(swing / 100f - 0.5f);
            RefreshSwingUI();
        }
    }

    /*
    a note for future reference:

    all stored swing values (like ones in remix.json) are the SWING RATIO - 0.5
    for example, a swing ratio of 50% would be 0.5 - 0.5 = 0
    and a ratio of 66.67% would be 0.6667 - 0.5 = 0.1667

    you put that final value into tempoObj.SetSwing() to set it,
    hence why the above functions use that weird formula and the below functions set it directly

    in addition, you can get the swing ratio (as a percentage) from this value by
    multiplying the value by 100, then adding 50

    this is really stupid lol
    minenice why didnt you write any of this down lmao - zeo
    */

    public void SetStraightSwing()
    {
        if (tempoObj != null)
        {
            tempoObj.SetSwing(0f);
            RefreshSwingUI();
        }
    }

    public void SetLightSwing()
    {
        if (tempoObj != null)
        {
            tempoObj.SetSwing(0.1f);
            RefreshSwingUI();
        }
    }

    public void SetNormalSwing()
    {
        if (tempoObj != null)
        {
            tempoObj.SetSwing(0.1667f);
            RefreshSwingUI();
        }
    }

    public void SetHeavySwing()
    {
        if (tempoObj != null)
        {
            tempoObj.SetSwing(0.25f);
            RefreshSwingUI();
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
