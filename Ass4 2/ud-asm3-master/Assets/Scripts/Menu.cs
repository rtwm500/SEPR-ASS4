using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class Menu : MonoBehaviour {

	public GameObject gridItemPrefab;
	private GameObject targetGrid;
	
	private BinaryFormatter bf;

	public void Play(int realPlayers){
        Data.RealPlayers = realPlayers;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
	}

	private void LoadGame(GameObject obj){
        
        string slotIDStr = "";
        slotIDStr = obj.transform.GetChild(1).GetComponent<Text>().text.Substring(6,3);

        int slotID = 0;
		slotID = int.Parse(slotIDStr);

        SaveGameHandler.LoadGame(slotID);

	}

	public void Quit(){
		Debug.Log("Application Quit");
		Application.Quit();
	}

	public void QuitToMainMenu(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);
	}

    public void DemoMode(bool value)
    {
        Data.IsDemo = value;
    }

	public void GetSaves(){

		targetGrid = GameObject.Find("Grid");

		Text playerNumText;
		Text saveSlotText;
		Button currentButton;
		int numSaves = 0;

		string filePath = Application.persistentDataPath + "/";
		BinaryFormatter bf = new BinaryFormatter();
		
		foreach (string file in System.IO.Directory.GetFiles(filePath)){
			
			if (Path.GetExtension(file) == ".bin"){
				Debug.Log("Found Correct Extension"); 
				FileStream currentFile = File.Open(file, FileMode.Open);
				System.Object data = bf.Deserialize(currentFile);
				
				if (data is SaveGame) {

					numSaves+=1;

					SaveGame saveGame = (SaveGame)data;
					
					GameObject newGridItem = Instantiate(gridItemPrefab, targetGrid.transform) as GameObject;
					//Edit GridItems Text Child
					playerNumText = newGridItem.transform.GetChild(0).GetComponent<Text>();
					saveSlotText = newGridItem.transform.GetChild(1).GetComponent<Text>();
					currentButton = newGridItem.GetComponent<Button>();

					playerNumText.text = saveGame.RealPlayers.ToString() + " Player";

					string fileName = Path.GetFileName(file);
                	string slotNumberStr = fileName.Substring(8, 3);
					saveSlotText.text = "Save #" + slotNumberStr;

					currentButton.onClick.AddListener(delegate{LoadGame(newGridItem);});

				}

				currentFile.Close();

			}

		}
		RectTransform gridRectTransform = targetGrid.GetComponent<RectTransform>();
		gridRectTransform.offsetMin = new Vector2(gridRectTransform.offsetMin.x, (-1 * 90 * numSaves) + 820);
	
	}

	void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");

    }
	
}
