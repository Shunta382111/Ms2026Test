#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Framework.Attribute
{
    /// <summary>
    /// PropertyDrawer 側でフィールド右端に「ギズモ表示トグル（小ボタン）」を描画。
    /// Scene インジェクタは、トグルONのプロパティのみハンドルを表示・編集します。
    /// </summary>
    [InitializeOnLoad]
    public static class GizmoSceneInjector
    {
        private const string PrefsPrefix = "Framework.Attribute.Gizmo.Toggled.";
        static GizmoSceneInjector()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sv)
        {
            // 現在表示中のインスペクターで編集対象になり得るのは Selection
            foreach (var go in Selection.gameObjects)
            {
                foreach (var mb in go.GetComponents<MonoBehaviour>())
                {
                    if (mb == null) continue;
                    DrawForComponent(mb);
                }
            }
        }

        private static void DrawForComponent(MonoBehaviour comp)
        {
            var so = new SerializedObject(comp);
            var type = comp.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var f in fields)
            {
                // サポート型のみ
                bool isVec3 = f.FieldType == typeof(Vector3);
                bool isQuat = f.FieldType == typeof(Quaternion);
                if (!isVec3 && !isQuat) continue;

                // 付いているアトリビュートを取得
                var pos = f.GetCustomAttribute<GizmoPositionAttribute>(true);
                var rot = f.GetCustomAttribute<GizmoRotationAttribute>(true);
                var scl = f.GetCustomAttribute<GizmoScaleAttribute>(true);

                BaseGizmoAttribute attr = pos as BaseGizmoAttribute ?? rot as BaseGizmoAttribute ?? scl as BaseGizmoAttribute;
                if (attr == null) continue;

                // ランタイム制限
                if (attr.EditorOnly && EditorApplication.isPlaying) continue;

                var prop = so.FindProperty(f.Name);
                if (prop == null) continue;

                // トグル状態チェック（オフなら描かない）
                string key = MakeToggleKey(comp, prop.propertyPath);
                if (!EditorPrefs.GetBool(key, false)) continue;

                // 基準Transform
                Transform basis = ResolveBasisTransform(comp, attr);

                Handles.color = attr.Color;

                if (pos != null && prop.propertyType == SerializedPropertyType.Vector3)
                    DrawPositionHandle(comp, so, prop, basis, pos);
                else if (rot != null && prop.propertyType == SerializedPropertyType.Quaternion)
                    DrawRotationHandle(comp, so, prop, basis, rot);
                else if (scl != null && prop.propertyType == SerializedPropertyType.Vector3)
                    DrawScaleHandle(comp, so, prop, basis, scl);
            }
        }

        private static Transform ResolveBasisTransform(MonoBehaviour comp, BaseGizmoAttribute attr)
        {
            if (string.IsNullOrEmpty(attr.RelativeTo)) return comp.transform;

            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            var f = comp.GetType().GetField(attr.RelativeTo, flags);
            if (f != null && typeof(Transform).IsAssignableFrom(f.FieldType))
                return (Transform)(f.GetValue(comp) ?? comp.transform);

            var p = comp.GetType().GetProperty(attr.RelativeTo, flags);
            if (p != null && typeof(Transform).IsAssignableFrom(p.PropertyType))
                return (Transform)(p.GetValue(comp, null) ?? comp.transform);

            return comp.transform;
        }

        private static void DrawPositionHandle(MonoBehaviour comp, SerializedObject so, SerializedProperty prop, Transform basis, GizmoPositionAttribute attr)
        {
            Vector3 stored = prop.vector3Value;
            Vector3 world = (attr.Space == GizmoSpace.Local) ? basis.TransformPoint(stored) : stored;

            Vector3 snap = (attr.Snap > 0f) ? Vector3.one * attr.Snap : Vector3.zero;
            EditorGUI.BeginChangeCheck();
            Vector3 newWorld;

            if (attr.UseFreeMove)
            {
                float size = HandleUtility.GetHandleSize(world) * Mathf.Max(0.01f, attr.HandleSize);
                var fmh_109_58_638968381777409348 = Quaternion.identity; newWorld = Handles.FreeMoveHandle(world, size, snap, Handles.SphereHandleCap);
            }
            else
            {
                // PositionHandle は Scene の回転設定に合わせたい時は Tools.pivotRotation を参照する
                Quaternion rot = (Tools.pivotRotation == PivotRotation.Local) ? basis.rotation : Quaternion.identity;
                newWorld = Handles.PositionHandle(world, rot);
                // snap は PositionHandle では自前で適用（必要なら丸め）
                if (attr.Snap > 0f)
                {
                    newWorld.x = Mathf.Round(newWorld.x / attr.Snap) * attr.Snap;
                    newWorld.y = Mathf.Round(newWorld.y / attr.Snap) * attr.Snap;
                    newWorld.z = Mathf.Round(newWorld.z / attr.Snap) * attr.Snap;
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(comp, "Move (GizmoPosition)");
                Vector3 newStored = (attr.Space == GizmoSpace.Local) ? basis.InverseTransformPoint(newWorld) : newWorld;
                prop.vector3Value = newStored;
                so.ApplyModifiedProperties();
                MarkDirty(comp);
            }

            // ラベル
            string label = string.IsNullOrEmpty(attr.Label) ? prop.displayName : attr.Label;
            Handles.Label(world, label, EditorStyles.miniBoldLabel);
        }

        private static void DrawRotationHandle(MonoBehaviour comp, SerializedObject so, SerializedProperty prop, Transform basis, GizmoRotationAttribute attr)
        {
            Quaternion stored = prop.quaternionValue;
            Quaternion worldQ = (attr.Space == GizmoSpace.Local) ? basis.rotation * stored : stored;

            Vector3 pivot = basis.position + attr.PivotOffset;

            EditorGUI.BeginChangeCheck();
            Quaternion newWorldQ = Handles.RotationHandle(worldQ, pivot);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(comp, "Rotate (GizmoRotation)");
                Quaternion newStored = (attr.Space == GizmoSpace.Local) ? Quaternion.Inverse(basis.rotation) * newWorldQ : newWorldQ;
                prop.quaternionValue = newStored;
                so.ApplyModifiedProperties();
                MarkDirty(comp);
            }

            string label = string.IsNullOrEmpty(attr.Label) ? prop.displayName : attr.Label;
            Handles.Label(pivot + Vector3.up * 0.2f, label, EditorStyles.miniBoldLabel);
        }

        private static void DrawScaleHandle(MonoBehaviour comp, SerializedObject so, SerializedProperty prop, Transform basis, GizmoScaleAttribute attr)
        {
            Vector3 stored = prop.vector3Value;
            Vector3 world = (attr.Space == GizmoSpace.Local) ? Vector3.Scale(basis.lossyScale, stored) : stored;

            EditorGUI.BeginChangeCheck();
            Vector3 newWorld = Handles.ScaleHandle(world, basis.position, basis.rotation, Mathf.Max(0.01f, attr.HandleSize));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(comp, "Scale (GizmoScale)");
                Vector3 clamped = new Vector3(
                    Mathf.Max(attr.MinPerAxis, newWorld.x),
                    Mathf.Max(attr.MinPerAxis, newWorld.y),
                    Mathf.Max(attr.MinPerAxis, newWorld.z)
                );

                Vector3 newStored = (attr.Space == GizmoSpace.Local)
                    ? new Vector3(
                        SafeDiv(clamped.x, basis.lossyScale.x),
                        SafeDiv(clamped.y, basis.lossyScale.y),
                        SafeDiv(clamped.z, basis.lossyScale.z))
                    : clamped;

                prop.vector3Value = newStored;
                so.ApplyModifiedProperties();
                MarkDirty(comp);
            }

            string label = string.IsNullOrEmpty(attr.Label) ? prop.displayName : attr.Label;
            Handles.Label(basis.position + Vector3.up * 0.2f, label, EditorStyles.miniBoldLabel);
        }

        private static float SafeDiv(float a, float b) => Mathf.Approximately(b, 0f) ? 0f : a / b;

        private static void MarkDirty(UnityEngine.Object o)
        {
            EditorUtility.SetDirty(o);
            if (!Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(((o as Component)?.gameObject.scene).GetValueOrDefault());
        }

        internal static string MakeToggleKey(UnityEngine.Object target, string propertyPath)
        {
            // GlobalObjectId で永続的キー化（Prefab/Scene問わず）
            var gid = GlobalObjectId.GetGlobalObjectIdSlow(target);
            if (gid.identifierType != 0) // null GlobalObjectId 判定（identifierType==0が無効）
            {
                return $"{PrefsPrefix}{gid.ToString()}::{propertyPath}";
            }
            // 取れない場合は InstanceID ベース（非永続）
            return $"{PrefsPrefix}IID{target.GetInstanceID()}::{propertyPath}";
        }
    }

    /// <summary>
    /// 右端の小ボタン（トグル）を描画するための共通基盤。
    /// ON の間だけ Scene にギズモを表示します。
    /// </summary>
    public static class GizmoToggleDrawerUtil
    {
        private static readonly GUIContent _iconOn = EditorGUIUtility.IconContent("d_SceneViewTools");   // 適当なアイコン
        private static readonly GUIContent _iconOff = EditorGUIUtility.IconContent("d_ToolHandleCenter");

        public static bool DrawRightMiniToggle(Rect totalRect, UnityEngine.Object target, SerializedProperty prop)
        {
            const float w = 22f;
            var btnRect = new Rect(totalRect.xMax - w, totalRect.y, w, EditorGUIUtility.singleLineHeight);

            string key = GizmoSceneInjector.MakeToggleKey(target, prop.propertyPath);
            bool current = EditorPrefs.GetBool(key, false);

            var content = current ? _iconOn : _iconOff;
            content.tooltip = current ? "ギズモを非表示" : "ギズモを表示";

            bool pressed = GUI.Button(btnRect, content, EditorStyles.miniButtonRight);
            if (pressed)
            {
                current = !current;
                EditorPrefs.SetBool(key, current);
                SceneView.RepaintAll();
            }

            // フィールド本体用の矩形（右側のボタン幅を差し引く）
            return current;
        }

        public static Rect GetFieldRect(Rect totalRect)
        {
            const float w = 22f;
            return new Rect(totalRect.x, totalRect.y, totalRect.width - w - 2f, totalRect.height);
        }
    }

    // ---------- PropertyDrawer: Position ----------
    [CustomPropertyDrawer(typeof(GizmoPositionAttribute))]
    public class GizmoPositionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 右端トグル（押すとギズモ表示ON/OFF）
            bool isOn = GizmoToggleDrawerUtil.DrawRightMiniToggle(position, property.serializedObject.targetObject, property);

            // 本体フィールド
            var fieldRect = GizmoToggleDrawerUtil.GetFieldRect(position);
            EditorGUI.PropertyField(fieldRect, property, label, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);
    }

    // ---------- PropertyDrawer: Rotation ----------
    [CustomPropertyDrawer(typeof(GizmoRotationAttribute))]
    public class GizmoRotationDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool isOn = GizmoToggleDrawerUtil.DrawRightMiniToggle(position, property.serializedObject.targetObject, property);

            // Quaternion のデフォルト描画
            var fieldRect = GizmoToggleDrawerUtil.GetFieldRect(position);
            EditorGUI.PropertyField(fieldRect, property, label, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);
    }

    // ---------- PropertyDrawer: Scale ----------
    [CustomPropertyDrawer(typeof(GizmoScaleAttribute))]
    public class GizmoScaleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            bool isOn = GizmoToggleDrawerUtil.DrawRightMiniToggle(position, property.serializedObject.targetObject, property);

            var fieldRect = GizmoToggleDrawerUtil.GetFieldRect(position);
            EditorGUI.PropertyField(fieldRect, property, label, true);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);
    }
}
#endif
