using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Core.Pooling
{
    public abstract class AbstractMemoryPool<T> : IDisposable where T : class
    {
        public int MaxSize
        {
            get => _maxSize;
            set
            {
                if (value < _maxSize && value < _stack.Count)
                    for (int i = 0; i < _stack.Count - value; i++)
                        _onReturn?.Invoke(_stack.Pop(), true);

                _maxSize = value;
            }
        }

        int _maxSize;

        readonly Stack<T> _stack = new();
        Action<T, bool>? _onReturn;
        protected readonly object _locker = new();

        protected AbstractMemoryPool(Action<T, bool>? onReturn, int maxSize = int.MaxValue)
        {
            _onReturn = onReturn;
            MaxSize = maxSize;
        }

        public void Dispose() { }

        /// <summary>
        /// Returns element back to the pool if <see cref="MaxSize"/> has not been reached.
        /// Invokes <see cref="_onReturn"/> function.
        /// </summary>
        public void Return(T element)
        {
            if (_stack.Count >= MaxSize)
            {
                _onReturn?.Invoke(element, true);

                return;
            }

            _onReturn?.Invoke(element, false);

            lock (_locker)
                Assert.IsFalse(_stack.Contains(element), "Attempted to return the same element to the pool twice!");

            _stack.Push(element);
        }

        // We assume here that we're in a lock
        protected T GetInternal() => _stack.Count == 0 ? Alloc() : _stack.Pop();

        protected abstract T Alloc();
    }
}