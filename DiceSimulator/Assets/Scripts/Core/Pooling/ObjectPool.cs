using System;

namespace Core.Pooling
{
    public class ObjectPool<T> : AbstractMemoryPool<T> where T : class, new()
    {
        protected Action<T>? OnGetMethod { set => _onGetMethod = value; }
        Action<T>? _onGetMethod;

        protected Func<T>? CustomAlloc { set => _customAlloc = value; }
        Func<T>? _customAlloc;

        public ObjectPool(Func<T>? customAlloc = null,
            Action<T>? onGetMethod = null,
            Action<T, bool>? onReturnedMethod = null,
            int maxSize = int.MaxValue) : base(onReturnedMethod, maxSize)
        {
            _onGetMethod = onGetMethod;
            _customAlloc = customAlloc;
        }

        public T Get()
        {
            lock (_locker)
            {
                T item = GetInternal();
                _onGetMethod?.Invoke(item);

                return item;
            }
        }

        protected override T Alloc() => _customAlloc == null ? new T() : _customAlloc();
    }
}