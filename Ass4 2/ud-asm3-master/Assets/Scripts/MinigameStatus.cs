using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameStatus : MonoBehaviour {

    [SerializeField] private bool active = false;
    private int player;

    [SerializeField] private GameObject PVCSprite;
    [SerializeField] private GameObject startGame;
    [SerializeField] private GameObject buffAwarded;
    [SerializeField] private GameObject buffNotAwarded;

    private Game game;
    private ChanceCards chanceCards;

    public bool GetActive() {
        return active;
    }

    public void SetActive(bool active) {
        this.active = active;

        if (active == true)
        {
            game.PauseTurnTimer();
        }
        else
        {
            game.UnpauseTurnTimer();
        }
    }

    public int GetPlayer() {
        return player;
    }

    public void SetPlayer(int player) {
        this.player = player;
    }

    void Start() {

        game = this.gameObject.GetComponent<Game>();
        chanceCards = GameObject.Find("CardUI").GetComponent<ChanceCards>();

    }

    public void ResetMinigame() {

        PVCSprite.SetActive(false);
        startGame.SetActive(true);
        buffAwarded.SetActive(false);
        buffNotAwarded.SetActive(false);

    }

    public void AwardMinigameBonus() {

        if (player == 1) {
            chanceCards.SetPlayerOneChance(chanceCards.GetPlayerOneChance() + 1);
        } else if (player == 2) {
            chanceCards.SetPlayerTwoChance(chanceCards.GetPlayerTwoChance() + 1);
        } else if (player == 3) {
            chanceCards.SetPlayerThreeChance(chanceCards.GetPlayerThreeChance() + 1);
        }

    }

}
