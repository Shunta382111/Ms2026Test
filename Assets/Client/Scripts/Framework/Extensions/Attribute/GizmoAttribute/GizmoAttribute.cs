using System;
using UnityEngine;

namespace Framework.Attribute
{
    public enum GizmoSpace
    {
        World,
        Local
    }

    /// <summary>ギズモ系アトリビュートの共通基底</summary>
    public abstract class BaseGizmoAttribute : PropertyAttribute
    {
        /// <summary>Scene 上に表示するラベル（null/空ならプロパティ名）</summary>
        public string Label { get; }

        /// <summary>ローカル or ワールド空間</summary>
        public GizmoSpace Space { get; set; } = GizmoSpace.Local;

        /// <summary>相対基準 Transform のメンバ名（フィールド/プロパティ）。未指定なら target.transform</summary>
        public string RelativeTo { get; set; } = null;

        /// <summary>色</summary>
        public Color Color { get; set; } = new Color(0.2f, 0.8f, 1f, 1f);

        /// <summary>エディタ停止中のみ編集可</summary>
        public bool EditorOnly { get; set; } = false;

        protected BaseGizmoAttribute(string label = null)
        {
            Label = label;
        }
    }

    /// <summary>Vector3 を移動ハンドルで編集</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class GizmoPositionAttribute : BaseGizmoAttribute
    {
        /// <summary>グリッドスナップ（0以下で無効）</summary>
        public float Snap { get; set; } = 0f;

        /// <summary>FreeMove を使う（falseなら PositionHandle）</summary>
        public bool UseFreeMove { get; set; } = false;

        /// <summary>FreeMove の見た目サイズ係数</summary>
        public float HandleSize { get; set; } = 0.15f;

        public GizmoPositionAttribute(string label = null) : base(label) { }
    }

    /// <summary>Quaternion を回転ハンドルで編集</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class GizmoRotationAttribute : BaseGizmoAttribute
    {
        /// <summary>回転ハンドルのピボットを basis.position + PivotOffset にする</summary>
        public Vector3 PivotOffset { get; set; } = Vector3.zero;

        public GizmoRotationAttribute(string label = null) : base(label) { }
    }

    /// <summary>Vector3 をスケールハンドルで編集</summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class GizmoScaleAttribute : BaseGizmoAttribute
    {
        /// <summary>ScaleHandle の見た目サイズ</summary>
        public float HandleSize { get; set; } = 1.0f;

        /// <summary>各軸の最小値クランプ（負スケール等を防ぎたい時）</summary>
        public float MinPerAxis { get; set; } = -Mathf.Infinity;

        public GizmoScaleAttribute(string label = null) : base(label) { }
    }
}
