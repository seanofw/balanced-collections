using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BalancedCollections.Base;

namespace BalancedCollections.Shared
{
	/// <summary>
	/// This wrapper class simulates a mutatable collection of keys on the given
	/// red-black tree.  (Adapter pattern)
	/// </summary>
	internal class RedBlackTreeKeyCollection<K, V, N> : ICollection<K>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		private readonly IRedBlackTreeBase<K, V, N> _redBlackTree;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RedBlackTreeKeyCollection(IRedBlackTreeBase<K, V, N> redBlackTree)
		{
			_redBlackTree = redBlackTree;
		}

		public int Count => _redBlackTree.Count;

		public bool IsReadOnly => _redBlackTree.IsReadOnly;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(K item)
			=> _redBlackTree.Add(item, default);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
			=> _redBlackTree.Clear();

		public bool Contains(K item)
			=> _redBlackTree.ContainsKey(item);

		public void CopyTo(K[] array, int arrayIndex)
		{
			for (N node = _redBlackTree.MinimumNode; node != null; node = node.Next())
			{
				array[arrayIndex++] = node.Key;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<K> GetEnumerator()
			=> new RedBlackTreeKeyEnumerator<K, V, N>(_redBlackTree);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(K item)
			=> _redBlackTree.Remove(item);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
	}
}