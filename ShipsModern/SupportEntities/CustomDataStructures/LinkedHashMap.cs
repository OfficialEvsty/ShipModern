using ShipsForm.Logic.NodeSystem;
using ShipsForm.Logic.TilesSystem;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ShipsModern.SupportEntities.CustomDataStructures
{
    public class LinkedHashMap<TKey, TValue> where TKey : Tile where TValue : TKey
    {
        private Dictionary<TKey, TValue> dictionary;
        private PriorityQueue<TKey, float> priority;

        public LinkedHashMap()
        {
            dictionary = new Dictionary<TKey, TValue>();
            priority = new PriorityQueue<TKey, float>();
        }

        public void Add(TKey key, TValue value)
        {
            var res = dictionary.TryGetValue(key, out var oldKey);
            if (!res)
            {
                dictionary.Add(key, value);
                priority.Enqueue(key, key.CostDistance);
            }
            else if(res && oldKey.Cost > key.Cost)
            {
                ChangeAttributes(key, key);
            }
        }

        public void ChangeAttributes(TKey oldKey, TKey newKey)
        {
            priority.Enqueue(newKey, newKey.CostDistance);
            dictionary[oldKey] = newKey as TValue;     
        }

        public TKey DequeueMin()
        {
            var minKey = priority.Dequeue();
            dictionary.Remove(minKey);
            return minKey;
        }

        public void Clear()
        {
            dictionary.Clear();
            priority.Clear();
        }

        public TValue this[TKey key]
        {
            get
            {
                if (dictionary.TryGetValue(key, out TValue? value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                if (dictionary.TryGetValue(key, out TValue? val))
                {
                    val = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public int Count => dictionary.Count;
        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }
        public TValue First => dictionary[priority.Peek()];
    }
}
