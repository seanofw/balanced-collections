using BalancedCollections.Base;

namespace BalancedCollections.RedBlackTree
{
	public interface IRedBlackTree<K, V>
		: IRedBlackTreeBase<K, V, RedBlackTreeNode<K, V>>
	{
	}
}