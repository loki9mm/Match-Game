using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : MonoBehaviour
{
    public Sprite cardBack;
    public Sprite[] cardFronts;
    public Transform cardHolder;
    public GameObject cardPrefab;
    public TextMeshProUGUI scoreText;
    int _score = 0;
    private int gridSizeX, gridSizeY;

    public Card firstFlippedCard, secondFlippedCard;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            string size = PlayerPrefs.GetString("size");
            string[] gridSize = size.Split("x");
            gridSizeX = Convert.ToInt32(gridSize[0]);
            gridSizeY = Convert.ToInt32(gridSize[1]);

            InitializeCards();
        }
    }

    private void InitializeCards()
    {
        int totalCards = gridSizeX * gridSizeY;
        int pairsNeeded = totalCards / 2;

        GridLayoutGroup gridLayout = cardHolder.GetComponent<GridLayoutGroup>();
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        gridLayout.constraintCount = gridSizeX;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = gridSizeY;

        //float cellWidth = gridLayout.cellSize.x + gridLayout.spacing.x;
        //float cellHeight = gridLayout.cellSize.y + gridLayout.spacing.y;

        float totalWidth = cardHolder.GetComponent<RectTransform>().rect.width;
        float totalHeight = cardHolder.GetComponent<RectTransform>().rect.height;

        // Calculate default aspect ratio
        float defaultAspectRatio = gridLayout.cellSize.x / gridLayout.cellSize.y;

        // Calculate cell size dynamically based on grid size while maintaining aspect ratio
        int rows = gridSizeX;
        int columns = gridSizeY;

        float aspectRatio = totalWidth / totalHeight;
        float cellWidth;
        float cellHeight;

        if (aspectRatio > defaultAspectRatio)
        {
            // Fit to height
            cellHeight = totalHeight / rows - (gridLayout.spacing.y * (rows - 1));
            cellWidth = cellHeight * defaultAspectRatio;
        }
        else
        {
            // Fit to width
            cellWidth = totalWidth / columns - (gridLayout.spacing.x * (columns - 1));
            cellHeight = cellWidth / defaultAspectRatio;
        }

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);


        ShuffleCards(cardFronts);

        for (int i = 0; i < pairsNeeded; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int row = i / gridSizeX;
                int col = i % gridSizeX;

                float posX = col * cellWidth;
                float posY = row * cellHeight;
                Vector3 spawnPosition = new Vector3(posX, posY, 0f);

                GameObject card = Instantiate(cardPrefab, cardHolder);
                card.transform.localPosition = spawnPosition;

                Card cardScript = card.GetComponent<Card>();
                cardScript.cardID = i;
                Debug.LogError(cardScript.cardID);
                cardScript.GetComponent<Button>().onClick.RemoveAllListeners();
                cardScript.GetComponent<Button>().onClick.AddListener(() =>
                {
                    cardScript.FlipCard(cardFronts[cardScript.cardID]);

                    OnCardClicked(cardScript);
                });
            }
        }
    }
    private void OnCardClicked(Card clickedCard)
    {
        if (!firstFlippedCard)
        {
            firstFlippedCard = clickedCard;
        }
        else if (!secondFlippedCard && firstFlippedCard != clickedCard)
        {
            secondFlippedCard = clickedCard;
            if (firstFlippedCard.CheckMatch(secondFlippedCard))
            {
                Debug.Log("Matched! You earned 1 point.");
                UpdateScore(1);
            }
            else
            {
                Debug.Log("Not matched! You lost 1 point.");
                StartCoroutine(firstFlippedCard.UnflipCard(cardBack));
                StartCoroutine(secondFlippedCard.UnflipCard(cardBack));
                UpdateScore(-1);
            }
            //firstFlippedCard = null;
            //secondFlippedCard = null;
        }

        // Flip the clicked card
        //clickedCard.FlipCard(clickedCard.cardFront);
    }
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void ShuffleCards(Sprite[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Sprite temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
    public void UpdateScore(int score) 
    {
        _score = _score + score;
        scoreText.text = _score.ToString();

        firstFlippedCard = null;
        secondFlippedCard = null;
    }
    public void BackToMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
    }
}
