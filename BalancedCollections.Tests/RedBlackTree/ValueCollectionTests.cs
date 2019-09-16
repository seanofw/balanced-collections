using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BalancedCollections.RedBlackTree;

namespace BalancedCollections.Tests.RedBlackTree
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ValueCollectionTests
	{
		[Test]
		public void CanConstructValueCollections()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<string> values = redBlackTree.Values;
			Assert.That(values, Is.Not.Null);
			Assert.That(values.Count, Is.EqualTo(5));

			WeightedRedBlackTree<int, string> redBlackTree2 = new WeightedRedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			values = redBlackTree2.Values;
			Assert.That(values, Is.Not.Null);
			Assert.That(values.Count, Is.EqualTo(5));
		}

		[Test]
		public void CanEnumerateValuesNewSchool()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<string> values = redBlackTree.Values;

			IEnumerator<string> enumerator = ((IEnumerable<string>)values).GetEnumerator();
			string[] expectedValues = { "1", "2", "3", "4", "5" };

			int index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(enumerator.Current, Is.EqualTo(expectedValues[index]));
				index++;
			}

			enumerator.Reset();

			index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(enumerator.Current, Is.EqualTo(expectedValues[index]));
				index++;
			}
		}

		[Test]
		public void CanEnumerateValuesOldSchool()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<string> values = redBlackTree.Values;

			IEnumerator enumerator = ((IEnumerable)values).GetEnumerator();
			string[] expectedValues = { "1", "2", "3", "4", "5" };

			int index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That((string)(enumerator.Current), Is.EqualTo(expectedValues[index]));
				index++;
			}

			enumerator.Reset();

			index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That((string)(enumerator.Current), Is.EqualTo(expectedValues[index]));
				index++;
			}
		}

		[Test]
		public void CannotModifyTheUnderlyingCollection()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<string> values = redBlackTree.Values;

			Assert.Throws<ImmutableException>(() => values.Add("6"));
			Assert.Throws<ImmutableException>(() => values.Remove("3"));
			Assert.Throws<ImmutableException>(() => values.Clear());
		}

		[Test]
		public void CanCopyToAnArray()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<string> values = redBlackTree.Values;
			string[] valueArray = values.ToArray();
			CollectionAssert.AreEqual(valueArray, new[] { "1", "2", "3", "4", "5" });
		}

		[Test]
		public void CanTestForContainment()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<string> values = redBlackTree.Values;
			Assert.That(values.Contains("3"), Is.True);
			Assert.That(values.Contains("6"), Is.False);
		}

		[Test]
		public void CanInvokeContainsWithNull()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
				{ 6, null },
			};

			ICollection<string> values = redBlackTree.Values;
			Assert.That(values.Contains(null), Is.True);
			Assert.That(values.Contains("6"), Is.False);
		}

		[Test]
		public void ValueCollectionsShouldAlwaysBeReadOnly()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<string> values = redBlackTree.Values;
			Assert.That(values.IsReadOnly, Is.True);

			ReadOnlyRedBlackTree<int, string> readOnlyTree = new ReadOnlyRedBlackTree<int, string>(redBlackTree);

			values = readOnlyTree.Values;
			Assert.That(values.IsReadOnly, Is.True);
		}
	}
}
