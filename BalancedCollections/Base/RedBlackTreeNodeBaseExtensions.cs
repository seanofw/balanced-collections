using BalancedCollections.Shared;
using System.Runtime.CompilerServices;

namespace BalancedCollections.Base
{
	/// <summary>
	/// Helper extension methods that can safely access node properties, even
	/// if the node is null.
	/// </summary>
	internal static class RedBlackTreeNodeBaseExtensions
	{
		/// <summary>
		/// Determine if the given node is red (must not be null, and must have Color=Red).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRed<K, V, N>(this RedBlackTreeNodeBase<K, V, N> node)
			where N : RedBlackTreeNodeBase<K, V, N>
			=> node != null && node.Color == RedBlackTreeNodeColor.Red;

		/// <summary>
		/// Determine if the given node is black (either null, or must have Color=Black).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlack<K, V, N>(this RedBlackTreeNodeBase<K, V, N> node)
			where N : RedBlackTreeNodeBase<K, V, N>
			=> node == null || node.Color == RedBlackTreeNodeColor.Black;

		/// <summary>
		/// Turn the given node red, if it's non-null.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void MakeNodeRed<K, V, N>(this RedBlackTreeNodeBase<K, V, N> node)
			where N : RedBlackTreeNodeBase<K, V, N>
		{
			if (node != null)
				node.Color = RedBlackTreeNodeColor.Red;
		}

		/// <summary>
		/// Turn the given node black, if it's non-null.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void MakeNodeBlack<K, V, N>(this RedBlackTreeNodeBase<K, V, N> node)
			where N : RedBlackTreeNodeBase<K, V, N>
		{
			if (node != null)
				node.Color = RedBlackTreeNodeColor.Black;
		}
	}
}
