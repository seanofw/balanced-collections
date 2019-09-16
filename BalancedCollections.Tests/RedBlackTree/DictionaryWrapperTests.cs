using System.Collections.Generic;
using BalancedCollections.RedBlackTree;
using NUnit.Framework;

namespace BalancedCollections.Tests.RedBlackTree
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DictionaryWrapperTests
	{
		#region RedBlackTree

		[Test]
		public void CanReadThroughAsDictionary()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			IDictionary<int, string> dictionary = tree.AsDictionary();

			Assert.That(dictionary[2], Is.EqualTo("2"));
			Assert.That(dictionary.Count, Is.EqualTo(5));
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, "2")), Is.True);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, "6")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(6, "2")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(6, "6")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, null)), Is.False);
			Assert.That(dictionary.ContainsKey(2), Is.True);
			Assert.That(dictionary.ContainsKey(6), Is.False);
			Assert.That(dictionary.IsReadOnly, Is.False);

			Assert.That(dictionary.TryGetValue(2, out string value), Is.True);
			Assert.That(value, Is.EqualTo("2"));

			Assert.That(dictionary.TryGetValue(6, out value), Is.False);
		}

		[Test]
		public void CanReadAsCollectionThroughAsDictionary()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			IDictionary<int, string> dictionary = tree.AsDictionary();

			KeyValuePair<int, string>[] array = new KeyValuePair<int, string>[5];
			dictionary.CopyTo(array, 0);
			CollectionAssert.AreEquivalent(array, new[] {
				new KeyValuePair<int, string>(1, "1"),
				new KeyValuePair<int, string>(2, "2"),
				new KeyValuePair<int, string>(3, "3"),
				new KeyValuePair<int, string>(4, "4"),
				new KeyValuePair<int, string>(5, "5"),
			});

			CollectionAssert.AreEquivalent(dictionary.Keys, new[] { 1, 2, 3, 4, 5 });
			CollectionAssert.AreEquivalent(dictionary.Values, new[] { "1", "2", "3", "4", "5" });

			int index = 0;
			array = new KeyValuePair<int, string>[5];
			foreach (KeyValuePair<int, string> pair in dictionary)
			{
				array[index++] = pair;
			}
			CollectionAssert.AreEquivalent(array, new[] {
				new KeyValuePair<int, string>(1, "1"),
				new KeyValuePair<int, string>(2, "2"),
				new KeyValuePair<int, string>(3, "3"),
				new KeyValuePair<int, string>(4, "4"),
				new KeyValuePair<int, string>(5, "5"),
			});
		}

		[Test]
		public void CanMutateThroughAsDictionary()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			IDictionary<int, string> dictionary = tree.AsDictionary();

			dictionary[3] = "foo";
			Assert.That(tree.Find(3).Value, Is.EqualTo("foo"));

			dictionary.Add(6, "6");
			Assert.That(tree.Find(6).Value, Is.EqualTo("6"));

			dictionary.Add(new KeyValuePair<int, string>(7, "7"));
			Assert.That(tree.Find(7).Value, Is.EqualTo("7"));

			Assert.That(dictionary.Remove(3), Is.True);
			Assert.That(tree.Find(3), Is.Null);

			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(8, "8")), Is.False);
			dictionary.Add(8, null);
			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(2, null)), Is.False);
			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(2, "2")), Is.True);
			Assert.That(tree.Find(2), Is.Null);
			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(8, "8")), Is.False);
			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(8, null)), Is.True);
			Assert.That(tree.Find(8), Is.Null);

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<int, string>(1, "1"),
				new KeyValuePair<int, string>(4, "4"),
				new KeyValuePair<int, string>(5, "5"),
				new KeyValuePair<int, string>(6, "6"),
				new KeyValuePair<int, string>(7, "7"),
			});

			dictionary.Clear();
			Assert.That(tree.Root, Is.Null);
			Assert.That(tree.Count, Is.Zero);
		}

		#endregion

		#region WeightedRedBlackTree

		[Test]
		public void CanReadThroughAsDictionaryWeighted()
		{
			WeightedRedBlackTree<int, string> tree = new WeightedRedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			IDictionary<int, string> dictionary = tree.AsDictionary();

			Assert.That(dictionary[2], Is.EqualTo("2"));
			Assert.That(dictionary.Count, Is.EqualTo(5));
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, "2")), Is.True);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, "6")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(6, "2")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(6, "6")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, null)), Is.False);
			Assert.That(dictionary.ContainsKey(2), Is.True);
			Assert.That(dictionary.ContainsKey(6), Is.False);
			Assert.That(dictionary.IsReadOnly, Is.False);

			Assert.That(dictionary.TryGetValue(2, out string value), Is.True);
			Assert.That(value, Is.EqualTo("2"));

			Assert.That(dictionary.TryGetValue(6, out value), Is.False);
		}

		[Test]
		public void CanReadAsCollectionThroughAsDictionaryWeighted()
		{
			WeightedRedBlackTree<int, string> tree = new WeightedRedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			IDictionary<int, string> dictionary = tree.AsDictionary();

			KeyValuePair<int, string>[] array = new KeyValuePair<int, string>[5];
			dictionary.CopyTo(array, 0);
			CollectionAssert.AreEquivalent(array, new[] {
				new KeyValuePair<int, string>(1, "1"),
				new KeyValuePair<int, string>(2, "2"),
				new KeyValuePair<int, string>(3, "3"),
				new KeyValuePair<int, string>(4, "4"),
				new KeyValuePair<int, string>(5, "5"),
			});

			CollectionAssert.AreEquivalent(dictionary.Keys, new[] { 1, 2, 3, 4, 5 });
			CollectionAssert.AreEquivalent(dictionary.Values, new[] { "1", "2", "3", "4", "5" });

			int index = 0;
			array = new KeyValuePair<int, string>[5];
			foreach (KeyValuePair<int, string> pair in dictionary)
			{
				array[index++] = pair;
			}
			CollectionAssert.AreEquivalent(array, new[] {
				new KeyValuePair<int, string>(1, "1"),
				new KeyValuePair<int, string>(2, "2"),
				new KeyValuePair<int, string>(3, "3"),
				new KeyValuePair<int, string>(4, "4"),
				new KeyValuePair<int, string>(5, "5"),
			});
		}

		[Test]
		public void CanMutateThroughAsDictionaryWeighted()
		{
			WeightedRedBlackTree<int, string> tree = new WeightedRedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			IDictionary<int, string> dictionary = tree.AsDictionary();

			dictionary[3] = "foo";
			Assert.That(tree.Find(3).Value, Is.EqualTo("foo"));

			dictionary.Add(6, "6");
			Assert.That(tree.Find(6).Value, Is.EqualTo("6"));

			dictionary.Add(new KeyValuePair<int, string>(7, "7"));
			Assert.That(tree.Find(7).Value, Is.EqualTo("7"));

			Assert.That(dictionary.Remove(3), Is.True);
			Assert.That(tree.Find(3), Is.Null);

			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(8, "8")), Is.False);
			dictionary.Add(8, null);
			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(2, null)), Is.False);
			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(2, "2")), Is.True);
			Assert.That(tree.Find(2), Is.Null);
			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(8, "8")), Is.False);
			Assert.That(dictionary.Remove(new KeyValuePair<int, string>(8, null)), Is.True);
			Assert.That(tree.Find(8), Is.Null);

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<int, string>(1, "1"),
				new KeyValuePair<int, string>(4, "4"),
				new KeyValuePair<int, string>(5, "5"),
				new KeyValuePair<int, string>(6, "6"),
				new KeyValuePair<int, string>(7, "7"),
			});

			dictionary.Clear();
			Assert.That(tree.Root, Is.Null);
			Assert.That(tree.Count, Is.Zero);
		}

		#endregion
	}
}
