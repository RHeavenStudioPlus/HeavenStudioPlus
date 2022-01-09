using UnityEngine;
using UnityEngine.UI;

namespace RhythmHeavenMania.Editor
{
    public class GridGameSelector : MonoBehaviour
    {
        public GameObject GameTitlePreview;

        public void OnEnter()
        {
            GameTitlePreview.GetComponent<Image>().enabled = true;
        }

        public void OnExit()
        {
            GameTitlePreview.GetComponent<Image>().enabled = false;
        }
    }
}