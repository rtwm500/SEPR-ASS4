using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PVC : MonoBehaviour
{
    public Vector3 vector;										//this is the vector the PVC sprite is transformed by
	public Button background;									//this is used to assign the button component of the minigame background later on

    // Use this for initialization
    void Start()
    {
        vector = new Vector3(17, 14, 0);												//sets the sprite vector values
		background = GameObject.Find("PVCBackgroundButton").GetComponent<Button>();		//finds the button component of the PVCBackround
		background.interactable = true;													//sets the button component to interactable, so it doesnt register misses before the game is started
    }

    public void OnCollisionEnter(Collision collision)           //For each different side of the border there is a seperate if function, as each side has a different reflection normal
    {                                                           //There is a different gameobject referenced for each plane of border i.e."Border_Right"
        if (collision.gameObject.name == "Border_Right")        //If the sprite collides with the right side of the minigame border, reflect with respect to the right normal (Vector3.right)
        {
            vector = Vector3.Reflect(vector, Vector3.right);
        }

        if (collision.gameObject.name == "Border_Left")         //If the sprite collides with the right side of the minigame border, reflect with respect to the left normal (Vector3.left)
        {
			vector = Vector3.Reflect(vector, Vector3.left);
        }

        if (collision.gameObject.name == "Border_Top")          //If the sprite collides with the right side of the minigame border, reflect with respect to right normal (Vector3.top)
        {
			vector = Vector3.Reflect(vector, Vector3.up);
        }

        if (collision.gameObject.name == "Border_Bottom")       //If the sprite collides with the right side of the minigame border, reflect with respect to right normal (Vector3.bottom)
        {
			vector = Vector3.Reflect(vector, Vector3.down);
        }


    }
    // Update is called once per frame
    void Update()
    {
        transform.Translate(vector);							//This transforms the sprite by the vector created above every frame, giving the vector movement in the minigame space
    }
}
