using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DijkstraShortPathStandf
{
    class Heap
    {
        int nodesInHeap = 0;
        int minNodeIdx = 0;
        HeapNode[] heap;

        public Heap(Vertix[] vertices)
        {
            heap = new HeapNode[vertices.Length - 1];
            for (int i = 1; i < vertices.Length; i++)
            {
                heap[i - 1] = new HeapNode(vertices[i]);
                heap[i - 1].MyIdx = i - 1;
                heap[i - 1].ParentIdx = (i / 2) - 1;
                if ((i * 2) - 1 < heap.Length) heap[i - 1].LeftChildIdx = (i * 2) - 1;
                else heap[i - 1].LeftChildIdx = -1;
                if ((i * 2) < heap.Length) heap[i - 1].RightChildIdx = (i * 2);
                else heap[i - 1].RightChildIdx = -1;
            }
            nodesInHeap = heap.Length;
        }

        public HeapNode Deheapify()
        {
            if (heap[minNodeIdx] == null)
            {
                return null;
            }
            int idxToSwap = FindLeastChildIdx(minNodeIdx);
            HeapNode nodeToReturn;
            if (heap[heap[minNodeIdx].LeftChildIdx] == null
                &&
                heap[heap[minNodeIdx].RightChildIdx] == null)
            {
                nodeToReturn = heap[minNodeIdx];
                heap[minNodeIdx] = null;
                return nodeToReturn;
            }
            SwapWithParent(minNodeIdx, idxToSwap);
            nodeToReturn = heap[minNodeIdx];
            heap[minNodeIdx] = null;


            RearrengeWithChild(heap[idxToSwap]);

            int tempMin = -1;
            for (int i = 0; i < nodeToReturn.MyVertix.Edges.Length; i++)
            {
                int idx = nodeToReturn.MyVertix.Edges[i] - 1;
                if (heap[idx] != null)
                {
                    int newDistance = nodeToReturn.ShortestDistance + nodeToReturn.MyVertix.Distances[i];
                    if (heap[idx].ShortestDistance > newDistance)
                    {
                        heap[idx].ShortestDistance = newDistance;
                    }
                    RearrengeWithParent(heap[idx]);
                    tempMin = FindMinNodeIdx(idx);

                }
            }
            if (tempMin == -1)
            {
                for (int i = 0; i < heap.Length; i++)
                {
                    tempMin = FindMinNodeIdx(i);
                    if (heap[tempMin] != null)
                    {
                        break;
                    }
                }
            }
            minNodeIdx = tempMin;

            nodesInHeap--;
            return nodeToReturn;
        }

        int FindMinNodeIdx(int nodeIdx)
        {
            if (heap[nodeIdx] == null) return nodeIdx;
            int parentIdx = heap[nodeIdx].ParentIdx;
            if (parentIdx == -1) return nodeIdx;
            if (heap[parentIdx] == null) return parentIdx;
            else return FindMinNodeIdx(parentIdx);
        }

        int FindLeastChildIdx(int myIdx)
        {
            if (heap[myIdx].RightChildIdx != -1 && heap[heap[myIdx].RightChildIdx] != null)
            {
                return FindLeastChildIdx(heap[myIdx].RightChildIdx);
            }
            if (heap[myIdx].LeftChildIdx != -1 && heap[heap[myIdx].LeftChildIdx] != null)
            {
                return FindLeastChildIdx(heap[myIdx].LeftChildIdx);
            }
            return myIdx;
        }

        bool RearrengeWithChild(HeapNode node)
        {
            bool swapped = false;
            if (node.LeftChildIdx != -1 && heap[node.LeftChildIdx] != null && node.ShortestDistance > heap[node.LeftChildIdx].ShortestDistance)
            {
                swapped = true;
                SwapWithParent(node.MyIdx, node.LeftChildIdx);
                RearrengeWithChild(heap[node.MyIdx]);
            }
            else if (node.RightChildIdx != -1 && heap[node.RightChildIdx] != null && node.ShortestDistance > heap[node.RightChildIdx].ShortestDistance)
            {
                swapped = true;
                SwapWithParent(node.MyIdx, node.RightChildIdx);
                RearrengeWithChild(heap[node.MyIdx]);
            }
            return swapped;
        }

        void RearrengeWithParent(HeapNode node)
        {
            if (node.ParentIdx != -1&& heap[node.ParentIdx]!=null)
            {
                int parent = node.ParentIdx;
                if (RearrengeWithChild(heap[parent]))
                {
                    RearrengeWithParent(node);
                }
            }
        }

        void SwapWithParent(int firstIdx, int secondIdx)
        {

            int firstParent = heap[firstIdx].ParentIdx;
            int firstLeftChild = heap[firstIdx].LeftChildIdx;
            int firstRightChild = heap[firstIdx].RightChildIdx;

            int secondParent = heap[secondIdx].ParentIdx;
            int secondLeftChild = heap[secondIdx].LeftChildIdx;
            int secondRightChild = heap[secondIdx].RightChildIdx;

            if (secondParent == firstIdx)
            {
                heap[firstIdx].ParentIdx = secondIdx;
            }
            else
            { heap[firstIdx].ParentIdx = secondParent; }

            if (secondLeftChild == firstIdx)
            {
                heap[firstIdx].LeftChildIdx = secondIdx;
            }
            else
            { heap[firstIdx].LeftChildIdx = secondLeftChild; }

            if (secondRightChild == firstIdx)
            {
                heap[firstIdx].RightChildIdx = secondIdx;
            }
            else
            { heap[firstIdx].RightChildIdx = secondRightChild; }



            if (firstParent == secondIdx)
            {
                heap[secondIdx].ParentIdx = firstIdx;
            }
            else
            { heap[secondIdx].ParentIdx = firstParent; }

            if (firstLeftChild == secondIdx)
            {
                heap[secondIdx].LeftChildIdx = firstIdx;
            }
            else
            { heap[secondIdx].LeftChildIdx = firstLeftChild; }

            if (firstRightChild == secondIdx)
            {
                heap[secondIdx].RightChildIdx = firstIdx;
            }
            else
            { heap[secondIdx].RightChildIdx = firstRightChild; }


            bool thereIsParentIdx = heap[firstIdx].ParentIdx != -1 && heap[heap[firstIdx].ParentIdx] != null;
            if (thereIsParentIdx)
            {
                if (heap[heap[firstIdx].ParentIdx].LeftChildIdx == secondIdx)
                    heap[heap[firstIdx].ParentIdx].LeftChildIdx = firstIdx;
                else if (heap[heap[firstIdx].ParentIdx].RightChildIdx == secondIdx)
                    heap[heap[firstIdx].ParentIdx].RightChildIdx = firstIdx;
            }

            bool thereIsLeftChild = heap[firstIdx].LeftChildIdx != -1 && heap[heap[firstIdx].LeftChildIdx] != null;
            if (thereIsLeftChild)
            {
                heap[heap[firstIdx].LeftChildIdx].ParentIdx = firstIdx;
            }

            bool thereIsRightChild = heap[firstIdx].RightChildIdx != -1 && heap[heap[firstIdx].RightChildIdx] != null;
            if (thereIsRightChild)
            {
                heap[heap[firstIdx].RightChildIdx].ParentIdx = firstIdx;
            }

            thereIsParentIdx = heap[secondIdx].ParentIdx != -1 && heap[heap[secondIdx].ParentIdx] != null;
            if (thereIsParentIdx)
            {
                if (heap[heap[secondIdx].ParentIdx].LeftChildIdx == firstIdx)
                    heap[heap[secondIdx].ParentIdx].LeftChildIdx = secondIdx;
                else if (heap[heap[secondIdx].ParentIdx].RightChildIdx == firstIdx)
                    heap[heap[secondIdx].ParentIdx].RightChildIdx = secondIdx;
            }

            thereIsLeftChild = heap[secondIdx].LeftChildIdx != -1 && heap[heap[secondIdx].LeftChildIdx] != null;
            if (thereIsLeftChild)
            {
                heap[heap[secondIdx].LeftChildIdx].ParentIdx = secondIdx;
            }

            thereIsRightChild = heap[secondIdx].RightChildIdx != -1 && heap[heap[secondIdx].RightChildIdx] != null;
            if (thereIsRightChild)
            {
                heap[heap[secondIdx].RightChildIdx].ParentIdx = secondIdx;
            }

        }

        
    }

    class HeapNode
    {
        public Vertix MyVertix;
        public int MyIdx;
        public int ShortestDistance;
        public int RightChildIdx;
        public int LeftChildIdx;
        public int ParentIdx;

        public HeapNode(Vertix vertix)
        {
            if (vertix.Number == 1)
            {
                ShortestDistance = 0;
            }
            else
            {
                ShortestDistance = int.MaxValue;
            }
            MyVertix = vertix;
        }
    }
}
