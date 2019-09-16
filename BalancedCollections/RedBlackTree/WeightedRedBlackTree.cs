using System;
using BalancedCollections.Base;

namespace BalancedCollections.RedBlackTree
{
	/// <summary>
	/// This collection class implements a traditional [1] red-black tree, but also includes
	/// a weight value for each node.  The nodes of the tree are exposed to the consumer of
	/// this collection for reading and traversal, but they may not be mutated without calling
	/// methods on this collection.
	/// </summary>
	/// <typeparam name="K">The type of the keys of this red-black tree.  Any type that
	/// is comparable may be used, either via a custom Compare() function or via IComparable{K}.
	/// Keys may legally be null.</typeparam>
	/// <typeparam name="V">The type of values of this red-black tree.  Any type may be used.
	/// Values may legally be null.</typeparam>
	/// <remarks>[1] Cormen, Thomas, with Charles Lieserson and Ronald Rivest. "Introduction to
	/// Algorithms," 1st edition. McGraw Hill, 1994. pp.263-280.</remarks>
	public class WeightedRedBlackTree<K, V> : RedBlackTreeBase<K, V, WeightedRedBlackTreeNode<K, V>>,
		IRedBlackTreeBase<K, V, WeightedRedBlackTreeNode<K, V>>
	{
		#region Construction

		/// <summary>
		/// Construct a new, empty red-black tree. O(1).
		/// </summary>
		/// <param name="compare">An optional comparison function for keys.  This may be
		/// null; if it is null, the red-black tree will use the key's implementation of
		/// IComparable{T}.</param>
		public WeightedRedBlackTree(Func<K, K, int> compare = null)
			: base(compare)
		{
		}

		#endregion

		#region Core mechanics

		/// <summary>
		/// Find the node whose key matches the given key. O(lg n).
		/// </summary>
		/// <param name="key">The key to search for.</param>
		/// <returns>The matching node, or null if no node matches.</returns>
		public override WeightedRedBlackTreeNode<K, V> Find(K key)
		{
			int cmp;
			for (WeightedRedBlackTreeNode<K, V> node = Root; node != null;
				node = cmp < 0 ? node.Left : node.Right)
			{
				if ((cmp = Compare(key, node.Key)) == 0)
					return node;
			}

			return null;
		}

		/// <summary>
		/// Search the tree for the given key.  If it does not exist, create a new node
		/// with the given key and value; but if it does exist, do nothing, and simply
		/// return the preexisting node. O(lg n).
		/// </summary>
		/// <param name="key">The key to search for or create.</param>
		/// <param name="value">The value to assign to that key, if and only if is node is
		/// newly-created.</param>
		/// <param name="resultingNode">The resulting node, either found or created.</param>
		/// <returns>True if the node was created, false if the node was found.  If the node
		/// was found, not created, the node will *not* be altered by this method.</returns>
		public override bool FindOrInsert(K key, V value, out WeightedRedBlackTreeNode<K, V> resultingNode)
		{
			int cmp = 0;
			WeightedRedBlackTreeNode<K, V> node = Root, parent = null;

			// Search for it, keeping track of any parents we pass through.
			while (node != null)
			{
				if ((cmp = Compare(key, node.Key)) == 0)
				{
					resultingNode = node;
					return false;
				}
				parent = node;
				node = cmp < 0 ? node.Left : node.Right;
			}

			// Didn't find it, so create it and attach it to its new parent.
			node = new WeightedRedBlackTreeNode<K, V>(key, value);
			if (parent == null)
				Root = node;
			else if (cmp > 0)
				parent.Right = node;
			else
				parent.Left = node;
			node.Parent = parent;

			// Increase the weights up the tree.
			if (parent != null)
			{
				for (WeightedRedBlackTreeNode<K, V> temp = parent; temp != null; temp = temp.Parent)
				{
					temp.Weight++;
				}
			}

			// Rotate the tree to keep it balanced.
			InsertFixup(node);

			// And return the new node.
			resultingNode = node;
			return true;
		}

		/// <summary>
		/// This corrects the tree for a newly-inserted node, rotating it as
		/// necessary to keep it balanced.  The algorithm is the same as RB-INSERT in [1],
		/// minus the first line that performs TREE-INSERT. O(lg n).
		/// </summary>
		/// <param name="node">The newly-inserted node.</param>
		protected void InsertFixup(WeightedRedBlackTreeNode<K, V> node)
		{
			node.MakeNodeRed();
			WeightedRedBlackTreeNode<K, V> parent = node.Parent;

			while (node != Root && parent.IsRed())
			{
				if (parent == parent.Parent.Left)
				{
					WeightedRedBlackTreeNode<K, V> uncle = parent.Parent.Right;
					if (uncle.IsRed())
					{
						parent.MakeNodeBlack();
						uncle.MakeNodeBlack();
						parent.Parent.MakeNodeRed();
						node = parent.Parent;
						parent = node.Parent;
					}
					else
					{
						if (node == parent.Right)
						{
							node = parent;
							RotateLeft(node);
							parent = node.Parent;
						}
						parent.MakeNodeBlack();
						parent.Parent.MakeNodeRed();
						RotateRight(parent.Parent);
						parent = node.Parent;
					}
				}
				else
				{
					WeightedRedBlackTreeNode<K, V> uncle = parent.Parent.Left;
					if (uncle.IsRed())
					{
						parent.MakeNodeBlack();
						uncle.MakeNodeBlack();
						parent.Parent.MakeNodeRed();
						node = parent.Parent;
						parent = node.Parent;
					}
					else
					{
						if (node == parent.Left)
						{
							node = parent;
							RotateRight(node);
							parent = node.Parent;
						}
						parent.MakeNodeBlack();
						parent.Parent.MakeNodeRed();
						RotateLeft(parent.Parent);
						parent = node.Parent;
					}
				}
			}

			Root.MakeNodeBlack();
		}

		/// <summary>
		/// Delete (detach) a node from the given tree.
		///
		/// This uses standard red-black deletion, with auxiliary routine to
		/// reorder and recolor nodes if necessary.  However, unlike most RB delete
		/// routines, we do this (A) without sentinel nodes and (B) by only changing
		/// pointers and colors --- we never copy the data, so the pointers of the
		/// tree remain valid.  Still O(lg n).
		/// </summary>
		/// <param name="node"></param>
		public override void DeleteNode(WeightedRedBlackTreeNode<K, V> node)
		{
			if (node == null || Root == null) return;

			WeightedRedBlackTreeNode<K, V> next;   // Will take the place of "atom"
			WeightedRedBlackTreeNode<K, V> child;  // First non-null child of "next", if any
			WeightedRedBlackTreeNode<K, V> parent; // The supposed parent of "child"
			bool rightSide;                        // Which side of the parent "child" is on
			bool needFixup;                        // Do we need to call fixup()?

			if (node.Left == null || node.Right == null) next = node;
			else next = node.Next();

			child = (next.Left != null ? next.Left : next.Right);
			parent = next.Parent;

			if (child != null)
				child.Parent = parent;

			if (parent == null)
			{
				Root = child;
				rightSide = false;
			}
			else
			{
				if (next == parent.Left)
				{
					parent.Left = child;
					rightSide = false;
				}
				else
				{
					parent.Right = child;
					rightSide = true;
				}

				for (WeightedRedBlackTreeNode<K, V> temp = parent; temp != null; temp = temp.Parent)
				{
					temp.Weight = (temp.Left?.Weight ?? 0) + (temp.Right?.Weight ?? 0) + 1;
				}
			}

			needFixup = next.IsBlack();

			if (node != next)
			{
				// Surreptitiously, node.contents <- next.contents by
				// just changing around a lot of pointers.

				if (parent == node)
					parent = next;      // Special case

				next.Parent = node.Parent;
				next.Left = node.Left;
				next.Right = node.Right;

				if (node.Parent == null)
					Root = next;
				else if (node == node.Parent.Left)
					node.Parent.Left = next;
				else
					node.Parent.Right = next;

				if (next.Left != null)
					next.Left.Parent = next;
				if (next.Right != null)
					next.Right.Parent = next;

				if (node.IsBlack())
					next.MakeNodeBlack();
				else
					next.MakeNodeRed();

				int temp = node.Weight;
				node.Weight = next.Weight;
				next.Weight = temp;
			}

			// Nulling the pointers on the node ensures that any active enumerations
			// or traversals that still reference this node will stop.
			node.Left = node.Right = node.Parent = null;

			if (needFixup)
				DetachFixup(child, parent, rightSide);
		}

		/// <summary>
		/// Perform rebalancing on the red-black tree for a deletion.  O(lg n).
		/// If we broke the tree by moving a black node, we need to reorder
		/// and/or recolor part of it.  This is not pretty, and is basically
		/// derived from the algorithm in Cormen/Leiserson/Rivest, just like
		/// every other red-black tree implementation.  We have, however, gone
		/// to the necessary work to drop out the required sentinel value in
		/// their version, so this is a little more flexible.
		/// </summary>
		/// <param name="child">The child that was removed.</param>
		/// <param name="parent">The parent it was removed from.</param>
		/// <param name="rightSide">Which side of the parent it was removed from.</param>
		private void DetachFixup(WeightedRedBlackTreeNode<K, V> child, WeightedRedBlackTreeNode<K, V> parent, bool rightSide)
		{
			while (child != Root && child.IsBlack())
			{
				if (!rightSide)
				{
					WeightedRedBlackTreeNode<K, V> other = parent.Right;
					if (other.IsRed())
					{
						other.MakeNodeBlack();
						parent.MakeNodeRed();
						RotateLeft(parent);
						other = parent.Right;
					}
					if (other == null
						|| (other.Left.IsBlack()
						&& other.Right.IsBlack()))
					{
						other.MakeNodeRed();
						child = parent;
					}
					else
					{
						if (other.Right.IsBlack())
						{
							other.Left.MakeNodeBlack();
							other.MakeNodeRed();
							RotateRight(other);
							other = parent.Right;
						}
						if (parent.IsRed()) other.MakeNodeRed();
						else other.MakeNodeBlack();
						parent.MakeNodeBlack();
						other.Right.MakeNodeBlack();
						RotateLeft(parent);
						child = Root;
					}
				}
				else
				{
					WeightedRedBlackTreeNode<K, V> other = parent.Left;
					if (other.IsRed())
					{
						other.MakeNodeBlack();
						parent.MakeNodeRed();
						RotateRight(parent);
						other = parent.Left;
					}
					if (other == null
						|| (other.Right.IsBlack()
						&& other.Left.IsBlack()))
					{
						other.MakeNodeRed();
						child = parent;
					}
					else
					{
						if (other.Left.IsBlack())
						{
							other.Right.MakeNodeBlack();
							other.MakeNodeRed();
							RotateLeft(other);
							other = parent.Left;
						}
						if (parent.IsRed()) other.MakeNodeRed();
						else other.MakeNodeBlack();
						parent.MakeNodeBlack();
						other.Left.MakeNodeBlack();
						RotateRight(parent);
						child = Root;
					}
				}
				parent = child.Parent;
				if (child != Root)
					rightSide = (child == parent.Right);
			}

			child.MakeNodeBlack();
		}

		/// <summary>
		/// Rotate the subtree left around the given node, as per LEFT-ROTATE algorithm in [1].  O(1).
		/// </summary>
		/// <param name="node">The node (x) to rotate around.</param>
		private void RotateLeft(WeightedRedBlackTreeNode<K, V> node)
		{
			WeightedRedBlackTreeNode<K, V> child = node.Right;

			node.Right = child.Left;

			if (child.Left != null)
				child.Left.Parent = node;

			child.Parent = node.Parent;

			if (node.Parent == null)
				Root = child;
			else if (node.Parent.Left == node)
				node.Parent.Left = child;
			else
				node.Parent.Right = child;

			child.Left = node;
			node.Parent = child;
			child.Weight = node.Weight;
			node.Weight = (node.Left?.Weight ?? 0) + (node.Right?.Weight ?? 0) + 1;
		}

		/// <summary>
		/// Rotate the subtree right around the given node, as per the mirror of the
		/// LEFT-ROTATE algorithm in [1].  O(1).
		/// </summary>
		/// <param name="node">The node (x) to rotate around.</param>
		private void RotateRight(WeightedRedBlackTreeNode<K, V> node)
		{
			WeightedRedBlackTreeNode<K, V> child = node.Left;

			node.Left = child.Right;

			if (child.Right != null)
				child.Right.Parent = node;

			child.Parent = node.Parent;

			if (node.Parent == null)
				Root = child;
			else if (node.Parent.Right == node)
				node.Parent.Right = child;
			else
				node.Parent.Left = child;

			child.Right = node;
			node.Parent = child;
			child.Weight = node.Weight;
			node.Weight = (node.Left?.Weight ?? 0) + (node.Right?.Weight ?? 0) + 1;
		}

		/// <summary>
		/// Validate the correctness of the red-black tree according to the four
		/// red-black properties [1]:
		/// 
		/// 1. Every node is either red or black.
		/// 2. Every leaf (null) is black.
		/// 3. If a node is red, then both its children are black.
		/// 4. Every simple path from a node to a descendant leaf contains the
		///    same number of black nodes.
		///
		/// In addition, we also test that:
		/// 
		/// 5. Only the Root node may have a null Parent pointer.
		/// 6. For all non-null Left and Right pointers, Left.Parent == parent,
		///     and Right.Parent == parent.
		/// 7. The weight of each node equals the weight of its Left child plus
		///     the weight of its Right child plus one.
		/// 
		/// The first three are trivial to test.  Properties 5 and 6 are also
		/// easy, and ensure that the tree is a valid binary search tree.  Property
		/// 4 is the only one that's interesting; to test it, we recursively walk
		/// the tree inorder after everything else has been checked, and compare
		/// the count of black nodes each time we reach 'null' against the count of
		/// black nodes from a strict walk of Left pointers.
		/// </summary>
		/// <throws cref="InvalidOperationException">Thrown if the tree is invalid
		/// or damaged.</throws>
		public override void Validate()
		{
			void Assert(bool result)
			{
				if (!result)
					throw new InvalidOperationException("Tree is invalid.");
			}

			// Test #2. Every leaf (null) is black.
			Assert(((WeightedRedBlackTreeNode<K, V>)null).IsBlack());
			Assert(!((WeightedRedBlackTreeNode<K, V>)null).IsRed());

			// Count up the black nodes in a simple path from root to leaf (null).
			int expectedBlackCount = 0;
			for (WeightedRedBlackTreeNode<K, V> node = Root; node != null; node = node.Left)
			{
				if (node.IsBlack())
					expectedBlackCount++;
			}
			expectedBlackCount++;           // Count up one more for the null.

			// Recursively walk the entire tree, validating each node.
			void RecursivelyValidate(WeightedRedBlackTreeNode<K, V> node, int currentBlackCount)
			{
				// Count up an additional black node in this path, if this node is black.
				if (node.IsBlack())
					currentBlackCount++;

				// Test #1. Every node is either red or black.
				Assert(node.IsRed() || node.IsBlack());

				// Test #3. If a node is red, then both its children are black.
				if (node.IsRed())
				{
					Assert(node.Left.IsBlack());
					Assert(node.Right.IsBlack());
				}

				// Test #5. Only the Root node may have a null Parent pointer.
				if (node != null && node.Parent == null)
					Assert(node == Root);

				// Test #6. For all non-null Left and Right pointers,
				//     Left.Parent == parent, and Right.Parent == parent.
				if (node != null && node.Left != null)
					Assert(node.Left.Parent == node);
				if (node != null && node.Right != null)
					Assert(node.Right.Parent == node);

				// Test #4. Every simple path from a node to a descendant leaf
				// contains the same number of black nodes.
				if (currentBlackCount == expectedBlackCount)
					Assert(node == null);

				if (node != null)
				{
					// Test #7. The weight of each node equals the weight of its Left child plus
					// the weight of its Right child plus one.
					int expectedWeight = (node.Left?.Weight ?? 0) + (node.Right?.Weight ?? 0) + 1;
					Assert(node.Weight == expectedWeight);
				}

				// Recurse to test the rest of the tree.
				if (node != null)
				{
					RecursivelyValidate(node.Left, currentBlackCount);
					RecursivelyValidate(node.Right, currentBlackCount);
				}
			}

			// Recursively check the entire tree, starting at the root.
			RecursivelyValidate(Root, 0);
		}

		#endregion

		#region IDictionary (almost) and ICollection implementation

		/// <summary>
		/// Get the count of nodes in this tree.  O(1).
		/// </summary>
		public override int Count => Root?.Weight ?? 0;

		/// <summary>
		/// Ask if this tree is read-only (or read-write).  O(1).
		/// </summary>
		public override bool IsReadOnly => false;

		/// <summary>
		/// Delete all nodes in the tree.  O(1).
		/// </summary>
		public override void Clear()
		{
			Root = null;
		}

		#endregion

		#region ToString

		/// <summary>
		/// Convert this to a string for easy debugging.
		/// </summary>
		public override string ToString()
			=> $"WeightedRedBlackTree<{typeof(K)},{typeof(V)}> of {Count} nodes ({MinimumKey} to {MaximumKey})";

		#endregion
	}
}
