using System;
using System.Collections.Generic;

namespace BalancedCollections.Base
{
	public interface IRedBlackTreeBase<K, V, N> : ICollection<N>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		Func<K, K, int> Compare { get; }

		V this[K key] { get; set; }
		bool TryGetValue(K key, out V value);

		ICollection<K> Keys { get; }
		ICollection<V> Values { get; }
		ICollection<KeyValuePair<K, V>> KeyValuePairs { get; }
		N MaximumNode { get; }
		N MinimumNode { get; }

		void Add(K key, V value);
		bool ContainsKey(K key);
		N Find(K key);
		bool Remove(K key);
		int RemoveRange(K minimum, K maximum, bool inclusiveMinimum = true, bool inclusiveMaximum = true);
		IRedBlackTreeBase<K, V, N> Slice(K minimum, K maximum);
		IDictionary<K, V> AsDictionary();
		Dictionary<K, V> ToDictionary();

		void DeleteNode(N node);
		bool FindOrInsert(K key, V value, out N resultingNode);

		N GreatestBeforeOrEqualTo(K key);
		N GreatestBefore(K key);
		N LeastAfterOrEqualTo(K key);
		N LeastAfter(K key);
		N LeastInRange(K minimum, K maximum,
			bool inclusiveMinimum = true, bool inclusiveMaximum = true);
		N GreatestInRange(K minimum, K maximum,
			bool inclusiveMinimum = true, bool inclusiveMaximum = true);
	}
}
