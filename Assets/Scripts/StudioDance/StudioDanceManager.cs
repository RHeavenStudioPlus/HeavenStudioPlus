using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavenStudio;

namespace HeavenStudio.StudioDance
{
    public class StudioDanceManager : MonoBehaviour
    {
        [SerializeField] private GameObject windowBase;
        [SerializeField] private Transform windowHolder;
        [SerializeField] private GameObject content;

        public void OpenDanceWindow()
        {
            var mobj = GameObject.Instantiate(windowBase, windowHolder);
            mobj.SetActive(true);
            content.SetActive(true);
        }

        public void CloseDanceWindow()
        {
            content.SetActive(false);
        }
    }
}