using UnityEngine;

/// <summary>
/// Вспомогательный класс, который позволяет менять значение параметров вне диалога
/// </summary>
public class ScenarioParameterChanger : MonoBehaviour
{
    [Tooltip("Пакет параметров")] public ParameterPack pack;
    [Tooltip("Название параметра, который нужно изменить")] public string parameterName;
    [Tooltip("При указанном значении для целочисленных будет идти присвоение поля Value, иначе это значение поля будет прибавляться " +
        "к имеющемуся значению параметра")]
    public bool useNewValue;
    public bool boolValue;
    public int intValue;

    /// <summary>
    /// Изменить параметр
    /// </summary>
    [ContextMenu("Изменить параметр")]
    public void ChangeParameter()
    {
        if(pack.FindCondition(parameterName, out ScenarioParameter parameter))
        {
            if (parameter.type == ParameterType.Bool)
                parameter.boolValue = boolValue;
            else
            {
                if (useNewValue)
                    parameter.intValue = intValue;
                else
                    parameter.intValue += intValue;
            }
        }
        else
        {
            Debug.LogError(gameObject.name + " в пакете " + pack.name + "не найден параметр с именем " + parameterName);
        }
    }
}
