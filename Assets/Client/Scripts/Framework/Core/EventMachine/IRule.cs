
namespace Framework.Core.Event
{
    /// <summary>
    /// ルール基底。ルートイベントの起動判定（自動入場）に使用。
    /// 必要に応じてイベントや外部情報を参照できるように引数で受け取る。
    /// </summary>
    public abstract class IRule<TEvent>
        where TEvent : IEvent<TEvent>
    {
        public abstract bool CanExecute(IEvent<TEvent> e, EventMachine<TEvent> machine);
    }
}
