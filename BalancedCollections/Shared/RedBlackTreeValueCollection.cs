using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BalancedCollections.Base;

namespace BalancedCollections.Shared
{
	/// <summary>
	/// This wrapper class simulates an IMMUTABLE collection of values on the given
	/// red-black tree.  (Adapter pattern)
	/// </summary>
	internal class RedBlackTreeValueCollection<K, V, N> : ICollection<V>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		private readonly IRedBlackTreeBase<K, V, N> _redBlackTree;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RedBlackTreeValueCollection(IRedBlackTreeBase<K, V, N> redBlackTree)
		{
			_redBlackTree = redBlackTree;
		}

		public int Count => _redBlackTree.Count;

		public bool IsReadOnly => true;

		public void Add(V item)
			=> throw new ImmutableException();

		public void Clear()
			=> throw new ImmutableException();

		public bool Contains(V item)
		{
			foreach (N node in _redBlackTree)
			{
				if (ReferenceEquals(node.Value, null))
				{
					if (ReferenceEquals(item, null))
						return true;
				}
				else if (node.Value.Equals(item))
					return true;
			}
			return false;
		}

		public void CopyTo(V[] array, int arrayIndex)
		{
			for (N node = _redBlackTree.MinimumNode; node != null; node = node.Next())
			{
				array[arrayIndex++] = node.Value;
			}
		}

		public IEnumerator<V> GetEnumerator()
			=> new RedBlackTreeValueEnumerator<K, V, N>(_redBlackTree);

		public bool Remove(V item)
			=> throw new ImmutableException();

		IEnumerator IEnumerable.GetEnumerator()
			=> new RedBlackTreeValueEnumerator<K, V, N>(_redBlackTree);
	}
}