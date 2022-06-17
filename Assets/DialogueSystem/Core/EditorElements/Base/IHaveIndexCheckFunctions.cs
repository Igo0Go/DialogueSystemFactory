public interface IHaveIndexCheckFunctions
{
    /// <summary>
    /// —корректировать индексы после удалени€ узла с указанным индексом 
    /// (¬се индексы большие удалЄнного будут снижатьс€ на единицу, занима€ место удалЄнного)
    /// </summary>
    /// <param name="removedNodeIndex">индекс удалЄнного узла</param>
    void CheckIndexesAfterRemovingNodeWithIndex(int removedNodeIndex);

    /// <summary>
    /// —корректировать индексы после вставки узла в указанное место списка 
    ///(используетс€ при откате операции удалени€. ”зел возвращаетс€ назад с возвращением индекса.
    /// ¬се индексы, большие или равные индексу вставл€емого, будут увеличены на единицу, освобожда€ ему место)
    /// </summary>
    /// <param name="insertedNodeIndex">»ндекс вставл€емого узла</param>
    void CheckIndexesAfterInsertingNodeWithIndex(int insertedNodeIndex);
}
