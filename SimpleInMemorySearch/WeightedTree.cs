﻿using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SimpleInMemorySearch
{
    /// <summary>
    /// Weighted tree implementation.
    /// </summary>
    /// <typeparam name="T">Type of the object that is searched for.</typeparam>
    /// <typeparam name="TZ">Type of the value at each node.</typeparam>
    public class WeightedTree<T, TZ> : IWeightedTree<T, TZ>
    {
        /// <summary>
        /// Create a new weighted tree.
        /// </summary>
        public WeightedTree()
        {
            this.Edges = new ConcurrentDictionary<TZ, IWeightedTree<T, TZ>>();
            this.Weight = 0;
            this.Items = new ConcurrentDictionary<T, double>();
        }

        /// <summary>
        /// The edges from this tree.
        /// </summary>
        public IDictionary<TZ, IWeightedTree<T, TZ>> Edges { get; private set; }

        /// <summary>
        /// The weight of the link to this node.
        /// </summary>
        public int Weight { get; private set; }

        /// <summary>
        /// The items that terminate at this node and their respective weight.
        /// </summary>
        public IDictionary<T, double> Items { get; private set; }

        /// <summary>
        /// Index the key enumeration with the specified weight (defaulting to 1).
        /// </summary>
        /// <param name="nodeValues">The array of node values to index.</param>
        /// <param name="indexObject">The object to index at the location.</param>
        /// <param name="weight">The weight to assign to the array, default 1.</param>
        /// <returns>When complete.</returns>
        public void Index(IEnumerable<TZ> nodeValues, T indexObject, int weight = 1)
        {
            this.Index(nodeValues.GetEnumerator(), indexObject, weight);
        }

        /// <summary>
        /// Index the key enumerator with the specified weight (defaulting to 1).
        /// </summary>
        /// <param name="nodeValues">The array of node values to index.</param>
        /// <param name="indexObject">The object to index at the location.</param>
        /// <param name="weight">The weight to assign to the array, default 1.</param>
        /// <returns>When complete.</returns>
        public void Index(IEnumerator<TZ> nodeValues, T indexObject, int weight = 1)
        {
            // Add the weight on for the result
            this.Weight += weight;

            // If we have reached the end of the node values, add the index object to our item collection
            if (!nodeValues.MoveNext())
            {
                if (this.Items.ContainsKey(indexObject))
                {
                    this.Items[indexObject] += weight;
                }
                else
                {
                    this.Items.Add(indexObject, weight);
                }
                return;
            }

            var nextValue = nodeValues.Current;

            if (!this.Edges.ContainsKey(nextValue))
            {
                this.Edges[nextValue] = new WeightedTree<T, TZ>();
            }

            // Index the rest of the enumerable down the tree.
            this.Edges[nextValue].Index(nodeValues, indexObject, weight);
        }

        /// <summary>
        /// Search the tree with the given prefix, returning all matching results.
        /// </summary>
        /// <param name="prefix">The prefix nodes.</param>
        /// <returns>The matched results, unsorted and potentially duplicated.</returns>
        public IEnumerable<ITreeResult<T, TZ>> Search(IEnumerable<TZ> prefix)
        {
            foreach (var item in this.Search(prefix.GetEnumerator()))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Search the tree with the given prefix, previous key information and cumulative weight, returning all matching results.
        /// </summary>
        /// <param name="prefix">The prefix nodes.</param>
        /// <param name="previousKeys">The previous keys searched so far.</param>
        /// <param name="cumulativeWeight">The cumulative weight acquired so far.</param>
        /// <returns>The matched results, unsorted and potentially duplicated.</returns>
        public IEnumerable<ITreeResult<T, TZ>> Search(IEnumerator<TZ> prefix, IReadOnlyList<TZ> previousKeys = null, int cumulativeWeight = 0)
        {
            if (previousKeys == null)
            {
                previousKeys = new List<TZ>();
            }

            var updatedKeys = new List<TZ>(previousKeys);

            // We have reached the end of the prefix, so return all values from here
            if (!prefix.MoveNext())
            {
                foreach (var item in this.AllItems(previousKeys))
                {
                    yield return item;
                }
                yield break;
            }

            var nextValue = prefix.Current;

            cumulativeWeight += this.Weight;
            updatedKeys.Add(nextValue);

            if (!this.Edges.ContainsKey(nextValue))
            {
                yield break;
            }

            // Search the found node for the rest of the search prefix
            foreach (var result in this.Edges[nextValue].Search(prefix, updatedKeys, cumulativeWeight))
            {
                yield return result;
            }
        }

        /// <summary>
        /// Recursively retrieve all items from this node down the tree.
        /// </summary>
        /// <returns>All items, unsorted and potentially duplicated.</returns>
        public IEnumerable<ITreeResult<T, TZ>> AllItems()
        {
            foreach (var item in this.AllItems(null))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Recursively retrieve all items from this node down the tree, with the given previous keys and cumulative weight.
        /// </summary>
        /// <param name="previousKeys">The previous keys searched so far.</param>
        /// <returns>All items, unsorted and potentially duplicated.</returns>
        public IEnumerable<ITreeResult<T, TZ>> AllItems(IReadOnlyList<TZ> previousKeys)
        {
            if (previousKeys == null)
            {
                previousKeys = new List<TZ>();
            }

            // Return all this node's items
            foreach (var item in this.Items)
            {
                yield return new TreeResult<T, TZ>(previousKeys, item.Key, item.Value);
            }

            // Return all child node's items
            foreach (var child in this.Edges)
            {
                var updatedKeys = new List<TZ>(previousKeys);
                updatedKeys.Add(child.Key);

                foreach (var childItem in child.Value.AllItems(updatedKeys))
                {
                    yield return childItem;
                }
            }
        }
    }
}
