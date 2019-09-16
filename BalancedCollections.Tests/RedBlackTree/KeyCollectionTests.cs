using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BalancedCollections.RedBlackTree;

namespace BalancedCollections.Tests.RedBlackTree
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class KeyCollectionTests
	{
		[Test]
		public void CanConstructKeyCollections()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<int> keys = redBlackTree.Keys;
			Assert.That(keys, Is.Not.Null);
			Assert.That(keys.Count, Is.EqualTo(5));

			WeightedRedBlackTree<int, string> redBlackTree2 = new WeightedRedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			keys = redBlackTree2.Keys;
			Assert.That(keys, Is.Not.Null);
			Assert.That(keys.Count, Is.EqualTo(5));
		}

		[Test]
		public void CanEnumerateKeysNewSchool()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<int> keys = redBlackTree.Keys;

			IEnumerator<int> enumerator = ((IEnumerable<int>)keys).GetEnumerator();
			int[] expectedKeys = { 1, 2, 3, 4, 5 };

			int index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(enumerator.Current, Is.EqualTo(expectedKeys[index]));
				index++;
			}

			enumerator.Reset();

			index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(enumerator.Current, Is.EqualTo(expectedKeys[index]));
				index++;
			}
		}

		[Test]
		public void CanEnumerateKeysOldSchool()
		{
			RedBlackTree<int, string> redBlackTree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ICollection<int> keys = redBlackTree.Keys;

			IEnumerator enumerator = ((IEnumerable)keys).GetEnumerator();
			int[] expectedKeys = { 1, 2, 3, 4, 5 };

			int index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That((int)(enumerator.Current), Is.EqualTo(expectedKeys[index]));
				index++;
			}

			enumerator.Reset();

			index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That((int)(enumerator.Current), Is.EqualTo(expectedKeys[index]));
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

			ICollection<int> keys = redBlackTree.Keys;

			keys.Add(6);

			List<KeyValuePair<int, string>> pairs = redBlackTree.Select(n => n.ToKeyValuePair()).ToList();
			CollectionAssert.AreEqual(pairs,
				new[] {
					new KeyValuePair<int, string>(1, "1"),
					new KeyValuePair<int, string>(2, "2"),
					new KeyValuePair<int, string>(3, "3"),
					new KeyValuePair<int, string>(4, "4"),
					new KeyValuePair<int, string>(5, "5"),
					new KeyValuePair<int, string>(6, null),
				});

			keys.Remove(3);

			pairs = redBlackTree.Select(n => n.ToKeyValuePair()).ToList();
			CollectionAssert.AreEqual(pairs,
				new[] {
					new KeyValuePair<int, string>(1, "1"),
					new KeyValuePair<int, string>(2, "2"),
					new KeyValuePair<int, string>(4, "4"),
					new KeyValuePair<int, string>(5, "5"),
					new KeyValuePair<int, string>(6, null),
				});

			keys.Clear();

			pairs = redBlackTree.Select(n => n.ToKeyValuePair()).ToList();
			CollectionAssert.AreEqual(pairs, new KeyValuePair<int, string>[0]);
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

			ICollection<int> keys = redBlackTree.Keys;
			int[] keyArray = keys.ToArray();
			CollectionAssert.AreEqual(keyArray, new[] { 1, 2, 3, 4, 5 });
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

			ICollection<int> keys = redBlackTree.Keys;
			Assert.That(keys.Contains(3), Is.True);
			Assert.That(keys.Contains(6), Is.False);
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

			ICollection<int> keys = redBlackTree.Keys;
			Assert.That(keys.IsReadOnly, Is.False);

			ReadOnlyRedBlackTree<int, string> readOnlyTree = new ReadOnlyRedBlackTree<int, string>(redBlackTree);

			keys = readOnlyTree.Keys;
			Assert.That(keys.IsReadOnly, Is.True);
		}
	}
}
