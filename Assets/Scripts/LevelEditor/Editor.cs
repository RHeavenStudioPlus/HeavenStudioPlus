using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

namespace RhythmHeavenMania.Editor
{
    public class Editor : MonoBehaviour
    {
        private Initializer Initializer;

        [SerializeField] private Canvas MainCanvas;

        [Header("Rect")]
        [SerializeField] private RenderTexture ScreenRenderTexture;
        [SerializeField] private RawImage Screen;

        [Header("Components")]
        [SerializeField] private Timeline Timeline;

        private void Start()
        {
            Initializer = GetComponent<Initializer>();

            MainCanvas.gameObject.SetActive(false);
            print("d");
        }

        public void Init()
        {
            GameManager.instance.GameCamera.targetTexture = ScreenRenderTexture;
            GameManager.instance.CursorCam.targetTexture = ScreenRenderTexture;
            Screen.texture = ScreenRenderTexture;

            MainCanvas.gameObject.SetActive(true);

            Timeline.Init();
        }
    }
}