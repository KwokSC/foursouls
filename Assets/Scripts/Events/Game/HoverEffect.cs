using System.Collections;
using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    public float hoverScaleFactor = 1.2f;
    public float hoverDuration = 0.5f;
    private Vector3 originalScale;
    private IEnumerator currentHoverCoroutine;

    void Start()
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
