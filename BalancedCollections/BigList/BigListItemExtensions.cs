using System.Runtime.CompilerServices;
using BalancedCollections.Shared;

namespace BalancedCollections.BigList
{
	internal static class BigListItemExtensions
	{
		/// <summary>
		/// Determine if the given node is red (must not be null, and must have Color=Red).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRed<V>(this BigListItem<V> node)
			=> node != null && node.Color == RedBlackTreeNodeColor.Red;

		/// <summary>
		/// Determine if the given node is black (either null, or must have Color=Black).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBlack<V>(this BigListItem<V> node)
			=> node == null || node.Color == RedBlackTreeNodeColor.Black;

		/// <summary>
		/// Turn the given node red, if it's non-null.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void MakeNodeRed<V>(this BigListItem<V> node)
		{
			if (node != null)
				node.Color = RedBlackTreeNodeColor.Red;
		}

		/// <summary>
		/// Turn the given node black, if it's non-null.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void MakeNodeBlack<V>(this BigListItem<V> node)
		{
			if (node != null)
				node.Color = RedBlackTreeNodeColor.Black;
		}
	}
}
