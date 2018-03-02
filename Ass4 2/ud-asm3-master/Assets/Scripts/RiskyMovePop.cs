using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiskyMovePop : MonoBehaviour {
	
    /* For clarity, the following fields were renamed:
     *      windowRect  ->  window
     *      attu        ->  attackingUnits
     *      defu        ->  defendingUnits
     */ 

    //Define 200x130 px window will apear in the center of the screen.
    private Rect window = new Rect ((Screen.width - 200)/2, (Screen.height - 130)/2, 200, 130);
	
    //Define a bool that will show/hide the popup
	private bool show = false;			
	
    private int attackingUnits; 					//Define an integer that wild hold the number of attacking units
    private int defendingUnits; 					//Define an integer that wild hold the number of defending units
	
    public GameObject EventManager;		//Publicly define a gameobject that will manage general events (like conflict resolution)


	void OnGUI () 
	{
		if(show)																	//Whenever show is set to true make the window appear, when false hide it
			window = GUI.Window (0, window, DialogWindow, "Risky Attack");	//Set title of box
	}
	

	void DialogWindow (int windowID)
	{
		float y = 20;
		GUI.Label (new Rect (5, y, window.width, 20), "You are outmanned " + attackingUnits +  " to " + defendingUnits); //Set first line of description
		GUI.Label (new Rect (5, y+20, window.width,20), "do you want to continue?");				//Set second line of description

		if(GUI.Button(new Rect(5,y+55, window.width - 10, 20), "Continue anyway"))					//Define one button that says "Continue anyway"
		{
			EventManager.BroadcastMessage ("ResolveConflict");													//Calls conflict resolution
			show = false;																				//And closes popup when clicked	
		}

		if(GUI.Button(new Rect(5,y+80, window.width - 10, 20), "Select new move"))					//Define one button that says "Select new move"
		{																								//And closes popup when clicked	
			show = false;
		}
	}
	
	// To open the dialogue from outside of the script.
	public void Open()
	{ 
		show = true;
	} 

    /* The parameter previously named 'AU' was renamed 'attackingUnits'
     */ 
	public void SetAttackingUnits(int attackingUnits)
	{ 
		this.attackingUnits = attackingUnits;
	} 

    /* The parameter previously named 'DU' was renamed 'defendingUnits'
     */ 
	public void SetDefendingUnits(int defendingUnits)
	{ 
		this.defendingUnits = defendingUnits;
	}
}
