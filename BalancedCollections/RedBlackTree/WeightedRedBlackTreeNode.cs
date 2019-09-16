using System.Runtime.CompilerServices;
using BalancedCollections.Base;

namespace BalancedCollections.RedBlackTree
{
	/// <summary>
	/// A red-black tree node that also includes the weight of its subtree.
	/// </summary>
	public class WeightedRedBlackTreeNode<K, V> : RedBlackTreeNodeBase<K, V, WeightedRedBlackTreeNode<K, V>>
	{
		/// <summary>
		/// The weight of each node is the sums of the weights of each of its
		/// children, plus one.  (Therefore, the weight of any node is an exact
		/// count of the number of nodes in its subtree, including itself; and
		/// the weight of the root is the count of all nodes in the tree.)
		/// </summary>
		public int Weight { get; internal set; }

		/// <summary>
		/// Construct a new node with the given key and value.
		/// </summary>
		/// <param name="key">The key to permanently assign to the new node.</param>
		/// <param name="value">The current value for the new node.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public WeightedRedBlackTreeNode(K key, V value)
			: base(key, value)
		{
			Weight = 1;
		}
	}
}
