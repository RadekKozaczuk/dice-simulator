using System;

namespace Core.Pooling
{
    public class ObjectPool<T> : AbstractMemoryPool<T> where T : class, new()
    {
        readonly Action<T> _onGetMethod;
        readonly Func<T> _customAlloc;

        public ObjectPool(Func<T> customAlloc = null,
            Action<T> onGetMethod = null,
            Action<T, bool> onReturnedMethod = null,
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