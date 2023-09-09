using System.Collections;
using UnityEngine;

public class ActivateHoverEffect : MonoBehaviour
{
    public PlayerManager player;
    public float hoverScaleFactor = 1.2f;
    public float hoverDuration = 0.5f;
    private Vector3 originalScale;
    private IEnumerator currentHoverCoroutine;
    public bool isActivated;

    private void Start()
    {
        originalScale = transform.localScale;
        switch (name) {
            case "Avatar":
                player = transform.parent.GetComponent<GamePlayerDisplay>().player;
                isActivated = player.isActivated;
                break;
            case "Item":
                player = GetComponent<Item>().player;
                isActivated = GetComponent<Item>().isActivated;
                break;
        }
    }

    public void OnMouseEnter()
    {
        if (!isActivated && player.isLocalPlayer)
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
        if (!isActivated && player.isLocalPlayer)
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
        if (!isActivated && player.isLocalPlayer)
        {
            if (currentHoverCoroutine != null)
            {
                StopCoroutine(currentHoverCoroutine);
            }
            switch (name) {
                case "Avatar":
                    player.CmdActivateCharacter();
                    break;
                case "Item":
                    player.CmdActivateItem(gameObject);
                    break;
            }
        }
    }

    public void OnIsActivatedChanged(bool isActivated) {
        StartCoroutine(RotateCard(isActivated));
    }

    IEnumerator ScaleCardSmoothly(Vector3 targetScale)
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

    IEnumerator RotateCard(bool isActivated)
    {
        float elapsedTime = 0f;
        Vector3 originalRotation = transform.eulerAngles;
        Quaternion targetRotation = isActivated ? Quaternion.Euler(originalRotation.x, originalRotation.y, originalRotation.z + 90f) : Quaternion.Euler(Vector3.zero);
        while (elapsedTime < hoverDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, elapsedTime / hoverDuration);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, elapsedTime / hoverDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = Vector3.one;
        transform.rotation = targetRotation;
    }
}
