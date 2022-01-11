using UnityEngine;
using UnityEngine.UI;

namespace RhythmHeavenMania.Editor
{
    public class GridGameSelectorGame : MonoBehaviour
    {
        public GameObject GameTitlePreview;

        public GridGameSelector GridGameSelector;

        public void OnClick()
        {
            GridGameSelector.SelectGame(this.gameObject.name);
        }

        public void OnEnter()
        {
            GameTitlePreview.GetComponent<Image>().enabled = true;
            GameTitlePreview.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = this.gameObject.name;
            GameTitlePreview.transform.GetChild(0).gameObject.SetActive(true);
        }

        public void OnExit()
        {
            GameTitlePreview.GetComponent<Image>().enabled = false;
            GameTitlePreview.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}