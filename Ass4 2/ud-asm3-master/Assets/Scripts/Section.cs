using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Section : MonoBehaviour {

    /* For clarity, the following fields were renamed:
     * 
     *      manager     ->  gameManager
     *      Owner       ->  owner
     *      Units       ->  units
     *      Label       ->  unitsText
     *      FC          ->  flashCounter
     *      landm       ->  landmarkImage
     *      im          ->  sectorImage
     *      LMtxt       ->  landmarkNameText
     *      LMName      ->  landmarkNameString
     * 
     * The field 'GameObject[] AttOptions' was refactored to 'Section[] adjacentSectors'
     * 
     * To support new features, the following fields were added:
     *
     *      numberOfNewUnitsPerAdjacentSector
     *      PVCHere
     *
     * The following obselete fields were removed:
     *      
     *      sectorim
     *      sectortext
     *      sectxt
     *      secname
     *      LMEff
     *      LMtxtGO
     */

    private GameObject gameManager;         //Define a gameobject which is used to controll general events
    public Section[] adjacentSectors;       //Publicly declare the sections this current section can attack, so they can be defined in the editor
    private int owner;                      //Define a owner of the current setion.

    private int units;                      //Define the units on the current section.
    public Text unitsText;                  //Publicly declare a text object, so a label can be assigned to each section in the editor
    private int numberOfNewUnitsPerAdjacentSector = 1;

    public Sprite landmarkImage;            //Publicly define a sprite which can be attached to any sector with a landmark
    private Image sectorImage;                       //Define the image component for the game object above

    private Text landmarkNameText;          //Define the text component for the game object above
    public string landmarkNameString;             //Publicly define the name of the landmark in current sector (so it can be assigned in editor if there is one)
	private int postgrads;
    public bool PVCHere;

    // Use this for initialization
    void Start() {

        /* For clarity, the obselete field 'GameObject me' was removed
         */ 

        Renderer renderer = gameObject.GetComponent<SpriteRenderer>();
        Color color = renderer.material.color;
        color.a = 0.5f;                     //Change the opacity of the section so it is 50% transparent.
        renderer.material.color = color;

        gameManager = GameObject.Find("EventManager");  //Find the gameobject used to manage events 

        sectorImage = GameObject.Find("LandmarkImage").GetComponent<Image>();        //Find the image component of the above 

        landmarkNameText = GameObject.Find("LandmarkDescription").GetComponent<Text>();         //Find the text component of the above

        unitsText = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update() {

        ColorByOwner();                     //Update the colour of the section to represent the owner.

		unitsText.text = "P: " + postgrads.ToString () + "\nU: "+ units.ToString();
    }                                      


    public void Flash() {

        foreach (Section sector in adjacentSectors) {
            //This function iteratively takes every object that this section can attack,
            StartCoroutine(FlashSelfCo(sector));
        }

    }

    public void FlashSelf () {
        // Flash only this section
        StartCoroutine(FlashSelfCo(this));
    }

    IEnumerator FlashSelfCo (Section section) {
        // Flash only this section
        Renderer renderer = section.GetComponent<SpriteRenderer>();
        Color color = renderer.material.color;

        for (int i = 0; i < 6; i++) {

            if (color.a == 1) {
                color.a = 0.5f;
            }
            else {
                color.a = 1;
            }

            renderer.material.color = color;

            yield return new WaitForSeconds(0.1f);
        }

        color.a = 0.5f;
        renderer.material.color = color;
    }



    void OnMouseDown() {	//This function is called whenever the sector is clicked

        /* To support new features, the following fields were added:
         * 
         *      game
         *      conflictResolution
         */

        Game game = gameManager.GetComponent<Game>();
        ConflictResolution conflictResolution = gameManager.GetComponent<ConflictResolution>();

        //Detect activity for the demo mode's timeout timer
        if (Data.IsDemo) {
            game.PassHadUpdate();
        }

        //Only respond to the click if a section belonging to the current player is clicked
		if (game.GetTurn() == GetOwner() || ((conflictResolution.GetMode() == 3)|(conflictResolution.GetMode() == 4))) { 
            if (landmarkImage != null) {                       //If this sector has a sprite attached to it for the landmark it contains
                sectorImage.gameObject.SetActive(true);                //Enable the image game object (make it visible and editable)
                landmarkNameText.gameObject.SetActive(true);             //Enable the text game object used for landmark information(make it visible and editable)
                sectorImage.sprite = landmarkImage;                      //Set the image to the sprite attached to the sector
                landmarkNameText.text = landmarkNameString;         // display landmark information in the text box

            }
            else {                                   //if the sector does not contain a landmark
                sectorImage.gameObject.SetActive(false);           //make sure the landmark image, 
                landmarkNameText.gameObject.SetActive(false);                //and text is disabled
            }

            if (conflictResolution.GetMode() == 1) {
                Flash();
            }

            gameManager.BroadcastMessage("SetUnits", this.units);               //Send the relevant information of the sector to the conflict resolution script 
			gameManager.BroadcastMessage("SetPostgrads", this.postgrads);
            gameManager.BroadcastMessage("SetPlayer", this.owner);              //for use when both an attacking and defending sector have been selected
            gameManager.BroadcastMessage("SetAdjacentSectors", this.adjacentSectors);
            gameManager.BroadcastMessage("SelectSector", this);
        }
    }

	public void SetPostgrads(int x){
		postgrads = x;
	}
	public int GetPostgrads(){
		return postgrads;
	}
	public void AddPostgrads(int x){
		postgrads += x;
	}


    public void SetOwner(int x) {
        owner = x;              //When this function is called it sets the owner of the section to whatever it is passed
    }

    public int GetOwner() {
        return owner;
    }

    public void SetUnits(int x) {
        units = x;              //When this function is called it sets the units on this section to whatever it is passed
    }

    public int GetUnits() {
        return units;
    }

    public void AddUnits(int x) {
        units = units + x;      //When this function is called it adds the number it was passed to the units to the section
        if (units < 1) {
            units = 1;
        }
    }

    void TakeUnits(int x) {
        units = units - x;      //When this function is called it subtracts the number it was passed from the units to the section
    }

    /* For clarity, this method was renamed from 'TurnUnits' to
     * 'AllocateNewUnits', and the parameter 'p' was renamed to 'player'
     */ 
    void AllocateNewUnits(int player) {     //When this function is called it adds one unit to the current sector units for every object that can attack it (and hence be attacked by it)	

        if (owner == player) {      //if the number it was passed is the same as its current owner
            AddUnits(adjacentSectors.GetLength(0) * numberOfNewUnitsPerAdjacentSector);
			for (int i = 0; i < postgrads; i++) {
				AddUnits (5);
			}
        }
    }

    /* For clarity, this method was renamed from 'OwnerColour' to
     * 'ColorByOwner'
     */ 
    void ColorByOwner() {
        switch (this.owner) {  //When this function is called it updates the colour of the section to represent the current owner.
            case (1):            //If the owner is player 1 the colour is set to red.
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case (2):           //If the owner is player 2 the colour is set to blue.
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case (3):           //If the owner is player 3 the colour is set to green.
                GetComponent<SpriteRenderer>().color = Color.green;
                break;
            case (4):           //If the owner is player 3 the colour is set to green.
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            default:            //If the owner is no-one the colour is set to white.
                GetComponent<SpriteRenderer>().color = Color.clear;
                break;
        }

    }

    /* This method was added to support new features
     */
	public void spawnPVC(){
		PVCHere = true;
	}

}
