using System;
using System.Collections;
using System.Collections.Generic;

namespace BalancedCollections.BigList
{
	internal class BigListEnumerator<V> : IEnumerator<V>
	{
		private readonly BigList<V> _bigList;
		private bool _isStarted;

		public BigListEnumerator(BigList<V> bigList)
		{
			_bigList = bigList;
		}

		public V Current
		{
			get
			{
				if (_current != null)
					return _current.Value;
				if (!_isStarted)
					throw new InvalidOperationException("Enumeration has not yet started.");
				else
					throw new InvalidOperationException("Enumeration was completed.");
			}
		}
		private BigListItem<V> _current;
		private BigListItem<V> _next;

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (!_isStarted)
			{
				_current = _bigList.FirstItem;
				_isStarted = true;
			}
			else
			{
				_current = _next;
			}
			_next = _current?.Next;
			return _current != null;
		}

		public void Reset()
		{
			_isStarted = false;
			_current = null;
		}
	}
}