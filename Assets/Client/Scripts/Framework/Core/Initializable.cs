namespace Framework
{
    public interface IInitializable
    {
        public bool IsInitialized { get; }

        /// <summary>
        /// 初期化関数
        /// </summary>
        /// <remarks>
        /// ※ 初期化関数内で、IsInitialized の中身を true に変更してください<br/>
        /// 　 初期化が成功しているか、IsInitialized で判定します
        /// </remarks>
        public void Initialise();



        /// <summary>
        /// 初期化したことを保証します
        /// </summary>
        /// <remarks>
        /// 未初期化の場合、アサートを出します
        /// </remarks>
        public void EnsureInitialized()
        {
            if (IsInitialized) return;
            DebugEx.Assertion($"未初期化です: {GetType().Name}");
        }
    }
}
