using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "IgoGoTools/Parameter")]
public class ParameterPack : ScriptableObject
{
    public List<ScenarioParameter> parametres = new List<ScenarioParameter>();

    #region Получение типа параметра и проверка

    /// <summary>
    /// Производит сравнение параметра с именем parameterName со значением value способом checkType.
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="checkType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Check(string parameterName, CheckType checkType, bool value)
    {
        if(FindCondition(parameterName, out ScenarioParameter condition))
        {
            switch(checkType)
            {
                case CheckType.Equlas:
                    return condition.boolValue == value;
                case CheckType.NotEqulas:
                    return condition.boolValue != value;
            }
            Debug.LogError("В пакет условий " + this.name + " передан не верный тип проверки " + checkType.ToString());
        }
        Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        return false;
    }
    /// <summary>
    /// Производит сравнение параметра с именем parameterName со значением value способом checkType.
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="checkType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Check(string parameterName, CheckType checkType, int value)
    {
        if (FindCondition(parameterName, out ScenarioParameter condition))
        {
            switch (checkType)
            {
                case CheckType.Equlas:
                    return condition.intValue == value;
                case CheckType.NotEqulas:
                    return condition.intValue != value;
                case CheckType.Greate:
                    return condition.intValue > value;
                case CheckType.Less:
                    return condition.intValue < value;
            }
            Debug.LogError("В пакет условий " + this.name + " передан не верный тип проверки " + checkType.ToString());
        }
        Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        return false;
    }
    /// <summary>
    /// Производит сравнение параметра с именем parameterNumber со значением value способом checkType.
    /// </summary>
    /// <param name="parameterNumber"></param>
    /// <param name="checkType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Check(int parameterNumber, CheckType checkType, bool value)
    {
        if (FindCondition(parameterNumber, out ScenarioParameter condition))
        {
            switch (checkType)
            {
                case CheckType.Equlas:
                    return condition.boolValue == value;
                case CheckType.NotEqulas:
                    return condition.boolValue != value;
            }
            Debug.LogError("В пакет условий " + this.name + " передан не верный тип проверки " + checkType.ToString());
        }
        Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        return false;
    }
    /// <summary>
    /// Производит сравнение параметра с именем parameterNumber со значением value способом checkType.
    /// </summary>
    /// <param name="parameterNumber"></param>
    /// <param name="checkType"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Check(int parameterNumber, CheckType checkType, int value)
    {
        if (FindCondition(parameterNumber, out ScenarioParameter condition))
        {
            switch (checkType)
            {
                case CheckType.Equlas:
                    return condition.intValue == value;
                case CheckType.NotEqulas:
                    return condition.intValue != value;
                case CheckType.Greate:
                    return condition.intValue > value;
                case CheckType.Less:
                    return condition.intValue < value;
            }
            Debug.LogError("В пакет условий " + this.name + " передан не верный тип проверки " + checkType.ToString());
        }
        Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        return false;
    }
    #endregion

    #region Получение и задание значений параметров
     
    /// <summary>
    /// Задаёт параметру с именем name значение value
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetBool(string name, bool value)
    {
        if (FindCondition(name, out ScenarioParameter condition))
        {
            condition.boolValue = value;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
    }
    /// <summary>
    /// Задаёт параметру с номером number значение value
    /// </summary>
    /// <param name="number"></param>
    /// <param name="value"></param>
    public void SetBool(int number, bool value)
    {
        if (FindCondition(number, out ScenarioParameter condition))
        {
            condition.boolValue = value;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
    }
    /// <summary>
    /// Задаёт параметру с именем name значение value
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void SetInt(string name, int value)
    {
        if (FindCondition(name, out ScenarioParameter condition))
        {
            condition.intValue = value;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
    }
    /// <summary>
    /// Задаёт параметру с номером number значение value
    /// </summary>
    /// <param name="number"></param>
    /// <param name="value"></param>
    public void SetInt(int number, int value)
    {
        if (FindCondition(number, out ScenarioParameter condition))
        {
            condition.intValue = value;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
    }

    /// <summary>
    /// Получает значени параметра с именем name из пакета.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool GetBool(string name)
    {
        if (FindCondition(name, out ScenarioParameter condition))
        {
            return condition.boolValue;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
        return false;
    }
    /// <summary>
    /// Получает значени параметра с номером number из пакета.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public bool GetBool(int number)
    {
        if (FindCondition(number, out ScenarioParameter condition))
        {
            return condition.boolValue;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
        return false;
    }
    /// <summary>
    /// Получает значени параметра с именем name из пакета.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public int GetInt(string name)
    {
        if (FindCondition(name, out ScenarioParameter condition))
        {
            return condition.intValue;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
        return 0;
    }
    /// <summary>
    /// Получает значени параметра с номером number из пакета.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public int GetInt(int number)
    {
        if (FindCondition(number, out ScenarioParameter condition))
        {
            return condition.intValue;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
        return 0;
    }

    #endregion

    #region Служебные методы

    /// <summary>
    /// Проверяет тип параметра с указанным индексом в пакете. Записывает его в переменную checkType. Если параметра с таким индексом нет, сообщает об ошибке в консоль. 
    /// </summary>
    /// <param name="number"></param>
    /// <param name="checkType"></param>
    /// <returns></returns>
    public bool GetType(int number, out ParameterType checkType)
    {
        checkType = ParameterType.Bool;
        if (FindCondition(number, out ScenarioParameter condition))
        {
            checkType = condition.type;
            return true;
        }
        else
        {
            Debug.LogError("В пакете условий " + this.name + " не найдено условие с именем " + name + ". Проверьте написание");
        }
        return false;
    }

    /// <summary>
    /// Выдаёт список имён всех параметров из пакета
    /// </summary>
    /// <returns></returns>
    public string[] GetCharacteristic()
    {
        string[] result = new string[parametres.Count];

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = parametres[i].parameterName;
        }
        return result;
    }
    
    /// <summary>
    /// Проверяет, есть ли параметр с указанным именем в пакете. Если нашёл, то запишет в parameter. 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool FindCondition(string name, out ScenarioParameter parameter)
    {
        parameter = null;
        foreach (var item in parametres)
        {
            if(item.parameterName.Equals(name))
            {
                parameter = item;
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Проверяет, есть ли параметр с указанным номером в пакете. Если нашёл, то запишет в parameter.
    /// </summary>
    /// <param name="number"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    public bool FindCondition(int number, out ScenarioParameter parameter)
    {
        parameter = null;
        if(parametres.Count >= number+1)
        {
            parameter = parametres[number];
            return true;
        }
        return false;
    }

    #endregion
}

