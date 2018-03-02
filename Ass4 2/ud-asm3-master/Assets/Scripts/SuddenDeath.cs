using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuddenDeath : MonoBehaviour {

    [SerializeField] private int countdown = 60;
    [SerializeField] private int severity = 2;
    [SerializeField] private bool suddenDeathMode = false;
    [SerializeField] private GameObject indicator;
    [SerializeField] private Section[] sectors;

    void Start() {
        indicator = GameObject.Find("SuddenDeathUI");
        indicator.SetActive(false);
        sectors = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
    }

    public void DecrementCountdown() {

        countdown -= 1;

        if (countdown == 0) {
            // initiate sudden death mode
            suddenDeathMode = true;
            indicator.SetActive(true);
        }
    }

    public void KillUnitsOnBorderSectors() {

        // if sudden death mode is activated, randomly destroy units in sectors
        // that border hostile sectors

        if (suddenDeathMode)
        {
            System.Random random = new System.Random();

            foreach (Section sector in sectors)
            {
                foreach (Section adjacentSector in sector.adjacentSectors)
                {

                    if (sector.GetOwner() != adjacentSector.GetOwner())
                    {
                        
                        sector.SetUnits(sector.GetUnits() - random.Next(0, severity));
                        adjacentSector.SetUnits(adjacentSector.GetUnits() - random.Next(0, severity));

                        // ensure that this does not make it so that any sector has less than 1 unit
                        if (sector.GetUnits() < 1)
                            sector.SetUnits(1);
                        
                        if (adjacentSector.GetUnits() < 1)
                            adjacentSector.SetUnits(1);
                    }
                }
            }
        }
    }
}