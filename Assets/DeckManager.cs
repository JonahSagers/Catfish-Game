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
    public int lives;
    public List<Transform> hearts;
    public Transform UI;
    public Sprite gameOverSprite;
    public float score;

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
        card.transform.localPosition = Vector3.Lerp(card.transform.localPosition, Vector3.right * cardOffset - Vector3.forward, Time.deltaTime * 10);
        card.transform.rotation = Quaternion.Euler(0,0,card.transform.position.x * -5);
        card.transform.localScale = Vector3.Lerp(card.transform.localScale, Vector3.one * (Input.GetMouseButton(0) ? 0.9f : 1), Time.deltaTime * 10);
        if(lives > 0){
            health -= Time.deltaTime;
            health = Mathf.Clamp(health, 0, 10);
            if(hearts[lives - 1].localScale.x <= 0.4f){
                hearts[lives - 1].localScale = Vector3.one * 0.45f;
            } else {
                hearts[lives - 1].localScale -= Vector3.one * Time.deltaTime * 0.06f;
            }
        }
        hpBar.value = health;
    }

    void GameOver()
    {
        nextCard.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = gameOverSprite;
    }

    void Swipe(int direction)
    {
        cardOffset = 0;
        Rigidbody2D rb = card.GetComponent<Rigidbody2D>();
        rb.simulated = true;
        card.transform.parent = null;
        if(lives > 0){
            if((card.tag == "Real" && direction > 0)){
                health += 1;
            } else if((card.tag == "Catfish" && direction > 0)){
                hearts[lives - 1].gameObject.SetActive(false);
                health = 10;
                lives -= 1;
                if(lives == 0){
                    GameOver();
                }
            }
        } else {
            lives = 3;
            health = 10;
            score = 0;
            foreach(Transform heart in hearts){
                heart.gameObject.SetActive(true);
            }
        }
        rb.AddTorque(-direction / 2 * Random.Range(1, 4), ForceMode2D.Impulse);
        rb.AddForce(new Vector3(direction * 5 + Random.Range(-3, 3), 2 + Random.Range(-2, 2)), ForceMode2D.Impulse);
        StartCoroutine(FadeOut(card));
        DrawCard();
    }

    void DrawCard()
    {
        card = nextCard;
        card.transform.position -= transform.forward;
        nextCard = Instantiate(cardPre[Random.Range(0, cardPre.Count - 1)], transform);
    }

    public IEnumerator FadeOut(GameObject target){
        float elapsed = 0;
        while(elapsed < 2){
            target.transform.position -= transform.forward * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return 0;
        }
        Destroy(target);
    }
}
