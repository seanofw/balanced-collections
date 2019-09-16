using BalancedCollections.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BalancedCollections.Base
{
	public abstract class RedBlackTreeBase<K, V, N> : IRedBlackTreeBase<K, V, N>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		#region Core properties

		/// <summary>
		/// The current root of the tree. O(1).
		/// </summary>
		public N Root { get; protected set; }

		/// <summary>
		/// The comparison function to use. O(1).
		/// </summary>
		public Func<K, K, int> Compare { get; }

		#endregion

		#region Minima and Maxima

		/// <summary>
		/// The node with the smallest key. O(lg n).
		/// </summary>
		public N MinimumNode
			=> Root != null ? Root.MinimumSubnode() : null;

		/// <summary>
		/// The smallest key in the tree (default if the tree is empty). O(lg n).
		/// </summary>
		public K MinimumKey
			=> Root != null ? Root.MinimumSubnode().Key : default;

		/// <summary>
		/// The node with the largest key. O(lg n).
		/// </summary>
		public N MaximumNode
			=> Root != null ? Root.MaximumSubnode() : null;

		/// <summary>
		/// The largest key in the tree (default if the tree is empty). O(lg n).
		/// </summary>
		public K MaximumKey
			=> Root != null ? Root.MaximumSubnode().Key : default;

		#endregion

		#region Construction

		protected RedBlackTreeBase(Func<K, K, int> compare)
		{
			if (compare == null)
			{
				if (typeof(IComparable<K>).IsAssignableFrom(typeof(K)))
				{
					compare = CompareViaIComparableGeneric;
				}
				else if (typeof(IComparable).IsAssignableFrom(typeof(K)))
				{
					compare = CompareViaIComparable;
				}
				else throw new ArgumentException($"No explicit comparer function for type {typeof(K)} provided, and {typeof(K)} does not implement IComparable<{typeof(K)}> either.");
			}

			Compare = compare;
		}

		#endregion

		#region Implicit comparers

		/// <summary>
		/// Given two keys that are known to use IComparable{K}, invoke IComparable{K}.CompareTo().
		/// Runtime is proportional to that of IComparable{K}.CompareTo() (typically O(1)).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int CompareViaIComparableGeneric(K a, K b)
			=> ((IComparable<K>)a).CompareTo(b);

		/// <summary>
		/// Given two keys that are known to use IComparable, invoke IComparable.CompareTo().
		/// Runtime is proportional to that of IComparable.CompareTo() (typically O(1)).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int CompareViaIComparable(K a, K b)
			=> ((IComparable)a).CompareTo(b);

		#endregion

		#region Access by key

		/// <summary>
		/// Access items within the red-black tree by key, as though it's an IDictionary{K,V}.
		/// O(lg n).
		/// </summary>
		/// <param name="key">The key to read or write within the tree.</param>
		/// <returns>The value for that key.</returns>
		public V this[K key]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				N node = Find(key);
				if (node == null)
					throw new KeyNotFoundException($"\"{key}\" was not found in the red-black tree.");
				return node.Value;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				if (!FindOrInsert(key, value, out N node))
				{
					node.Value = value;
				}
			}
		}

		/// <summary>
		/// Try to read the current value for a key, if a value is assigned for that key.
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <param name="value">The associated value for that key, if there is one.</param>
		/// <returns>True if the key exists in the tree, false if it does not.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetValue(K key, out V value)
		{
			N node = Find(key);
			if (node != null)
			{
				value = node.Value;
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}

		#endregion

		#region Abstract parts

		public abstract N Find(K key);
		public abstract bool FindOrInsert(K key, V value, out N resultingNode);
		public abstract void DeleteNode(N node);
		public abstract void Validate();
		public abstract void Clear();
		public abstract int Count { get; }
		public abstract bool IsReadOnly { get; }

		#endregion

		#region IDictionary (almost) implementation parts

		/// <summary>
		/// Obtain an ICollection{K} mutatable wrapper/adapter around the keys of this tree.
		/// O(1).
		/// </summary>
		public ICollection<K> Keys => new RedBlackTreeKeyCollection<K, V, N>(this);

		/// <summary>
		/// Obtain an ICollection{K} immutable wrapper/adapter around the values of this tree.
		/// O(1).
		/// </summary>
		public ICollection<V> Values => new RedBlackTreeValueCollection<K, V, N>(this);

		/// <summary>
		/// Obtain an ICollection{KeyValuePair{K, V}} mutatable wrapper/adapter around
		/// the nodes of this tree, making them appear to be KeyValuePair{K, V}.  O(1).
		/// </summary>
		public ICollection<KeyValuePair<K, V>> KeyValuePairs => new RedBlackTreePairCollection<K, V, N>(this);

		/// <summary>
		/// Add the given key/value pair as a new node of the tree.  This will fail
		/// if the given key already exists.  O(lg n).
		/// </summary>
		/// <param name="key">The key to add to the new node.</param>
		/// <param name="value">The value to add to the new node.</param>
		/// <throws cref="ArgumentException">Thrown if the given key already exists.</throws>
		public void Add(K key, V value)
		{
			if (!FindOrInsert(key, value, out N node))
				throw new ArgumentException($"Duplicate key \"{key}\" added.");
		}

		/// <summary>
		/// Add a copy of a preexisting node to the tree.  This will fail if another
		/// node with the same key already exists.  O(lg n).
		/// </summary>
		/// <param name="oldNode">The node to copy into a new node of this tree.  The
		/// oldNode does not necessarily have to be a member of this tree or of any
		/// other.</param>
		/// <throws cref="ArgumentException">Thrown if the given key already exists.</throws>
		public void Add(N oldNode)
			=> Add(oldNode.Key, oldNode.Value);

		/// <summary>
		/// Add a range of key/value pairs as new nodes of this tree.  This will fail
		/// if any of the keys already exist (and will leave the tree in an undefined
		/// state where some pairs have been added and some have not).  O(m * lg n),
		/// where m is the number of pairs in the provided collection.
		/// </summary>
		/// <param name="pairs">The key/value pairs to add.</param>
		/// <returns>The number of new nodes that were created.</returns>
		/// <throws cref="ArgumentException">Thrown if one of the given keys already exists.</throws>
		public int AddRange(IEnumerable<KeyValuePair<K, V>> pairs)
		{
			int numAdded = 0;
			foreach (KeyValuePair<K, V> pair in pairs)
			{
				Add(pair.Key, pair.Value);
				numAdded++;
			}
			return numAdded;
		}

		/// <summary>
		/// Add copies of existing nodes as new nodes of this tree.  This will fail
		/// if any of the keys already exist (and will leave the tree in an undefined
		/// state where some nodes have been added and some have not).  O(m * lg n),
		/// where m is the number of nodes in the provided collection.
		/// </summary>
		/// <param name="nodes">The nodes to copy into the tree.</param>
		/// <returns>The number of new nodes that were created.</returns>
		/// <throws cref="ArgumentException">Thrown if one of the given keys already exists.</throws>
		public void AddRange(IEnumerable<N> nodes)
		{
			foreach (N node in nodes)
			{
				Add(node.Key, node.Value);
			}
		}

		/// <summary>
		/// Determine if the tree contains the given key.  O(lg n).
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>True if the key exists in the tree, false if it does not.</returns>
		public bool ContainsKey(K key)
			=> Find(key) != null;

		/// <summary>
		/// Determine if this tree contains the given node.  O(lg n).
		/// </summary>
		/// <param name="node">The node to test for inclusion.</param>
		/// <returns>True if the node is a member of this tree, false if it is not.</returns>
		public bool Contains(N node)
			=> node != null && Find(node.Key) == node;

		/// <summary>
		/// Copy all of the nodes in this tree to the given array.  The array
		/// must have enough room to contain all of the nodes.  O(n lg n).
		/// </summary>
		/// <param name="array">The array to copy to.</param>
		/// <param name="arrayIndex">The index within the array at which to start copying.</param>
		public void CopyTo(N[] array, int arrayIndex)
		{
			if (Root != null)
			{
				for (N node = Root.MinimumSubnode(); node != null; node = node.Next())
				{
					array[arrayIndex++] = node;
				}
			}
		}

		/// <summary>
		/// Remove the node with the given key from the tree.  O(lg n).
		/// </summary>
		/// <param name="key">The key of the node to remove.</param>
		/// <returns>True if the key was found (and the node removed); false if the
		/// key was not found (and the tree was left unchanged).</returns>
		public bool Remove(K key)
		{
			N node = Find(key);
			if (node == null)
				return false;

			DeleteNode(node);
			return true;
		}

		/// <summary>
		/// Remove the given node from the tree, if and only if it is a member of
		/// the tree.  O(lg n).
		/// </summary>
		/// <param name="node">The node to remove.  This will be tested to see if it
		/// is a member of the tree.</param>
		/// <returns>True if the node was a member of this tree (and thus was removed);
		/// false if the node is not a member of this tree (and the tree was left
		/// unchanged).</returns>
		public bool Remove(N node)
		{
			if (node == null)
				return false;
			N realNode = Find(node.Key);
			if (realNode == null)
				return false;
			DeleteNode(realNode);
			return true;
		}

		/// <summary>
		/// Remove a subset of the tree within the given range.  O(m lg n), where
		/// m is the number of nodes removed and n is the number of nodes in the tree.
		/// </summary>
		/// <param name="minimum">The minimum key to remove (does not necessarily need to exist in the tree).</param>
		/// <param name="maximum">The maximum key to remove (does not necessarily need to exist in the tree).</param>
		/// <param name="inclusiveMinimum">Whether the minimum is inclusive (the minimum will also be removed),
		/// or exclusive (the minimum will not be removed, but the node after it will).</param>
		/// <param name="inclusiveMaximum">Whether the minimum is inclusive (the maximum will also be removed),
		/// or exclusive (the maximum will not be removed, but the node before it will).</param>
		/// <returns>The number of nodes removed from the tree.</returns>
		public int RemoveRange(K minimum, K maximum, bool inclusiveMinimum = true, bool inclusiveMaximum = true)
		{
			if (Compare(minimum, maximum) > 0)
				throw new ArgumentException($"Minimum key cannot be greater than maximum key (was given {minimum} to {maximum}).");

			N startNode = inclusiveMinimum
				? LeastAfterOrEqualTo(minimum)
				: LeastAfter(minimum);

			N endNode = inclusiveMaximum
				? GreatestBeforeOrEqualTo(maximum)
				: GreatestBefore(maximum);

			if (startNode == null || endNode == null)
				return 0;

			int removedCount = 0;
			for (N node = startNode, next; ; node = next)
			{
				next = node.Next();
				DeleteNode(node);
				removedCount++;
				if (node == endNode) break;
			}

			return removedCount;
		}

		/// <summary>
		/// Return a wrapper around a "slice" of the tree, that is, a mutatable proxy to
		/// a subset of the nodes of the tree.  Mutating the proxy will mutate the tree
		/// in-place.
		/// </summary>
		/// <param name="minimum">The minimum (inclusive) key of the slice.</param>
		/// <param name="maximum">The maximum (inclusive) key of the slice.</param>
		/// <returns>A mutatable subset of the tree.</returns>
		public IRedBlackTreeBase<K, V, N> Slice(K minimum, K maximum)
			=> new RedBlackTreeSlice<K, V, N>(this, minimum, maximum);

		#endregion

		#region Dictionary (for real)

		/// <summary>
		/// Convert this RedBlackTree{K, V} to a concrete Dictionary{K, V} instance by
		/// copying all of its data to a new Dictionary{K, V} instance.  The new dictionary
		/// will have no further relationship to the red-black tree.
		/// </summary>
		/// <returns>The data from the red-black tree, copied into a new dictionary.</returns>
		public Dictionary<K, V> ToDictionary()
		{
			Dictionary<K, V> dictionary = new Dictionary<K, V>();
			for (N node = Root?.MinimumSubnode(); node != null; node = node.Next())
			{
				dictionary.Add(node.Key, node.Value);
			}
			return dictionary;
		}

		/// <summary>
		/// Convert this RedBlackTree{K, V} to an IDictionary{K, V} interface by wrapping
		/// it with a lightweight adapter class that implements IDictionary{K, V} on the
		/// same RedBlackTree{K, V} instance.  Construction of the wrapper class costs O(1).
		/// </summary>
		/// <returns>This same red-black tree instance, as an IDictionary{K, V} interface.</returns>
		public IDictionary<K, V> AsDictionary()
			=> new RedBlackTreeDictionaryWrapper<K, V, N>(this);

		#endregion

		#region Relative searches

		/// <summary>
		/// Find the node of the tree that either matches the given key, or if the key
		/// is not found, this finds the node that would immediately precede it in order,
		/// if the key actually existed.  O(lg n).
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>The node with that key, or if there is no node with that key, the
		/// node that would precede it.  If there is no predecessor node, this returns
		/// null.</returns>
		public N GreatestBeforeOrEqualTo(K key)
		{
			int cmp = 0;
			N node = Root, parent = null;

			// Nothing in the tree, so give up.
			if (node == null)
				return null;

			// Search for it, keeping track of any parents we pass through.
			while (node != null)
			{
				if ((cmp = Compare(key, node.Key)) == 0)
					return node;
				parent = node;
				node = cmp < 0 ? node.Left : node.Right;
			}

			// Didn't find it, so find the nearest one before it.
			return cmp < 0 ? parent.Previous() : parent.MinimumSubnode();
		}

		/// <summary>
		/// Find the node of the tree that immediately precedes the given key in order,
		/// even if the key itself is not present in the tree.  O(lg n).
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>The node that immediately precedes the given key in order.  If there
		/// is no predecessor node, this returns null.</returns>
		public N GreatestBefore(K key)
		{
			int cmp = 0;
			N node = Root, parent = null;

			// Nothing in the tree, so give up.
			if (node == null)
				return null;

			// Search for it, keeping track of any parents we pass through.
			while (node != null)
			{
				if ((cmp = Compare(key, node.Key)) == 0)
				{
					// We found it exactly, but we're not looking for *it*, so
					// return the node before it.
					return node.Previous();
				}
				parent = node;
				node = cmp < 0 ? node.Left : node.Right;
			}

			// Didn't find it, so find the nearest one before it.
			return cmp < 0 ? parent.Previous() : parent.MinimumSubnode();
		}

		/// <summary>
		/// Find the node of the tree that either matches the given key, or if the key
		/// is not found, this finds the node that would immediately follow it in order,
		/// if the key actually existed.  O(lg n).
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>The node with that key, or if there is no node with that key, the
		/// node that would follow it.  If there is no successor node, this returns
		/// null.</returns>
		public N LeastAfterOrEqualTo(K key)
		{
			int cmp = 0;
			N node = Root, parent = null;

			// Nothing in the tree, so give up.
			if (node == null)
				return null;

			// Search for it, keeping track of any parents we pass through.
			while (node != null)
			{
				if ((cmp = Compare(key, node.Key)) == 0)
					return node;
				parent = node;
				node = cmp < 0 ? node.Left : node.Right;
			}

			// Didn't find it, so find the nearest one before it.
			return cmp < 0 ? parent.MaximumSubnode() : parent.Next();
		}

		/// <summary>
		/// Find the node of the tree that immediately follows the given key in order,
		/// even if the key itself is not present in the tree.  O(lg n).
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>The node that immediately follows the given key in order.  If there
		/// is no successor node, this returns null.</returns>
		public N LeastAfter(K key)
		{
			int cmp = 0;
			N node = Root, parent = null;

			// Nothing in the tree, so give up.
			if (node == null)
				return null;

			// Search for it, keeping track of any parents we pass through.
			while (node != null)
			{
				if ((cmp = Compare(key, node.Key)) == 0)
				{
					// We found it exactly, but we're not looking for *it*, so
					// return the node after it.
					return node.Next();
				}
				parent = node;
				node = cmp < 0 ? node.Left : node.Right;
			}

			// Didn't find it, so find the nearest one before it.
			return cmp < 0 ? parent.MaximumSubnode() : parent.Next();
		}

		/// <summary>
		/// Given a range of possible keys, find the node with the smallest actual
		/// key in that range.  O(lg n).
		/// </summary>
		/// <param name="minimum">The minimum key of the range.</param>
		/// <param name="maximum">The maximum key of the range.</param>
		/// <param name="inclusiveMinimum">Whether the minimum key is inclusive (true) or
		/// exclusive (false).  If the minimum key is inclusive, the result may match it;
		/// if the minimum key is exclusive, the result must be strictly greater than it.</param>
		/// <param name="inclusiveMaximum">Whether the maximum key is inclusive (true) or
		/// exclusive (false).  If the maximum key is inclusive, the result may match it;
		/// if the maximum key is exclusive, the result must be strictly less than than it.</param>
		/// <returns>The node with the smallest key in the given range, or null if no node was found.</returns>
		public N LeastInRange(K minimum, K maximum,
			bool inclusiveMinimum = true, bool inclusiveMaximum = true)
		{
			N node = inclusiveMinimum
				? LeastAfterOrEqualTo(minimum)
				: LeastAfter(minimum);

			if (node == null)
				return null;

			if (inclusiveMaximum)
				return Compare(maximum, node.Key) >= 0 ? node : null;
			else
				return Compare(maximum, node.Key) > 0 ? node : null;
		}

		/// <summary>
		/// Given a range of possible keys, find the node with the largest actual
		/// key in that range.  O(lg n).
		/// </summary>
		/// <param name="minimum">The minimum key of the range.</param>
		/// <param name="maximum">The maximum key of the range.</param>
		/// <param name="inclusiveMinimum">Whether the minimum key is inclusive (true) or
		/// exclusive (false).  If the minimum key is inclusive, the result may match it;
		/// if the minimum key is exclusive, the result must be strictly greater than it.</param>
		/// <param name="inclusiveMaximum">Whether the maximum key is inclusive (true) or
		/// exclusive (false).  If the maximum key is inclusive, the result may match it;
		/// if the maximum key is exclusive, the result must be strictly less than than it.</param>
		/// <returns>The node with the largest key in the given range, or null if no node was found.</returns>
		public N GreatestInRange(K minimum, K maximum,
			bool inclusiveMinimum = true, bool inclusiveMaximum = true)
		{
			N node = inclusiveMaximum
				? GreatestBeforeOrEqualTo(maximum)
				: GreatestBefore(maximum);

			if (node == null)
				return null;

			if (inclusiveMinimum)
				return Compare(minimum, node.Key) <= 0 ? node : null;
			else
				return Compare(minimum, node.Key) < 0 ? node : null;
		}

		#endregion

		#region Enumeration

		/// <summary>
		/// Obtain an enumerator that can yield all of the nodes of the tree, in order.
		/// This enumerator is lazy, and safe across tree mutations.  Construction of the
		/// enumerator is O(1).  Each invocation of MoveNext costs O(lg n), so a total
		/// enumeration of the tree costs O(n lg n).
		/// 
		/// (Note, however, that if the enumerator's CURRENT key is removed from the tree,
		/// the enumerator will stop, since its "next" key then no longer exists.  But you
		/// may safely add and remove nodes NEAR the current key without any ill effect:
		/// Changes that occur before the current key will be ignored, and changes that
		/// occur after the current key will be included in the enumeration.)
		/// </summary>
		/// <returns>An enumerator that can yield each of the nodes of the tree, in order.</returns>
		public IEnumerator<N> GetEnumerator()
			=> new RedBlackTreeEnumerator<K, V, N>(this);

		/// <summary>
		/// A non-generic wrapper around GetEnumerator().  See GetEnumerator for details.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

		#endregion
	}
}
