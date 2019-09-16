# Balanced Collections

_Copyright (C) 2019 by Sean Werkema. All rights reserved._
_Licensed under the terms of the [MIT open-source license](License.txt)._

-----------------------------------------------------------------------------

## Overview

This is a C# library that contains various _balanced collection classes_,
for .NET 4.5+ and .NET Core 2+.

What are _balanced collection classes_?  They are classes that store
collections of data in such a way that the operations on the data can be
performed efficiently.

### License

This code is licensed under the terms of the [MIT open-source license](License.txt).

### What's included?

There are many classes provided to handle your efficient data-storage-and-
lookup needs.  Major classes include:

- `RedBlackTree<K, V>` - Represents data as a traditional red-black tree.
- `WeightedRedBlackTree<K, V>` - Represents data as a red-black tree, with
    weight counts in each node.
- `BigList<T>` - Represents data as a _keyless_ red-black tree.

### Why use balanced collections?

Consider a `Dictionary<int, string>`.  Accessing a member of the dictionary,
such as `dict[123]`, is fast — it runs in O(1) time.  But if
you want to find the answer to _What is the next key after 123?_, you
have no choice but to search through the entire dictionary, which is very
slow:  `dict.Keys.Where(k > 123).OrderBy(k => k).First()`.

The Balanced Collections library is designed to solve that.  The
`RedBlackTree<K, V>` class can be thought of as a _super dictionary_:
You can access elements in it like a dictionary — `tree[123]` — but you
can also ask complicated questions about the keys; to find the next key
after 123, you simply invoke `tree.Find(123).Next()`.  To find keys in
a range, or even just find keys _near_ another key, the methods you need
are already built-in.

Also, `RedBlackTree<K, V>` implements all of the same methods you're
used to seeing on `Dictionary<K, V>`, so in many cases, it's a drop-in
replacement in existing code (and even includes an `AsDictionary()` method
that turns it into a real `IDictionary<K, V>`!).

### Code quality

You should be able to trust the software you use, so this library goes to some
effort to be high-quality code:

- The collections currently have 98% unit-test coverage.
- The collections are designed to be as efficient as possible, while avoiding
  code duplication, and make heavy use of generics.
- The collections provide substantial but safe access to their internal
  implementation, so you can build even more advanced operations on top of them.
- Base level operations are decoupled from specialized implementations, so you
  can readily extend and reuse them.
- The source code is heavily documented, both inside method bodies, and with
  XML documentation on each class and method.
- The source code includes performance notations for most methods (i.e.,
  `Find()` runs in O(lg n) time), so you know what to expect for large data sets.

### Reference documentation

The current reference documentation (API usage on each class and method) can be
found in the [docs](docs/) folder.
  
-----------------------------------------------------------------------------

## RedBlackTree and WeightedRedBlackTree 

These classes can be treated like `Dictionary<K, V>`, but they support many other
use cases that `Dictionary<K, V>` cannot.  Here are the standard Dictionary-like
methods they support:

- `this[key]` - Get or set a value for a key.
- `TryGetValue(K key, out V value)` - Find a value, but don't fail if it doesn't exist.
- `Add(K key, V value)` - Add a new element to the tree.
- `AddRange(IEnumerable<KeyValuePair<K, V>> items)` - Add copies of many existing items (or another dictionary!).
- `ContainsKey(K key)` - Test to see if a key exists in the tree.
- `Remove(K key)` - Remove a key from the tree.

They also implement `ICollection<RedBlackTreeNode<K, V>>`, so you can easily treat them as
a linear, ordered collection of tree nodes:

- `Add(RedBlackTreeNode<K, V> node)` - Add a copy of an existing node.
- `AddRange(IEnumerable<RedBlackTreeNode<K, V>> nodes)` - Add copies of many existing nodes (or an entire tree!).
- `Contains(RedBlackTreeNode<K, V> node)` - Test to see if a node is a member of the tree.
- `CopyTo(RedBlackTreeNode<K, V>[] array, int index)` - Copy the tree's nodes into an array.
- `Remove(RedBlackTreeNode<K, V> node)` - Remove a node from the tree.
- `RemoveRange(IEnumerable<RedBlackTreeNode<K, V>> nodes)` - Remove many existing nodes (or an entire tree!).

### Unique abilities

There are also many things these classes can do _efficiently_ and _natively_ that `Dictionary<K, V>`
cannot:

- `Find(K key)` - Find a node by key (returns null if not found).
- `node.Next()` - Get the next node (key/value pair) after this one.
- `node.Previous()` - Get the previous node (key/value pair) before this one.
- `Minimum` and `MinimumKey` - Get the first node or key in the tree, in order.
- `Maximum` and `MaximumKey` - Get the last node or key in the tree, in order.
- `GreatestBefore(K key)` - Find the node with the largest key _less than_ the given
    key (which does not necessarily need to exist in the tree).
- `GreatestBeforeOrEqualTo(K key)` - Find the node with the largest key _less than or
    equal to_ the given key (which does not necessarily need to exist in the tree).
- `GreatestInRange(K minimum, K maximum)` - Find the node with the largest key within
    the given range of keys (neither of which need to exist in the tree).
- `LeastAfter(K key)` - Find the node with the smallest key _greater than_ the given
    key (which does not necessarily need to exist in the tree).
- `LeastAfterOrEqualTo(K key)` - Find the node with the smallest key _greater than or
    equal to_ the given key (which does not necessarily need to exist in the tree).
- `LeastInRange(K minimum, K maximum)` - Find the node with the smallest key within the
    given range of keys (neither of which need to exist in the tree).

Each of the operations above runs in O(lg n) time:  So for a tree with a million entries,
you can reasonably expect each of those to take about 20 units of time — compare that to
a million units of time for the same operation on a `Dictionary<K, V>`!

Unlike many other red-black tree implementations (including the native `SortedDictionary<K, V>`
and `SortedSet<K, V>` implementations in .NET itself), you have _direct_ access to the
underlying `RedBlackTreeNode<K, V>` nodes themselves:  You can access the `Root`, and
walk down the `Left` and `Right` children of a node, and back up to the `Parent` node
itself.  You cannot _mutate_ those pointers, but by providing direct access to the tree,
you can manually perform additional optimized, efficient searches other than those above
(for example, operations like `DoRangesOverlap()` or `GreatestExcluding()`).

### Wrappers, adapters, and variants

In addition, these classes have various wrappers and adapters, designed to make it
easy to use them in a variety of scenarios:

- `ReadOnlyRedBlackTree<K, V>` - A wrapper around `RedBlackTree<K, V>` that
    makes its data immutable to consumers.
- `WeightedReadOnlyRedBlackTree<K, V>` - A wrapper around `WeightedRedBlackTree<K, V>`
    that makes it immutable to consumers.
- `WeightedReadOnlyRedBlackTree<K, V>` - A wrapper around `WeightedRedBlackTree<K, V>`
    that makes it immutable to consumers.

You can also access properties on most of the above to get _variant_ forms
of the data:

- `tree.Keys` returns an `ICollection<K>` that is fully capable (and supports read-only mode).
- `tree.KeyValuePairs` returns an `ICollection<KeyValuePair<K, V>>` that is fully capable
   (and supports read-only mode).
- `tree.Values` returns a read-only `ICollection<V>`.
- `tree.ToDictionary()` creates a `Dictionary<K, V>` copy of the data.
- `tree.AsDictionary()` wraps the data with an `IDictionary<K, V>` proxy that is fully capable
   (and supports read-only mode).

-----------------------------------------------------------------------------

## BigList

BigList is designed to solve the problem of when you need an ordered list of items
like `List<T>`, but you need to frequently insert or remove items in the middle or at
the beginning of the list.  `List<T>` is notoriously inefficient at operations like
`InsertAt()` and `RemoveAt()` — O(n) — while `BigList<T>` can handle those operations
very efficiently, in O(lg n) time.

-----------------------------------------------------------------------------

## Guidance and usage advice

So when should you use `RedBlackTree<K, V>` instead of `Dictionary<K, V>`?  When should
you prefer `Dictionary<K, V>` instead?  When should you use `WeightedRedBlackTree<K, V>`?
When should you use `BigList<T>` instead of `List<T>`?  And what about
`SortedDictionary<K, V>` and `SortedSet<T>`?  Here are some tips that will help you
decide which tool to use and when:

- In general, prefer `Dictionary<K, V>` if you need a mapping of keys to values.
  Seriously!  `Dictionary<K, V>` is well-implemented, and it's both fast and memory-efficient.
  Only use another type if you have to do something where `Dictionary<K, V>` is slow
  (like you need to be able to answer `next key` or `first key`).

- Likewise, prefer `List<T>` if you only need an array-like construct or
  you only intend to insert at the end of it.  `BigList<T>` is more efficient for _random_
  insertions and deletions.

- Prefer `HashSet<T>` if you only need an unordered collection of keys, if you're
  only `Add()`ing and `Remove()`ing keys and testing for `Contains()` in the set.

- Prefer `SortedSet<T>` if you need the same behaviors as `HashSet<T>` but you also need
  `Min` or `Max` efficiently, or you need `foreach` to always return the items in order.

- Prefer `SortedDictionary<K, V>` if you need a `Dictionary<K, V>` but the only additional
  feature you need is to have `foreach` return the keys in order.  Note that
  `SortedDictionary<K, V>` _cannot_ provide you with `Min` or `Max` keys efficiently.
  
- In general, avoid `SortedList<K, V>`.  This is only really useful if you intend to
  pre-populate the collection all at once and then ask questions about its data.
  `SortedList<K, V>` is an array under the hood, and just invokes `.Sort()` at regular
  intervals.  It is more memory-efficient than any of these collections, but it is not
  very time-efficient except in special situations.

- If you need dictionary-like behavior, but you _also_ need to efficiently answer
  `Min` or `Max`, or you need to be able to `Next` and `Previous` through the
  keys, then that's when you should use a `RedBlackTree<K, V>` instead of a
  `Dictionary<K, V>`.
  
- If you need special operations like "largest/smallest key in a range," or
  "largest key less than" or "smallest key greater than," then use `RedBlackTree<K, V>`.
  
- If you need to perform custom traversal of a binary tree representation of the
  data, use `RedBlackTree<K, V>`, since `SortedDictionary<K, V>` does not give you
  access to the underlying tree.
  
- So when should you use `WeightedRedBlackTree<K, V>` instead of `RedBlackTree<K, V>`?
  Rarely.  The weighted form calculates weights for each tree node, and that
  calculation comes at some additional constant-time overhead.  But the weights can
  be used to efficiently answer "How many keys are there _before_ or _after_ this key
  in the data?  How many keys are there _between_ these keys?" and similar
  positional-index questions.

### Guidance Table

This table can help you decide which data structure to use, based on the operations
you need to perform.

| Operation               | BC.RBT | BC.WRBT | BC.BL | D  | HS  | L   | LL  | SD | SL  | SS  |
|-------------------------|--------|---------|-------|----|-----|-----|-----|----|-----|-----|
| Add at end              | X      | X       | X     | -  | -   | XX  | XX  |    | X   |     |
| Remove from end         | X      | X       | X     | -  | -   | XX  | XX  |    | X   |     |
| Contains key            | X      | X       | -     | XX | XX  | -   | -   | X  | X   | X   |
| Insert in middle        | X      | X       | X     | XX | XX  | -   | (1) | X  | -   | X   |
| Remove from middle      | X      | X       | X     | XX | XX  | -   | (1) | X  | -   | X   |
| Insert at head          | X      | X       | X     | -  | -   | -   | XX  |    | -   |     |
| Remove at head          | X      | X       | X     | -  | -   | -   | XX  |    | -   |     |
| Get value by key        | X      | X       | -     | XX | (2) | -   | -   | X  | X   | (2) |
| Set value by key        | X      | X       | -     | XX | (2) | -   | -   | X  | X   | (2) |
| Get value at index      |        | X       | X     |    |     | XX  | -   |    | XX  |     |
| Set value at index      |        | X       | X     |    |     | XX  | -   |    | XX  |     |
| Minimum key             | X      | X       | -     | -  | -   | -   | -   | X  | XX  | X   |
| Maximum key             | X      | X       | -     | -  | -   | -   | -   |    | XX  |     |
| Next key                | X      | X       | -     | -  | -   | -   | (1) |    | X   |     |
| Previous key            | X      | X       | -     | -  | -   | -   | (1) |    | X   |     |
| Count items             | XX     | XX      | XX    | XX | XX  | XX  | XX  | XX | XX  | XX  |
| Greatest key in range   | X      | X       | -     | -  | -   | -   | -   |    | X   |     |
| Least key in range      | X      | X       | -     | -  | -   | -   | -   |    | X   |     |
| Custom range ops (3)    | -      | -       |       |    |     |     |     |    | X   |     |
| Sort by key             | XX     | XX      |       |    |     |     |     | XX | XX  | XX  |
| Sort by value (4)       |        |         |       |    |     |     |     |    |     |     |
| Memory overhead (5)     | 5      | 6       | 5     | 2  | 2   | 1   | 3   | 4  | 1   | 4   |
| Time overhead (6)       | 10     | 12      | 10    | 2  | 2   | 1   | 1   | 10 | 8   | 10  |

`RedBlackTree<K, V>` and `WeightedRedBlackTree<K, V>` attempt to be good all around for
most operations, but that comes at a cost in time and space overhead.  For a restricted
subset of operations, other data structures may be faster or use less memory.

In short, no one data structure is best for all operations:  Decide which operations
you need to perform, and then choose a data structure that matches them.

**Name abbreviations used above:**

- **D** = `System.Collections.Generic.Dictionary<K, V>`
- **HS** = `System.Collections.Generic.HashSet<T>`
- **L** = `System.Collections.Generic.List<T>`
- **LL** = `System.Collections.Generic.LinkedList<T>`
- **SD** = `System.Collections.Generic.SortedDictionary<K, V>`
- **SL** = `System.Collections.Generic.SortedList<K, V>`
- **SS** = `System.Collections.Generic.SortedSet<T>`
- **BC.RBT** = `BalancedCollections.RedBlackTree<K, V>`
- **BC.WRBT** = `BalancedCollections.WeightedRedBlackTree<K, V>`

**Performance abbreviations used above:**

- *XX* = O(1) time = Best possible efficiency
- *X* = O(lg n) time = Good
- *-* = O(n) time = Meh
- (blank) = O(n lg n) time = Bad
- *:(* = O(n^2) time or worse = VERY VERY BAD

**Notes:**

1. LinkedList is very efficient for middle insertions/removals (O(1), or "XX") if the
   preceding or following node is already known; but if it is unknown, this collection will
   require a search (O(n), or "-").  Likewise, `Next` and `Previous` are also efficient
   if the node is already known, but if it is unknown, they'll require a search as well.
2. This operation isn't meaningful for `HashSet<T>` or `SortedSet<T>`, since there are
   no values for each key.
3. Custom range operations are things like `DoRangesOverlap()` or `GreatestExcluding()`.
   Red-black trees that provide access to their nodes make this fairly straightforward; but
   for all other data structures, you'll have to extract the data, sort by key, and then
   perform the operation on the resulting arrays.
4. No matter which collection you use, if you want to sort by something other than the
   current key, you'll have to re-sort the data manually, which is going to take O(n lg n)
   time, multiplied by the time overhead of the collection type.
5. This is a rough estimate of the memory overhead per item, in machine words (64-bit values
   on a 64-bit machine, 32-bit values on a 32-bit machine).  This will vary depending on the
   OS, the version of .NET, and various other implementation-specific details, but it's a
   good rule of thumb:  Trees are generally memory-inefficient, while `List<T>` and
   `Dictionary<K, V>` are generally memory-efficient.
6. This is a rough estimate of how much constant time is wasted per operation.  Trees are
   generally bad at this, so they're often wasteful for small data sets; but they're very
   good at large data sets.  `List<T>` and `Dictionary<K, V>` provide fairly direct access
   to the data, so they're good for small data sets, but for many operations, they're
   very inefficient on large data sets.

-----------------------------------------------------------------------------

## Questions / Comments / Contact

This library was written in C# in September 2019 by Sean Werkema based on
a set of C++ classes he wrote in 2004.  Those classes were based on the red-black
tree algorithms in Cormen, Liesersen, and Rivest's _Introduction to Algorithms_,
enhanced to avoid requiring sentinel nodes, and to fill out missing operations.

For questions, bugs, or suggestions, please file an
[issue on GitHub](https://github.com/seanofw/balanced-collections).
For general comments, visit Sean's blog at https://www.werkema.com .
