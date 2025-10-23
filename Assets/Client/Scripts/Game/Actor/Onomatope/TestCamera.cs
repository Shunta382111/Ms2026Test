using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("追尾対象")]
    public Transform target;           // プレイヤー参照

    [Header("カメラ設定")]
    public float distance = 5f;        // プレイヤーとの距離
    public float heightOffset = 1.5f;  // 注視点の高さ
    public float rotationSpeed = 100f; // 矢印キー回転速度（度/秒）
    public float pitchLimitUp = 70f;   // 上方向の限界角度
    public float pitchLimitDown = -40f;// 下方向の限界角度

    private float yaw = 0f;   // 左右回転角度
    private float pitch = 15f;// 上下回転角度（初期少し下向き）

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("FollowCamera: Target が未設定です。");
        }

        // カメラの初期角度を現在値で初期化
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 矢印キーで回転制御
        if (Input.GetKey(KeyCode.LeftArrow)) yaw -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) yaw += rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow)) pitch -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) pitch += rotationSpeed * Time.deltaTime;

        // 上下回転角度を制限
        pitch = Mathf.Clamp(pitch, pitchLimitDown, pitchLimitUp);

        // 回転を適用
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // プレイヤー位置から距離分後ろに配置
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        transform.position = target.position + offset;

        // プレイヤーを注視（少し上の位置を見る）
        Vector3 lookTarget = target.position + Vector3.up * heightOffset;
        transform.LookAt(lookTarget);
    }
}
