using System.Collections;
using UnityEngine;

public class CardHoverEffect : MonoBehaviour
{
    private Vector3 originalLocalPosition;
    private IEnumerator currentHoverCoroutine;

    public float hoverHeight = 20f;
    public float transitionSpeed = 10f;
    public bool isHovering;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void OnMouseEnter()
    {
        if (currentHoverCoroutine != null)
        {
            StopCoroutine(currentHoverCoroutine);
        }

        currentHoverCoroutine = MoveCardSmoothly(originalLocalPosition + new Vector3(0, hoverHeight, 0));
        StartCoroutine(currentHoverCoroutine);
    }

    public void OnMouseExit()
    {
        if (currentHoverCoroutine != null)
        {
            StopCoroutine(currentHoverCoroutine);
        }

        currentHoverCoroutine = MoveCardSmoothly(originalLocalPosition);
        StartCoroutine(currentHoverCoroutine);
    }

    public void OnCardPosXChanged(float x) {
        transform.localPosition = new Vector3(x, 0, 0);
        originalLocalPosition.x = x;
    }

    private IEnumerator MoveCardSmoothly(Vector3 targetLocalPosition)
    {
        while (Vector3.Distance(transform.localPosition, targetLocalPosition) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPosition, Time.deltaTime * transitionSpeed);
            yield return null;
        }

        transform.localPosition = targetLocalPosition;
    }
}

