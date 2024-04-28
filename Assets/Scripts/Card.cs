using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class Card : MonoBehaviour
{
    public int cardID;
    public bool isFlipped = false;
    public bool isMatched = false;
    private Image spriteRenderer;

    private Quaternion startRotation;
    private Quaternion endRotation;

    void Start()
    {
        spriteRenderer = GetComponent<Image>();
        startRotation = transform.rotation;
        endRotation = startRotation * Quaternion.Euler(0, 180, 0);
    }

    public void FlipCard(Sprite cardFront)
    {
        if (!isFlipped && !isMatched)
        {
            isFlipped = true;
            transform.DORotate(endRotation.eulerAngles, 0.5f);
            transform.DOScaleX(-1,0.5f);
            spriteRenderer.sprite = cardFront;
        }
    }
    public bool CheckMatch(Card otherCard)
    {
        return cardID == otherCard.cardID;
    }

    public IEnumerator UnflipCard(Sprite cardBack)
    {
        yield return new WaitForSeconds(0.5f);
        isFlipped = false;
        transform.DORotate(startRotation.eulerAngles, 0.5f);
        transform.DOScaleX(1, 0.5f);
        spriteRenderer.sprite = cardBack;
    }

    public void MarkAsMatched()
    {
        isMatched = true;
    }
}
