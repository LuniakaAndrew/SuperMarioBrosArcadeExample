using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHit : MonoBehaviour
{
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 2f;

    private Vector2 originalPosition;

    public Sprite emptyBlockSprite;

    private bool canBounce = true;

    //private Animator anim;
    // Use this for initialization
    void Start()
    {
        originalPosition = transform.localPosition;
        //anim = GetComponent<Animator>();
    }

    public void QuestionBlockBounce()
    {
        if (canBounce)
        {
            canBounce = false;
            StartCoroutine(Bounce());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ChangeSprite()
    {
        // anim.enabled = false;
        GetComponent<SpriteRenderer>().sprite = emptyBlockSprite;
    }


    IEnumerator Bounce()
    {

        ChangeSprite();
        while (true)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + bounceSpeed * Time.deltaTime);
            if (transform.localPosition.y >= originalPosition.y + bounceHeight)
            {
                break;
            }
            yield return null;
        }

        while (true)
        {
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - bounceSpeed * Time.deltaTime);
            if (transform.localPosition.y <= originalPosition.y + bounceHeight)
            {
                transform.position = originalPosition;
                break;
            }
            yield return null;
        }
        canBounce = true;
    }
}
