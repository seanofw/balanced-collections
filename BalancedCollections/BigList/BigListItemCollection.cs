using System.Collections;
using System.Collections.Generic;

namespace BalancedCollections.BigList
{
	internal class BigListItemCollection<V> : ICollection<BigListItem<V>>
	{
		private readonly BigList<V> _bigList;

		public BigListItemCollection(BigList<V> bigList)
		{
			_bigList = bigList;
		}

		public int Count => _bigList.Count;

		public bool IsReadOnly => _bigList.IsReadOnly;

		public void Add(BigListItem<V> item)
			=> _bigList.Add(item.Value);

		public void Clear()
			=> _bigList.Clear();

		public bool Contains(BigListItem<V> item)
			=> _bigList.ContainsItem(item);

		public void CopyTo(BigListItem<V>[] array, int arrayIndex)
		{
			for (BigListItem<V> item = _bigList.FirstItem; item != null; item = item.Next)
			{
				array[arrayIndex++] = item;
			}
		}

		public IEnumerator<BigListItem<V>> GetEnumerator()
			=> new BigListItemEnumerator<V>(_bigList);

		public bool Remove(BigListItem<V> item)
			=> _bigList.DeleteItem(item);

		IEnumerator IEnumerable.GetEnumerator()
			=> new BigListItemEnumerator<V>(_bigList);
	}
}