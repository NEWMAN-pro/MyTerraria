using System;
using System.Collections.Generic;

namespace Soultia.Util
{
    // ����С��ʵ�����ȶ���
    public class PriorityQueue<T> where T : IComparable<T>
    {
        public List<T> heap;

        public PriorityQueue()
        {
            heap = new List<T>();
        }

        // ����Ԫ��
        public void Enqueue(T item)
        {
            // �ȼ���ĩβ
            heap.Add(item);
            // ��ǰ�ڵ�
            int i = heap.Count - 1;
            // ���������ڵ�
            int parent = (i - 1) / 2;

            // �ҵ��ڶ������ϵ���ȷλ��
            while (i > 0 && heap[parent].CompareTo(heap[i]) > 0)
            {
                T temp = heap[i];
                heap[i] = heap[parent];
                heap[parent] = temp;

                i = parent;
                parent = (i - 1) / 2;
            }
        }

        // ɾ�����ж�Ԫ��
        public T Dequeue()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            // ���Ѷ�Ԫ��������һ��Ԫ�ؽ���������ɾ����
            T item = heap[0];
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            // ���¶Ѷ�Ԫ���������Ҷ��ӱȽϣ��ع���
            int i = 0;
            while (true)
            {
                int leftChild = 2 * i + 1;
                int rightChild = 2 * i + 2;

                if (leftChild >= heap.Count)
                {
                    // ����ֹͣ
                    break;
                }

                int minChild = leftChild;
                if (rightChild < heap.Count && heap[rightChild].CompareTo(heap[leftChild]) < 0)
                {
                    // ��������Ҷ����ұ������С��������Сֵ
                    minChild = rightChild;
                }

                if (heap[i].CompareTo(heap[minChild]) > 0)
                {
                    // �����ǰ�ڵ�ȶ�����Сֵ���򽻻������븸��
                    T temp = heap[i];
                    heap[i] = heap[minChild];
                    heap[minChild] = temp;

                    i = minChild;
                }
                else
                {
                    // ��ǰ�ڵ�����ж���С���ѵ���ȷλ�ã�����ѭ��
                    break;
                }
            }

            return item;
        }

        // ɾ��ָ��Ԫ��
        public bool Remove(T item)
        {
            // ���ҵ�Ԫ���ڶ��е�λ��
            int index = heap.IndexOf(item);
            if (index == -1)
            {
                return false; // Ԫ�ز����ڣ�����false
            }

            // ����Ԫ��������һ��Ԫ�ؽ���������ɾ����
            heap[index] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            // �ع���
            int i = index;
            while (true)
            {
                int leftChild = 2 * i + 1;
                int rightChild = 2 * i + 2;

                if (leftChild >= heap.Count)
                {
                    // ����ֹͣ
                    break;
                }

                int minChild = leftChild;
                if (rightChild < heap.Count && heap[rightChild].CompareTo(heap[leftChild]) < 0)
                {
                    // ��������Ҷ����ұ������С��������Сֵ
                    minChild = rightChild;
                }

                if (heap[i].CompareTo(heap[minChild]) > 0)
                {
                    // �����ǰ�ڵ�ȶ�����Сֵ���򽻻������븸��
                    T temp = heap[i];
                    heap[i] = heap[minChild];
                    heap[minChild] = temp;

                    i = minChild;
                }
                else
                {
                    // ��ǰ�ڵ�����ж���С���ѵ���ȷλ�ã�����ѭ��
                    break;
                }
            }
            // ɾ���ɹ�
            return true;
        }

        // ��ѯ�Ѷ�Ԫ�ؼ���Сֵ
        public T Peek()
        {
            if (heap.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty");
            }

            return heap[0];
        }

        // ��ѯ�Ƿ��и�ֵ
        public bool Contains(T t)
        {
            return heap.Contains(t);
        }

        // ��ѯ�Ѵ�С
        public int Count
        {
            get { return heap.Count; }
        }

        // �ж϶��Ƿ�Ϊ��
        public bool IsEmpty
        {
            get { return heap.Count == 0; }
        }

        // ��ն���
        public void Clear()
        {
            heap.Clear();
        }
    }
}
