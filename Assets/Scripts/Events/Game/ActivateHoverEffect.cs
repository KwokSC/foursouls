using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateHoverEffect : MonoBehaviour
{
    private Vector3 originalScale;
    public float hoverScaleFactor = 1.2f;
    public float hoverDuration = 0.5f;
    private IEnumerator currentHoverCoroutine;
    bool isActivated = false;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnMouseEnter()
    {
        if (!isActivated)
        {
            if (currentHoverCoroutine != null)
            {
                StopCoroutine(currentHoverCoroutine);
            }
            currentHoverCoroutine = ScaleCardSmoothly(originalScale * hoverScaleFactor);
            StartCoroutine(currentHoverCoroutine);
        }

    }

    public void OnMouseExit()
    {
        if (!isActivated)
        {
            if (currentHoverCoroutine != null)
            {
                StopCoroutine(currentHoverCoroutine);
            }
            currentHoverCoroutine = ScaleCardSmoothly(originalScale);
            StartCoroutine(currentHoverCoroutine);
        }

    }

    public void OnClick() {
        if (!isActivated)
        {
            isActivated = true;
            StartCoroutine(RotateAndShrink());
        }
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

    private IEnumerator RotateAndShrink()
    {
        float elapsedTime = 0f;
        Vector3 originalRotation = transform.eulerAngles;
        Quaternion targetRotation = Quaternion.Euler(originalRotation.x, originalRotation.y, originalRotation.z + 90f);

        while (elapsedTime < hoverDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale , elapsedTime / hoverDuration);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedTime / hoverDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        transform.rotation = targetRotation;
    }
}
