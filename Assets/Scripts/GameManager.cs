using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Gameplay Root")]
    public GameObject spriteHolder;

    [Header("Sprites")]
    public SpriteRenderer[] sprites;

    [Header("UI Panels")]
    public GameObject homePanel;
    public GameObject gamePanel;
    public GameObject resultPanel;

    [Header("UI Text")]
    public TMP_Text scoreText;
    public TMP_Text accuracyText;
    public TMP_Text bestReactionText;
    public TMP_Text finalText;

    [Header("Buttons")]
    public Button stopButton;

    int totalScore;
    int totalClicks;
    int correctClicks;
    int wrongClicks;

    float roundStartTime;
    float totalReactionTime;
    float bestReaction = 999f;
    float maxReactionTime = 1.5f;

    bool roundActive;
    bool gameRunning;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        stopButton.onClick.AddListener(StopGame);
        ShowHome();
    }

    void ShowHome()
    {
        homePanel.SetActive(true);
        gamePanel.SetActive(false);
        resultPanel.SetActive(false);
        spriteHolder.SetActive(false);
    }

    void ShowGame()
    {
        homePanel.SetActive(false);
        resultPanel.SetActive(false);
        gamePanel.SetActive(true);
        spriteHolder.SetActive(true);
    }

    void ShowResults()
    {
        gamePanel.SetActive(false);
        resultPanel.SetActive(true);
        spriteHolder.SetActive(false);

        float avgReaction = correctClicks > 0 ? totalReactionTime / correctClicks : 0f;

        finalText.text =
            "Score: " + totalScore +
            "\nCorrect: " + correctClicks +
            "\nWrong: " + wrongClicks +
            "\nAccuracy: " + ((float)correctClicks / Mathf.Max(1, totalClicks) * 100f).ToString("0") + "%" +
            "\nAvg Reaction: " + avgReaction.ToString("0.00") + "s" +
            "\nBest Reaction: " + (bestReaction < 999 ? bestReaction.ToString("0.00") + "s" : "-");
    }

    public void PlayGame()
    {
        StopAllCoroutines();
        ShowGame();

        totalScore = 0;
        totalClicks = 0;
        correctClicks = 0;
        wrongClicks = 0;
        totalReactionTime = 0;
        bestReaction = 999f;

        foreach (SpriteRenderer sr in sprites)
        {
            sr.color = new Color(0f, 1f, 0f, 0f);
            sr.transform.localScale = Vector3.one;
            sr.enabled = true;
        }

        UpdateUI();
        gameRunning = true;
        StartCoroutine(GameLoop());
    }

    public void StopGame()
    {
        gameRunning = false;
        roundActive = false;
        StopAllCoroutines();
        ShowResults();
    }

    IEnumerator GameLoop()
    {
        while (gameRunning)
        {
            foreach (SpriteRenderer sr in sprites)
            {
                sr.color = new Color(0f, 1f, 0f, 0f);
                sr.transform.localScale = Vector3.one;
            }

            yield return StartCoroutine(FadeAll(0f, 1f, 0.5f));

            foreach (SpriteRenderer sr in sprites)
                sr.color = Color.green;

            int red = Random.Range(0, sprites.Length);
            sprites[red].color = Color.red;

            roundStartTime = Time.time;
            roundActive = true;

            float timer = 0f;
            while (roundActive && timer < maxReactionTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (roundActive)
            {
                roundActive = false;
                totalClicks++;
                wrongClicks++;
                totalScore -= 20;
                if (totalScore < 0) totalScore = 0;
                UpdateUI();
            }

            yield return StartCoroutine(PopOut(0.25f));
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void SelectSprite(SpriteRenderer sr)
    {
        if (!roundActive || !gameRunning) return;

        roundActive = false;
        totalClicks++;

        float reaction = Time.time - roundStartTime;

        if (sr.color == Color.red)
        {
            correctClicks++;
            totalReactionTime += reaction;

            if (reaction < bestReaction)
                bestReaction = reaction;

            int points =
                reaction < 0.5f ? 100 :
                reaction < 1f ? 80 :
                reaction < 1.5f ? 60 :
                reaction < 2f ? 40 : 20;

            totalScore += points;
        }
        else
        {
            wrongClicks++;
            totalScore -= 30;
            if (totalScore < 0) totalScore = 0;
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + totalScore;

        float accuracy = totalClicks > 0 ? (float)correctClicks / totalClicks * 100f : 0;
        accuracyText.text = "Accuracy: " + accuracy.ToString("0") + "%";

        bestReactionText.text = "Best: " + (bestReaction < 999 ? bestReaction.ToString("0.00") + "s" : "-");
    }

    IEnumerator FadeAll(float from, float to, float time)
    {
        float t = 0f;
        while (t < time)
        {
            float a = Mathf.Lerp(from, to, t / time);
            foreach (SpriteRenderer sr in sprites)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
            t += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator PopOut(float time)
    {
        float t = 0f;
        while (t < time)
        {
            float s = Mathf.Lerp(1f, 0f, t / time);
            foreach (SpriteRenderer sr in sprites)
                sr.transform.localScale = Vector3.one * s;
            t += Time.deltaTime;
            yield return null;
            
        }
    }
    public void ExitGame()
    {
    Application.Quit();
    }

}
