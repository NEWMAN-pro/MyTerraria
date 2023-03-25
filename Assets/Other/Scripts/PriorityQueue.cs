using System;
using System.Collections.Generic;

namespace Soultia.Util
{
    // 用最小堆实现优先队列
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public List<T> heap;

        public PriorityQueue()
        {
            heap = new List<T>();
        }

        // 插入元素
        public void Enqueue(T item)
        {
            // 先加入末尾
            heap.Add(item);
            // 当前节点
            int i = heap.Count - 1;
            // 二叉树父节点
            int parent = (i - 1) / 2;

            // 找到在二叉树上的正确位置
            while (i > 0 && heap[parent].CompareTo(heap[i]) > 0)
            {
                T temp = heap[i];
                heap[i] = heap[parent];
                heap[parent] = temp;

                i = parent;
                parent = (i - 1) / 2;
            }
        }

        // 删除队列顶元素
        public T Dequeue()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            // 将堆顶元素与最有一个元素交换，并且删除它
            T item = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            // 将新堆顶元素与其左右儿子比较，重构堆
            int i = 0;
            while (true)
            {
                int leftChild = 2 * i + 1;
                int rightChild = 2 * i + 2;

                if (leftChild >= heap.Count)
                {
                    // 找完停止
                    break;
                }

                int minChild = leftChild;
                if (rightChild < heap.Count && heap[rightChild].CompareTo(heap[leftChild]) < 0)
                {
                    // 如果存在右儿子且比左儿子小，更新最小值
                    minChild = rightChild;
                }

                if (heap[i].CompareTo(heap[minChild]) > 0)
                {
                    // 如果当前节点比儿子最小值大，则交换儿子与父亲
                    T temp = heap[i];
                    heap[i] = heap[minChild];
                    heap[minChild] = temp;

                    i = minChild;
                }
                else
                {
                    // 当前节点比所有儿子小，已到正确位置，结束循环
                    break;
                }
            }

            return item;
        }

        // 删除指定元素
        public bool Remove(T item)
        {
            // 先找到元素在堆中的位置
            int index = heap.IndexOf(item);
            if (index == -1)
            {
                return false; // 元素不存在，返回false
            }

            // 将该元素与最有一个元素交换，并且删除它
            heap[index] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            // 重构堆
            int i = index;
            while (true)
            {
                int leftChild = 2 * i + 1;
                int rightChild = 2 * i + 2;

                if (leftChild >= heap.Count)
                {
                    // 找完停止
                    break;
                }

                int minChild = leftChild;
                if (rightChild < heap.Count && heap[rightChild].CompareTo(heap[leftChild]) < 0)
                {
                    // 如果存在右儿子且比左儿子小，更新最小值
                    minChild = rightChild;
                }

                if (heap[i].CompareTo(heap[minChild]) > 0)
                {
                    // 如果当前节点比儿子最小值大，则交换儿子与父亲
                    T temp = heap[i];
                    heap[i] = heap[minChild];
                    heap[minChild] = temp;

                    i = minChild;
                }
                else
                {
                    // 当前节点比所有儿子小，已到正确位置，结束循环
                    break;
                }
            }
            // 删除成功
            return true;
        }

        // 查询堆顶元素及最小值
        public T Peek()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return heap[0];
        }

        // 查询是否有该值
        public bool Contains(T t)
        {
            return heap.Contains(t);
        }

        // 查询堆大小
        public int Count
        {
            get { return heap.Count; }
        }

        // 判断堆是否为空
        public bool IsEmpty
        {
            get { return heap.Count == 0; }
        }

        // 清空队列
        public void Clear()
        {
            heap.Clear();
        }
    }
}
