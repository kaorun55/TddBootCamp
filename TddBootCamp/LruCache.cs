using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace TddBootCamp
{
	public static class LinkedListExtension {
		public static IEnumerable<LinkedListNode<T>> GetNodes<T>(this LinkedList<T> list)
		{
			var node = list.First;
			while(node != null) {
				yield return node;
				node = node.Next;
			}
		}
	}

	public class Node
	{
		public string key;
		public string value;
		public int tick;

		public Node(string key, int tick, string value)
		{
			// TODO: Complete member initialization
			this.key = key;
			this.tick = tick;
			this.value = value;
		}
	}

	public class LruCache
	{
		public LinkedList<Node> _orderedList;
		private int _maxSize;
		private int _msSpan;

		public int MaxSize { get { return _maxSize; } }

		public LruCache()
			: this(2)
		{
		}

		public LruCache(int size)
			: this(size, 0)
		{
		}

		public LruCache(int size, int span)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException("size");

			_orderedList = new LinkedList<Node>();
			_maxSize = size;
			_msSpan = span;
		}

		public void Put(string key, string value)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			Remove();

			var first = findNode(key);
			if (first != null)
			{
				_orderedList.Remove(first);
			}

			_orderedList.AddFirst(new Node(key, Environment.TickCount, value));
		}

		private void Remove()
		{
			while (_orderedList.Count > _maxSize)
			{
				_orderedList.RemoveLast();
			}

			if (_msSpan == 0)
				return;
			while ((_orderedList.Count != 0) && (Environment.TickCount - _orderedList.Last.Value.tick > _msSpan))
			{
				Thread.Sleep(100);
				_orderedList.RemoveLast();
			}
		}

		public string Get(string key)
		{
			Remove();
			var first = findNode(key);
			if (first != null)
			{
				_orderedList.Remove(first);
				_orderedList.AddFirst(first);
			}

			var node = findNode(key);
			if (node == null)
			{
				return null;
			}
			return node.Value.value;
		}

		private LinkedListNode<Node> findNode(string key)
		{
			var first = _orderedList.GetNodes()
				.FirstOrDefault(node => node.Value.key == key);
			return first;
		}

		public void Resize(int maxSize)
		{
			if (maxSize <= 0)
				throw new ArgumentOutOfRangeException("maxSize");

			_maxSize = maxSize;
			Remove();
		}
	}
}
