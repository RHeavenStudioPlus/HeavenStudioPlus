using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

namespace HeavenStudio.Editor
{
    public class GridGameSelectorGame : MonoBehaviour
    {
        public GameObject GameTitlePreview;
        public Animator StarAnim;
        public bool StarActive;

        public GridGameSelector GridGameSelector;

        public Texture MaskTex;
        public Texture BgTex;
        private Material m_Material;
        private Image image;

        private void Start()
        {
            // image = GetComponent<Image>();
            Tooltip.AddTooltip(gameObject, EventCaller.instance.GetMinigame(gameObject.name).displayName);
        }

        private void OnEnable()
        {
            if (StarActive) StarAnim.Play("Appear", 0, 1);
        }

        public void SetupTextures()
        {
            if (m_Material == null)
            {
                if (image == null) image = GetComponent<Image>();

                m_Material = Instantiate(image.material);
                image.material = m_Material;
            }
            m_Material.SetTexture("_MaskTex", MaskTex);
            m_Material.SetTexture("_BgTex", BgTex);
        }

        public void OnClick()
        {
            if (Input.GetMouseButtonUp(0)) 
            {
                GridGameSelector.SelectGame(this.gameObject.name);
            }
        }

        public void OnDown()
        {
            if (Input.GetMouseButtonDown(1)) 
            {
                // while holding shift and the game icon clicked has a star, it will disable all stars.
                if (Input.GetKey(KeyCode.LeftShift) && StarActive) {
                    for (int i = 0; i < transform.parent.childCount; i++) {
                        var ggsg = transform.parent.GetChild(i).GetComponent<GridGameSelectorGame>();
                        if (ggsg.StarActive) ggsg.Star();
                    }
                } else {
                    Star();
                }
            } 
        }

        public void Star() 
        {
            StarAnim.CrossFade(StarActive ? "Disappear" : "Appear", 0.3f);
            StarActive = !StarActive;
        }

        //TODO: animate between shapes
        public void ClickIcon()
        {
            transform.DOScale(new Vector3(1.15f, 1.15f, 1f), 0.1f);
            BgTex = GridGameSelector.Circle;
            SetupTextures();
        }

        public void UnClickIcon()
        {
            transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
            BgTex = GridGameSelector.Square;
            SetupTextures();
        }
    }
}