using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace HeavenStudio.Util
{
    public class BeatAction
    {
        public delegate void EventCallback();

        public class Action
        {
            public double beat { get; set; }
            public EventCallback function { get; set; }

            public Action(double beat, EventCallback function, bool ignoreSwing = false)
            {
                if (ignoreSwing) this.beat = Conductor.instance.GetSwungBeat(beat);
                else this.beat = beat;
                this.function = function;
            }
        }

        public static CancellationTokenSource New(MonoBehaviour behaviour, List<Action> actions)
        {
            if (behaviour == null)
            {
                Debug.LogWarning("Starting a BeatAction with no assigned behaviour. The Game Manager will be used instead.");
                behaviour = GameManager.instance;
            }
            CancellationTokenSource cancelToken = new CancellationTokenSource();
            RunAsync(behaviour, actions, cancelToken.Token).Forget();

            return cancelToken;
        }

        static async UniTask RunAsync(MonoBehaviour behaviour, List<Action> actions, CancellationToken token)
        {
            try
            {
                await BeatActionAsync(behaviour, actions, token);
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("BeatAction cancelled.");
            }
        }

        static async UniTask BeatActionAsync(MonoBehaviour behaviour, List<Action> actions, CancellationToken token)
        {
            Conductor conductor = Conductor.instance;
            int idx = 0;
            while (idx < actions.Count)
            {
                await UniTask.WaitUntil(() => (conductor.songPositionInBeatsAsDouble >= actions[idx].beat && !conductor.WaitingForDsp) || !(conductor.isPlaying || conductor.isPaused) || behaviour == null, cancellationToken: token);

                if (behaviour == null || !(conductor.isPlaying || conductor.isPaused))
                    return;

                while (conductor.songPositionInBeatsAsDouble >= actions[idx].beat)
                {
                    try
                    {
                        actions[idx].function.Invoke();
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Exception thrown while executing BeatAction: {e}");
                    }
                    idx++;
                    if (idx >= actions.Count) return;
                }
            }
        }
    }
}