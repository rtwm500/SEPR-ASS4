using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvalidUnitsPop : MonoBehaviour
{
    
    /* For clarity, the field which was previously named 
     * 'windowRect' has been renamed 'window'
     */

	//Define a 200x105 px window that will apear in the center of the screen.
    private Rect window = new Rect ((Screen.width - 200)/2, (Screen.height - 105)/2, 200, 125);
	
    //Define a bool that will show/hide the popup
	private bool show = false;

	void OnGUI () 
	{
		if(show)			//Whenever show is set to true make the window appear, when false hide it
			window = GUI.Window(0, window, DialogWindow, "Invalid Number of Units");	//Set title of box
	}


	void DialogWindow (int windowID)
	{
		float y = 20;
		GUI.Label (new Rect (5, y, window.width, 20), "You must move at least one unit,"); 	//Set first line of description
		GUI.Label (new Rect (5, y+20, window.width,20), "and leave at least one unit");		//Set second line of desription
		GUI.Label (new Rect (5, y+40, window.width,20), "on attacking sector");				//Set third line of desription 

		if(GUI.Button(new Rect(5,y+75, window.width - 10, 20), "Select New Number"))		//Define one button that says "Select New Number"
		{																						//And closes popup when clicked	
			show = false;
		}
			
	}
		
	public void Open()
	{					//when open is called
		show = true;	//show the popup
	}
}