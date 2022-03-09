using UnityEngine;

/// <summary>
/// Узел - реплика
/// </summary>
[System.Serializable]
public class ReplicaNode : DialogueNode
{
    #region Поля и свойства
    /// <summary>
    /// Следующий узел
    /// </summary>
    public int NextNodeNumber
    {
        get
        {
            return nextNodesNumbers[0];
        }
        set
        {
            nextNodesNumbers[0] = value;
        }
    }

    /// <summary>
    /// Совокупность всей информации о реплике
    /// </summary>
    public ReplicInfo replicaInformation;

    /// <summary>
    /// Смещение выхода
    /// </summary>
    public readonly Vector2 exitPointOffset = new Vector3(130, 21);
    #endregion

    #region Конструкторы

    /// <summary>
    /// Создать узел реплики с указанным индексом в указанных координатах
    /// </summary>
    /// <param name="pos">координаты узла в схеме</param>
    /// <param name="index">индекс узла</param>
    public ReplicaNode(Vector2 pos, int index) : base(pos, index)
    {
        transformRect = new Rect(pos.x, pos.y, 150, 50);
        replicaInformation = new ReplicInfo();
        nextNodesNumbers.Add(-1);
    }

    private ReplicaNode(){}

    #endregion
}

/// <summary>
/// Класс с информацией по реплике. Применяется как самих репликах, так и в выборах.
/// </summary>
[System.Serializable]
public class ReplicInfo
{
    /// <summary>
    /// Реплику уже чиатали
    /// </summary>
    public bool alreadyUsed;

    /// <summary>
    /// Ролевой пакет говорящего
    /// </summary>
    public DialogueCharacter character;

    /// <summary>
    /// Аудиоклип озвучки реплики (может быть пустым, если вы не используете озвучку в диалоге)
    /// </summary>
    public AudioClip clip;

    /// <summary>
    /// Анимационное состояние говорящего во время реплики
    /// </summary>
    public DialogueAnimType animType;

    /// <summary>
    /// Позиция ракурса камеры во время реплики
    /// </summary>
    public int camPositionNumber;

    /// <summary>
    /// Текст субтитров
    /// </summary>
    public string replicaText;

    /// <summary>
    /// Создать пакет информации о реплике. Она будет помечена как непрочитанная
    /// </summary>
    public ReplicInfo()
    {
        alreadyUsed = false;
    }
}
