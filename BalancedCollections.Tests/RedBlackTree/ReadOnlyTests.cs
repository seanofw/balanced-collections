using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BalancedCollections.RedBlackTree;
using System.Collections;

namespace BalancedCollections.Tests.RedBlackTree
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ReadOnlyTests
	{
		#region ReadOnlyRedBlackTree

		[Test]
		public void CanConstructAReadOnlyRedBlackTreeWrapper()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ReadOnlyRedBlackTree<int, string> wrapper = new ReadOnlyRedBlackTree<int, string>(tree);
			Assert.That(wrapper, Is.Not.Null);
			Assert.That(wrapper.Compare, Is.EqualTo(tree.Compare));
			Assert.That(wrapper.Root, Is.EqualTo(tree.Root));
			Assert.That(wrapper.Count, Is.EqualTo(5));
			Assert.That(wrapper.MinimumNode.Key, Is.EqualTo(1));
			Assert.That(wrapper.MaximumNode.Key, Is.EqualTo(5));
		}

		[Test]
		public void CanConstructAReadOnlyWeightedRedBlackTreeWrapper()
		{
			WeightedRedBlackTree<int, string> tree = new WeightedRedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ReadOnlyWeightedRedBlackTree<int, string> wrapper = new ReadOnlyWeightedRedBlackTree<int, string>(tree);
			Assert.That(wrapper, Is.Not.Null);
			Assert.That(wrapper.Compare, Is.EqualTo(tree.Compare));
			Assert.That(wrapper.Root, Is.EqualTo(tree.Root));
			Assert.That(wrapper.Count, Is.EqualTo(5));
			Assert.That(wrapper.MinimumNode.Key, Is.EqualTo(1));
			Assert.That(wrapper.MaximumNode.Key, Is.EqualTo(5));
		}

		[Test]
		public void CannotMutateTheTreeThroughAReadOnlyWrapper()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ReadOnlyRedBlackTree<int, string> wrapper = new ReadOnlyRedBlackTree<int, string>(tree);

			Assert.Throws<ImmutableException>(() => wrapper.Add(6, "6"));
			Assert.Throws<ImmutableException>(() => wrapper.Add(new RedBlackTreeNode<int, string>(6, "6")));
			Assert.Throws<ImmutableException>(() => wrapper.Clear());
			Assert.Throws<ImmutableException>(() => wrapper.DeleteNode(wrapper.MinimumNode));
			Assert.Throws<ImmutableException>(() => wrapper.FindOrInsert(6, "6", out RedBlackTreeNode<int, string> newNode));
			Assert.Throws<ImmutableException>(() => wrapper.Remove(5));
			Assert.Throws<ImmutableException>(() => wrapper.Remove(wrapper.MinimumNode));
			Assert.Throws<ImmutableException>(() => wrapper.RemoveRange(1, 3));
			Assert.Throws<ImmutableException>(() => wrapper[2] = "foo");
			Assert.Throws<ImmutableException>(() => wrapper[6] = "bar");
		}

		[Test]
		public void CanReadValuesAndNodesThroughAReadOnlyWrapper()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ReadOnlyRedBlackTree<int, string> wrapper = new ReadOnlyRedBlackTree<int, string>(tree);

			Assert.That(wrapper[2], Is.EqualTo("2"));
			Assert.That(wrapper.ContainsKey(2), Is.True);
			Assert.That(wrapper.Find(2), Is.EqualTo(tree.Find(2)));
			Assert.That(wrapper.Contains(tree.Find(2)), Is.True);

			Assert.That(wrapper.TryGetValue(2, out string value), Is.True);
			Assert.That(value, Is.EqualTo("2"));

			Assert.That(wrapper.FindOrInsert(2, null, out RedBlackTreeNode<int, string> node), Is.False);
			Assert.That(node, Is.EqualTo(tree.Find(2)));

			Assert.That(wrapper.ContainsKey(6), Is.False);
			Assert.That(wrapper.TryGetValue(6, out value), Is.False);
			Assert.That(wrapper.Contains(tree.Find(6)), Is.False);
		}

		[Test]
		public void CanReadTheWholeCollectionThroughAReadOnlyWrapper()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ReadOnlyRedBlackTree<int, string> wrapper = new ReadOnlyRedBlackTree<int, string>(tree);

			CollectionAssert.AreEquivalent(wrapper.ToDictionary(), new Dictionary<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			});

			RedBlackTreeNode<int, string>[] array = new RedBlackTreeNode<int, string>[5];
			wrapper.CopyTo(array, 0);
			CollectionAssert.AreEqual(array.Select(n => n.Key), new[] { 1, 2, 3, 4, 5 });

			array = new RedBlackTreeNode<int, string>[5];
			int index = 0;
			IEnumerator<RedBlackTreeNode<int, string>> enumerator = tree.GetEnumerator();
			while (enumerator.MoveNext())
			{
				array[index++] = enumerator.Current;
			}
			CollectionAssert.AreEqual(array.Select(n => n.Key), new[] { 1, 2, 3, 4, 5 });

			array = new RedBlackTreeNode<int, string>[5];
			index = 0;
			IEnumerator enumerator2 = ((IEnumerable)tree).GetEnumerator();
			while (enumerator2.MoveNext())
			{
				array[index++] = (RedBlackTreeNode<int, string>)enumerator2.Current;
			}
			CollectionAssert.AreEqual(array.Select(n => n.Key), new[] { 1, 2, 3, 4, 5 });
		}

		[Test]
		public void CanPerformRangeQueriesThroughAReadOnlyWrapper()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ReadOnlyRedBlackTree<int, string> wrapper = new ReadOnlyRedBlackTree<int, string>(tree);

			Assert.That(wrapper.GreatestBefore(3).Value, Is.EqualTo("2"));
			Assert.That(wrapper.GreatestBeforeOrEqualTo(3).Value, Is.EqualTo("3"));
			Assert.That(wrapper.GreatestBeforeOrEqualTo(6).Value, Is.EqualTo("5"));
			Assert.That(wrapper.GreatestInRange(2, 4).Value, Is.EqualTo("4"));

			Assert.That(wrapper.LeastAfter(3).Value, Is.EqualTo("4"));
			Assert.That(wrapper.LeastAfterOrEqualTo(3).Value, Is.EqualTo("3"));
			Assert.That(wrapper.LeastAfterOrEqualTo(0).Value, Is.EqualTo("1"));
			Assert.That(wrapper.LeastInRange(2, 4).Value, Is.EqualTo("2"));
		}

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

			ReadOnlyRedBlackTree<int, string> wrapper = new ReadOnlyRedBlackTree<int, string>(tree);
			IDictionary<int, string> dictionary = wrapper.AsDictionary();

			Assert.That(dictionary[2], Is.EqualTo("2"));
			Assert.That(dictionary.Count, Is.EqualTo(5));
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, "2")), Is.True);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, "6")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(6, "2")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(6, "6")), Is.False);
			Assert.That(dictionary.Contains(new KeyValuePair<int, string>(2, null)), Is.False);
			Assert.That(dictionary.ContainsKey(2), Is.True);
			Assert.That(dictionary.ContainsKey(6), Is.False);
			Assert.That(dictionary.IsReadOnly, Is.True);

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

			ReadOnlyRedBlackTree<int, string> wrapper = new ReadOnlyRedBlackTree<int, string>(tree);
			IDictionary<int, string> dictionary = wrapper.AsDictionary();

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
		public void CannotMutateThroughAsDictionary()
		{
			RedBlackTree<int, string> tree = new RedBlackTree<int, string>
			{
				{ 1, "1" },
				{ 2, "2" },
				{ 3, "3" },
				{ 4, "4" },
				{ 5, "5" },
			};

			ReadOnlyRedBlackTree<int, string> wrapper = new ReadOnlyRedBlackTree<int, string>(tree);
			IDictionary<int, string> dictionary = wrapper.AsDictionary();

			Assert.Throws<ImmutableException>(() => dictionary[3] = "foo");
			Assert.Throws<ImmutableException>(() => dictionary.Add(6, "6"));
			Assert.Throws<ImmutableException>(() => dictionary.Add(new KeyValuePair<int, string>(6, "6")));
			Assert.Throws<ImmutableException>(() => dictionary.Clear());
			Assert.Throws<ImmutableException>(() => dictionary.Remove(3));
			Assert.Throws<ImmutableException>(() => dictionary.Remove(new KeyValuePair<int, string>(3, "3")));
			Assert.Throws<ImmutableException>(() => dictionary.Keys.Add(6));
			Assert.Throws<ImmutableException>(() => dictionary.Values.Add("bar"));
		}

		#endregion
	}
}
