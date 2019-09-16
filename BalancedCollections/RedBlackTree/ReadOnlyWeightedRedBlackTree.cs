using BalancedCollections.Shared;

namespace BalancedCollections.RedBlackTree
{
	/// <summary>
	/// This wrapper class ensures its consumers treat the red-black tree's shape and keys
	/// as read-only, just like a ReadOnlyDictionary{K,V} does for Dictionary{K,V}.  Consumers
	/// can still mutate the values if those values are themselves not immutable.
	/// </summary>
	public class ReadOnlyWeightedRedBlackTree<K, V> : ReadOnlyRedBlackTreeBase<K, V, WeightedRedBlackTreeNode<K, V>>
	{
		public WeightedRedBlackTreeNode<K, V> Root => ((WeightedRedBlackTree<K, V>)RedBlackTree).Root;

		public ReadOnlyWeightedRedBlackTree(WeightedRedBlackTree<K, V> tree)
			: base(tree)
		{
		}
	}
}
