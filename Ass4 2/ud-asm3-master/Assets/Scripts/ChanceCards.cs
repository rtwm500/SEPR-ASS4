using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChanceCards : MonoBehaviour {

	private int playerOneChanceCards = 5;							//Define a variable that will count player one's chance cards
	private int playerTwoChanceCards = 5;							//Define a variable that will count player two's chance cards
    private int playerThreeChanceCards = 5;                         //Define a variable that will count player three's chance cards

    private Game game;
	private Section section;
	private GameObject gameManager;
	
	public Text chanceCardsText;									//Define the text variable to display the chance card counter
    private GameObject cardImage;
    public Sprite[] cardSprites;
	public GameObject popup;
    public Text chanceCardsEffect;                                  //Define the variable to display the effects of the chance cards used
    private string targetSectors;
    private string effect;

	public Section[] lakeside;
	public Section[] heseast;
	public Section Lib;
	public GameObject CardImage;

	void Start(){
		gameManager = GameObject.Find("EventManager"); 				//Find the gameobject used to manage events
		game = gameManager.GetComponent<Game>();
		section = gameManager.GetComponent<Section>();
		chanceCardsText = GameObject.Find("CardText").GetComponent<Text>();
        cardImage = GameObject.Find("CardImage");
        chanceCardsEffect = GameObject.Find("CardEffect").GetComponent<Text>();

        if (Data.GameFromLoaded) {
            playerOneChanceCards = SaveGameHandler.loadedGame.ChanceCards[0];
            playerTwoChanceCards = SaveGameHandler.loadedGame.ChanceCards[1];
            playerThreeChanceCards = SaveGameHandler.loadedGame.ChanceCards[2];
        }

	}
	
	void Update(){
		UpdateChanceCardText();
	}
	
	private void UpdateChanceCardText(){						//Declare a function to update the on-screen chance card counter
		if (game.GetTurn() == 1) {
            cardImage.GetComponent<SpriteRenderer>().sprite = cardSprites[0];
            chanceCardsText.text = GetPlayerOneChance().ToString();
		} else if (game.GetTurn() == 2) {
            cardImage.GetComponent<SpriteRenderer>().sprite = cardSprites[1];
            chanceCardsText.text = GetPlayerTwoChance().ToString();
		} else if (game.GetTurn() == 3 && Data.RealPlayers == 3) {
            cardImage.GetComponent<SpriteRenderer>().sprite = cardSprites[2];
            chanceCardsText.text = GetPlayerThreeChance().ToString();
        }
    }
	
	public int GetPlayerOneChance(){
		return playerOneChanceCards;
	}
	
	public int GetPlayerTwoChance(){
		return playerTwoChanceCards;
	}

    public int GetPlayerThreeChance() {
        return playerThreeChanceCards;
    }

    public void SetPlayerOneChance(int val){
		playerOneChanceCards = val;
	}
	
	public void SetPlayerTwoChance(int val){
		playerTwoChanceCards = val;
	}

    public void SetPlayerThreeChance(int val) {
        playerThreeChanceCards = val;
    }

    public void OnClick(){								//Declare a function that will be called when the chance card icon is clicked
		popup.BroadcastMessage("Open",this.gameObject);
    }
	
	void FriendlySectors(){								//Declare a function to add gang members to friendly sectors
		Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
		int units = 4;									//Define the initial number of gang members to be added to each sector
		float rand = Random.Range(0f, 1f);				//Generate a random floating point number between 0 and 1
		float negChance = 0.1f;							//Define a float which will control the chance of card's effect being reversed
		
        if (rand < negChance)							//If the randomly generated number is less than the float 'negChance'
        {
            this.effect = "weakened";
            units *= -1;								//Negate the value of 'units'
        }
        else
        {
            this.effect = "fortified";
        }

		foreach (var sect in sections) {				//For each sector on the map
			if (sect.GetOwner() == game.GetTurn()) {	//If the sector belongs to the current player
				sect.AddUnits(units);					//Add 'units' to its current gang members
			}
		}
        this.targetSectors = "Friendly";
	}
	
	void AllSectors(){									//Declare a function to add gang members to all sectors
		Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
		int units = 2;									//Define the initial number of gang members to be added to each sector
		float rand = Random.Range(0f, 1f);				//Generate a random floating point number between 0 and 1
		float negChance = 0.1f;							//Define a float which will control the chance of card's effect being reversed
		
        if (rand < negChance)							//If the randomly generated number is less than the float 'negChance'
        {
            this.effect = "weakened";
            units *= -1;								//Negate the value of 'units'
        }
        else
        {
            this.effect = "fortified";
        }

		foreach (var sect in sections) {				//For each sector on the map
				sect.AddUnits(units);					//Add 'units' to its current gang members
		}
        this.targetSectors = "All";
	}

	public void usecard(bool chance){
		CardImage.SetActive (true);
		if (chance) {
			if (game.GetTurn () == 1) {						//If it is currently player 1's turn
				if (playerOneChanceCards > 0) {			//If player 1 has any chance cards
					playerOneChanceCards -= 1;				//Remove one chance card  from player 1
					float rand = Random.Range (0f, 3f);		//Generate a random floating point number from 0 to 3
					if (rand < 1) {							//If this number is less than 1
						FriendlySectors ();					//Call the function that adds gang members to friendly sectors
					} else if (1 < rand && rand < 2) {		//If the number is between 1 and 2
						AllSectors ();						//Call the function that adds gang members to all sectors
					} else {								//If the number is greater than 2
						EnemySectors ();						//Call the function that takes gang members from enemy sectors
					}
				}
			}
			if (game.GetTurn () == 2) {						//If it is currently player 2's turn
				if (playerTwoChanceCards > 0) {			//If player 1 has any chance cards
					playerTwoChanceCards -= 1;				//Remove one chance card  from player 1
					float rand = Random.Range (0f, 3f);		//Generate a random floating point number from 0 to 3
					if (rand < 1) {							//If this number is less than 1
						FriendlySectors ();					//Call the function that adds gang members to friendly sectors
					} else if (1 < rand && rand < 2) {		//If the number is between 1 and 2
						AllSectors ();						//Call the function that adds gang members to all sectors
					} else {								//If the number is greater than 2
						EnemySectors ();						//Call the function that takes gang members from enemy sectors
					}
				}
			}
			if (game.GetTurn () == 3 && Data.RealPlayers == 3) { 	//If it is currently player 3's turn and player 3 is not an AI player
				if (playerThreeChanceCards > 0) {					//If player 1 has any chance cards
					playerThreeChanceCards -= 1;					//Remove one chance card from player 3
					float rand = Random.Range (0f, 3f);				//Generate a random floating point number from 0 to 3
					if (rand < 1) {									//If this number is less than 1
						FriendlySectors ();							//Call the function that adds gang members to friendly sectors
					} else if (1 < rand && rand < 2) {				//If the number is between 1 and 2
						AllSectors ();								//Call the function that adds gang members to all sectors
					} else {										//If the number is greater than 2
						EnemySectors ();								//Call the function that takes gang members from enemy sectors
					}
				}
			}
			chanceCardsEffect.text = "CHANCE!\n" + targetSectors + " sectors\n" + effect;
		}else{
			switch (game.GetTurn()) { 
			case 1:
				playerOneChanceCards -= 1;
				break; 
			case 2:
				playerTwoChanceCards -= 1;
				break;
			case 3:
				playerThreeChanceCards -= 1;
				break;
			}
			punishmentcard ();
		} 
	}

	void EnemySectors(){								//Declare a function to take gang members from enemy sectors
		Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
		int units = -4;									//Define the initial number of gang members to be taken from each sector. Note that this is negative as this number will be added to the current values.
		float rand = Random.Range(0f, 1f);				//Generate a random floating point number between 0 and 1
		float negChance = 0.1f;							//Define a float which will control the chance of card's effect being reversed
		
        if (rand < negChance)							//If the randomly generated number is less than the float 'negChance'
        {
            this.effect = "fortified";
            units *= -1;								//Negate the value of 'units'
        }
        else
        {
            this.effect = "weakened";
        }

		foreach (var sect in sections) {				//For each sector on the map
			if (sect.GetOwner() != game.GetTurn()) {	//If the sector does not belong to the current player
				sect.AddUnits(units);					//Add 'units' to its current gang members
			}
		}
        this.targetSectors = "Enemy";
	} 

	void punishmentcard(){ 
		Section[] sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
		int rand = Random.Range (0,5); 
		switch (rand) {
		case 0:
			chanceCardsEffect.text = "PUNISHMENT!\n Strikes\n Lightning strikes 3 sectors";
			for (int i = 0; i < 3; i++) {
				int rsec = Random.Range (0, sections.Length); 
				sections [rsec].SetUnits (1);
			}
			break;
		case 1:
			chanceCardsEffect.text = "PUNISHMENT!\n Flood\n The lake claims 5 victims from each sector"; 
			foreach (Section sect in lakeside) {
				sect.AddUnits (-5);
			}
			break;
		case 2:
			chanceCardsEffect.text = "PUNISHMENT!\n Freshers Cooking";
			int rsec2 = Random.Range (0, sections.Length); 
			sections [rsec2].SetUnits (1);
			foreach (Section sect in sections [rsec2].adjacentSectors){
				sect.AddUnits (-5);
			}
			break;
		case 3:
			chanceCardsEffect.text = "PUNISHMENT!\n Isolation";
			foreach (Section sect in heseast) {
				sect.AddUnits (-10);
			}
			break;
		case 4:
			chanceCardsEffect.text = "PUNISHMENT!\n Finals";
			Lib.gameObject.SetActive (false);
			break;
		}
	}
}
	