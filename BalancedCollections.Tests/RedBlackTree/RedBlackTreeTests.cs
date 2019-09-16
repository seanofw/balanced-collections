using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BalancedCollections.RedBlackTree;
using System.Collections;

namespace BalancedCollections.Tests.RedBlackTree
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class RedBlackTreeTests
	{
		/// <summary>
		/// To make some tests run faster, we only perform them on Fibonacci(n) items,
		/// instead of on every item.  This answers whether 'n' is a Fibonacci number for
		/// any integer 'n'.
		/// </summary>
		private static bool IsFibonacci(int n)
		{
			bool IsPerfectSquare(int x)
			{
				int s = (int)Math.Sqrt(x);
				return s * s == x;
			}

			return IsPerfectSquare(5 * n * n + 4)
				|| IsPerfectSquare(5 * n * n - 4);
		}

		[Test]
		public void NewTreeShouldBeEmpty()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			Assert.That(tree.Root, Is.Null);
			Assert.That(tree.Count, Is.Zero);

			tree.Validate();
		}

		[Test]
		public void TreeShouldBeValidAfterManyRandomInsertions()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();

				Assert.That(tree.Count, Is.EqualTo(i));
				Assert.That(tree.TryGetValue(key, out int oldValue), Is.False);
				Assert.That(tree.ContainsKey(key), Is.False);

				tree.Add(key, i);

				if (IsFibonacci(i))
					tree.Validate();

				Assert.That(tree.ContainsKey(key), Is.True);
				Assert.That(tree[key], Is.EqualTo(i));
				Assert.That(tree.TryGetValue(key, out int newValue), Is.True);
				Assert.That(newValue, Is.EqualTo(i));
				Assert.That(tree.Count, Is.EqualTo(i + 1));
			}
		}

		[Test]
		public void TreeShouldBeValidAfterManyRandomDeletions()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			List<Tuple<int, int>> insertions = new List<Tuple<int, int>>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				insertions.Add(new Tuple<int, int>(key, i));

				Assert.That(tree.Count, Is.EqualTo(i));
				Assert.That(tree.TryGetValue(key, out int oldValue), Is.False);
				Assert.That(tree.ContainsKey(key), Is.False);

				tree.Add(key, i);

				if (IsFibonacci(i))
					tree.Validate();

				Assert.That(tree.ContainsKey(key), Is.True);
				Assert.That(tree[key], Is.EqualTo(i));
				Assert.That(tree.TryGetValue(key, out int newValue), Is.True);
				Assert.That(newValue, Is.EqualTo(i));
				Assert.That(tree.Count, Is.EqualTo(i + 1));
			}

			for (int i = 0; i < 500; i++)
			{
				int key = insertions[i].Item1;
				int value = insertions[i].Item2;

				Assert.That(tree.ContainsKey(key), Is.True);
				Assert.That(tree[key], Is.EqualTo(value));
				Assert.That(tree.TryGetValue(key, out int newValue), Is.True);
				Assert.That(newValue, Is.EqualTo(value));
				Assert.That(tree.Count, Is.EqualTo(500 - i));

				Assert.That(tree.Remove(key), Is.True);

				if (IsFibonacci(i))
					tree.Validate();

				Assert.That(tree.ContainsKey(key), Is.False);
				Assert.That(tree.TryGetValue(key, out int oldValue), Is.False);
				Assert.That(tree.Count, Is.EqualTo(500 - i - 1));
			}
		}

		[Test]
		public void ShouldBeAbleToFindMinimaAndMaxima()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			Assert.That(tree.MinimumKey, Is.EqualTo(default(int)));
			Assert.That(tree.MaximumKey, Is.EqualTo(default(int)));
			Assert.That(tree.MinimumNode, Is.Null);
			Assert.That(tree.MaximumNode, Is.Null);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				int expectedMinimum = keys.Min();
				int actualMinimum = tree.MinimumKey;
				Assert.That(expectedMinimum, Is.EqualTo(actualMinimum));

				RedBlackTreeNode<int, int> actualMinimumNode = tree.MinimumNode;
				Assert.That(expectedMinimum, Is.EqualTo(actualMinimumNode.Key));

				int expectedMaximum = keys.Max();
				int actualMaximum = tree.MaximumKey;
				Assert.That(expectedMaximum, Is.EqualTo(actualMaximum));

				RedBlackTreeNode<int, int> actualMaximumNode = tree.MaximumNode;
				Assert.That(expectedMaximum, Is.EqualTo(actualMaximumNode.Key));
			}
		}

		[Test]
		public void NextShouldBeAbleToWalkTheTreeForward()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				if (IsFibonacci(i))
				{
					List<RedBlackTreeNode<int, int>> actualNodes = new List<RedBlackTreeNode<int, int>>();
					for (RedBlackTreeNode<int, int> node = tree.MinimumNode; node != null; node = node.Next())
					{
						actualNodes.Add(node);
					}
					int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();
					int[] expectedKeys = keys.OrderBy(k => k).ToArray();

					CollectionAssert.AreEqual(actualKeys, expectedKeys);
				}
			}
		}

		[Test]
		public void PreviousShouldBeAbleToWalkTheTreeBackward()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				if (IsFibonacci(i))
				{
					List<RedBlackTreeNode<int, int>> actualNodes = new List<RedBlackTreeNode<int, int>>();
					for (RedBlackTreeNode<int, int> node = tree.MaximumNode; node != null; node = node.Previous())
					{
						actualNodes.Add(node);
					}
					int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();
					int[] expectedKeys = keys.OrderByDescending(k => k).ToArray();

					CollectionAssert.AreEqual(actualKeys, expectedKeys);
				}
			}
		}

		[Test]
		public void CannotRemoveKeysThatDontExist()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			Assert.That(tree.Remove(4), Is.False);

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<int, int>(1, 10),
				new KeyValuePair<int, int>(3, 20),
				new KeyValuePair<int, int>(5, 30),
				new KeyValuePair<int, int>(7, 40),
				new KeyValuePair<int, int>(9, 50),
			});

			Assert.That(tree.Remove(3), Is.True);

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<int, int>(1, 10),
				new KeyValuePair<int, int>(5, 30),
				new KeyValuePair<int, int>(7, 40),
				new KeyValuePair<int, int>(9, 50),
			});

			Assert.That(tree.Remove(1), Is.True);

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<int, int>(5, 30),
				new KeyValuePair<int, int>(7, 40),
				new KeyValuePair<int, int>(9, 50),
			});

			Assert.That(tree.Remove(7), Is.True);

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<int, int>(5, 30),
				new KeyValuePair<int, int>(9, 50),
			});

			Assert.That(tree.Remove(9), Is.True);

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<int, int>(5, 30),
			});

			RedBlackTreeNode<int, int> fiveNode = tree.Find(5);
			Assert.That(tree.Remove(5), Is.True);
			tree.DeleteNode(null);          // Make this is always a no-op.

			CollectionAssert.AreEqual(tree.KeyValuePairs, new KeyValuePair<int, int>[0]);

			Assert.That(tree.Remove(3), Is.False);

			Assert.That(tree.Count, Is.Zero);
			Assert.That(tree.Root, Is.Null);
			tree.DeleteNode(fiveNode);      // Make sure nothing bad happens with an empty tree.
		}

		[Test]
		public void ForeachShouldWalkTheTreeForward()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			{
				List<RedBlackTreeNode<int, int>> actualNodes = new List<RedBlackTreeNode<int, int>>();
				foreach (RedBlackTreeNode<int, int> node in tree)
				{
					actualNodes.Add(node);
				}
				int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();

				CollectionAssert.AreEqual(actualKeys, new int[0]);
			}

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				if (IsFibonacci(i))
				{
					List<RedBlackTreeNode<int, int>> actualNodes = new List<RedBlackTreeNode<int, int>>();
					foreach (RedBlackTreeNode<int, int> node in tree)
					{
						actualNodes.Add(node);
					}
					int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();
					int[] expectedKeys = keys.OrderBy(k => k).ToArray();

					CollectionAssert.AreEqual(actualKeys, expectedKeys);
				}
			}
		}

		[Test]
		public void CopyToShouldWalkTheTreeForward()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			{
				List<RedBlackTreeNode<int, int>> actualNodes = tree.ToList();
				int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();

				CollectionAssert.AreEqual(actualKeys, new int[0]);
			}

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				if (IsFibonacci(i))
				{
					List<RedBlackTreeNode<int, int>> actualNodes = tree.ToList();
					int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();
					int[] expectedKeys = keys.OrderBy(k => k).ToArray();

					CollectionAssert.AreEqual(actualKeys, expectedKeys);
				}
			}
		}

		[Test]
		public void CanCopyToSpecialCaseTrees()
		{
			{
				RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

				Assert.That(tree.Root, Is.Null);

				// CopyTo should safely bail if the root is null.
				RedBlackTreeNode<int, int>[] actualNodes = new RedBlackTreeNode<int, int>[0];
				tree.CopyTo(actualNodes, 0);
				int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();
				CollectionAssert.AreEqual(actualKeys, new int[0]);
			}
			{
				RedBlackTree<int, int> tree = new RedBlackTree<int, int>
				{
					{ 1, 10 },
				};

				// The above is a valid red-black tree with only one node.
				Assert.That(tree.Root.Key, Is.EqualTo(1));

				// CopyTo performs an inorder walk using Next(), starting at
				// Root's minimum subnode.
				List<RedBlackTreeNode<int, int>> actualNodes = tree.ToList();
				int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();
				CollectionAssert.AreEqual(actualKeys, new[] { 1 });
			}
			{
				RedBlackTree<int, int> tree = new RedBlackTree<int, int>
				{
					{ 2, 20 },
					{ 1, 10 },
				};

				// The above is a valid red-black tree in which the root is 2
				// and it has a left child of 1.  Let's check that to be sure...
				Assert.That(tree.Root.Key, Is.EqualTo(2));
				Assert.That(tree.Root.Left.Key, Is.EqualTo(1));

				// CopyTo performs an inorder walk using Next(), starting at
				// Root's minimum subnode.  In this case, we ensure that
				// Root is non-null and has a minimum subnode to start at.
				List<RedBlackTreeNode<int, int>> actualNodes = tree.ToList();
				int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();
				CollectionAssert.AreEqual(actualKeys, new[] { 1, 2 });
			}
			{
				RedBlackTree<int, int> tree = new RedBlackTree<int, int>
				{
					{ 1, 10 },
					{ 2, 20 },
				};

				// The above is a valid red-black tree in which the root is 1
				// and it has a right child of 2.  Let's check that to be sure...
				Assert.That(tree.Root.Key, Is.EqualTo(1));
				Assert.That(tree.Root.Right.Key, Is.EqualTo(2));

				// CopyTo performs an inorder walk using Next(), starting at
				// Root's minimum subnode.  In this case, we ensure that
				// Root is non-null but has no minimum subnode to start at.
				List<RedBlackTreeNode<int, int>> actualNodes = tree.ToList();
				int[] actualKeys = actualNodes.Select(n => n.Key).ToArray();
				CollectionAssert.AreEqual(actualKeys, new[] { 1, 2 });
			}
		}

		[Test]
		public void ToDictionaryShouldGenerateAnEquivalentDictionary()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			{
				Dictionary<int, int> dictionary = tree.ToDictionary();
				Assert.That(dictionary.Count, Is.Zero);
			}

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				if (IsFibonacci(i))
				{
					Dictionary<int, int> dictionary = tree.ToDictionary();

					List<KeyValuePair<int, int>> expectedPairs = tree.Select(n => n.ToKeyValuePair()).ToList();
					List<KeyValuePair<int, int>> actualPairs = dictionary.OrderBy(p => p.Key).ToList();

					CollectionAssert.AreEqual(actualPairs, expectedPairs);
				}
			}
		}

		[Test]
		public void CanUseTheIndexerToReplaceOrInsertValues()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			tree[5] = 35;
			tree[7] = 45;
			tree[9] = 55;
			tree[10] = 60;

			tree[int.MinValue] = -1;
			tree[0] = 12345;
			tree[int.MaxValue] = 999;

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<int, int>(int.MinValue, -1),
				new KeyValuePair<int, int>(0, 12345),
				new KeyValuePair<int, int>(1, 10),
				new KeyValuePair<int, int>(3, 20),
				new KeyValuePair<int, int>(5, 35),
				new KeyValuePair<int, int>(7, 45),
				new KeyValuePair<int, int>(9, 55),
				new KeyValuePair<int, int>(10, 60),
				new KeyValuePair<int, int>(int.MaxValue, 999),
			});
		}

		private class Foo
		{
			public readonly int Value;

			public Foo(int value)
			{
				Value = value;
			}

			public override bool Equals(object obj)
				=> (obj is Foo foo) && foo.Value == Value;
			public override int GetHashCode()
				=> Value;
		}

		[Test]
		public void CanUseAnExplicitComparisonFunction()
		{
			int CompareFoos(Foo a, Foo b)
			{
				return a.Value.CompareTo(b.Value);
			}

			RedBlackTree<Foo, int> tree = new RedBlackTree<Foo, int>(CompareFoos)
			{
				{ new Foo(1), 10 },
				{ new Foo(3), 20 },
				{ new Foo(5), 30 },
				{ new Foo(7), 40 },
				{ new Foo(9), 50 },
			};

			tree[new Foo(5)] = 35;
			tree[new Foo(7)] = 45;
			tree[new Foo(9)] = 55;
			tree[new Foo(10)] = 60;

			tree[new Foo(int.MinValue)] = -1;
			tree[new Foo(0)] = 12345;
			tree[new Foo(int.MaxValue)] = 999;

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<Foo, int>(new Foo(int.MinValue), -1),
				new KeyValuePair<Foo, int>(new Foo(0), 12345),
				new KeyValuePair<Foo, int>(new Foo(1), 10),
				new KeyValuePair<Foo, int>(new Foo(3), 20),
				new KeyValuePair<Foo, int>(new Foo(5), 35),
				new KeyValuePair<Foo, int>(new Foo(7), 45),
				new KeyValuePair<Foo, int>(new Foo(9), 55),
				new KeyValuePair<Foo, int>(new Foo(10), 60),
				new KeyValuePair<Foo, int>(new Foo(int.MaxValue), 999),
			});
		}

		private class Bar : IComparable
		{
			public readonly int Value;

			public Bar(int value)
			{
				Value = value;
			}

			public int CompareTo(object obj)
				=> (obj is Bar bar) ? Value.CompareTo(bar.Value) : +1;
			public override bool Equals(object obj)
				=> (obj is Bar bar) && bar.Value == Value;
			public override int GetHashCode()
				=> Value;
		}

		[Test]
		public void CanUseAnExplicitIComparableImplementation()
		{
			RedBlackTree<Bar, int> tree = new RedBlackTree<Bar, int>
			{
				{ new Bar(1), 10 },
				{ new Bar(3), 20 },
				{ new Bar(5), 30 },
				{ new Bar(7), 40 },
				{ new Bar(9), 50 },
			};

			tree[new Bar(5)] = 35;
			tree[new Bar(7)] = 45;
			tree[new Bar(9)] = 55;
			tree[new Bar(10)] = 60;

			tree[new Bar(int.MinValue)] = -1;
			tree[new Bar(0)] = 12345;
			tree[new Bar(int.MaxValue)] = 999;

			CollectionAssert.AreEqual(tree.KeyValuePairs, new[]
			{
				new KeyValuePair<Bar, int>(new Bar(int.MinValue), -1),
				new KeyValuePair<Bar, int>(new Bar(0), 12345),
				new KeyValuePair<Bar, int>(new Bar(1), 10),
				new KeyValuePair<Bar, int>(new Bar(3), 20),
				new KeyValuePair<Bar, int>(new Bar(5), 35),
				new KeyValuePair<Bar, int>(new Bar(7), 45),
				new KeyValuePair<Bar, int>(new Bar(9), 55),
				new KeyValuePair<Bar, int>(new Bar(10), 60),
				new KeyValuePair<Bar, int>(new Bar(int.MaxValue), 999),
			});
		}

		private class Baz { }

		[Test]
		public void CannotUseAnUnknownTypeAsAKeyWithoutAComparerFunction()
		{
			Assert.Throws<ArgumentException>(() => new RedBlackTree<Baz, int>());
		}

		[Test]
		public void CannotUseTheIndexerToRetrieveNonexistentValues()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			Assert.That(tree[3], Is.EqualTo(20));
			Assert.That(tree[7], Is.EqualTo(40));

			int x;
			Assert.Throws<KeyNotFoundException>(() => x = tree[11]);
		}

		[Test]
		public void TreesShouldAlwaysBeReadWrite()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			Assert.That(tree.IsReadOnly, Is.False);
		}

		[Test]
		public void CannotAddDuplicateKeys()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			Assert.Throws<ArgumentException>(() => tree.Add(7, 45));
		}

		[Test]
		public void CanCopyTreesByNode()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			RedBlackTree<int, int> newTree = new RedBlackTree<int, int>
			{
				tree.Find(9),
				tree.Find(7),
				tree.Find(5),
				tree.Find(3),
				tree.Find(1),
			};

			CollectionAssert.AreEqual(tree.KeyValuePairs, newTree.KeyValuePairs);
		}

		[Test]
		public void CanCopyTreesByAddRangeFromAnotherTree()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			RedBlackTree<int, int> newTree = new RedBlackTree<int, int>();
			newTree.AddRange(tree);

			CollectionAssert.AreEqual(tree.KeyValuePairs, newTree.KeyValuePairs);
		}

		[Test]
		public void CanCopyTreesByAddRangeFromKeyValuePairs()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			RedBlackTree<int, int> newTree = new RedBlackTree<int, int>();
			newTree.AddRange(dictionary);

			CollectionAssert.AreEquivalent(dictionary, newTree.KeyValuePairs);
		}

		[Test]
		public void CanRemoveAllNodesInOneFellSwoop()
		{
			RedBlackTree<int, int> tree = new RedBlackTree<int, int>
			{
				{ 1, 10 },
				{ 3, 20 },
				{ 5, 30 },
				{ 7, 40 },
				{ 9, 50 },
			};

			tree.Clear();

			Assert.That(tree.Root, Is.Null);
			Assert.That(tree.Count, Is.Zero);
		}

		[Test]
		public void CanFindGreatestUnderALimit()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			Assert.That(tree.GreatestBefore(12345), Is.Null);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				// Only do this every twenty values, or the sorting will take too long.
				if ((i % 20) == 19)
				{
					int valueUnderLastKey = keys.OrderBy(k => k).Last(k => k < key - 1);
					RedBlackTreeNode<int, int> node = tree.GreatestBefore(key - 1);
					Assert.That(node.Key, Is.EqualTo(valueUnderLastKey));

					int valueUnderLastKey2 = keys.OrderBy(k => k).Last(k => k < key);
					RedBlackTreeNode<int, int> node2 = tree.GreatestBefore(key);
					Assert.That(node2.Key, Is.EqualTo(valueUnderLastKey2));
				}
			}
		}

		[Test]
		public void CanFindGreatestAtOrUnderALimit()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			Assert.That(tree.GreatestBeforeOrEqualTo(12345), Is.Null);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				// Only do this every twenty values, or the sorting will take too long.
				if ((i % 20) == 19)
				{
					int valueUnderLastKey = keys.OrderBy(k => k).Last(k => k <= key - 1);
					RedBlackTreeNode<int, int> node = tree.GreatestBeforeOrEqualTo(key - 1);
					Assert.That(node.Key, Is.EqualTo(valueUnderLastKey));

					int valueUnderLastKey2 = keys.OrderBy(k => k).Last(k => k <= key);
					RedBlackTreeNode<int, int> node2 = tree.GreatestBeforeOrEqualTo(key);
					Assert.That(node2.Key, Is.EqualTo(valueUnderLastKey2));
				}
			}
		}

		[Test]
		public void CanFindLowestAboveALimit()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			Assert.That(tree.LeastAfter(12345), Is.Null);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				// Only do this every twenty values, or the sorting will take too long.
				if ((i % 20) == 19)
				{
					int valueUnderFirstKey = keys.OrderBy(k => k).First(k => k > key + 1);
					RedBlackTreeNode<int, int> node = tree.LeastAfter(key + 1);
					Assert.That(node.Key, Is.EqualTo(valueUnderFirstKey));

					int valueUnderFirstKey2 = keys.OrderBy(k => k).First(k => k > key);
					RedBlackTreeNode<int, int> node2 = tree.LeastAfter(key);
					Assert.That(node2.Key, Is.EqualTo(valueUnderFirstKey2));
				}
			}
		}

		[Test]
		public void CanFindLowestAtOrAboveALimit()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			Assert.That(tree.LeastAfterOrEqualTo(12345), Is.Null);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				// Only do this every twenty values, or the sorting will take too long.
				if ((i % 20) == 19)
				{
					int valueUnderFirstKey = keys.OrderBy(k => k).First(k => k >= key + 1);
					RedBlackTreeNode<int, int> node = tree.LeastAfterOrEqualTo(key + 1);
					Assert.That(node.Key, Is.EqualTo(valueUnderFirstKey));

					int valueUnderFirstKey2 = keys.OrderBy(k => k).First(k => k >= key);
					RedBlackTreeNode<int, int> node2 = tree.LeastAfterOrEqualTo(key);
					Assert.That(node2.Key, Is.EqualTo(valueUnderFirstKey2));
				}
			}
		}

		[Test]
		public void CanFindLowestInARange()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			Assert.That(tree.LeastInRange(10, 20), Is.Null);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				// Only do this every twenty values, or the sorting will take too long.
				if ((i % 20) == 19)
				{
					keys.Sort();
					int startRange = keys[i / 3];
					int endRange = keys[i * 2 / 3];
					int lowestInclusive = startRange;
					int lowestExclusive = keys[i / 3 + 1];

					RedBlackTreeNode<int, int> node;
					node = tree.LeastInRange(startRange, endRange, true, true);
					Assert.That(node.Key, Is.EqualTo(lowestInclusive));
					node = tree.LeastInRange(startRange, endRange, false, true);
					Assert.That(node.Key, Is.EqualTo(lowestExclusive));
					node = tree.LeastInRange(startRange, endRange, true, false);
					Assert.That(node.Key, Is.EqualTo(lowestInclusive));
					node = tree.LeastInRange(startRange, endRange, false, false);
					Assert.That(node.Key, Is.EqualTo(lowestExclusive));
				}
			}
		}

		[Test]
		public void CanFindGreatestInARange()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			Assert.That(tree.GreatestInRange(10, 20), Is.Null);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);

				tree.Add(key, i);

				// Only do this every twenty values, or the sorting will take too long.
				if ((i % 20) == 19)
				{
					keys.Sort();
					int startRange = keys[i / 3];
					int endRange = keys[i * 2 / 3];
					int greatestInclusive = endRange;
					int greatestExclusive = keys[i * 2 / 3 - 1];

					RedBlackTreeNode<int, int> node;
					node = tree.GreatestInRange(startRange, endRange, true, true);
					Assert.That(node.Key, Is.EqualTo(greatestInclusive));
					node = tree.GreatestInRange(startRange, endRange, false, true);
					Assert.That(node.Key, Is.EqualTo(greatestInclusive));
					node = tree.GreatestInRange(startRange, endRange, true, false);
					Assert.That(node.Key, Is.EqualTo(greatestExclusive));
					node = tree.GreatestInRange(startRange, endRange, false, false);
					Assert.That(node.Key, Is.EqualTo(greatestExclusive));
				}
			}
		}

		[Test]
		public void CanEnumerateNewSchool()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			IEnumerator<RedBlackTreeNode<int, int>> enumerator = ((IEnumerable<RedBlackTreeNode<int, int>>)tree).GetEnumerator();
			Assert.That(enumerator.MoveNext(), Is.False);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);
				tree.Add(key, i);
			}

			keys.Sort();

			enumerator = ((IEnumerable<RedBlackTreeNode<int, int>>)tree).GetEnumerator();
			int index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(enumerator.Current.Key, Is.EqualTo(keys[index]));
				index++;
			}
			Assert.That(index, Is.EqualTo(keys.Count));

			enumerator.Reset();

			index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(enumerator.Current.Key, Is.EqualTo(keys[index]));
				index++;
			}
			Assert.That(index, Is.EqualTo(keys.Count));
		}

		[Test]
		public void CanEnumerateOldSchool()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();

			IEnumerator enumerator = ((IEnumerable)tree).GetEnumerator();
			Assert.That(enumerator.MoveNext(), Is.False);

			List<int> keys = new List<int>();
			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);
				tree.Add(key, i);
			}

			keys.Sort();
			enumerator = ((IEnumerable)tree).GetEnumerator();
			int index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(((RedBlackTreeNode<int, int>)enumerator.Current).Key,
					Is.EqualTo(keys[index]));
				index++;
			}
			Assert.That(index, Is.EqualTo(keys.Count));

			enumerator.Reset();

			index = 0;
			while (enumerator.MoveNext())
			{
				Assert.That(((RedBlackTreeNode<int, int>)enumerator.Current).Key,
					Is.EqualTo(keys[index]));
				index++;
			}
			Assert.That(index, Is.EqualTo(keys.Count));
		}

		[Test]
		public void CanRemoveRanges()
		{
			int seed = 12345;
			RedBlackTree<int, int> tree;
			List<int> keys;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			void MakeRandomTreeAndListOfKeys()
			{
				seed = 12345;
				tree = new RedBlackTree<int, int>();
				keys = new List<int>();

				for (int i = 0; i < 500; i++)
				{
					int key = RandomNext();
					keys.Add(key);
					tree.Add(key, i);
				}

				keys.Sort();
			}

			MakeRandomTreeAndListOfKeys();
			int oneThird = keys.Count / 3;
			int twoThirds = oneThird * 2;
			int startRange = keys[oneThird];
			int endRange = keys[twoThirds];

			MakeRandomTreeAndListOfKeys();
			tree.RemoveRange(startRange, endRange, true, true);
			keys.RemoveRange(oneThird, twoThirds - oneThird + 1);
			CollectionAssert.AreEqual(tree.Select(n => n.Key), keys);

			MakeRandomTreeAndListOfKeys();
			tree.RemoveRange(startRange, endRange, false, true);
			keys.RemoveRange(oneThird + 1, twoThirds - oneThird);
			CollectionAssert.AreEqual(tree.Select(n => n.Key), keys);

			MakeRandomTreeAndListOfKeys();
			tree.RemoveRange(startRange, endRange, true, false);
			keys.RemoveRange(oneThird, twoThirds - oneThird);
			CollectionAssert.AreEqual(tree.Select(n => n.Key), keys);

			MakeRandomTreeAndListOfKeys();
			tree.RemoveRange(startRange, endRange, false, false);
			keys.RemoveRange(oneThird + 1, twoThirds - oneThird - 1);
			CollectionAssert.AreEqual(tree.Select(n => n.Key), keys);

			// Now test that we can't remove from broken requests.

			MakeRandomTreeAndListOfKeys();
			Assert.Throws<ArgumentException>(() => tree.RemoveRange(endRange, startRange, false, false));
			CollectionAssert.AreEqual(tree.Select(n => n.Key), keys);

			MakeRandomTreeAndListOfKeys();
			Assert.That(tree.RemoveRange(-100, -1, false, false), Is.Zero);
			CollectionAssert.AreEqual(tree.Select(n => n.Key), keys);

			MakeRandomTreeAndListOfKeys();
			Assert.That(tree.RemoveRange(65536, 1048576, false, false), Is.Zero);
			CollectionAssert.AreEqual(tree.Select(n => n.Key), keys);
		}

		[Test]
		public void CanTestForNodeContains()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();
			List<int> keys = new List<int>();

			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);
				tree.Add(key, i);
			}

			// Shouldn't contain a node it doesn't know about.
			Assert.That(tree.Contains(new RedBlackTreeNode<int, int>(5, 5)), Is.False);

			RedBlackTreeNode<int, int> node = tree.MinimumNode;
			Assert.That(node, Is.Not.Null);

			// Should contain a node it's known to contain.
			Assert.That(tree.Contains(node), Is.True);
		}

		[Test]
		public void CanExplicitlyRemoveKnownNodes()
		{
			int seed = 12345;

			int RandomNext()
			{
				seed = (seed * 69069) + 12345;
				return (seed >> 16) & 0x7FFF;
			}

			RedBlackTree<int, int> tree = new RedBlackTree<int, int>();
			List<int> keys = new List<int>();

			for (int i = 0; i < 500; i++)
			{
				int key = RandomNext();
				keys.Add(key);
				tree.Add(key, i);
			}
			tree.Validate();

			// Shouldn't be able to remove a node it doesn't know about.
			Assert.That(tree.Remove(new RedBlackTreeNode<int, int>(5, 5)), Is.False);
			Assert.That(tree.Remove(null), Is.False);

			RedBlackTreeNode<int, int> node = tree.MinimumNode;
			Assert.That(node, Is.Not.Null);

			int count = tree.Count;

			// Can remove a node it's known to contain.
			Assert.That(tree.Remove(node), Is.True);

			// The node should no longer be in the tree.
			Assert.That(tree.Find(node.Key), Is.Null);
			Assert.That(tree.Count, Is.EqualTo(count - 1));
			tree.Validate();
		}
	}
}
