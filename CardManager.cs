using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    public bool isHomeRoom;

    public List<Sprite> SpriteList = new List<Sprite>();

    [SerializeField]private List<GameObject> buttonList = new List<GameObject>();
    [SerializeField] private List<GameObject> hiddenButtonList = new List<GameObject>();

    private List<GameObject> selectedCards = new List<GameObject>();

    private int lastMatchId;
    [SerializeField] private bool selected;
    [Header("How many pairs you want to play?")]
    public int pairs;
    [Header("Card Prefab Button:")]
    public GameObject cardPrefab;
    [Header("The Parent Spacer to sort Cards in:")]
    public Transform spacer;

    public int choice1;
    public int choice2;

    public GameObject menu;
    public GameObject trigger;
    public Animator shelvesOpen;
    public GameObject puzzleCompletedUI;
    public GameObject objAnim;
    public GameObject[] fuseBoxes; // 0 = inactive, 1 = active;

    public AudioSource confirm;

    public LightActivator lightActivator;

    public GameManager gm;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        FillPlayField();
        if (lightActivator == null) // this is for the first level so it doesn't throw an error.
        { }
    }


    void FillPlayField()
    {
        for (int i = 0; i < (pairs * 2); i++)
        {
            GameObject newCard = Instantiate(cardPrefab, spacer);
            buttonList.Add(newCard);
            hiddenButtonList.Add(newCard);
        }
        ShuffleCards();
    }

    void ShuffleCards()
    {
        int num = 0;
        int cardPairs = buttonList.Count / 2;

        for (int i = 0; i < cardPairs; i++)
        {
            num++;
            for (int j = 0; j < 2; j++) //COUNT CARD AMOUNT PER MATCH
            {
                int cardIndex = Random.Range(0, buttonList.Count);
                Card tempCard = buttonList[cardIndex].GetComponent<Card>();
                tempCard.id = num;
                tempCard.cardFront = SpriteList[num - 1];

                buttonList.Remove(buttonList[cardIndex]);
            }
        }
    }

    public void AddSelectedCard(GameObject card)
    {
        selectedCards.Add(card);
    }
        

    public IEnumerator CompareCards()
    {
        if (choice2 == 0 || selected)
        {
            yield break;
        }
        selected = true;
        yield return new WaitForSeconds(1f);
        //NO MATCH
        if ((choice1 != 0 && choice2 != 0) && (choice1 != choice2))
        {
            //FLIP BACK THE OPEN CARDS
            FlipAllBack();
        }
        else if((choice1 != 0 && choice2 != 0) && (choice1 == choice2))
        {
            lastMatchId = choice1;

            //REMOVE THE MATCH
            RemoveMatch();

            //CLEAR SELECTED CARDS
            selectedCards.Clear();
        }
        //RESET ALL CHOICES
        choice1 = 0;
        choice2 = 0;
        selected = false;

        //CHECK IF WON
        CheckWin();
    }

    void FlipAllBack()
    {
        foreach (GameObject card in selectedCards)
        {
            card.GetComponent<Card>().CloseCard();
        }
        selectedCards.Clear();
    }

    void RemoveMatch()
    {
        for (int i = hiddenButtonList.Count - 1; i >= 0; i--)
        {
            Card tempCard = hiddenButtonList[i].GetComponent<Card>();
            if (tempCard.id == lastMatchId)
            {
                //REMOVE CARD
                hiddenButtonList.RemoveAt(i);
            }
        }
    }

    void CheckWin()
    {
        if(hiddenButtonList.Count < 1)
        {
            //Debug.Log("Yay!");
            StartCoroutine(PuzzleCompletion());
        }
    }

    public void ClosePuzzle()
    {
        this.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        PlayerStats.ps.canMove = true;
    }

    public IEnumerator PuzzleCompletion()
    {
        gm.PuzzleEnd();
        puzzleCompletedUI.SetActive(true);
        confirm.Play();
        yield return new WaitForSeconds(1f);
        if (isHomeRoom)
        {
            objAnim.SetActive(true);
            fuseBoxes[0].SetActive(false);
            fuseBoxes[1].SetActive(true);
        }
        puzzleCompletedUI.SetActive(false);
        menu.SetActive(false);
        trigger.SetActive(false);
        shelvesOpen.SetBool("Open", true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        PlayerStats.ps.canMove = true;
    }
}
