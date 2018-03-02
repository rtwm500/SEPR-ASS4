using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour {

    /* For clarity, the field which was previously named
     * 'SettingPop' was renamed 'SettingPopup'
     */ 

    private GameObject SettingPopup; 				//Define a gameobject that will be popped up
	private Game game;


	// Use this for initialization
	void Start () {
		SettingPopup = GameObject.Find("SettingsPopup");   //Find the gameobject that holds all the objects that will appear in the settings popup
		SettingPopup.SetActive (false);			//Hide this object
		game = GameObject.Find("EventManager").GetComponent<Game>();
	}
	
	void SettingsClick(){ 					//When settings is clicked
		SettingPopup.SetActive (true);		//Show settings popup
		game.DisableRayCast();
	} 

	void CloseClick(){ 						//When close is clicked
		SettingPopup.SetActive (false);		//hide settings popup
		game.EnableRayCast();
	}
}
