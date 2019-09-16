using BalancedCollections.Base;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BalancedCollections.Shared
{
	/// <summary>
	/// This lightweight wrapper class makes a RedBlackTree{K, V} look like an implementor of
	/// IDictionary{K, V}, even though it isn't.  (Adapter pattern)
	/// </summary>
	internal class RedBlackTreeDictionaryWrapper<K, V, N> : IDictionary<K, V>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		private readonly IRedBlackTreeBase<K, V, N> _redBlackTree;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RedBlackTreeDictionaryWrapper(IRedBlackTreeBase<K, V, N> redBlackTree)
		{
			_redBlackTree = redBlackTree;
		}

		public V this[K key]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _redBlackTree[key];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set => _redBlackTree[key] = value;
		}

		public ICollection<K> Keys
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _redBlackTree.Keys;
		}

		public ICollection<V> Values
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _redBlackTree.Values;
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _redBlackTree.Count;
		}

		public bool IsReadOnly
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _redBlackTree.IsReadOnly;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(K key, V value)
			=> _redBlackTree.Add(key, value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(KeyValuePair<K, V> item)
			=> _redBlackTree.Add(item.Key, item.Value);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
			=> _redBlackTree.Clear();

		public bool Contains(KeyValuePair<K, V> item)
		{
			N node = _redBlackTree.Find(item.Key);
			if (node == null) return false;
			if (ReferenceEquals(item.Value, null))
				return ReferenceEquals(node.Value, null);
			else
				return item.Value.Equals(node.Value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsKey(K key)
			=> _redBlackTree.ContainsKey(key);

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			foreach (N node in _redBlackTree)
			{
				array[arrayIndex++] = new KeyValuePair<K, V>(node.Key, node.Value);
			}
		}

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			foreach (N node in _redBlackTree)
			{
				yield return new KeyValuePair<K, V>(node.Key, node.Value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove(K key)
			=> _redBlackTree.Remove(key);

		public bool Remove(KeyValuePair<K, V> item)
		{
			N node = _redBlackTree.Find(item.Key);
			if (node == null) return false;
			if (ReferenceEquals(item.Value, null))
			{
				if (ReferenceEquals(node.Value, null))
				{
					_redBlackTree.DeleteNode(node);
					return true;
				}
			}
			else
			{
				if (item.Value.Equals(node.Value))
				{
					_redBlackTree.DeleteNode(node);
					return true;
				}
			}
			return false;
		}

		public bool TryGetValue(K key, out V value)
			=> _redBlackTree.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
	}
}