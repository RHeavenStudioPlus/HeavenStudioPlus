using UnityEngine;
using System;

namespace HeavenStudio.Util
{
    public static class AnimationHelpers
    {
        public static bool IsAnimationNotPlaying(this Animator anim)
        {
            if (anim == null) return true;
            var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            return (stateInfo.normalizedTime >= stateInfo.speed || stateInfo.loop) && !anim.IsInTransition(0);
        }

        public static bool IsAnimationNotPlaying(this Animator anim, int layer)
        {
            if (anim == null) return true;
            var stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
            return (stateInfo.normalizedTime >= stateInfo.speed || stateInfo.loop) && !anim.IsInTransition(layer);
        }

        /// <summary>
        /// Returns true if animName is currently playing on animator
        /// </summary>
        /// <param name="anim">Animator to check</param>
        /// <param name="animNames">name(s) of animation to look out for</param>
        public static bool IsPlayingAnimationNames(this Animator anim, params string[] animNames)
        {
            if (anim == null) return false;
            var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            var isPlaying = Array.Exists(animNames, animName => stateInfo.IsName(animName));
            return (stateInfo.normalizedTime < stateInfo.speed || stateInfo.loop) && isPlaying;
        }

        /// <summary>
        /// Returns true if animName is currently playing on animator on a specific layer
        /// </summary>
        /// <param name="anim">Animator to check</param>
        /// <param name="animNames">name(s) of animation to look out for</param>
        public static bool IsPlayingAnimationNames(this Animator anim, int layer, params string[] animNames)
        {
            if (anim == null) return false;
            var stateInfo = anim.GetCurrentAnimatorStateInfo(layer);
            var isPlaying = Array.Exists(animNames, animName => stateInfo.IsName(animName));
            return (stateInfo.normalizedTime < stateInfo.speed || stateInfo.loop) && isPlaying;
        }

        /// <summary>
        /// Sets animator's progress on an animation based on current song beat between startTime and length
        /// function must be called in actor's Update loop to update properly
        /// </summary>
        /// <param name="anim">Animator to update</param>
        /// <param name="animName">name of animation to play</param>
        /// <param name="startTime">reference start time of animation (progress 0.0)</param>
        /// <param name="length">duration of animation (progress 1.0)</param>
        /// <param name="timeScale">multiplier for animation progress (smaller values make animation slower)</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void DoScaledAnimation(this Animator anim, string animName, double startTime, double length = 1, float timeScale = 1f, int animLayer = -1, bool clamp = false, bool ignoreSwing = true)
        {
            if (anim == null) return;
            float pos = Conductor.instance.GetPositionFromBeat(startTime, length, ignoreSwing: ignoreSwing) * timeScale;
            if (clamp) pos = Mathf.Clamp01(pos);
            anim.Play(animName, animLayer, pos);
            anim.speed = 1f; //not 0 so these can still play their script events
        }

        /// <summary>
        /// Sets animator's progress on a state based on current song beat between startTime and length
        /// function must be called in actor's Update loop to update properly
        /// also sets the speed of the state 1x speed
        /// Animator must have a float parameter named "{state}_Speed" for this to work, and have the state's speed multiplier mapped to that parameter in the state tree
        /// </summary>
        /// <param name="anim">Animator to update</param>
        /// <param name="state">name of animation to play</param>
        /// <param name="startTime">reference start time of animation (progress 0.0)</param>
        /// <param name="length">duration of animation (progress 1.0)</param>
        /// <param name="timeScale">multiplier for animation progress (smaller values make animation slower)</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void DoScaledState(this Animator anim, string state, double startTime, double length = 1, float timeScale = 1f, int animLayer = -1, bool clamp = false, bool ignoreSwing = true)
        {
            if (anim == null) return;
            float pos = Conductor.instance.GetPositionFromBeat(startTime, length, ignoreSwing: ignoreSwing) * timeScale;
            if (clamp) pos = Mathf.Clamp01(pos);
            anim.Play(state, animLayer, pos);
            anim.SetFloat($"{state}_Speed", 1);
        }

        /// <summary>
        /// Sets animator progress on an animation according to pos
        /// </summary>
        /// <param name="anim">Animator to update</param>
        /// <param name="animName">name of animation to play</param>
        /// <param name="pos">position to set animation progress to (0.0 - 1.0)</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void DoNormalizedAnimation(this Animator anim, string animName, float pos = 0f, int animLayer = -1)
        {
            if (anim == null) return;
            anim.Play(animName, animLayer, pos);
            anim.speed = 1f;
        }

        /// <summary>
        /// Plays animation on animator, scaling speed to song BPM 
        /// call this function once, when playing an animation
        /// </summary>
        /// <param name="anim">Animator to play animation on</param>
        /// <param name="animName">name of animation to play</param>
        /// <param name="timeScale">multiplier for animation speed</param>
        /// <param name="startBeat">beat that this animation would start on</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void DoScaledAnimationFromBeatAsync(this Animator anim, string animName, float timeScale = 1f, double startBeat = 0, int animLayer = -1)
        {
            if (anim == null) return;
            float pos = 0;
            if (!double.IsNaN(startBeat)) {
                var cond = Conductor.instance;
                var animClip = Array.Find(anim.runtimeAnimatorController.animationClips, x => x.name == animName);
                double animLength = cond.SecsToBeats(animClip.length, cond.GetBpmAtBeat(startBeat));
                pos = cond.GetPositionFromBeat(startBeat, animLength) * timeScale;
            } else {
                Debug.LogWarning("DoScaledAnimationFromBeatAsync()'s startBeat was NaN; using DoScaledAnimationAsync() instead.");
            }
            anim.DoScaledAnimationAsync(animName, timeScale, pos, animLayer);
        }

        /// <summary>
        /// Sets the speed of an animation state on an animator, scaled to the BPM, then plays it
        /// Animator must have a float parameter named "{state}Speed" for this to work, and have the state's speed multiplier mapped to that parameter in the state tree
        /// </summary>
        /// <param name="anim">Animator to play animation on</param>
        /// <param name="state">name of animation to play</param>
        /// <param name="timeScale">multiplier for animation speed</param>
        /// <param name="startBeat">beat that this animation would start on</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void DoScaledStateFromBeatAsync(this Animator anim, string animName, float timeScale = 1f, double startBeat = 0, int animLayer = -1)
        {
            if (anim == null) return;
            float pos = 0;
            if (!double.IsNaN(startBeat)) {
                var cond = Conductor.instance;
                var animClip = Array.Find(anim.runtimeAnimatorController.animationClips, x => x.name == animName);
                if (animClip == null) {
                    Debug.LogError("Animation clip " + animName + " not found!");
                    return;
                }
                double animLength = cond.SecsToBeats(animClip.length, cond.GetBpmAtBeat(startBeat));
                pos = cond.GetPositionFromBeat(startBeat, animLength) * timeScale;
            } else {
                Debug.LogWarning("DoScaledAnimationFromBeatAsync()'s startBeat was NaN; using DoScaledAnimationAsync() instead.");
            }
            anim.DoScaledStateAsync(animName, timeScale, pos, animLayer);
        }



        /// <summary>
        /// Plays animation on animator, scaling speed to song BPM
        /// call this function once, when playing an animation
        /// </summary>
        /// <param name="anim">Animator to play animation on</param>
        /// <param name="animName">name of animation to play</param>
        /// <param name="timeScale">multiplier for animation speed</param>
        /// <param name="startPos">starting progress of animation</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void DoScaledAnimationAsync(this Animator anim, string animName, float timeScale = 1f, float startPos = 0f, int animLayer = -1)
        {
            if (anim == null) return;
            anim.Play(animName, animLayer, startPos);
            anim.speed = (1f / Conductor.instance.pitchedSecPerBeat) * timeScale;
        }

        /// <summary>
        /// Sets the speed of an animation state on an animator, scaled to the BPM, then plays it
        /// Animator must have a float parameter named "{state}Speed" for this to work, and have the state's speed multiplier mapped to that parameter in the state tree
        /// </summary>
        /// <param name="anim">Animator to play animation on</param>
        /// <param name="state">name of animation to play</param>
        /// <param name="timeScale">multiplier for animation speed</param>
        /// <param name="startPos">starting progress of animation</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void DoScaledStateAsync(this Animator anim, string state, float timeScale = 1f, float startPos = 0f, int animLayer = -1)
        {
            if (anim == null) return;
            anim.PlayStateAtSpeed(state, (1f / Conductor.instance.pitchedSecPerBeat) * timeScale, startPos, animLayer);
        }

        public static void SetScaledAnimationSpeed(this Animator anim, float timeScale = 0.5f)
        {
            if (anim == null) return;
            anim.speed = (1f / Conductor.instance.pitchedSecPerBeat) * timeScale;
        }

        /// <summary>
        /// Plays animation on animator, at default speed
        /// this is the least nessecary function here lol
        /// </summary>
        /// <param name="anim">Animator to play animation on</param>
        /// <param name="animName">name of animation to play</param>
        /// <param name="startPos">starting progress of animation</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void DoUnscaledAnimation(this Animator anim, string animName, float startPos = 0f, int animLayer = -1)
        {
            if (anim == null) return;
            anim.Play(animName, animLayer, startPos);
            anim.speed = 1f;
        }

        /// <summary>
        /// Sets the speed of an animation state on an animator, then plays it
        /// Animator must have a float parameter named "{state}_Speed" for this to work, and have the state's speed multiplier mapped to that parameter in the state tree
        /// </summary>
        /// <param name="anim">Animator to play animation on</param>
        /// <param name="state">name of animation to play</param>
        /// <param name="timeScale">multiplier for animation speed</param>
        /// <param name="startPos">starting progress of animation</param>
        /// <param name="animLayer">animator layer to play animation on</param>
        public static void PlayStateAtSpeed(this Animator anim, string state, float timeScale = 1f, float startPos = 0f, int animLayer = -1)
        {
            if (anim == null) return;
            anim.SetFloat($"{state}_Speed", timeScale);
            anim.Play(state, animLayer, startPos);
        }
    }
}