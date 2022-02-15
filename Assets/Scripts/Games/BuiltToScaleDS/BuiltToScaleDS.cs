using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyBezierCurves;
using DG.Tweening;

using RhythmHeavenMania.Util;
namespace RhythmHeavenMania.Games.BuiltToScaleDS
{
    public class BuiltToScaleDS : Minigame
    {
        [Header("Camera")]
        public Transform renderQuadTrans;
        public Transform cameraPos;

        [Header("References")]
        public SkinnedMeshRenderer environmentRenderer;

        [Header("Properties")]
        public float beltSpeed = 1f;

        private Material beltMaterial;
        private Material[] environmentMaterials;
        private float currentBeltOffset;
        
        public static BuiltToScaleDS instance;

        private void Awake()
        {
            instance = this;

            environmentMaterials = environmentRenderer.materials;
            beltMaterial = Instantiate(environmentMaterials[8]);
            environmentMaterials[8] = beltMaterial;
        }

        
        void Start()
        {
            renderQuadTrans.gameObject.SetActive(true);
            
            var cam = GameCamera.instance.camera;
            var camHeight = 2f * cam.orthographicSize;
            var camWidth = camHeight * cam.aspect;
            renderQuadTrans.localScale = new Vector3(camWidth, camHeight, 1f);
        }

        void Update()
        {
            currentBeltOffset = (currentBeltOffset + Time.deltaTime * -beltSpeed) % 1f;
            beltMaterial.mainTextureOffset = new Vector2(0f, currentBeltOffset);
            environmentRenderer.materials = environmentMaterials;
        }
    }
}