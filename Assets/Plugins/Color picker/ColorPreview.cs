using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Starpelly;

public class ColorPreview : MonoBehaviour
{
    public Graphic previewGraphic;

    public ColorPicker colorPicker;

    public TMP_InputField hex;

    private void Start()
    {
        previewGraphic.color = colorPicker.color;
        colorPicker.onColorChanged += OnColorChanged;
    }

    public void ChangeColor(Color c)
    {
        colorPicker.color = c;
        hex.text = c.Color2Hex();
    }

    public void OnColorChanged(Color c)
    {
        previewGraphic.color = c;
        hex.text = c.Color2Hex();
    }

    public void SetColorFromHex(string hex)
    {
        colorPicker.color = Starpelly.Colors.Hex2RGB(hex);
    }

    private void OnDestroy()
    {
        if (colorPicker != null)
            colorPicker.onColorChanged -= OnColorChanged;
    }

    public void SetColorFromTMP()
    {
        SetColorFromHex(hex.text);
    }
}