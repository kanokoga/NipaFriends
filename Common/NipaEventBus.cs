using System;
using System.Collections.Generic;

namespace NipaFriends
{
    public static class NipaEventBus
    {
        // 全ての型のリセット用デリゲートを保持するリスト
        private static readonly List<Action> _clearActions = new List<Action>();

        private static class Messenger<T> where T : struct
        {
            private static Action<T> _onMessageReceived;

            // 初回アクセス時にクリア用のアクションを親クラスに登録する
            static Messenger()
            {
                _clearActions.Add(Dispose);
            }

            public static void AddListener(Action<T> handler) => _onMessageReceived += handler;
            public static void RemoveListener(Action<T> handler) => _onMessageReceived -= handler;
            public static void Broadcast(T message) => _onMessageReceived?.Invoke(message);

            /// <summary>
            /// この型(T)に関する購読をすべて解除する
            /// </summary>
            public static void Dispose()
            {
                _onMessageReceived = null;
            }
        }

        public static void Subscribe<T>(Action<T> handler) where T : struct => Messenger<T>.AddListener(handler);
        public static void Unsubscribe<T>(Action<T> handler) where T : struct => Messenger<T>.RemoveListener(handler);
        public static void Publish<T>(T message) where T : struct => Messenger<T>.Broadcast(message);

        /// <summary>
        /// 特定のメッセージ型のみを明示的に破棄する
        /// </summary>
        public static void Dispose<T>() where T : struct
        {
            Messenger<T>.Dispose();
        }

        /// <summary>
        /// 全てのメッセージ型の購読を一括で解除する（シーン遷移時などに推奨）
        /// </summary>
        public static void DisposeAll()
        {
            foreach(var clearAction in _clearActions)
            {
                clearAction?.Invoke();
            }
            // リスト自体は型ごとのDisposeを呼び出すための「窓口」なのでクリアする必要はないが、
            // 気になる場合は clearAction 実行後に保持し続けても問題ない設計。
        }
    }
}
