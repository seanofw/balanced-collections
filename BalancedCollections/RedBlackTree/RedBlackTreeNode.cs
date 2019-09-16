using System.Runtime.CompilerServices;
using BalancedCollections.Base;

namespace BalancedCollections.RedBlackTree
{
	/// <summary>
	/// A single node of the red-black tree.
	/// </summary>
	/// <typeparam name="K">The type of the keys in the tree.</typeparam>
	/// <typeparam name="V">The type of the values in the tree.</typeparam>
	public class RedBlackTreeNode<K, V> : RedBlackTreeNodeBase<K, V, RedBlackTreeNode<K, V>>
	{
		/// <summary>
		/// Construct a new node with the given key and value.
		/// </summary>
		/// <param name="key">The key to permanently assign to the new node.</param>
		/// <param name="value">The current value for the new node.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RedBlackTreeNode(K key, V value)
			: base(key, value)
		{
		}
	}
}
