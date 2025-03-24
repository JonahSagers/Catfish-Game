using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public List<GameObject> cardPre;
    public GameObject card;
    public GameObject nextCard;
    public int cardOffset;
    public float health;
    public Slider hpBar;

    //scoring notes
    //swiping incorrectly loses time
    //health decay increases with score
    //incorrect swipe reduces max health



    // Start is called before the first frame update
    void Start()
    {
        DrawCard();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)){
            cardOffset = -2;
        }
        if(Input.GetKeyDown(KeyCode.D)){
            cardOffset = 2;
        }
        if((Input.GetKeyUp(KeyCode.A) && cardOffset < 0) || (Input.GetKeyUp(KeyCode.D) && cardOffset > 0)){
            Swipe(cardOffset);
        }
        card.transform.localPosition = Vector2.Lerp(card.transform.localPosition, Vector2.right * cardOffset, Time.deltaTime * 10);
        card.transform.rotation = Quaternion.Euler(0,0,card.transform.position.x * -5);
        card.transform.localScale = Vector3.Lerp(card.transform.localScale, Vector3.one * (Input.GetMouseButton(0) ? 0.9f : 1), Time.deltaTime * 10);
        health -= Time.deltaTime;
        health = Mathf.Clamp(health, 0, 10);
        hpBar.value = health;
    }

    void Swipe(int direction)
    {
        cardOffset = 0;
        Rigidbody2D rb = card.GetComponent<Rigidbody2D>();
        rb.simulated = true;
        card.transform.parent = null;
        if((card.tag == "Real" && direction > 0) || (card.tag == "Catfish" && direction < 0)){
            health += 1;
        } else if((card.tag == "Catfish" && direction > 0)){
            health -= 10;
        }
        rb.AddTorque(-direction / 2 * Random.Range(1, 4), ForceMode2D.Impulse);
        rb.AddForce(new Vector2(direction * 5 + Random.Range(-3, 3), 2 + Random.Range(-2, 2)), ForceMode2D.Impulse);
        StartCoroutine(FadeOut(card));
        DrawCard();
    }

    void DrawCard()
    {
        card = nextCard;
        nextCard = Instantiate(cardPre[0], transform);
    }

    public IEnumerator FadeOut(GameObject target){
        yield return new WaitForSeconds(2);
        Destroy(target);
    }
}
