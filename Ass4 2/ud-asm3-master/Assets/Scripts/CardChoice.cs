using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardChoice : MonoBehaviour {

	/* For clarity, the following fields were renamed:
     *      windowRect  ->  window
     *      attu        ->  attackingUnits
     *      defu        ->  defendingUnits
     */ 

	//Define 200x130 px window will apear in the center of the screen.
	private Rect window = new Rect ((Screen.width - 200)/2, (Screen.height - 130)/2, 200, 130);

	//Define a bool that will show/hide the popup
	private bool show = false;			

	GameObject caller;

	void OnGUI () 
	{
		if(show)																	//Whenever show is set to true make the window appear, when false hide it
			window = GUI.Window (0, window, DialogWindow, "Pick a card");	//Set title of box
	}


	void DialogWindow (int windowID)
	{
		float y = 20;
		GUI.Label (new Rect (5, y, window.width, 20), "Do you want to use a chance"); //Set first line of description
		GUI.Label (new Rect (5, y+20, window.width,20), "card or a punishment card?");				//Set second line of description

		if(GUI.Button(new Rect(5,y+55, window.width - 10, 20), "Chance"))					//Define one button that says "Continue anyway"
		{
			caller.BroadcastMessage ("usecard",true);												//Calls conflict resolution
			show = false;																				//And closes popup when clicked	
		}

		if(GUI.Button(new Rect(5,y+80, window.width - 10, 20), "Punishment"))					//Define one button that says "Select new move"
		{	
			caller.BroadcastMessage ("usecard",false);	
			show = false;
		}
	}

	// To open the dialogue from outside of the script.
	public void Open(GameObject caller)
	{ 
		this.caller = caller;
		show = true;
	} 

}
