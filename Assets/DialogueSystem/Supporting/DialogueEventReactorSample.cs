using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueEventReactorSample : DialogueEventReactor
{
    public List<SampleModule> modules;

    public override void OnEvent()
    {
        foreach (var item in modules)
        {
            item.Use();
        }
    }
}

public abstract class SampleModule : MonoBehaviour //класс-пример
{
    /// <summary>
    /// каждый новый модуль может производить какое-то действие
    /// </summary>
    public abstract void Use();
}
