using BalancedCollections.Shared;

namespace BalancedCollections.BigList
{
	public class BigListItem<V>
	{
		#region Properties

		public V Value { get; set; }

		internal int Weight { get; set; }
		internal RedBlackTreeNodeColor Color { get; set; }
		internal BigListItem<V> Parent { get; set; }
		internal BigListItem<V> Left { get; set; }
		internal BigListItem<V> Right { get; set; }

		#endregion

		#region Local searches

		/// <summary>
		/// Retrieve the zero-based index of this BigListItem relative to those around it.
		/// </summary>
		public int Index
		{
			get
			{
				int index = Left?.Weight ?? 0;
				BigListItem<V> parent;
				for (BigListItem<V> node = this; node != null; node = parent)
				{
					parent = node.Parent;
					if (parent != null && node == parent.Right)
						index += (parent.Left?.Weight ?? 0) + 1;
				}
				return index;
			}
		}

		/// <summary>
		/// The minimum subnode of the subtree rooted by this node.  O(lg n).
		/// This cannot be null.
		/// </summary>
		internal BigListItem<V> MinimumSubnode()
		{
			BigListItem<V> node = this;
			while (node.Left != null)
				node = node.Left;
			return node;
		}

		/// <summary>
		/// The maximum subnode of the subtree rooted by this node.  O(lg n).
		/// This cannot be null.
		/// </summary>
		internal BigListItem<V> MaximumSubnode()
		{
			BigListItem<V> node = this;
			while (node.Right != null)
				node = node.Right;
			return node;
		}

		/// <summary>
		/// The successor of this node, or null if this node has no successor.  O(lg n).
		/// </summary>
		public BigListItem<V> Next
		{
			get
			{
				if (Right != null)
					return Right.MinimumSubnode();
				BigListItem<V> parent = Parent, node = this;
				while (parent != null && node == parent.Right)
				{
					node = parent;
					parent = node.Parent;
				}
				return parent;
			}
		}

		/// <summary>
		/// The predecessor of this node, or null if this node has no predecessor.  O(lg n).
		/// </summary>
		public BigListItem<V> Previous
		{
			get
			{
				if (Left != null)
					return Left.MaximumSubnode();
				BigListItem<V> parent = Parent, node = this;
				while (parent != null && node == parent.Left)
				{
					node = parent;
					parent = node.Parent;
				}
				return parent;
			}
		}

		#endregion
	}
}
