using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio.Editor.Track;

using TMPro;

namespace HeavenStudio.Editor 
{
    public class TabsManager : MonoBehaviour
    {
        [SerializeField] GameObject activeContent;

        public void SetActiveContent(GameObject content)
        {
            if (activeContent != null)
            {
                activeContent.SetActive(false);
            }
            activeContent = content;
            activeContent.SetActive(true);
        }
    }
}