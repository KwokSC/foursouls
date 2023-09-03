using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateHoverEffect : MonoBehaviour
{
    private Vector3 originalScale;
    public float hoverScaleFactor = 1.2f;
    public float hoverDuration = 0.2f;
    private IEnumerator currentHoverCoroutine;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnMouseEnter()
    {
        if (currentHoverCoroutine != null)
        {
            StopCoroutine(currentHoverCoroutine);
        }
        currentHoverCoroutine = ScaleCardSmoothly(originalScale * hoverScaleFactor);
        StartCoroutine(currentHoverCoroutine);
    }

    public void OnMouseExit()
    {
        if (currentHoverCoroutine != null)
        {
            StopCoroutine(currentHoverCoroutine);
        }
        currentHoverCoroutine = ScaleCardSmoothly(originalScale);
        StartCoroutine(currentHoverCoroutine);
    }

    public void OnClick() {
        transform.Rotate(Vector3.forward, 90f);
    }

    private IEnumerator ScaleCardSmoothly(Vector3 targetScale)
    {
        float elapsedTime = 0f;

        while (elapsedTime < hoverDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / hoverDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

}
