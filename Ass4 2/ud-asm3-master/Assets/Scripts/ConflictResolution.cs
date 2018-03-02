using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI;
using System;   


public class ConflictResolution : MonoBehaviour {

    /* For clarity, the following fields have been renamed:
     * 
     *      IF          ->  inputFieldObject
     *      IFT         ->  inputFieldText
     *      SB          ->  goButton
     *      UD          ->  backButton
     *      CmbDsc      ->  combatDescription
     *      GmInstrctns ->  gameIntstructions
     *      AttOwn      ->  attackingPlayer
     *      orgUnits    ->  initialUnits
     *      attUnits    ->  attackingUnits
     *      DefOwn      ->  defendingPlayer
     *      defUnits    ->  defendingUnits
     *      Sectors     ->  sectors
     *      InvUnitNumGO->  invalidNumberOfUnitsPopup
     *      RiskyMoveGO ->  riskyMovePopup
     * 
     * The following fields have also been refactored:
     * 
     *      GameObject AttSec   ->  Section attackingSector
     *      GameObject[] AttOps ->  Section[] sectorsAdjacentToAttackingSector
     *      GameObject DefSec   ->  Section defendingSector
     * 
     * To support new features, the following fields have been added:
     *      
     *      gameManager
     *      chanceCards
     *      game
     *      assignUnits
     */ 

    private GameObject gameManager;
	public GameObject convpostgrads;
    private GameObject inputFieldObject; 
    private Text inputFieldText;                            //Publicly define the text component of the input field

    private GameObject goButton; 
    private GameObject backButton;  						//Define gameobjects for the input field, submit button and undo button
	
    public Text combatDescription;  						//Publicly define a text component used for combat description
	public Text gameInstructions;							//Publicly define a text component used for game instructions 

    private int mode;  										//define a integer used for controlling the current mode conflict resolution is in 
															//this is used for differentiating if user is picking a sector to move units to or from
	
    private Section attackingSector; 							//Define a variable that will hold the game object of the attacking sector
    private Section[] sectorsAdjacentToAttackingSector;         //Define a variable that will hold the neighbouring sectors of the attacking sector
    private int attackingPlayer;                                //Define a variable that will hold the owner of the attacking sector                                
    private int initialUnits;                               	//Define a variable that will hold the original number of units on the attacking sector
	private int initialPostgrads;
	private int movingPostgrads;
	private int nextPostgrads;
	private int attackingUnits;                                 //Define a variable that will hold the number of units being used to attack

    private Section defendingSector; 							//Define a variable that will hold the game object of the defending sector
    private int defendingPlayer;								//Define a variable that will hold the owner of the defending sector
    private int defendingUnits; 								//Define a variable that will hold the number of units on the defending sector

    private GameObject[] sectors;									//Define a variable that will hold the gameobject of every sector on the map
	public GameObject invalidNumberOfUnitsPopup;				//Publicly define a variable that is attached to the script generating a popup for if an invalid number of units is selected
	public GameObject riskyMovePopup;							//Publicly define a variable that is attached to the script generating a popup for if a user selects an attack with low chance of success

	private ChanceCards chanceCards;
	private Game game;
	private AssignUnits assignUnits;

    private Button settingsButton;
    private Button helpButton;
    private Button chanceCardButton;

    private GameObject minigame;
    private MinigameStatus minigameStatus;

    public int GetMode(){
        return mode;
    }

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("EventManager"); 	//Find the gameobject used to manage events 

        sectors = GameObject.FindGameObjectsWithTag("Sector"); 	//Find all gameobjects of sectors
		
        inputFieldObject = GameObject.Find("UnitsTextbox");  					//Find the gameobject of the input field
        inputFieldText = inputFieldObject.GetComponentInChildren<UnityEngine.UI.Text> ();//Find the text compontnt of the input field
        goButton = GameObject.Find("GoButton");   					//Find the gameobject of the submit button
		backButton = GameObject.Find ("BackButton");				//Find the gameobject of the submit button
		
        mode = 1;  													//Set the mode to 1...where the attacking sector is picked (where units are moved from)
		
        inputFieldObject.SetActive(false); 							//Disable (and hide) the input field
		goButton.SetActive(false); 									//submit button 
		backButton.SetActive (false);								//and undo button
		
        gameInstructions.text =  "Pick a sector to attack with..."; //Set the game instructions to tell user next action they should take
		
		chanceCards = GameObject.Find("CardUI").GetComponent<ChanceCards>();
        assignUnits = this.gameObject.GetComponentInChildren<AssignUnits>();

        settingsButton = GameObject.Find("SettingsButton").GetComponent<Button>();
        helpButton = GameObject.Find("HelpButton").GetComponent<Button>();
        chanceCardButton = GameObject.Find("CardButton").GetComponent<Button>();

        minigame = GameObject.Find("Minigame");
        minigame.SetActive(false);
        minigameStatus = this.gameObject.GetComponent<MinigameStatus>();
	}
	
    /* For clarity, the parameter 'U' has been renamed 'units'
     */
    void SetUnits(int units){ 
		if (mode == 1) { 										//If this is the first sector that has been picked
			initialUnits = units;										//set original units to the number passed (number of units on sector clicked)
			gameInstructions.text =  "How many units? Max: " + (units-1).ToString();	//Set the game instructions to tell user next action they should take
		}
		if (mode == 3) { 								//If this is the second sector to be picked 
			defendingUnits = units;								//set defending units to the number passed (number of units on sector clicked)
		}
	}  
	void SetPostgrads(int postgrads){
		if (mode == 1) { 										//If this is the first sector that has been picked
			initialPostgrads = postgrads;										//set original units to the number passed (number of units on sector clicked)
		}
		if (mode == 4) { 								//If this is the second sector to be picked 
			nextPostgrads = postgrads;								//set defending units to the number passed (number of units on sector clicked)
		}
	}

    /* For clarity, the method 'SetOwn' has been renamed 'SetPlayer',
     * and the parameter 'owner' has been renamed 'player'
     */
    void SetPlayer(int player){ 
		if (mode == 1) { 								//If this is the first sector that has been picked
			attackingPlayer = player;								//Set the attacking owner to the owner of the sector picked
		}
		if ((mode == 3)|(mode == 4)) { 								//If this is the second sector that has been picked
			defendingPlayer = player;								//Set the defending owner to the owner of the sector picked
		}
	}

    /* For clarity, the method, SetAttOps has been renamed 'SetAdjacentSectors',
     * and the parameter 'GameObject[] Ops' has been refactored to 
     * 'Section[] adjacentSectors'
     */
    void SetAdjacentSectors(Section[] adjacentSectors){ 
		if (mode == 1) {								//If this is the first sector that has been picked
			sectorsAdjacentToAttackingSector = adjacentSectors;								//set attacking options to all the neighbouring sectors of the clicked one
		}
	}

    /* The field 'GameObject Sector' has been refactored to 'Section sector'
     */
    void SelectSector(Section sector){
		
        switch (mode) {
            case 1:												//If this is the first sector that has been picked
			    
                attackingSector = sector; 								//store the gameobject in attacking sector
			    mode = 2; 										//update the mode to 2...where users should enter number of units they intend to move/attack with
			    
                inputFieldObject.SetActive (true); 						//Enable (and show) the input field
			    goButton.SetActive (true);  							//submit button 
			    backButton.SetActive (true);							//and undo button
			    
                break;
		
            case 3:   											//If this is the second sector that has been picked

                /* For clarity, the variable 'ValidOp' was renamed to
                 * 'sectorIsAdjacentToAttackingSector'
                 */
                bool sectorIsAdjacentToAttackingSector = false;							//Check if the second sector clicked is neighbouring the first sector clicked
			    
                foreach (Section G in sectorsAdjacentToAttackingSector) {
				    if (sector == G) { 						
					    sectorIsAdjacentToAttackingSector = true;
				    }
			    }
			    
                if (sectorIsAdjacentToAttackingSector) { 								//If it is 
				    
                    defendingSector = sector;						//update defending sector to the second sector clicked
				    
                    if (defendingPlayer != attackingPlayer) {					//check if it is a friendly sector
					    
                        if (attackingUnits < (defendingUnits * 0.66)) {  			//check if attacking units are less than 2/3 of the defending (unfriendly) units and if they are
						    
                            riskyMovePopup.BroadcastMessage ("SetAttackingUnits", attackingUnits);	//send details to a popup checking this is the move users meant to make
						    riskyMovePopup.BroadcastMessage ("SetDefendingUnits", defendingUnits);
						    riskyMovePopup.BroadcastMessage ("Open");	//and open the popup
					    
                        } 
						else {  								//If the attacking units are more than 2/3 of the defending units 
						    ResolveConflict (); 							//call the function to resolve the conflict
                        } 
				    
                    }
					else {  								
					    ResolveConflict (); 							//Friendly section to friendly section movement
                    } 
			    
                } 
				else {                                  //If the second sector is not a neighbour of the first
                    attackingSector.Flash(); //make the neighbouring sectors of the first sector flash, to remind the users where they can move units to
			    }
			    
                break;
			case 4: 
			sectorIsAdjacentToAttackingSector = false;							//Check if the second sector clicked is neighbouring the first sector clicked

			foreach (Section G in sectorsAdjacentToAttackingSector) {
				if (sector == G) { 						
					sectorIsAdjacentToAttackingSector = true;
				}
			}

			if (sectorIsAdjacentToAttackingSector) { 								//If it is 
				
				defendingSector = sector;						//update defending sector to the second sector clicked
			
				if (defendingPlayer != attackingPlayer) {					//check if it is a friendly sector
					while ((nextPostgrads > 0)&(movingPostgrads>0)) {

						attackingSector.AddPostgrads (-1); 
						defendingSector.AddPostgrads (-1); 
						nextPostgrads -= 1;
						movingPostgrads -= 1; 
					
					}
				} 
				else {  	
					attackingSector.AddPostgrads (-movingPostgrads); 
					defendingSector.AddPostgrads (+movingPostgrads);	 
				} 

			} 
			else {                                  //If the second sector is not a neighbour of the first
				attackingSector.Flash(); //make the neighbouring sectors of the first sector flash, to remind the users where they can move units to
			}
			mode=1; 											//After sorting out units and owners
			convpostgrads.SetActive(false);
			Renderer renderer = attackingSector.GetComponent<SpriteRenderer> (); 
			Color color; 													
			color = renderer.material.color; 					//Update the transparency of the attacking sector to be semi-transparent
			color.a = 0.5f; 
			renderer.material.color = color; 	  

			gameInstructions.text = "Pick a sector to attack with..."; //Update instructions to show user next step
			break;

            default: 
			    break;
		    }
	}

	public void UndoPress(){ 
        
		switch(mode){
		    case 2:												//When undo is clicked in mode 2
			    
                mode = 1; 										//set mode to 1
			    
                inputFieldObject.SetActive(false); 								//Disable (and hide) the input field
			    goButton.SetActive(false); 										//submit button 
			    backButton.SetActive (false);									//and undo button
			    
                gameInstructions.text =  "Pick a sector to attack with..."; 	//Set the game instructions to tell user next action they should take   
			    
                break;
		    
            case 3: 											//When undo is clicked in mode 3
			
                mode = 2; 										//set mode to 2
				convpostgrads.SetActive(false);
                inputFieldObject.SetActive (true); 						//Enable (and show) the input field
			    goButton.SetActive (true);  							//submit button 
			    backButton.SetActive (true);							//and undo button
			    
                gameInstructions.text =  "How many units? Max: " + (initialUnits-1).ToString();   
			    
                Renderer renderer = attackingSector.GetComponent<SpriteRenderer> (); //gets its renderer, 
			    Color color; 													//and colour.
			    color = renderer.material.color; 
			    color.a = 0.5f; 								//ensure that the attacking sector is semi-transparent
			    renderer.material.color = color; 
			    
                break;
		case 4: 
			goto case 3; 
			break;
		} 
	
	}
		
	public void SubmitPress(){ 									//When submit is pressed
		if (Input.GetKey(KeyCode.LeftShift)==false)
		{
        attackingUnits = Convert.ToInt32(inputFieldText.text);  				//set the number of attacking units to the text the user has just entered and submitted
		
        if ((attackingUnits < initialUnits)&(attackingUnits > 0)) {  		//Check that the number of units the user is attacking with is positive but less than the number of units on the sector
		
            mode = 3;										//move the mode to 3...where users pick the sector they wish to attack/move units to 
			attackingSector.Flash();			//make the neighbouring sectors of the first sector flash...so user knows where they cna move units to
			
            Renderer renderer = attackingSector.GetComponent<SpriteRenderer> (); 
			Color color; 									//Change transparency of attacking sector so that it is fully opaque
			color = renderer.material.color; 				//so that user knows what sector they are moving units from
			color.a = 1; 
			renderer.material.color = color; 	 
			
            inputFieldObject.SetActive(false); 							//Disable (and hide) the input field
			goButton.SetActive(false); 							//and the submit button 
			convpostgrads.SetActive(true);
			gameInstructions.text = "Pick a sector to move units to..."; //Update instructions to show user next step
		
        } 
		else {  
			invalidNumberOfUnitsPopup.BroadcastMessage ("Open");			//If the user has selected an invalid number bring up a popup to give them information
		}
		}else{
			movingPostgrads = Convert.ToInt32(inputFieldText.text);  				//set the number of attacking units to the text the user has just entered and submitted

			if ((movingPostgrads <= initialPostgrads)&(movingPostgrads > 0)) {  		//Check that the number of units the user is attacking with is positive but less than the number of units on the sector

				mode = 4;										//move the mode to 3...where users pick the sector they wish to attack/move units to 
				attackingSector.Flash();			//make the neighbouring sectors of the first sector flash...so user knows where they cna move units to

				Renderer renderer = attackingSector.GetComponent<SpriteRenderer> (); 
				Color color; 									//Change transparency of attacking sector so that it is fully opaque
				color = renderer.material.color; 				//so that user knows what sector they are moving units from
				color.a = 1; 
				renderer.material.color = color; 	 

				inputFieldObject.SetActive(false); 							//Disable (and hide) the input field
				goButton.SetActive(false); 							//and the submit button 
				gameInstructions.text = "Pick a sector to move PostGrads to..."; //Update instructions to show user next step
			}else {  
				invalidNumberOfUnitsPopup.BroadcastMessage ("Open");			//If the user has selected an invalid number bring up a popup to give them information
			}
		}
	}

    /* To support new features, the field 'conflictOccurs' was added
     */
	void ResolveConflict() { 										//When it is time for the conflict to be resolved
		
        bool conflictOccurs;

        backButton.SetActive (false);								//Disable (and hide) the undo button
        System.Random random = new System.Random ();	 			
        String combatDescriptionString;
		attackingSector.BroadcastMessage ("TakeUnits", attackingUnits);	//Subtract the units attacking from the original number of units on the attacking sector
		
        if (defendingPlayer != attackingPlayer) {								//If the owners of the two sectors are different the resolution is needed 
										                                        //rather than just subtracting the units from one and adding to the other
            conflictOccurs = true;

            combatDescriptionString = "ATT   DEF\n " + attackingUnits.ToString ().PadRight(2) + " vs " + defendingUnits.ToString (); //Update combat description to show initial state of conflict
			
            while ((attackingUnits > 0) && (defendingUnits > 0)) {   	//Keep iterating until either the attacking units or defending units are all gone

                int attackingRandomInteger = random.Next (0, attackingUnits + 1); 		//Pick a random number between 0 and the current number of attacking units (call this attacking random integer)
                int defendingRandomInteger = random.Next (0, defendingUnits + 1); 		//Pick a random number between 0 and the current number of defending units (call this defending random integer)
				
                combatDescriptionString += "\n -" + defendingRandomInteger.ToString().PadRight(2) + " || -" + attackingRandomInteger.ToString(); //Update the combat description to show the random numbers selected
				
                attackingUnits = attackingUnits - defendingRandomInteger; 				//take the defending random integer from the attacking units
				defendingUnits = defendingUnits - attackingRandomInteger;  				//take the attacking random integer from the defending units
				
                combatDescriptionString += "\n  " + Math.Max(attackingUnits,0).ToString ().PadRight(2) + " vs " + Math.Max(defendingUnits,0).ToString (); //Update combat description to show remaining units
			} 												//When conflict is resolved
			
            defendingSector.BroadcastMessage ("SetUnits", defendingUnits); //Update units on defending sector to the remaining number of defending units
			
            if (attackingUnits > 0) { 							//If there are any attacking units left - attack has won
				
                defendingSector.BroadcastMessage ("SetUnits", attackingUnits); 		//Update units on defending sector to the remaining number of attacking units
				defendingSector.BroadcastMessage ("SetOwner", attackingPlayer);		//Update owner on defending sector to the owner of the attacking sector
				
                if (attackingPlayer == 1) {											//Add one chance card to the attacking player
					chanceCards.SetPlayerOneChance(chanceCards.GetPlayerOneChance() + 1);
					if (defendingSector.PVCHere == true) {
                        chanceCards.SetPlayerOneChance(chanceCards.GetPlayerOneChance() + 1);
						//PLAY MINI GAME FOR PLAYER 1
                        PlayMinigame(1);
                        defendingSector.PVCHere = false;
						assignUnits.SpawnPVC();
					}
				} 
				else if (attackingPlayer == 2) {
					chanceCards.SetPlayerTwoChance(chanceCards.GetPlayerTwoChance() + 1);
					if (defendingSector.PVCHere == true) {
                        chanceCards.SetPlayerTwoChance(chanceCards.GetPlayerTwoChance() + 1);
						//PLAY MINI GAME FOR PLAYER 2
                        PlayMinigame(2);
						defendingSector.PVCHere = false;
						assignUnits.SpawnPVC();
					}
				} 
                else if (attackingPlayer == 3 && Data.RealPlayers == 3) {
                    chanceCards.SetPlayerThreeChance(chanceCards.GetPlayerThreeChance() + 1);
                    if (defendingSector.PVCHere == true) {
                        chanceCards.SetPlayerThreeChance(chanceCards.GetPlayerThreeChance() + 1);
                        //PLAY MINI GAME FOR PLAYER 3
                        PlayMinigame(3);
                        defendingSector.PVCHere = false;
                        assignUnits.SpawnPVC();
                    }
                }
            }
			
            combatDescription.text = combatDescriptionString;							//Update the actual text box to show the combat description
		
        }
		else { 									//If the owners of the two sectors are the same 
			
            conflictOccurs = false;
            defendingSector.BroadcastMessage ("AddUnits", attackingUnits); //Just add the number of units taken from the first sector to the second
		}
		
        mode=1; 											//After sorting out units and owners
		convpostgrads.SetActive(false);
        Renderer renderer = attackingSector.GetComponent<SpriteRenderer> (); 
		Color color; 													
		color = renderer.material.color; 					//Update the transparency of the attacking sector to be semi-transparent
		color.a = 0.5f; 
		renderer.material.color = color; 	  
		
        gameInstructions.text = "Pick a sector to attack with..."; //Update instructions to show user next step

        /* Only end the current turn if a conflict occus
         */
        if (conflictOccurs) {
            gameManager.GetComponent<SuddenDeath>().DecrementCountdown();
            gameManager.GetComponent<Game>().NextTurn();
        }
    } 

    private void PlayMinigame(int player) {
        // disables the main game board, activates the minigame, re-enables
        // the main game board, and returns whether or not the player won the minigame

        // disable all interactable elements of the main game:
        //     sectors, menu buttons, and chance card button

        foreach (GameObject sector in sectors)
        {
            // set the sector to the IgnoreRaycast layer (index 2)
            sector.layer = 2; 
        }

        // disable the various buttons in the main game
        settingsButton.interactable = false;
        helpButton.interactable = false;
        chanceCardButton.interactable = false;


        Debug.Log("starting minigame");

        minigameStatus.SetPlayer(player);
        minigameStatus.SetActive(true);
        minigameStatus.ResetMinigame();
        minigame.SetActive(true);

    }

    public void ResumeMainGame() {

        Debug.Log("minigame finished");

        // re-enable all interactable elements of the main game:

        foreach (GameObject sector in sectors)
        {
            // set the sector to its original layer (index 8)
            sector.layer = 8;
        }

        // enable the various buttons in the main game
        settingsButton.interactable = true;
        helpButton.interactable = true;
        chanceCardButton.interactable = true;

    }
	public void ConvertToPostGrads(){
		while (attackingUnits >= 15) {
			attackingUnits -= 15;
			attackingSector.AddUnits (-15);
			attackingSector.AddPostgrads (1); 
		} 
		mode = 1;											//After sorting out units and owners
		convpostgrads.SetActive(false);
		Renderer renderer = attackingSector.GetComponent<SpriteRenderer> (); 
		Color color; 													
		color = renderer.material.color; 					//Update the transparency of the attacking sector to be semi-transparent
		color.a = 0.5f; 
		renderer.material.color = color; 	  

		gameInstructions.text = "Pick a sector to attack with..."; //Update instructions to show user next step
	}
}