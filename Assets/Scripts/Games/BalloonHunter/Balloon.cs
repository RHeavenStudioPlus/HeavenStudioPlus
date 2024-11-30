using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HeavenStudio.Util;

namespace HeavenStudio.Games.Scripts_BalloonHunter
{
    public class Balloon : MonoBehaviour
    {
        public double startBeat;
        public float balloonSpeed;

        [Header("Components")]
        [SerializeField] Animator anim;
        [SerializeField] Animator hunterAnim;
        [SerializeField] Animator popEffect;

        public BalloonHunter game;
        // Start is called before the first frame update
        private void Start()
        {
            game.ScheduleInput(startBeat, balloonSpeed-1, Minigame.InputAction_BasicPress, Pop, Miss, null);
        }

        // Update is called once per frame
        private void Update()
        {
            float normalizedBeat = Conductor.instance.GetPositionFromBeat(startBeat, balloonSpeed);
            if (normalizedBeat > 1) Destroy(gameObject);
            anim.DoNormalizedAnimation("Move", normalizedBeat, 0);
        }

        public void Pop(PlayerActionEvent caller, float state)
        {
            hunterAnim.DoScaledAnimationAsync("Shoot", 0.5f, animLayer : 0);
            hunterAnim.DoScaledAnimationAsync("Blow", 0.5f, animLayer: 1);
            SoundByte.PlayOneShotGame("balloonHunter/blow");
            game.hunterHold = false;

            if (state is >= 1 or <= -1)
            {
                SoundByte.PlayOneShotGame("balloonHunter/miss");
                anim.DoScaledAnimationAsync("Miss", 0.5f);
                game.bopExpression = "Sad";
            }
            else
            {
                SoundByte.PlayOneShotGame("balloonHunter/pop");
                popEffect.DoScaledAnimationAsync("Pop", 0.5f);
                if (game.bopExpression == "Neutral") { game.bopExpression = "Happy"; }
                Destroy(gameObject);
            }
        }

        public void Miss(PlayerActionEvent caller)
        {
            game.bopExpression = "Sad";
        }
    }
}