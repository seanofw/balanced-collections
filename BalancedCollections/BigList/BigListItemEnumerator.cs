using System.Collections;
using System.Collections.Generic;

namespace BalancedCollections.BigList
{
	internal class BigListItemEnumerator<V> : IEnumerator<BigListItem<V>>
	{
		private readonly BigList<V> _bigList;
		private bool _isStarted;

		public BigListItemEnumerator(BigList<V> bigList)
		{
			_bigList = bigList;
		}

		public BigListItem<V> Current { get; private set; }
		private BigListItem<V> _next;

		object IEnumerator.Current => Current;

		public void Dispose()
		{
		}

		public bool MoveNext()
		{
			if (!_isStarted)
			{
				Current = _bigList.FirstItem;
				_isStarted = true;
			}
			else
			{
				Current = _next;
			}
			_next = Current?.Next;
			return Current != null;
		}

		public void Reset()
		{
			_isStarted = false;
			Current = null;
		}
	}
}