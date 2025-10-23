using System;
using System.Diagnostics;
using UnityEngine;

[Conditional("UNITY_EDITOR")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class ButtonAttribute : PropertyAttribute
{
    /// 表示ラベル（null/空ならメソッド名）
    public readonly string Label;

    /// ツールチップ（任意）
    public readonly string Tooltip;

    /// 押下前に確認ダイアログを出す（nullで無効）
    public readonly string Confirm;

    /// 有効化条件
    public readonly ButtonEnableMode EnableMode;

    /// 明示的に無効化して表示だけする
    public readonly bool Disabled;

    /// 行間余白（ピクセル）
    public readonly int Spacing;

    /// ボタンの幅（0なら自動レイアウト）
    public readonly float Width;

    /// ボタンの高さ（0ならEditorGUIUtility.singleLineHeight*1.2f）
    public readonly float Height;

    public ButtonAttribute(
        string label = null,
        string tooltip = null,
        string confirm = null,
        ButtonEnableMode enableMode = ButtonEnableMode.Always,
        bool disabled = false,
        int spacing = 2,
        float width = 0f,
        float height = 0f)
    {
        Label = label;
        Tooltip = tooltip;
        Confirm = confirm;
        EnableMode = enableMode;
        Disabled = disabled;
        Spacing = Mathf.Max(0, spacing);
        Width = Mathf.Max(0f, width);
        Height = Mathf.Max(0f, height);
    }
}

public enum ButtonEnableMode
{
    /// 常に押せる
    Always,
    /// エディタ停止中のみ押せる（編集中だけ）
    EditorOnly,
    /// プレイモード中のみ押せる
    PlayModeOnly
}
