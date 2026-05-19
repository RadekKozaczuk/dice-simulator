using System;
using System.Collections.Generic;

namespace Core.Pooling
{
    public abstract class AbstractMemoryPool<T> : IDisposable where T : class
    {
        public int MaxSize
        {
            set
            {
                if (value < _maxSize && value < _stack.Count)
                    for (int i = 0; i < _stack.Count - value; i++)
                        _onReturn?.Invoke(_stack.Pop(), true);

                _maxSize = value;
            }
        }

        int _maxSize;

        // ReSharper disable once CollectionNeverUpdated.Local
        readonly Stack<T> _stack = new();
        Action<T, bool> _onReturn;
        protected readonly object _locker = new();

        protected AbstractMemoryPool(Action<T, bool> onReturn, int maxSize = int.MaxValue)
        {
            _onReturn = onReturn;
            MaxSize = maxSize;
        }

        public void Dispose() { }

        // We assume here that we're in a lock
        protected T GetInternal() => _stack.Count == 0 ? Alloc() : _stack.Pop();

        protected abstract T Alloc();
    }
}