using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BalancedCollections.Shared;

namespace BalancedCollections.Base
{
	public class RedBlackTreeNodeBase<K, V, N>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		#region Key and value properties

		/// <summary>
		/// The key for this node.  Keys must be unique within a tree and cannot
		/// be changed after their nodes are created.  (To change a key for a node,
		/// you must remove the node and insert a new one with the same value.)
		/// </summary>
		public K Key { get; }

		/// <summary>
		/// The value for this node.
		/// </summary>
		public V Value { get; set; }

		#endregion

		#region Tree properties

		/// <summary>
		/// The current color for this node.
		/// </summary>
		public RedBlackTreeNodeColor Color { get; internal set; }

		/// <summary>
		/// The left child of this node in the tree.  Null if there is no left subtree.
		/// </summary>
		public N Left { get; internal set; }

		/// <summary>
		/// The right child of this node in the tree.  Null if there is no right subtree.
		/// </summary>
		public N Right { get; internal set; }

		/// <summary>
		/// The parent of this node in the tree.  Null if this node is the root (or is detached).
		/// </summary>
		public N Parent { get; internal set; }

		#endregion

		#region Local searches

		/// <summary>
		/// The minimum subnode of the subtree rooted by this node.  O(lg n).
		/// This cannot be null.
		/// </summary>
		public N MinimumSubnode()
		{
			N node = (N)this;
			while (node.Left != null)
				node = node.Left;
			return node;
		}

		/// <summary>
		/// The maximum subnode of the subtree rooted by this node.  O(lg n).
		/// This cannot be null.
		/// </summary>
		public N MaximumSubnode()
		{
			N node = (N)this;
			while (node.Right != null)
				node = node.Right;
			return node;
		}

		/// <summary>
		/// The successor of this node, or null if this node has no successor.  O(lg n).
		/// </summary>
		public N Next()
		{
			if (Right != null)
				return Right.MinimumSubnode();
			N parent = Parent, node = (N)this;
			while (parent != null && node == parent.Right)
			{
				node = parent;
				parent = node.Parent;
			}
			return parent;
		}

		/// <summary>
		/// The predecessor of this node, or null if this node has no predecessor.  O(lg n).
		/// </summary>
		public N Previous()
		{
			if (Left != null)
				return Left.MaximumSubnode();
			N parent = Parent, node = (N)this;
			while (parent != null && node == parent.Left)
			{
				node = parent;
				parent = node.Parent;
			}
			return parent;
		}

		#endregion

		#region Construction / Type conversion

		/// <summary>
		/// Construct a new node with the given key and value.
		/// </summary>
		/// <param name="key">The key to permanently assign to the new node.</param>
		/// <param name="value">The current value for the new node.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected RedBlackTreeNodeBase(K key, V value)
		{
			Key = key;
			Value = value;
		}

		/// <summary>
		/// Make it easy to turn a node into a key/value pair.
		/// </summary>
		public KeyValuePair<K, V> ToKeyValuePair()
			=> new KeyValuePair<K, V>(Key, Value);

		#endregion

		#region ToString

		/// <summary>
		/// Convert this node to a convenient string form (for debugging).
		/// </summary>
		public override string ToString()
			=> $"\"{Key}\" => \"{Value}\"";

		#endregion
	}
}
