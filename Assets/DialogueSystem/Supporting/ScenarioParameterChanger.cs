using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioParameterChanger : SampleModule
{
    [Tooltip("Пакет параметров")] public ParameterPack pack;
    [Tooltip("Название параметра, который нужно изменить")] public string parameterName;
    [Tooltip("При указанном значении для целочисленных будет идти присвоение поля Value, иначе это поле будет прибавляться к имеющемуся значению параметра")]
    public bool useNewValue;
    public bool boolValue;
    public int intValue;

    public override void Use()
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
