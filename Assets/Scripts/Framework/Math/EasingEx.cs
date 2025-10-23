using System;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace Framework.Math
{
    public enum EasingKind
    {
        Linear,

        InSine, OutSine, InOutSine,
        InQuad, OutQuad, InOutQuad,
        InCubic, OutCubic, InOutCubic,
        InCirc, OutCirc, InOutCirc,
        InBack, OutBack, InOutBack,
        InElastic, OutElastic, InOutElastic,
        InBounce, OutBounce, InOutBounce,
    }

    public static class EasingEx
    {
        /// <summary>t(0..1)を指定のEaseで補正して返す（UI ToolkitのEasingを直呼び）</summary>
        public static float Evaluate(EasingKind ease, float t)
        {
            t = Mathf.Clamp01(t);
            return GetFunc(ease).Invoke(t);
        }

        /// <summary>区間[from..to]に対してイージング補間</summary>
        public static float Evaluate(EasingKind ease, float from, float to, float t)
        {
            return Mathf.LerpUnclamped(from, to, Evaluate(ease, t));
        }

        /// <summary>0..1のtを任意[min..max]にイージングで再マップ</summary>
        public static float Remap01(EasingKind ease, float t, float min, float max)
        {
            return Mathf.LerpUnclamped(min, max, Evaluate(ease, t));
        }

        /// <summary>指定イージングの形を AnimationCurve 化</summary>
        public static AnimationCurve ToAnimationCurve(EasingKind ease, int resolution = 32)
        {
            resolution = Mathf.Max(2, resolution);
            var keys = new Keyframe[resolution];
            var f = GetFunc(ease);
            for (int i = 0; i < resolution; i++)
            {
                float tt = i / (float)(resolution - 1);
                keys[i] = new Keyframe(tt, f(tt));
            }
            var curve = new AnimationCurve(keys);
#if UNITY_EDITOR
            // エディタ上でタングentを自動調整（必須ではない）
            for (int i = 0; i < curve.length; i++)
            {
                UnityEditor.AnimationUtility.SetKeyLeftTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.Auto);
                UnityEditor.AnimationUtility.SetKeyRightTangentMode(curve, i, UnityEditor.AnimationUtility.TangentMode.Auto);
            }
#endif
            return curve;
        }

        // --- 内部：列挙体 → Easing デリゲート解決 ---
        static Func<float, float> GetFunc(EasingKind k)
        {
            switch (k)
            {
                case EasingKind.Linear: return Easing.Linear;

                case EasingKind.InSine: return Easing.InSine;
                case EasingKind.OutSine: return Easing.OutSine;
                case EasingKind.InOutSine: return Easing.InOutSine;

                case EasingKind.InQuad: return Easing.InQuad;
                case EasingKind.OutQuad: return Easing.OutQuad;
                case EasingKind.InOutQuad: return Easing.InOutQuad;

                case EasingKind.InCubic: return Easing.InCubic;
                case EasingKind.OutCubic: return Easing.OutCubic;
                case EasingKind.InOutCubic: return Easing.InOutCubic;

                case EasingKind.InCirc: return Easing.InCirc;
                case EasingKind.OutCirc: return Easing.OutCirc;
                case EasingKind.InOutCirc: return Easing.InOutCirc;

                case EasingKind.InBack: return Easing.InBack;
                case EasingKind.OutBack: return Easing.OutBack;
                case EasingKind.InOutBack: return Easing.InOutBack;

                case EasingKind.InElastic: return Easing.InElastic;
                case EasingKind.OutElastic: return Easing.OutElastic;
                case EasingKind.InOutElastic: return Easing.InOutElastic;

                case EasingKind.InBounce: return Easing.InBounce;
                case EasingKind.OutBounce: return Easing.OutBounce;
                case EasingKind.InOutBounce: return Easing.InOutBounce;

                default: return Easing.Linear;
            }
        }
    }
}
