using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BalancedCollections.Base;

namespace BalancedCollections.Shared
{
	/// <summary>
	/// This efficiently enumerates the values of the given red-black tree, non-recursively.
	/// If the tree changes during enumeration, this will resume enumeration correctly
	/// starting from its previously-enumerated node (i.e., if the node is still in the tree,
	/// this will pick up at the next node; if the node has been removed, this will stop).
	/// </summary>
	internal class RedBlackTreeValueEnumerator<K, V, N> : IEnumerator<V>
		where N : RedBlackTreeNodeBase<K, V, N>
	{
		private readonly IRedBlackTreeBase<K, V, N> _redBlackTree;
		private readonly K _maximum;
		private readonly bool _hasMaximum;
		private N _currentNode, _next;
		private bool _isFirst;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public RedBlackTreeValueEnumerator(IRedBlackTreeBase<K, V, N> redBlackTree)
		{
			_redBlackTree = redBlackTree;
			_isFirst = true;

			if (redBlackTree is RedBlackTreeSlice<K, V, N> slice)
			{
				_maximum = slice.Maximum;
				_hasMaximum = true;
			}
		}

		public V Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get =>
				!_isFirst ? _currentNode.Value
					: throw new InvalidOperationException("RedBlackTreeValueEnumerator has not started yet.");
		}

		object IEnumerator.Current
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Current;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool MoveNext()
		{
			if (_hasMaximum)
			{
				if (_isFirst)
				{
					_currentNode = _redBlackTree.MinimumNode;
					_isFirst = false;
				}
				else
				{
					_currentNode = _next;
				}
				_next = _currentNode?.Next();
				return _currentNode != null && _redBlackTree.Compare(_currentNode.Key, _maximum) <= 0;
			}
			else
			{
				if (_isFirst)
				{
					_currentNode = _redBlackTree.MinimumNode;
					_isFirst = false;
				}
				else
				{
					_currentNode = _next;
				}
				_next = _currentNode?.Next();
				return _currentNode != null;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			_currentNode = null;
			_isFirst = true;
		}
	}
}