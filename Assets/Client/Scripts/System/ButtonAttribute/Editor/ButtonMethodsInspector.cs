#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// すべてのUnityEngine.Object派生ターゲットで、
/// ButtonAttributeが付いたメソッドをインスペクター末尾に描画する共通Editor。
/// 既存のCustomEditorとも共存可（下の CanBeUsedWithChildren = true）。
/// </summary>
[CanEditMultipleObjects]
[CustomEditor(typeof(UnityEngine.Object))]
public class ButtonMethodsInspector : Editor
{
    // 既存の派生Editor（Rendererや自作Editor）がある場合にも、末尾に追記できるようにする
    public override bool RequiresConstantRepaint() => false;

    public override void OnInspectorGUI()
    {
        Debug.Log("Hello");
        // まず通常のインスペクターを描画
        base.OnInspectorGUI();

        // 対象の型のメソッドを収集（継承含む、非公開も対象、静的も許可）
        var type = target.GetType();
        var methods = type
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .SelectMany(m => m.GetCustomAttributes(typeof(ButtonAttribute), true)
                               .Cast<ButtonAttribute>()
                               .Select(attr => (method: m, attr)))
            // 引数なしのみ
            .Where(x => x.method.GetParameters().Length == 0)
            .ToArray();

        if (methods.Length == 0) return;

        // 区切り
        EditorGUILayout.Space(6);
        DrawHeaderLine();
        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        foreach (var (method, attr) in methods)
        {
            DrawMethodButton(method, attr);
            if (attr.Spacing > 0) GUILayout.Space(attr.Spacing);
        }
    }

    private static void DrawHeaderLine()
    {
        var rect = GUILayoutUtility.GetRect(1f, 1f);
        rect.height = 1f;
        rect.xMin = 0f;
        rect.width = EditorGUIUtility.currentViewWidth;
        EditorGUI.DrawRect(rect, new Color(0, 0, 0, 0.2f));
        GUILayout.Space(4);
    }

    private void DrawMethodButton(MethodInfo method, ButtonAttribute attr)
    {
        // 表示名
        var label = string.IsNullOrEmpty(attr.Label) ? method.Name : attr.Label;

        // 有効/無効判定
        bool play = EditorApplication.isPlaying;
        bool enabledByMode =
            attr.EnableMode == ButtonEnableMode.Always ||
            (attr.EnableMode == ButtonEnableMode.EditorOnly && !play) ||
            (attr.EnableMode == ButtonEnableMode.PlayModeOnly && play);

        bool enabled = enabledByMode && !attr.Disabled;

        // 1行分の高さ
        float height = (attr.Height > 0f) ? attr.Height : EditorGUIUtility.singleLineHeight * 1.2f;

        using (new EditorGUI.DisabledScope(!enabled))
        {
            var content = string.IsNullOrEmpty(attr.Tooltip)
                ? new GUIContent(label)
                : new GUIContent(label, attr.Tooltip);

            bool clicked;
            if (attr.Width > 0f)
            {
                var rect = GUILayoutUtility.GetRect(attr.Width, height, GUILayout.ExpandWidth(false));
                clicked = GUI.Button(rect, content);
            }
            else
            {
                clicked = GUILayout.Button(content, GUILayout.Height(height));
            }

            if (!clicked) return;

            // 確認
            if (!string.IsNullOrEmpty(attr.Confirm))
            {
                if (!EditorUtility.DisplayDialog("確認", attr.Confirm, "OK", "キャンセル"))
                    return;
            }

            InvokeForAllTargets(method);
        }
    }

    private void InvokeForAllTargets(MethodInfo method)
    {
        // マルチオブジェクト対応
        foreach (var obj in targets)
        {
            try
            {
                // Undo（対象がUnityEngine.Objectなら記録）
                if (obj is UnityEngine.Object uo)
                {
                    Undo.RecordObject(uo, $"Invoke {method.Name}");
                }

                object instance = method.IsStatic ? null : obj;
                method.Invoke(instance, null);

                // 変更をDirtyに
                if (obj is UnityEngine.Object uo2)
                {
                    EditorUtility.SetDirty(uo2);
                }
            }
            catch (TargetInvocationException ex)
            {
                Debug.LogError($"[Button] {method.DeclaringType?.Name}.{method.Name} 実行中に例外: {ex.InnerException?.GetType().Name}\n{ex.InnerException}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Button] {method.DeclaringType?.Name}.{method.Name} 実行中に例外: {ex}");
            }
        }
    }
}
#endif
