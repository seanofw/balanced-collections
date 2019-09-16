using System;
using System.Collections;
using System.Collections.Generic;
using BalancedCollections.Base;

namespace BalancedCollections.Shared
{
	/// <summary>
	/// This wrapper class ensures its consumers treat the red-black tree's shape and keys
	/// as read-only, just like a ReadOnlyDictionary{K,V} does for Dictionary{K,V}.  Consumers
	/// can still mutate the values if those values are themselves not immutable.
	/// </summary>
	public abstract class ReadOnlyRedBlackTreeBase<K, V, N> : IRedBlackTreeBase<K, V, N>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		protected readonly IRedBlackTreeBase<K, V, N> RedBlackTree;

		protected ReadOnlyRedBlackTreeBase(IRedBlackTreeBase<K, V, N> tree)
		{
			RedBlackTree = tree;
		}

		public V this[K key]
		{
			get => RedBlackTree[key];
			set => throw new ImmutableException();
		}

		public Func<K, K, int> Compare => RedBlackTree.Compare;

		public ICollection<K> Keys => new RedBlackTreeKeyCollection<K, V, N>(this);

		public ICollection<V> Values => new RedBlackTreeValueCollection<K, V, N>(this);

		public ICollection<KeyValuePair<K, V>> KeyValuePairs => new RedBlackTreePairCollection<K, V, N>(this);

		public N MaximumNode => RedBlackTree.MaximumNode;

		public N MinimumNode => RedBlackTree.MinimumNode;

		public int Count => RedBlackTree.Count;

		public bool IsReadOnly => true;

		public void Add(K key, V value)
			=> throw new ImmutableException();

		public void Add(N item)
			=> throw new ImmutableException();

		public IDictionary<K, V> AsDictionary()
			=> new RedBlackTreeDictionaryWrapper<K, V, N>(this);

		public void Clear()
			=> throw new ImmutableException();

		public bool Contains(N item)
			=> RedBlackTree.Contains(item);

		public bool ContainsKey(K key)
			=> RedBlackTree.ContainsKey(key);

		public void CopyTo(N[] array, int arrayIndex)
			=> RedBlackTree.CopyTo(array, arrayIndex);

		public void DeleteNode(N node)
			=> throw new ImmutableException();

		public N Find(K key)
			=> RedBlackTree.Find(key);

		public bool FindOrInsert(K key, V value, out N resultingNode)
		{
			resultingNode = RedBlackTree.Find(key);
			if (resultingNode == null)
				throw new ImmutableException();
			return false;
		}

		public N GreatestBefore(K key)
			=> RedBlackTree.GreatestBefore(key);

		public N GreatestBeforeOrEqualTo(K key)
			=> RedBlackTree.GreatestBeforeOrEqualTo(key);

		public N GreatestInRange(K minimum, K maximum, bool inclusiveMinimum = true, bool inclusiveMaximum = true)
			=> RedBlackTree.GreatestInRange(minimum, maximum, inclusiveMinimum, inclusiveMaximum);

		public N LeastAfter(K key)
			=> RedBlackTree.LeastAfter(key);

		public N LeastAfterOrEqualTo(K key)
			=> RedBlackTree.LeastAfterOrEqualTo(key);

		public N LeastInRange(K minimum, K maximum, bool inclusiveMinimum = true, bool inclusiveMaximum = true)
			=> RedBlackTree.LeastInRange(minimum, maximum, inclusiveMinimum, inclusiveMaximum);

		public bool Remove(K key)
			=> throw new ImmutableException();

		public bool Remove(N item)
			=> throw new ImmutableException();

		public int RemoveRange(K minimum, K maximum, bool inclusiveMinimum = true, bool inclusiveMaximum = true)
			=> throw new ImmutableException();

		public IRedBlackTreeBase<K, V, N> Slice(K minimum, K maximum)
			=> new RedBlackTreeSlice<K, V, N>(RedBlackTree, minimum, maximum);

		public Dictionary<K, V> ToDictionary()
			=> RedBlackTree.ToDictionary();

		public bool TryGetValue(K key, out V value)
			=> RedBlackTree.TryGetValue(key, out value);

		public IEnumerator<N> GetEnumerator()
			=> RedBlackTree.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> RedBlackTree.GetEnumerator();
	}
}
