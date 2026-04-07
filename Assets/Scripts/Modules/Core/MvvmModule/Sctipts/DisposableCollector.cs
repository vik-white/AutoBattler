using System;
using System.Collections.Generic;

namespace vikwhite
{
    public class DisposableCollector : IDisposable
    {
        private readonly List<IDisposable> _disposables = new();
        
        public void AddDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
		
        public void AddDisposables(IReadOnlyList<IDisposable> disposables)
        {
            _disposables.AddRange(disposables);
        }
		
        public void AddDisposables(params IDisposable[] disposables)
        {
            _disposables.AddRange(disposables);
        }

        public virtual void Dispose()
        {
            for (int i = 0; i < _disposables.Count; i++) _disposables[i].Dispose();
            _disposables.Clear();
        }
    }
}