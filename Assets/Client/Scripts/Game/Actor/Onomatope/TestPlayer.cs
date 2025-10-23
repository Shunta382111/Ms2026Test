using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class TestPlayer : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 50f;        // 移動速度
    public Transform cameraTransform;   // カメラ参照
    public float rotateSpeed = 15f;     // 向き補間速度（小さくしても慣性はつかない）

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 入力取得
        float h = Input.GetAxisRaw("Horizontal"); // Rawを使うと滑らか補間なし（即値）
        float v = Input.GetAxisRaw("Vertical");

        // 入力ベクトル
        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        // カメラの正面方向を基準に変換
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 実際の移動方向
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // 移動
        if (inputDir.magnitude > 0.1f)
        {
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // プレイヤーの向き変更（瞬時でもいいならSlerpをLerpに変更可能）
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
        else
        {
            // 入力がないときに即停止（慣性を完全に打ち消す）
            controller.Move(Vector3.zero);
        }
    }
}
