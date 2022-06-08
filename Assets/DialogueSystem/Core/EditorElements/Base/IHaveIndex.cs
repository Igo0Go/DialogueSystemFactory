public interface IHaveIndex
{
    int Index { get; set; }
    void CheckIndexes(int removedIndex);
}
