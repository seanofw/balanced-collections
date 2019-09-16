using BalancedCollections.Shared;

namespace BalancedCollections.RedBlackTree
{
	/// <summary>
	/// This wrapper class ensures its consumers treat the red-black tree's shape and keys
	/// as read-only, just like a ReadOnlyDictionary{K,V} does for Dictionary{K,V}.  Consumers
	/// can still mutate the values if those values are themselves not immutable.
	/// </summary>
	public class ReadOnlyRedBlackTree<K, V> : ReadOnlyRedBlackTreeBase<K, V, RedBlackTreeNode<K, V>>
	{
		public RedBlackTreeNode<K, V> Root => ((RedBlackTree<K, V>)RedBlackTree).Root;

		public ReadOnlyRedBlackTree(RedBlackTree<K, V> tree)
			: base(tree)
		{
		}
	}
}
