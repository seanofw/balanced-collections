using System;
using System.Collections;
using System.Collections.Generic;
using BalancedCollections.Base;

namespace BalancedCollections.Shared
{
	internal class RedBlackTreeSlice<K, V, N> : IRedBlackTreeBase<K, V, N>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		private readonly IRedBlackTreeBase<K, V, N> _redBlackTree;

		internal K Minimum { get; }
		internal K Maximum { get; }

		public Func<K, K, int> Compare
			=> _redBlackTree.Compare;

		internal RedBlackTreeSlice(IRedBlackTreeBase<K, V, N> redBlackTree, K minimum, K maximum)
		{
			_redBlackTree = redBlackTree;

			if (Compare(minimum, maximum) < 0)
				throw new ArgumentException($"The maximum key of a slice must be >= the minimum key (was given {minimum} to {maximum}).");

			Minimum = minimum;
			Maximum = maximum;
		}

		internal RedBlackTreeSlice(RedBlackTreeSlice<K, V, N> parentSlice, K minimum, K maximum)
		{
			_redBlackTree = parentSlice._redBlackTree;

			if (Compare(minimum, parentSlice.Minimum) < 0)
				throw new ArgumentException($"The minimum key of a subslice must be >= the minimum key of the containing slice ({minimum} is not >= {parentSlice.Minimum}).");
			if (Compare(maximum, parentSlice.Maximum) > 0)
				throw new ArgumentException($"The maximum key of a subslice must be <= the maximum key of the containing slice ({maximum} is not <= {parentSlice.Maximum}).");
			if (Compare(minimum, maximum) < 0)
				throw new ArgumentException($"The maximum key of a slice must be >= the minimum key (was given {minimum} to {maximum}).");

			Minimum = minimum;
			Maximum = maximum;
		}

		public ICollection<K> Keys
			=> new RedBlackTreeKeyCollection<K, V, N>(this);

		public ICollection<V> Values
			=> new RedBlackTreeValueCollection<K, V, N>(this);

		public ICollection<KeyValuePair<K, V>> KeyValuePairs
			=> new RedBlackTreePairCollection<K, V, N>(this);

		public N MaximumNode
			=> _redBlackTree.GreatestBeforeOrEqualTo(Maximum);

		public N MinimumNode
			=> _redBlackTree.LeastAfterOrEqualTo(Minimum);

		/// <summary>
		/// To obtain a count of elements in a slice, we have no choice but
		/// to iterate over it.  Tree nodes don't have their weights stored inside
		/// them, so we can't simply do fancy math to answer this in O(lg n) time,
		/// but instead we have to walk the tree, for O(n lg n) time.
		/// </summary>
		public int Count
		{
			get
			{
				int count = 0;
				N endNode = MaximumNode;
				for (N node = MinimumNode; node != null; node = node.Next())
				{
					count++;
					if (node == endNode) break;
				}
				return count;
			}
		}

		public bool IsReadOnly => _redBlackTree.IsReadOnly;

		private string RangeError(K key)
			=> $"{key} is outside the range of this slice.";

		public V this[K key]
		{
			get
			{
				if (Compare(key, Minimum) < 0 || Compare(key, Maximum) > 0)
					throw new KeyNotFoundException(RangeError(key));
				return _redBlackTree[key];
			}
			set
			{
				if (Compare(key, Minimum) < 0 || Compare(key, Maximum) > 0)
					throw new KeyNotFoundException(RangeError(key));
				_redBlackTree[key] = value;
			}
		}

		public bool TryGetValue(K key, out V value)
		{
			if (Compare(key, Minimum) < 0 || Compare(key, Maximum) > 0)
			{
				value = default;
				return false;
			}

			return _redBlackTree.TryGetValue(key, out value);
		}

		public void Add(K key, V value)
		{
			if (Compare(key, Minimum) < 0 || Compare(key, Maximum) > 0)
				throw new ArgumentException(RangeError(key));

			_redBlackTree.Add(key, value);
		}

		public bool ContainsKey(K key)
		{
			if (Compare(key, Minimum) < 0 || Compare(key, Maximum) > 0)
				return false;
			return _redBlackTree.ContainsKey(key);
		}

		public N Find(K key)
		{
			if (Compare(key, Minimum) < 0 || Compare(key, Maximum) > 0)
				return null;
			return _redBlackTree.Find(key);
		}

		public bool Remove(K key)
		{
			if (Compare(key, Minimum) < 0 || Compare(key, Maximum) > 0)
				return false;
			return _redBlackTree.Remove(key);
		}

		public IRedBlackTreeBase<K, V, N> Slice(K minimum, K maximum)
			=> new RedBlackTreeSlice<K, V, N>(this, minimum, maximum);

		public IDictionary<K, V> AsDictionary()
			=> new RedBlackTreeDictionaryWrapper<K, V, N>(this);

		public Dictionary<K, V> ToDictionary()
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			N endNode = MaximumNode;
			for (N node = MinimumNode; node != null; node = node.Next())
			{
				dictionary.Add(node.Key, node.Value);
				if (node == endNode) break;
			}
			return dictionary;
		}

		public void DeleteNode(N node)
		{
			if (Compare(node.Key, Minimum) < 0 || Compare(node.Key, Maximum) > 0)
				return;
			_redBlackTree.DeleteNode(node);
		}

		public bool FindOrInsert(K key, V value, out N resultingNode)
		{
			if (Compare(key, Minimum) < 0 || Compare(key, Maximum) > 0)
				throw new ArgumentException(RangeError(key));
			return _redBlackTree.FindOrInsert(key, value, out resultingNode);
		}

		public void Add(N item)
			=> Add(item.Key, item.Value);

		public int RemoveRange(K minimum, K maximum, bool inclusiveMinimum = true, bool inclusiveMaximum = true)
		{
			if (Compare(minimum, Maximum) > 0)
				throw new ArgumentException(RangeError(minimum), "minimum");
			if (Compare(maximum, Minimum) < 0)
				throw new ArgumentException(RangeError(maximum), "maximum");

			if (inclusiveMinimum)
			{
				if (Compare(minimum, Minimum) < 0)
					throw new ArgumentException(RangeError(minimum), "minimum");
			}
			else
			{
				N startNode = _redBlackTree.LeastAfterOrEqualTo(minimum);
				if (startNode == null || Compare(startNode.Key, Minimum) < 0)
					throw new ArgumentException(RangeError(minimum), "minimum");
			}

			if (inclusiveMaximum)
			{
				if (Compare(maximum, Maximum) > 0)
					throw new ArgumentException(RangeError(maximum), "maximum");
			}
			else
			{
				N endNode = _redBlackTree.GreatestBeforeOrEqualTo(maximum);
				if (endNode == null || Compare(endNode.Key, Maximum) > 0)
					throw new ArgumentException(RangeError(maximum), "maximum");
			}

			return _redBlackTree.RemoveRange(minimum, maximum, inclusiveMinimum, inclusiveMaximum);
		}

		public void Clear()
		{
			_redBlackTree.RemoveRange(Minimum, Maximum);
		}

		public bool Contains(N item)
		{
			if (Compare(item.Key, Minimum) < 0 || Compare(item.Key, Maximum) > 0)
				return false;
			return _redBlackTree.Contains(item);
		}

		public void CopyTo(N[] array, int arrayIndex)
		{
			N endNode = MaximumNode;
			for (N node = MinimumNode; node != null; node = node.Next())
			{
				array[arrayIndex++] = node;
				if (node == endNode) break;
			}
		}

		public bool Remove(N item)
		{
			if (Compare(item.Key, Minimum) < 0 || Compare(item.Key, Maximum) > 0)
				return false;
			return _redBlackTree.Remove(item);
		}

		public IEnumerator<N> GetEnumerator()
			=> new RedBlackTreeEnumerator<K, V, N>(this);

		IEnumerator IEnumerable.GetEnumerator()
			=> new RedBlackTreeEnumerator<K, V, N>(this);

		public N GreatestBeforeOrEqualTo(K key)
		{
			N node = _redBlackTree.GreatestBeforeOrEqualTo(key);
			return (node != null && Compare(node.Key, Minimum) >= 0 && Compare(node.Key, Maximum) <= 0)
				? node : null;
		}

		public N GreatestBefore(K key)
		{
			N node = _redBlackTree.GreatestBefore(key);
			return (node != null && Compare(node.Key, Minimum) >= 0 && Compare(node.Key, Maximum) <= 0)
				? node : null;
		}

		public N LeastAfterOrEqualTo(K key)
		{
			N node = _redBlackTree.LeastAfterOrEqualTo(key);
			return (node != null && Compare(node.Key, Minimum) >= 0 && Compare(node.Key, Maximum) <= 0)
				? node : null;
		}

		public N LeastAfter(K key)
		{
			N node = _redBlackTree.LeastAfter(key);
			return (node != null && Compare(node.Key, Minimum) >= 0 && Compare(node.Key, Maximum) <= 0)
				? node : null;
		}

		public N LeastInRange(K minimum, K maximum,
			bool inclusiveMinimum = true, bool inclusiveMaximum = true)
		{
			N node = _redBlackTree.LeastInRange(minimum, maximum, inclusiveMinimum, inclusiveMaximum);
			return (node != null && Compare(node.Key, Minimum) >= 0 && Compare(node.Key, Maximum) <= 0)
				? node : null;
		}

		public N GreatestInRange(K minimum, K maximum,
			bool inclusiveMinimum = true, bool inclusiveMaximum = true)
		{
			N node = _redBlackTree.GreatestInRange(minimum, maximum, inclusiveMinimum, inclusiveMaximum);
			return (node != null && Compare(node.Key, Minimum) >= 0 && Compare(node.Key, Maximum) <= 0)
				? node : null;
		}
	}
}