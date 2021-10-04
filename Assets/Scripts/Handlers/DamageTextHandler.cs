using TMPro;
using UnityEngine;

public class DamageTextHandler : MonoBehaviour
{
    [HideInInspector] public TextMeshPro damageText;

    private void Awake()
    {
        PlayPopupAnimation();
        damageText = transform.GetChild(0).GetComponent<TextMeshPro>();
    }

    void PlayPopupAnimation ()
    {
        LeanTween.moveY(gameObject, transform.position.y + 1.5f, 1.2f);
        LeanTween.delayedCall(0.6f, () => LeanTween.alpha(gameObject, 0, 0.6f)).setOnComplete(
                () => Destroy(gameObject)
            );
    }
}
