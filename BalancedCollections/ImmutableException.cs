using System;

namespace BalancedCollections
{
	public class ImmutableException : Exception
	{
		private const string ImmutableMessage = "This collection is immutable and cannot be modified.";

		internal ImmutableException()
			: base(ImmutableMessage)
		{
		}
	}
}
