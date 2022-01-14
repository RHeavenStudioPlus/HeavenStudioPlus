using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json;

namespace RhythmHeavenMania.Editor
{
    public class EditorTheme : MonoBehaviour
    {
        public TextAsset ThemeTXT;
        public static Theme theme;

        [Header("Components")]
        [SerializeField] private Image[] Layers;

        private void Awake()
        {
            theme = JsonConvert.DeserializeObject<Theme>(ThemeTXT.text);
        }

        private void Start()
        {

            Layers[0].color = Starpelly.Colors.Hex2RGB(theme.properties.Layer1Col);
            Layers[1].color = Starpelly.Colors.Hex2RGB(theme.properties.Layer2Col);
            Layers[2].color = Starpelly.Colors.Hex2RGB(theme.properties.Layer3Col);
            Layers[3].color = Starpelly.Colors.Hex2RGB(theme.properties.Layer4Col);
        }
    }

}