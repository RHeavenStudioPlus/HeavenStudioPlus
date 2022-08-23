using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Editor;

public class NewRemixDialog : MonoBehaviour
{
    [SerializeField] private GameObject diag;

    public void SwitchNewDialog()
    {
        if(diag.activeSelf) {
            diag.SetActive(false);
        } else {
            diag.SetActive(true);
        }
    }

    public void Confirm()
    {
        Editor.instance.NewRemix();
        if(diag.activeSelf) {
            diag.SetActive(false);
        }
    }
}
