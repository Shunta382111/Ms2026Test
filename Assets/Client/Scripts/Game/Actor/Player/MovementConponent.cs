using Framework.Math;
using UnityEngine;

public class MovementConponent : MonoBehaviour
{
    private enum MovementState
    {
        /// <summary>
        /// 待機中
        /// </summary>
        Idle,

        /// <summary>
        /// 移動開始中
        /// </summary>
        Starting,

        /// <summary>
        /// 移動中
        /// </summary>
        Running,

        /// <summary>
        /// 移動停止中
        /// </summary>
        Stoping,
    }

    /// <summary>
    /// 現在の移動ベクトル
    /// </summary>
    private Vector3 _currentVelocityNormalize = Vector3.zero;

    /// <summary>
    /// 現在のスピード
    /// </summary>
    private float _currentSpeed = 0f;

    /// <summary>
    /// スピードの値を変動する際のステート
    /// </summary>
    private MovementState _movementState = MovementState.Idle;

    /// <summary>
    /// スピードの値を変動させる際の割合
    /// </summary>
    private float _movementTransitionRatio = 0f;

    /// <summary>
    /// 初速度の値の変動を表すイージング
    /// </summary>
    private EasingKind _initialVelocityKind = EasingKind.Linear;

    /// <summary>
    /// 初速度の最小値
    /// </summary>
    private float _initialVelocityMin = 0f;

    /// <summary>
    /// 初速度の最大値
    /// </summary>
    private float _initialVelocityMax = 10f;

    /// <summary>
    /// 初速度の反映させる時間
    /// </summary>
    private float _initialVelocityTime = 0.5f;

    /// <summary>
    /// 最後に計算した初速度
    /// </summary>
    private float _lastInitialVelocity = 0.0f;

    /// <summary>
    /// 終端速度の値の変動を表すイージング
    /// </summary>
    private EasingKind _terminalVelocityKind = EasingKind.Linear;

    /// <summary>
    /// 終端速度の最小値
    /// </summary>
    private float _terminalVelocityMin = 0f;

    /// <summary>
    /// 終端速度の最大値
    /// </summary>
    private float _terminalVelocityMax = 10f;

    /// <summary>
    /// 終端速度の反映させる時間
    /// </summary>
    private float _terminalVelocityTime = 0.5f;

    /// <summary>
    /// 最後に計算した終端速度
    /// </summary>
    private float _lastTerminalVelocity = 0.0f;

    /// <summary>
    /// 移動中に出してほしいスピード
    /// </summary>
    private float _runningSpeed = 0f;

    /// <summary>
    /// 初速→移動中　に遷移させるまでの時間
    /// </summary>
    private float _runningTransitionTime = 0.5f;

    /// <summary>
    /// 終端→待機中　に遷移させるまでの時間
    /// </summary>
    private float _idleTransitionTime = 0.5f;


    /// <summary>
    /// 初速度の設定
    /// </summary>
    /// <param name="initialVelocityMin">初速度の最小値（開始値）</param>
    /// <param name="initialVelocityMax">初速度の最大値（終了値）</param>
    /// <param name="time">初速度の反映させるまでの時間</param>
    /// <param name="kind">初速度の値の変動を表すイージング</param>
    public void SetInitialVelocity(float initialVelocityMin, float initialVelocityMax, float time, EasingKind kind = EasingKind.Linear)
    {
        _initialVelocityMin = initialVelocityMin;
        _initialVelocityMax = initialVelocityMax;
        _initialVelocityTime = time;
        _initialVelocityKind = kind;
    }

    /// <summary>
    /// 終端速度の設定
    /// </summary>
    /// <param name="terminalVelocityMax">終端速度の最大値（開始値）</param>
    /// <param name="terminalVelocityMin">終端速度の最小値（終了値）</param>
    /// <param name="time">終端速度の反映させるまでの時間</param>
    /// <param name="kind">終端速度の値の変動を表すイージング</param>
    public void SetTerminalVelocity(float terminalVelocityMax, float terminalVelocityMin, float time, EasingKind kind = EasingKind.Linear)
    {
        _terminalVelocityMax = terminalVelocityMax;
        _terminalVelocityMin = terminalVelocityMin;
        _terminalVelocityTime = time;
        _terminalVelocityKind = kind;
    }

    public bool IsStartingOrRunning() => _movementState == MovementState.Starting || _movementState == MovementState.Running;
    public bool IsStopingOrIdle() => _movementState == MovementState.Stoping || _movementState == MovementState.Idle;

    /// <summary>
    /// 移動
    /// </summary>
    public void Move(float speed, Vector3 velocityNormalize)
    {
        _runningSpeed = speed;
        _currentVelocityNormalize = velocityNormalize;
        if (!IsStopingOrIdle()) return; // 停止中でないなら遷移処理を実行しない

        _SetMovementState(MovementState.Starting);
    }

    /// <summary>
    /// 移動
    /// </summary>
    public void Move(float speed, float initialVelocity, Vector3 velocityNormalize)
    {
        _initialVelocityMax = initialVelocity;
        _runningSpeed = speed;
        _currentVelocityNormalize = velocityNormalize;
        if (IsStartingOrRunning()) return; // 移動中なら遷移処理を実行しない

        _SetMovementState(MovementState.Starting);
    }

    /// <summary>
    /// [強制] 移動開始状態にする
    /// </summary>
    public void ForceStartingMove(float initialVelocity, Vector3 velocityNormalize)
    {
        _initialVelocityMax = initialVelocity;
        _currentVelocityNormalize = velocityNormalize;
        if (!IsStopingOrIdle()) return; // 停止中でないなら遷移処理を実行しない

        _SetMovementState(MovementState.Starting);
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop(float time, Vector3 velocity)
    {
        _terminalVelocityTime = time;
        _currentVelocityNormalize = velocity;
        if (IsStopingOrIdle()) return; // 停止中なら遷移処理を実行しない

        _SetMovementState(MovementState.Stoping);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }


    // Update is called once per frame
    void Update()
    {
        float deltaTime = Time.deltaTime;

        switch (_movementState)
        {
            case MovementState.Idle: // 待機中
                _UpdateIdle(deltaTime);
                break;
            case MovementState.Starting: // 移動開始中
                _UpdateInitial(deltaTime);
                break;
            case MovementState.Running: // 移動中
                _UpdateRunning(deltaTime);
                break;
            case MovementState.Stoping: // 移動停止中
                _UpdateTerminal(deltaTime);
                break;
        }

        Vector3 move = Vector3.zero;
        if (_currentVelocityNormalize.sqrMagnitude != 0f)
        {
            move = _currentVelocityNormalize.normalized * _currentSpeed;
            transform.position += move;
        }
    }


    private void _UpdateInitial(float deltaTime)
    {
        // 遷移（0 ~ 1）の割合値を計算
        _movementTransitionRatio += _initialVelocityTime * deltaTime;
        _movementTransitionRatio = Mathf.Clamp01(_movementTransitionRatio);

        // 現在のスピードを計算
        _lastInitialVelocity = EasingEx.Evaluate(_initialVelocityKind, _initialVelocityMin, _initialVelocityMax, _movementTransitionRatio);
        _currentSpeed = _lastInitialVelocity;

        // 遷移が完了したなら、移動中
        if (1f <= _currentSpeed)
        {
            _SetMovementState(MovementState.Running);
        }
    }

    private void _UpdateTerminal(float deltaTime)
    {
        // 遷移（0 ~ 1）の割合値を計算
        _movementTransitionRatio += _terminalVelocityTime * deltaTime;
        _movementTransitionRatio = Mathf.Clamp01(_movementTransitionRatio);

        // 現在のスピードを計算
        _lastTerminalVelocity = EasingEx.Evaluate(_terminalVelocityKind, _terminalVelocityMax, _terminalVelocityMin, _movementTransitionRatio);
        _currentSpeed = _lastTerminalVelocity;

        // 遷移が完了したなら、待機中
        if (0f >= _currentSpeed)
        {
            _SetMovementState(MovementState.Idle);
        }
    }

    private void _UpdateRunning(float deltaTime)
    {
        if (_movementTransitionRatio >= 1f) return;

        // 遷移（0 ~ 1）の割合値を計算
        _movementTransitionRatio += _runningTransitionTime * deltaTime;
        _movementTransitionRatio = Mathf.Clamp01(_movementTransitionRatio);

        // 現在のスピードを計算
        _currentSpeed = EasingEx.Evaluate(EasingKind.Linear, _lastInitialVelocity, _runningSpeed, _movementTransitionRatio);
    }

    private void _UpdateIdle(float deltaTime)
    {
        if (_movementTransitionRatio >= 1f) return;

        // 遷移（0 ~ 1）の割合値を計算
        _movementTransitionRatio += _idleTransitionTime * deltaTime;
        _movementTransitionRatio = Mathf.Clamp01(_movementTransitionRatio);

        // 現在のスピードを計算
        _currentSpeed = EasingEx.Evaluate(EasingKind.Linear, _lastTerminalVelocity, 0f, _movementTransitionRatio);
    }

    private void _SetMovementState(MovementState state)
    {
        _movementState = state;
        _movementTransitionRatio = 0f; // リセット
    }
}
