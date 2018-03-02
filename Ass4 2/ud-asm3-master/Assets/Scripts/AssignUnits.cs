using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignUnits : MonoBehaviour {

    /* For clarity, the following fields were renamed:
     * 
     *      Sections    ->  sectors
     *      picked      ->  assignedSectors
     * 
     * To support new features, the 'game' field was added
     */
	
    public Section[] sectors; 					//Declare Sections publicly so they can be assigned in editor 
    private List<int> assignedSectors = new List<int>();		//Define a list of picked sections so that the same section will
	private Game game;
	
    // Use this for initialization					//not be assigned twice.
	void Start () {

        /* For clarity, the following fields were renamed:
         * 
         *      R2      ->  random
         *      Sect    ->  sector
         *      RUnits  ->  units
         */

        System.Random random = new System.Random ();    //To start with every, section is assigned to AI, 

        if (Data.GameFromLoaded) {
            //Do shiz
            SaveGame loadedGame = SaveGameHandler.loadedGame;

            foreach (Section liveSection in sectors) {
                foreach (SerialSection savedSection in loadedGame.Sections) {
                    if (savedSection.landmarkNameString == liveSection.landmarkNameString) {
                        liveSection.SetUnits(savedSection.units);
                        liveSection.SetOwner(savedSection.owner);
                        liveSection.PVCHere = savedSection.PVCHere;
                    }
                }
            }


            Data.GameFromLoaded = false;
        }
        else {

            foreach (Section sector in sectors) {        //the unbiased AI, with a random number of units between 1 and 10.

                // max number of units that the neutral AI's sector can
                // start with was increased from 11 to 25
                int units = random.Next(1, 25);

                sector.BroadcastMessage("SetOwner", Data.RealPlayers + 1);
                sector.BroadcastMessage("SetUnits", units);
				sector.BroadcastMessage ("SetPostgrads",0);
            }
            //After all sections are under AI's contol,

            for (int i = 1; i <= Data.RealPlayers; i++) {
                AssignPlayer(i, 3);
            }
            SpawnPVC();
        }

	}


    void AssignPlayer(int player, int numberOfSectors){ 			//This is the function that takes an integer representing the player
		
        /* For clarity, the parameter 'sections' was renamed 'numberOfSectors',
         * the following fields were renamed:
         * 
         *      Rand    ->  random
         *      RSect   ->  sectorID
         *      
         * and the obselete field 'pickedB' was removed
         */

        System.Random random = new System.Random (); 			//and a number of sections to assign to that player.
		int i = 0;

        while (i < numberOfSectors) { 								//Then iteratively...
			
            int sectorID = random.Next(this.sectors.Length);       //picks a random section
			
            /* The construct which was previously used to test
             * whether or not a sector had already been assigned
             * has been replaced by a .Contains operation on the
             * list of assigned sectors
             */
            if (!assignedSectors.Contains(sectorID)) { 									//and if it has not been assigned yet
				this.sectors[sectorID].BroadcastMessage ("SetOwner", player); 	//Assigns it to the player,
				this.sectors[sectorID].BroadcastMessage ("SetUnits", 25);		//Sets the number of units on that point to 25,
				this.sectors[sectorID].BroadcastMessage("SetPostgrads",0);
				i += 1; 									//updates the counter to show one section has been assigned,
				assignedSectors.Add(sectorID);							//and updates the list of picked sections to show this section has
			}												//now been assigned to a player.
		}
	} 
	
    /* The similarities of methods 'P1TurnUnits' and 'P2TurnUnits'
     * have been abstracted to the 'AllocateNewUnits' method, and the
     * original methods have been replaced by the methods
     * 'AllocatePlayer1NewUnits' and 'AllocatePlayer2NewUnits'
     */ 

    public void AllocateNewUnits(int player) {

        foreach (Section sector in sectors)
        {
            sector.BroadcastMessage("AllocateNewUnits", player);
        }

    }

    public void AllocatePlayer1NewUnits()
	{ 					
        AllocateNewUnits(1);
	}
	
    public void AllocatePlayer2NewUnits()
	{ 
        AllocateNewUnits(2);
	}

    public void AllocatePlayer3NewUnits() {
        AllocateNewUnits(3);
    }


    public void SpawnPVC(){ 			
        
		System.Random random = new System.Random (); 

		int sectorID = random.Next(this.sectors.Length);       //picks a random section
		this.sectors[sectorID].BroadcastMessage("spawnPVC"); //sets boolean for PVC to true
	}
}
