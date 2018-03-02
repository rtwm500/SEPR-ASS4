using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Help : MonoBehaviour {
	
    private GameObject HelpPopup; 				//Define a gameobject that will be popped up
	
    // Use this for initialization
	void Start () {		
		HelpPopup = GameObject.Find("HelpPopup");  //Find the gameobject that holds all the objects that will appear as help
		HelpPopup.SetActive (false);				//Hide this object
	}

	void OpenHelp(){ 							//When the help button is clicked
		HelpPopup.SetActive (true);				//show help popup
	} 

	void CloseHelp(){ 							//When close button is clicked
		HelpPopup.SetActive (false);			//hide popup
	}
}
