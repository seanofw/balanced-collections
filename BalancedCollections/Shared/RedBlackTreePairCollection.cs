using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BalancedCollections.Base;

namespace BalancedCollections.Shared
{
	/// <summary>
	/// This wrapper class simulates a mutatable collection of KeyValuePair{K, V} on the given
	/// red-black tree.  (Adapter pattern)
	/// </summary>
	internal class RedBlackTreePairCollection<K, V, N> : ICollection<KeyValuePair<K, V>>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		private readonly IRedBlackTreeBase<K, V, N> _redBlackTree;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RedBlackTreePairCollection(IRedBlackTreeBase<K, V, N> redBlackTree)
		{
			_redBlackTree = redBlackTree;
		}

		public int Count => _redBlackTree.Count;

		public bool IsReadOnly => _redBlackTree.IsReadOnly;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(KeyValuePair<K, V> item)
			=> _redBlackTree.Add(item.Key, item.Value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
			=> _redBlackTree.Clear();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(KeyValuePair<K, V> item)
		{
			N node = _redBlackTree.Find(item.Key);
			if (node == null) return false;
			if (ReferenceEquals(node.Value, null)) return ReferenceEquals(item.Value, null);
			return node.Value.Equals(item.Value);
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			for (N node = _redBlackTree.MinimumNode; node != null; node = node.Next())
			{
				array[arrayIndex++] = new KeyValuePair<K, V>(node.Key, node.Value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
			=> new RedBlackTreePairEnumerator<K, V, N>(_redBlackTree);

		public bool Remove(KeyValuePair<K, V> item)
		{
			N node = _redBlackTree.Find(item.Key);
			if (node == null) return false;
			if (ReferenceEquals(node.Value, null))
			{
				if (!ReferenceEquals(item.Value, null)) return false;
			}
			else if (!node.Value.Equals(item.Value)) return false;
			_redBlackTree.DeleteNode(node);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IEnumerator IEnumerable.GetEnumerator()
			=> new RedBlackTreePairEnumerator<K, V, N>(_redBlackTree);
	}
}