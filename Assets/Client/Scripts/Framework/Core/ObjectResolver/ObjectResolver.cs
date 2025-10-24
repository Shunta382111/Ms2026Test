using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Rendering.VirtualTexturing;

namespace Framework.Core.ObjectResolver
{
    public interface IInjectResolver
    {
        /// <summary>
        /// Resolve() でインスタンスを作成した際、親インスタンスの ObjectResolver を引き継いだ 子ObjectResolver を渡します
        /// </summary>
        /// <param name="resolver"></param>
        public void InjectResolver(ObjectResolver resolver);

        /// <summary>
        /// 親インスタンスが破棄される際に、注入を解除します
        /// </summary>
        /// <param name="resolver"></param>
        public void UninjectResolver(ObjectResolver resolver);
    }

    public class ObjectResolver
    {
        private class Object
        {
            /// <summary>
            /// インスタンス
            /// </summary>
            public object Instance = null;

            /// <summary>
            /// インスタンス内の Resolver
            /// </summary>
            public ObjectResolver Resolver = null;
        }

        private readonly Dictionary<Type, Object> _instances = new();
        private readonly HashSet<Type> _registerations = new();

        private readonly ObjectResolver _parent = null;


        public ObjectResolver() { }
        private ObjectResolver(ObjectResolver parent)
        {
            _parent = parent;
        }

        public void Register<T>()
        {
            Type type = typeof(T);
            _registerations.Add(type);
        }

        public void RegisterInstance<T>(T instance)
        {
            Type type = typeof(T);

            _instances[type] = new Object { Instance = instance, Resolver = null };
            _registerations.Add(type);
        }

        public void UnregisterInstance<T>()
        {
            Type type = typeof(T);

            // 子インスタンス内の Resolver を全て Uninject する
            if (_instances.TryGetValue(type, out var obj))
            {
                _Remove(obj);
            }

            _instances.Remove(type);
            _registerations.Remove(type);
        }

        public void Clear()
        {
            // 子インスタンス内の Resolver を全て Uninject する
            foreach (var obj in _instances.Values)
            {
                _Remove(obj);
            }

            _instances.Clear();
            _registerations.Clear();
        }

        public T Resolve<T>()
        {
            return (T)_Resolve(typeof(T));
        }



        private object _Resolve(Type instanceType)
        {
            // ローカルにキャッシュされたインスタンスを返す
            if (_instances.TryGetValue(instanceType, out var obj))
            {
                return obj.Instance;
            }

            // ローカルにインスタンス作成登録がされていれば、インスタンス作成
            if (_registerations.Contains(instanceType))
            {
                // インスタンスを作成
                var constructor = FindConstructor(instanceType);
                var args = constructor.GetParameters().Select(p => _Resolve(p.ParameterType)).ToArray(); // コンストラクタ内の引数の型を Resolver から取得。インスタンスが無ければ作成（再帰関数）
                var instance = Activator.CreateInstance(instanceType, args);

                // 自身が管理する子インスタンスに 専用のObjectResolver を渡す
                ObjectResolver childResolver = null;
                if (instance is IInjectResolver injectResolver)
                {
                    // 子インスタンス用の Resolver を作成
                    childResolver = new ObjectResolver(this);

                    // Resolver を渡す
                    injectResolver.InjectResolver(childResolver);
                }

                _instances[instanceType] = new Object { Instance = instance, Resolver = childResolver };
                return instance;
            }


            // ローカルに未登録なら、親に委譲
            if (_parent != null)
            {
                return _parent._Resolve(instanceType);
            }



            // 該当なし → エラー
            DebugEx.LogError($"ObjectResovler に指定した型が登録されていません: {instanceType.Name}");
            return default;
        }

        private static ConstructorInfo FindConstructor(Type t)
        {
            var ctors = t.GetConstructors();
            if (ctors.Length == 0)
                throw new InvalidOperationException($"{t.FullName} に public コンストラクタがありません。");
            // シンプルに引数が少ない順など。実戦では [Inject] 属性優先などにする
            return ctors.OrderBy(c => c.GetParameters().Length).First();
        }

        /// <summary>
        /// Resolver を全て Uninject する
        /// </summary>
        /// <param name="obj"></param>
        private void _Remove(Object obj)
        {
            if (obj.Resolver != null)
            {
                if (obj.Instance is IInjectResolver injectResolver) injectResolver.UninjectResolver(obj.Resolver);
                obj.Resolver.Clear();
            }

            if (obj.Instance is IDisposable d)
            {
                d.Dispose();
            }
        }
    }
}
