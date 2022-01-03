using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

namespace RhythmHeavenMania.Games.KarateMan
{
    public class BarrelDestroyEffect : MonoBehaviour
    {
        private SpriteRenderer SpriteRenderer;
        private Rigidbody2D rb2d;
        public int spriteIndex;

        private void Start()
        {
            SpriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
            SpriteRenderer.sprite = KarateMan.instance.BarrelSprites[spriteIndex];

            rb2d = this.gameObject.AddComponent<Rigidbody2D>();
            rb2d.gravityScale = 10;
            // rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;

            float randX = Random.Range(800, 1400);

            rb2d.AddForce(Vector3.up * Random.Range(900, 1666));
            rb2d.AddForce(Vector3.right * randX);
            // this.gameObject.AddComponent<Rotate>().rotateSpeed = Random.Range(60, 450);

            BoxCollider2D col = this.gameObject.AddComponent<BoxCollider2D>();
            col.offset = new Vector2(-0.0574677f, -0.07480353f);
            col.size = new Vector2(0.5694333f, 1.912059f);

            StartCoroutine(FadeOut());
        }

        private void Update()
        {
            this.transform.eulerAngles = new Vector3(0, 0, rb2d.velocity.magnitude * 4);
        }

        private IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(Conductor.instance.secPerBeat * 3);
            SpriteRenderer.DOColor(new Color(1, 1, 1, 0), Conductor.instance.secPerBeat * 3).OnComplete(delegate { Destroy(this.gameObject); });
        }
    }
}