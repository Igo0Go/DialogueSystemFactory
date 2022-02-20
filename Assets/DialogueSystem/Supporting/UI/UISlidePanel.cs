using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISlidePanel : MonoBehaviour
{
    [SerializeField] private RectTransform slidePanel;
    [SerializeField, Range(0, 1)] private float horizontalBorderValue = 1f;
    [SerializeField, Range(0.01f, 1)] private float panelMovementSpeed = 1;
    private bool isOpen;

    private void Start()
    {
        HidePanel();
    }

    /// <summary>
    /// Открыть/закрыть панель
    /// </summary>
    [ContextMenu("Двигать панель")]
    public void MovePanelToggle()
    {
        isOpen = !isOpen;

    }

    /// <summary>
    /// Скрыть панель
    /// </summary>
    [ContextMenu("Открыть панель")]
    public void OpenPanel()
    {
        isOpen = true;
        StopAllCoroutines();
        StartCoroutine(MovePanelCoroutine(0));
    }

    /// <summary>
    /// Зкрыть панель
    /// </summary>
    [ContextMenu("Скрыть панель")]
    public void HidePanel()
    {
        isOpen = false;
        StopAllCoroutines();
        StartCoroutine(MovePanelCoroutine(-horizontalBorderValue * slidePanel.rect.width));
    }

    private IEnumerator MovePanelCoroutine(float hor)
    {
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * panelMovementSpeed;
            slidePanel.anchoredPosition = new Vector2(Mathf.Lerp(slidePanel.anchoredPosition.x, hor, t), 0);
            yield return null;
        }
        slidePanel.anchoredPosition = new Vector2(hor, 0);
    }
}
