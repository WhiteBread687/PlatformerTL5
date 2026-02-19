using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class CoinManager : MonoBehaviour
{
    public int coincount;
    public int diecount;

    public Text coinText;
    public Text scoreText;

    public int coinscore;
    public int killscore;
    public int score;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coinText.text = "Coins Collected: " + coincount.ToString();

        coinscore = coincount * 10;
        killscore = diecount * 50;
        score = coinscore + killscore;

        scoreText.text = "Score: " + score.ToString();


    }
}
