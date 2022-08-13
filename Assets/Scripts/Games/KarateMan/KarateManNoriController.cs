using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_KarateMan
{
    public class KarateManNoriController : MonoBehaviour
    {
        public GameObject NoriController;
        public GameObject NoriHeart;
        public Transform NoriHolder;
        public Material NoriMaterial;

        public Color[] NoriColorsTengoku;
        public Color[] NoriColorsMania;

        public float Nori;
        public int MaxNori;
        Animator[] NoriHeartAnimators;
        Material[] NoriHeartMaterials;

        static Vector2 HeartScale = new Vector2(60, 52);
        static float ScaleFactorTengoku = 1/46f;
        static float ScaleFactorMania = 1/72f;

        static float CameraOffset = 10;
        static float PeriodLow = 24/60f;
        static float PeriodHigh = 15/60f;

        int noriMode = (int) KarateMan.NoriMode.None;


        void Start()
        {

        }

        public void SetNoriMode(int mode, int startingNori = 0)
        {
            float scaleFactor = 0f;
            //clear all children of the holder
            foreach (Transform child in NoriHolder)
            {
                Destroy(child.gameObject);
            }

            switch (mode)
            {
                case (int) KarateMan.NoriMode.Tengoku:
                    MaxNori = 5;
                    Nori = Mathf.Clamp(startingNori, 0, MaxNori);
                    scaleFactor = ScaleFactorTengoku;
                    break;
                case (int) KarateMan.NoriMode.Mania:
                    MaxNori = 10;
                    Nori = Mathf.Clamp(startingNori, 0, MaxNori);
                    scaleFactor = ScaleFactorMania;
                    break;
                default:
                    MaxNori = 0;
                    Nori = 0;
                    return;
            }

            NoriHeartAnimators = new Animator[MaxNori];
            NoriHeartMaterials = new Material[MaxNori];
            noriMode = mode;

            if (mode == (int) KarateMan.NoriMode.None) return;

            //add the label?

            for (int i = 0; i < MaxNori; i++)
            {
                GameObject h = GameObject.Instantiate(NoriHeart, NoriHolder);
                h.SetActive(true);

                Material m_Material = Instantiate(NoriMaterial);
                NoriHeartMaterials[i] = m_Material;
                h.GetComponent<Image>().material = m_Material;

                RectTransform hrect = h.GetComponent<RectTransform>();
                hrect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HeartScale.x * scaleFactor);
                hrect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, HeartScale.y * scaleFactor);

                NoriHeartAnimators[i] = h.GetComponent<Animator>();
                NoriHeartAnimators[i].Play(i <= (int) (startingNori - 1) ? "NoriFull" : "NoriNone", -1, (Time.time * PeriodLow) % 1f);
            }
        }

        public void DoHit()
        {
            if (noriMode == (int) KarateMan.NoriMode.None) return;
            if (noriMode == (int) KarateMan.NoriMode.Tengoku)
            {
                if (Nori >= MaxNori) return;
                Nori += 1;
                NoriHeartAnimators[(int) Nori - 1].Play("NoriFull", -1, (Time.time * PeriodHigh) % 1f);
                UpdateHeartColours();
            }
        }

        public void DoNG()
        {
            if (noriMode == (int) KarateMan.NoriMode.None) return;
            if (noriMode == (int) KarateMan.NoriMode.Tengoku)
            {
                if (Nori <= 0) return;
                Nori -= 1;
                NoriHeartAnimators[(int) Nori].Play("NoriNone", -1, (Time.time * PeriodLow) % 1f);
                UpdateHeartColours();
            }
        }

        public void DoThrough()
        {
            if (noriMode == (int) KarateMan.NoriMode.None) return;
            if (noriMode == (int) KarateMan.NoriMode.Tengoku)
            {
                Nori = 0;
                foreach (Animator anim in NoriHeartAnimators)
                {
                    anim.Play("NoriNone", -1, (Time.time * PeriodLow) % 1f);
                }
            }
        }

        void UpdateHeartColours()
        {
            if (noriMode == (int) KarateMan.NoriMode.None) return;
            if (noriMode == (int) KarateMan.NoriMode.Tengoku)
            {
                for (int i = 0; i < NoriHeartMaterials.Length; i++)
                {
                    Material mat = NoriHeartMaterials[i];
                    if (Nori == MaxNori)
                    {
                        mat.SetColor("_ColorAlpha", NoriColorsTengoku[3]);
                    }
                    else
                    {
                        if (KarateMan.instance.NoriPerformance < 0.6)
                            mat.SetColor("_ColorAlpha", NoriColorsTengoku[0]);
                        else
                        {
                            if (i < 2)
                                mat.SetColor("_ColorAlpha", NoriColorsTengoku[1]);
                            else
                                mat.SetColor("_ColorAlpha", NoriColorsTengoku[2]);
                        }
                    }
                }
            }
            else
            {

            }
        }

        void Update()
        {
            Transform target = GameCamera.instance.transform;

            Vector3 displacement = target.forward * CameraOffset; 
            transform.position = target.position + displacement;
            transform.rotation = target.rotation;

            UpdateHeartColours();
        }
    }
}