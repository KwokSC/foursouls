using System.Collections;
using UnityEngine;

public class ActivateHoverEffect : MonoBehaviour
{
    public PlayerManager player;
    public float hoverScaleFactor = 1.2f;
    public float hoverDuration = 0.5f;
    private Vector3 originalScale;
    private IEnumerator currentHoverCoroutine;
    bool isActivated = false;

    private void Start()
    {
        originalScale = transform.localScale;
        switch (name) {
            case "Avatar":
                player = transform.parent.GetComponent<GamePlayerDisplay>().player;
                break;
            case "Item":
                player = GetComponent<Item>().player;
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
                    player.CmdActivateObject(gameObject);
                    break;
            }
            isActivated = true;
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
}
