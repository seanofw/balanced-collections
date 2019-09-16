using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BalancedCollections.RedBlackTree;

namespace BalancedCollections.Tests.RedBlackTree
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class PairCollectionTests
	{
		[Test]
		public void CanConstructPairCollections()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<KeyValuePair<int, string>> pairs = redBlackTree.KeyValuePairs;
			Assert.That(pairs, Is.Not.Null);
			Assert.That(pairs.Count, Is.EqualTo(5));

			WeightedRedBlackTree<int, string> redBlackTree2 = new WeightedRedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			pairs = redBlackTree2.KeyValuePairs;
			Assert.That(pairs, Is.Not.Null);
			Assert.That(pairs.Count, Is.EqualTo(5));
		}

		[Test]
		public void CanEnumeratePairsNewSchool()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<KeyValuePair<int, string>> pairs = redBlackTree.KeyValuePairs;

			IEnumerator<KeyValuePair<int, string>> enumerator = ((IEnumerable<KeyValuePair<int, string>>)pairs).GetEnumerator();
			KeyValuePair<int, string>[] expectedPairs = {
				new KeyValuePair<int, string>(1, "1"),
				new KeyValuePair<int, string>(2, "2"),
				new KeyValuePair<int, string>(3, "3"),
				new KeyValuePair<int, string>(4, "4"),
				new KeyValuePair<int, string>(5, "5"),
			};

			int index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(enumerator.Current, Is.EqualTo(expectedPairs[index]));
				index++;
			}

			enumerator.Reset();

			index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(enumerator.Current, Is.EqualTo(expectedPairs[index]));
				index++;
			}
		}

		[Test]
		public void CanEnumeratePairsOldSchool()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<KeyValuePair<int, string>> pairs = redBlackTree.KeyValuePairs;

			IEnumerator enumerator = ((IEnumerable)pairs).GetEnumerator();
			KeyValuePair<int, string>[] expectedPairs = {
				new KeyValuePair<int, string>(1, "1"),
				new KeyValuePair<int, string>(2, "2"),
				new KeyValuePair<int, string>(3, "3"),
				new KeyValuePair<int, string>(4, "4"),
				new KeyValuePair<int, string>(5, "5"),
			};

			int index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That((KeyValuePair<int, string>)(enumerator.Current), Is.EqualTo(expectedPairs[index]));
				index++;
			}

			enumerator.Reset();

			index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That((KeyValuePair<int, string>)(enumerator.Current), Is.EqualTo(expectedPairs[index]));
				index++;
			}
		}

		[Test]
		public void CanModifyTheUnderlyingCollection()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<KeyValuePair<int, string>> pairs = redBlackTree.KeyValuePairs;

			pairs.Add(new KeyValuePair<int, string>(6, "6"));
			pairs.Add(new KeyValuePair<int, string>(7, null));

			List<KeyValuePair<int, string>> newPairs = redBlackTree.Select(n => n.ToKeyValuePair()).ToList();
			CollectionAssert.AreEqual(newPairs,
				new[] {
					new KeyValuePair<int, string>(1, "1"),
					new KeyValuePair<int, string>(2, "2"),
					new KeyValuePair<int, string>(3, "3"),
					new KeyValuePair<int, string>(4, "4"),
					new KeyValuePair<int, string>(5, "5"),
					new KeyValuePair<int, string>(6, "6"),
					new KeyValuePair<int, string>(7, null),
				});

			Assert.That(pairs.Remove(new KeyValuePair<int, string>(3, "3")), Is.True);
			Assert.That(pairs.Remove(new KeyValuePair<int, string>(6, null)), Is.False);
			Assert.That(pairs.Remove(new KeyValuePair<int, string>(7, "8")), Is.False);
			Assert.That(pairs.Remove(new KeyValuePair<int, string>(8, "3")), Is.False);

			newPairs = redBlackTree.Select(n => n.ToKeyValuePair()).ToList();
			CollectionAssert.AreEqual(newPairs,
				new[] {
					new KeyValuePair<int, string>(1, "1"),
					new KeyValuePair<int, string>(2, "2"),
					new KeyValuePair<int, string>(4, "4"),
					new KeyValuePair<int, string>(5, "5"),
					new KeyValuePair<int, string>(6, "6"),
					new KeyValuePair<int, string>(7, null),
				});

			Assert.That(pairs.Remove(new KeyValuePair<int, string>(7, null)), Is.True);

			newPairs = redBlackTree.Select(n => n.ToKeyValuePair()).ToList();
			CollectionAssert.AreEqual(newPairs,
				new[] {
					new KeyValuePair<int, string>(1, "1"),
					new KeyValuePair<int, string>(2, "2"),
					new KeyValuePair<int, string>(4, "4"),
					new KeyValuePair<int, string>(5, "5"),
					new KeyValuePair<int, string>(6, "6"),
				});

			pairs.Clear();

			newPairs = redBlackTree.Select(n => n.ToKeyValuePair()).ToList();
			CollectionAssert.AreEqual(newPairs, new KeyValuePair<int, string>[0]);
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

			ICollection<KeyValuePair<int, string>> pairs = redBlackTree.KeyValuePairs;
			KeyValuePair<int, string>[] pairArray = pairs.ToArray();
			CollectionAssert.AreEqual(pairArray,
				new[] {
					new KeyValuePair<int, string>(1, "1"),
					new KeyValuePair<int, string>(2, "2"),
					new KeyValuePair<int, string>(3, "3"),
					new KeyValuePair<int, string>(4, "4"),
					new KeyValuePair<int, string>(5, "5"),
				});
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
				{ 7, null },
			};

			ICollection<KeyValuePair<int, string>> pairs = redBlackTree.KeyValuePairs;

			Assert.That(pairs.Contains(new KeyValuePair<int, string>(3, "3")), Is.True);
			Assert.That(pairs.Contains(new KeyValuePair<int, string>(3, "6")), Is.False);
			Assert.That(pairs.Contains(new KeyValuePair<int, string>(6, "3")), Is.False);
			Assert.That(pairs.Contains(new KeyValuePair<int, string>(6, "6")), Is.False);

			Assert.That(pairs.Contains(new KeyValuePair<int, string>(5, null)), Is.False);
			Assert.That(pairs.Contains(new KeyValuePair<int, string>(7, null)), Is.True);
		}

		[Test]
		public void ReadOnlyShouldMirrorTheUnderlyingCollection()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<KeyValuePair<int, string>> pairs = redBlackTree.KeyValuePairs;
			Assert.That(pairs.IsReadOnly, Is.False);

			ReadOnlyRedBlackTree<int, string> readOnlyTree = new ReadOnlyRedBlackTree<int, string>(redBlackTree);

			pairs = readOnlyTree.KeyValuePairs;
			Assert.That(pairs.IsReadOnly, Is.True);
		}
	}
}
