

namespace Framework
{
    public interface IBinaryHeapElement
    {
        float SortScore { get; }

        int HeapIndex { set; }

        void RebuildHeap<T>(BinaryHeap<T> heap) where T : IBinaryHeapElement;
    }
}