using System;
using System.Collections;
using System.Collections.Generic;

namespace BalancedCollections.BigList
{
	public class BigList<V> : IList<V>
	{
		private BigListItem<V> _root;

		public int Count => _root?.Weight ?? 0;

		public ICollection<BigListItem<V>> Items
			=> new BigListItemCollection<V>(this);

		#region First and Last

		public BigListItem<V> FirstItem => _root?.MinimumSubnode();
		public BigListItem<V> LastItem => _root?.MaximumSubnode();

		public V First()
		{
			BigListItem<V> item = FirstItem;
			if (item == null)
				throw new InvalidOperationException("BigList is empty.");
			return item.Value;
		}

		public V FirstOrDefault()
		{
			BigListItem<V> item = FirstItem;
			return item != null ? item.Value : default;
		}

		public V Last()
		{
			BigListItem<V> item = LastItem;
			if (item == null)
				throw new InvalidOperationException("BigList is empty.");
			return item.Value;
		}

		public V LastOrDefault()
		{
			BigListItem<V> item = FirstItem;
			return item != null ? item.Value : default;
		}

		#endregion

		public V this[int index]
		{
			get
			{
				BigListItem<V> item = ItemAt(index);
				if (item == null)
					throw new IndexOutOfRangeException(nameof(index));
				return item.Value;
			}
			set
			{
				BigListItem<V> item = ItemAt(index);
				if (item == null)
					throw new IndexOutOfRangeException(nameof(index));
				item.Value = value;
			}
		}

		public bool TryGetValue(int index, out V value)
		{
			BigListItem<V> item = ItemAt(index);

			if (item != null)
			{
				value = item.Value;
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}

		public BigListItem<V> Prepend(V value)
		{
			if (_root == null)
				return Attach(new BigListItem<V> { Value = value }, null, false);
			else
				return InsertBefore(value, _root.MinimumSubnode());
		}

		public BigListItem<V> Append(V value)
		{
			if (_root == null)
				return Attach(new BigListItem<V> { Value = value }, null, false);
			else
				return AppendAfter(value, _root.MaximumSubnode());
		}

		public BigListItem<V> InsertBefore(V value, BigListItem<V> other)
		{
			if (other == null)
				throw new ArgumentNullException(nameof(other));

			if (other.Left == null)
				return Attach(new BigListItem<V> { Value = value }, other, false);
			else
				return Attach(new BigListItem<V> { Value = value }, other.MinimumSubnode(), false);
		}

		public BigListItem<V> AppendAfter(V value, BigListItem<V> other)
		{
			if (other == null)
				throw new ArgumentNullException(nameof(other));

			if (other.Right == null)
				return Attach(new BigListItem<V> { Value = value }, other, true);
			else
				return Attach(new BigListItem<V> { Value = value }, other.MaximumSubnode(), true);
		}

		public bool DeleteItem(BigListItem<V> item)
		{
			if (!ContainsItem(item))
				return false;
			Detach(item);
			return true;
		}

		public bool ContainsItem(BigListItem<V> item)
		{
			if (item == null) return false;
			while (item.Parent != null)
				item = item.Parent;
			return item == _root;
		}

		public BigListItem<V> ItemAt(int index)
		{
			BigListItem<V> node = _root;
			while (node != null)
			{
				int leftWeight = node.Left?.Weight ?? 0;
				if (index == leftWeight)
					return node;
				else if (index < leftWeight)
					node = node.Left;
				else
				{
					node = node.Right;
					index -= leftWeight + 1;
				}
			}
			return node;
		}

		#region IList implementation

		public void AddRange(IEnumerable<V> values)
		{
			foreach (V value in values)
			{
				Add(value);
			}
		}

		public void Insert(int index, V value)
		{
			if (index < 0 || index >= Count)
				throw new IndexOutOfRangeException(nameof(index));
			if (index == Count)
			{
				Append(value);
				return;
			}

			BigListItem<V> otherItem = ItemAt(index);
			InsertBefore(value, otherItem);
		}

		public void InsertRange(int index, IEnumerable<V> values)
		{
			if (index < 0 || index >= Count)
				throw new IndexOutOfRangeException(nameof(index));
			if (index == Count)
			{
				AddRange(values);
				return;
			}

			BigListItem<V> otherItem = ItemAt(index);
			foreach (V value in values)
			{
				InsertBefore(value, otherItem);
			}
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Count - 1)
				throw new IndexOutOfRangeException(nameof(index));

			BigListItem<V> otherItem = ItemAt(index);
			Detach(otherItem);
		}

		public void RemoveRange(int start, int length)
		{
			if (start < 0 || start >= Count - 1)
				throw new IndexOutOfRangeException(nameof(start));
			if (length <= 0)
				return;

			BigListItem<V> item, next;

			for (item = ItemAt(start); item != null && length > 0; item = next, length--)
			{
				next = item.Next;
				Detach(item);
			}
		}

		public int IndexOf(V value)
		{
			int index = 0;
			for (BigListItem<V> item = _root?.MinimumSubnode(); item != null; item = item.Next, index++)
			{
				if (ReferenceEquals(value, null))
				{
					if (ReferenceEquals(item.Value, null))
						return index;
				}
				else
				{
					if (value.Equals(item.Value))
						return index;
				}
			}
			return -1;
		}

		#endregion

		#region ICollection implementation

		public bool IsReadOnly => false;

		public void Add(V value)
			=> Append(value);

		public void Clear()
			=> _root = null;

		public bool Contains(V value)
			=> IndexOf(value) >= 0;

		public bool Remove(V value)
		{
			for (BigListItem<V> item = _root?.MinimumSubnode(); item != null; item = item.Next)
			{
				if (ReferenceEquals(value, null))
				{
					if (ReferenceEquals(item.Value, null))
					{
						Detach(item);
						return true;
					}
				}
				else
				{
					if (value.Equals(item.Value))
					{
						Detach(item);
						return true;
					}
				}
			}
			return false;
		}

		public void CopyTo(V[] array, int arrayIndex)
		{
			for (BigListItem<V> item = _root?.MinimumSubnode(); item != null; item = item.Next)
			{
				array[arrayIndex++] = item.Value;
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<V> GetEnumerator()
			=> new BigListEnumerator<V>(this);

		IEnumerator IEnumerable.GetEnumerator()
			=> new BigListEnumerator<V>(this);

		#endregion

		#region Internal mechanics

		private BigListItem<V> Attach(BigListItem<V> node, BigListItem<V> parentNode, bool rightSide)
		{
			// Attach it to its new parent.
			if (parentNode == null)
				_root = node;
			else if (rightSide)
				parentNode.Right = node;
			else
				parentNode.Left = node;
			node.Parent = parentNode;

			// Increase the weights up the tree.
			if (parentNode != null)
			{
				for (BigListItem<V> temp = parentNode; temp != null; temp = temp.Parent)
				{
					temp.Weight++;
				}
			}

			// Rotate the tree to keep it balanced.
			InsertFixup(node);

			return node;
		}

		/// <summary>
		/// This corrects the tree for a newly-inserted node, rotating it as
		/// necessary to keep it balanced.  The algorithm is the same as RB-INSERT in [1],
		/// minus the first line that performs TREE-INSERT. O(lg n).
		/// </summary>
		/// <param name="node">The newly-inserted node.</param>
		private void InsertFixup(BigListItem<V> node)
		{
			node.MakeNodeRed();
			BigListItem<V> parent = node.Parent;

			while (node != _root && parent.IsRed())
			{
				if (parent == parent.Parent.Left)
				{
					BigListItem<V> uncle = parent.Parent.Right;
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
					BigListItem<V> uncle = parent.Parent.Left;
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

			_root.MakeNodeBlack();
		}

		/// <summary>
		/// Remove (detach) a node from the given BigList.
		///
		/// This uses standard red-black deletion, with auxiliary routine to
		/// reorder and recolor nodes if necessary.  However, unlike most RB delete
		/// routines, we do this (A) without sentinel nodes and (B) by only changing
		/// pointers and colors --- we never copy the data, so the pointers of the
		/// tree remain valid.  Still O(lg n).
		/// </summary>
		/// <param name="node"></param>
		private BigListItem<V> Detach(BigListItem<V> node)
		{
			if (node == null || _root == null) return node;

			BigListItem<V> next;   // Will take the place of "atom"
			BigListItem<V> child;  // First non-null child of "next", if any
			BigListItem<V> parent; // The supposed parent of "child"
			bool rightSide;                        // Which side of the parent "child" is on
			bool needFixup;                        // Do we need to call fixup()?

			if (node.Left == null || node.Right == null) next = node;
			else next = node.Next;

			child = (next.Left != null ? next.Left : next.Right);
			parent = next.Parent;

			if (child != null)
				child.Parent = parent;

			if (parent == null)
			{
				_root = child;
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

				for (BigListItem<V> temp = parent; temp != null; temp = temp.Parent)
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
					_root = next;
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

			return node;
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
		private void DetachFixup(BigListItem<V> child, BigListItem<V> parent, bool rightSide)
		{
			while (child != _root && child.IsBlack())
			{
				if (!rightSide)
				{
					BigListItem<V> other = parent.Right;
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
						child = _root;
					}
				}
				else
				{
					BigListItem<V> other = parent.Left;
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
						child = _root;
					}
				}
				parent = child.Parent;
				if (child != _root)
					rightSide = (child == parent.Right);
			}

			child.MakeNodeBlack();
		}

		/// <summary>
		/// Rotate the subtree left around the given node, as per LEFT-ROTATE algorithm in [1].  O(1).
		/// </summary>
		/// <param name="node">The node (x) to rotate around.</param>
		private void RotateLeft(BigListItem<V> node)
		{
			BigListItem<V> child = node.Right;

			node.Right = child.Left;

			if (child.Left != null)
				child.Left.Parent = node;

			child.Parent = node.Parent;

			if (node.Parent == null)
				_root = child;
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
		private void RotateRight(BigListItem<V> node)
		{
			BigListItem<V> child = node.Left;

			node.Left = child.Right;

			if (child.Right != null)
				child.Right.Parent = node;

			child.Parent = node.Parent;

			if (node.Parent == null)
				_root = child;
			else if (node.Parent.Right == node)
				node.Parent.Right = child;
			else
				node.Parent.Left = child;

			child.Right = node;
			node.Parent = child;
			child.Weight = node.Weight;
			node.Weight = (node.Left?.Weight ?? 0) + (node.Right?.Weight ?? 0) + 1;
		}

		#endregion

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
		public void Validate()
		{
			void Assert(bool result)
			{
				if (!result)
					throw new InvalidOperationException("Tree is invalid.");
			}

			// Test #2. Every leaf (null) is black.
			Assert(((BigListItem<V>)null).IsBlack());
			Assert(!((BigListItem<V>)null).IsRed());

			// Count up the black nodes in a simple path from root to leaf (null).
			int expectedBlackCount = 0;
			for (BigListItem<V> node = _root; node != null; node = node.Left)
			{
				if (node.IsBlack())
					expectedBlackCount++;
			}
			expectedBlackCount++;           // Count up one more for the null.

			// Recursively walk the entire tree, validating each node.
			void RecursivelyValidate(BigListItem<V> node, int currentBlackCount)
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
					Assert(node == _root);

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

				// Test #7. The weight of each node equals the weight of its Left child plus
				// the weight of its Right child plus one.
				int expectedWeight = (node.Left?.Weight ?? 0) + (node.Right?.Weight ?? 0) + 1;
				Assert(node.Weight == expectedWeight);

				// Recurse to test the rest of the tree.
				if (node != null)
				{
					RecursivelyValidate(node.Left, currentBlackCount);
					RecursivelyValidate(node.Right, currentBlackCount);
				}
			}

			// Recursively check the entire tree, starting at the root.
			RecursivelyValidate(_root, 0);
		}
	}
}
