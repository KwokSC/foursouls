using System.Collections;
using UnityEngine;
using Mirror;

public class LootSelect : NetworkBehaviour
{
    private Vector3 originalLocalPosition;
    private IEnumerator currentHoverCoroutine;

    public float hoverHeight = 20f;
    public float transitionSpeed = 10f;
    public bool hoverEnabled;
    public bool isHovering;
    public bool isSelected;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
        hoverEnabled = true;
        isSelected = false;
    }

    public void OnMouseEnter()
    {
        if (isOwned) {
            if (currentHoverCoroutine != null)
            {
                StopCoroutine(currentHoverCoroutine);
            }

            if (hoverEnabled)
            {
                currentHoverCoroutine = MoveCardSmoothly(originalLocalPosition + new Vector3(0, hoverHeight, 0));
                StartCoroutine(currentHoverCoroutine);
            }
        }

    }

    public void OnMouseExit()
    {
        if (isOwned) {
            if (currentHoverCoroutine != null)
            {
                StopCoroutine(currentHoverCoroutine);
            }

            if (hoverEnabled)
            {
                currentHoverCoroutine = MoveCardSmoothly(originalLocalPosition);
                StartCoroutine(currentHoverCoroutine);
            }
        }
    }

    public void OnCardPosXChanged(float x) {
        transform.localPosition = new Vector3(x, 0, 0);
        originalLocalPosition.x = x;
    }

    public void OnClick() {
        if (isOwned) {
            isSelected = !isSelected;
            hoverEnabled = !hoverEnabled;
            if (isSelected)
            {
                if (currentHoverCoroutine != null)
                {
                    StopCoroutine(currentHoverCoroutine);
                }
                currentHoverCoroutine = MoveCardSmoothly(originalLocalPosition + new Vector3(0, hoverHeight, 0));
                StartCoroutine(currentHoverCoroutine);
            }
            else
            {
                if (currentHoverCoroutine != null)
                {
                    StopCoroutine(currentHoverCoroutine);
                }
                currentHoverCoroutine = MoveCardSmoothly(originalLocalPosition);
                StartCoroutine(currentHoverCoroutine);
            }
        }
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

