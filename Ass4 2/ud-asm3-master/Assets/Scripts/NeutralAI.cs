using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeutralAI : MonoBehaviour {
    private int playerID = Data.RealPlayers + 1;
    private Game game;
    private Section[] sections;

    void Start() {
        game = GameObject.Find("EventManager").GetComponent<Game>();
        sections = GameObject.Find("Sectors").GetComponentsInChildren<Section>();
    }

    private int EnemyMagnitudeOnBorder(Section section) {
        // Return the total number of enemies on the sections border
        int magnitude = 0;
        foreach (Section borderingSection in section.adjacentSectors) {
            if (borderingSection.GetOwner() != playerID) {
                magnitude += borderingSection.GetUnits();
            }
        }
        return magnitude;
    }

    private int NumberOfBorderingEnemies(Section section) {
        // Return the number of hostile sections that border this one
        int magnitude = 0;
        foreach (Section borderingSection in section.adjacentSectors) {
            if (borderingSection.GetOwner() != playerID) {
                magnitude += 1;
            }
        }
        return magnitude;
    }

    private int MaxHostileUnitDelta(Section section) {
        // Return the unit difference of the strongest enemy section
        int unitDiff = 0;
        int sectionUnitStrength = section.GetUnits();
        foreach (Section borderingSection in section.adjacentSectors) {
            if (borderingSection.GetOwner() != playerID) {

                int enemySectionStrength = borderingSection.GetUnits();

                if (enemySectionStrength > unitDiff && enemySectionStrength > sectionUnitStrength) {
                    unitDiff = enemySectionStrength - sectionUnitStrength;
                }
            }
        }
        return unitDiff;
    }

    private int MaxFriendlyUnitDelta(Section section) {
        // Return the unit difference of the strongest enemy section
        int unitDiff = 0;
        int sectionUnitStrength = section.GetUnits();
        foreach (Section borderingSection in section.adjacentSectors) {
            if (borderingSection.GetOwner() == playerID) {

                int otherSectionStrength = borderingSection.GetUnits();

                if (otherSectionStrength > unitDiff && otherSectionStrength > sectionUnitStrength) {
                    unitDiff = otherSectionStrength - sectionUnitStrength;
                }
            }
        }
        return unitDiff;
    }

    private int NumberOfFriendlySectors() {
        int magnitude = 0;
        foreach (Section section in sections) {
            if (section.GetOwner() == playerID) {
                magnitude += 1;
            }
        }
        return magnitude;
    }

    private int AvgFriendlyStrength() {
        int totalUnits = 0;
        foreach (Section section in sections) {
            if (section.GetOwner() == playerID) {
                totalUnits += section.GetUnits();
            }
        }

        return totalUnits / NumberOfFriendlySectors();
    }

    private bool Reinforce(Section section) {
        
        // Must be < 1
        float partialMoveCoeff = 0.7f;


        foreach  (Section adjsection in section.adjacentSectors) {
            // Check neighbour is friendly
            if (adjsection.GetOwner() == playerID) {
                int curUnits = section.GetUnits();
                int numAdjUnits = adjsection.GetUnits();
                int reqUnits = MaxHostileUnitDelta(section) - curUnits;
                int enemyMagForAdj = MaxHostileUnitDelta(adjsection);

                // No threats to neighbour, move as many as needed
                if (NumberOfBorderingEnemies(adjsection) == 0) {
                    // The neighbour has all the units required
                    if (numAdjUnits - 1 > reqUnits) {
                        MakeMove(adjsection, section, reqUnits);
                        return true;
                    }
                // The neighbour has units to spare
                } else if (enemyMagForAdj < numAdjUnits) {
                    // The neighbour has all the required units (And leaves itself defendable)
                    if ((numAdjUnits - enemyMagForAdj) + 2 > reqUnits + enemyMagForAdj) {
                        Debug.Log("(Full Reinforce)");
                        MakeMove(adjsection, section, reqUnits);
                        return true;
                    // There were sufficent units to make a 'partial' reinforce move
                    } else if (((numAdjUnits - enemyMagForAdj) + 2) > ((reqUnits + enemyMagForAdj) * partialMoveCoeff)) {
                        Debug.Log("(Partial Reinforce)");
                        int toMove = (int)((numAdjUnits - enemyMagForAdj) * partialMoveCoeff);
                        Debug.Assert(toMove < numAdjUnits, "Attempted to move more units than existed!");

                        MakeMove(adjsection, section, toMove);
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool Balance(Section section) {
        int maxDelta = MaxFriendlyUnitDelta(section);

        foreach (Section adjSection in section.adjacentSectors) {
            // Check neighbour is friendly
            if (adjSection.GetOwner() == playerID) {
                int adjStrength = adjSection.GetUnits();
                if (adjStrength == maxDelta) {
                    Debug.Assert(adjStrength > section.GetUnits());
                    int delta = adjStrength - section.GetUnits();
                    MakeMove(adjSection, section, delta);
                    return true;
                }
            }
        }
        return false;
    }

    private void MakeMove(Section source, Section destination, int amount) {
        source.SetUnits(source.GetUnits() - amount);
        source.FlashSelf();
        destination.SetUnits(destination.GetUnits() + amount);
        destination.FlashSelf();
    }

    public void DecideMove() {
        // Method called to perform a AI turn

        if (NumberOfFriendlySectors() != 0) {

            // Ordered List to aid in determining move
            OrderedList list = new OrderedList(NumberOfFriendlySectors());

            // Global calculations
            int fAvg = AvgFriendlyStrength();

            foreach (Section section in sections) {

                // Local calculations
                int eMag = EnemyMagnitudeOnBorder(section);
                int eBor = NumberOfBorderingEnemies(section);
                int eDel = MaxHostileUnitDelta(section);
                int fDel = MaxFriendlyUnitDelta(section);

                // Threat Heuristic
                int threatHeuristic = (5 * eDel) + (4 * eMag) + (3 * eBor);

                // Balance Heuristic
                int balanceHeuristic = 0;

                if (fAvg - fDel > 0) {
                    balanceHeuristic = (fAvg - fDel);
                }

                // Poppulate ordered list
                list.AddItem(section, threatHeuristic, eDel, balanceHeuristic, fDel);
            }

            if (Reinforce(list.GetMaxThreat().section)) {
                Debug.Log("AI Performed Reinforce action");
            }
            else if (Balance(list.GetMaxBalance().section)) {
                Debug.Log("AI Performed Balance action");
            }
            else {
                Debug.Log("AI Performed no action");
            }
        }
        game.NextTurn();
    }
}

class OrderedList {
    // A data type for retrieving sections based on several criteria

    private int numberFriendlySectors;
    private ListItem[] list;
    private int listPointer;

    public OrderedList(int numberFriendlySectors) {
        // Constructor
        this.numberFriendlySectors = numberFriendlySectors;
        this.list = new ListItem[numberFriendlySectors];
        this.listPointer = 0;
    }

    public bool AddItem(Section section, int threatHeuristic, int threatDelta, int balanceHeuristic, int balanceDelta) {
        // Add a new item to the ordered list
        if (listPointer >= numberFriendlySectors) {
            // List is full (Shouldn't ever occur)
            return false;
        }

        list[listPointer] = new ListItem(section, threatHeuristic, threatDelta, balanceHeuristic, balanceDelta);
        return true;


    }

    public ListItem GetMaxThreat() {
        // Returns the associated data for the item with the greatest threat heuristic
        int curMaxValue = 0;
        ListItem curMax = null;

        foreach (ListItem item in list) {
            if (curMax == null || curMax.threatHeuristic > curMaxValue) {
                curMax = item;
                curMaxValue = item.threatHeuristic;
            }
        }
        return curMax;
    }

    public ListItem GetMaxBalance() {
        // Returns the associated data for the item with the greatest balance heuristic
        int curMaxValue = 0;
        ListItem curMax = null;

        foreach (ListItem item in list) {
            if (curMax == null || curMax.balanceHeuristic > curMaxValue) {
                curMax = item;
                curMaxValue = curMax.balanceHeuristic;
            }
        }
        return curMax;

    }
}

class ListItem {
    // A data type for storing section heuristic data
    public int threatHeuristic;
    public int threatDelta;
    public int balanceHeuristic;
    public int balanceDelta;
    public Section section;

    public ListItem(Section section, int threatHeuristic, int threatDelta, int balanceHeuristic, int balanceDelta) {
        this.threatHeuristic = threatHeuristic;
        this.threatDelta = threatDelta;
        this.balanceHeuristic = balanceHeuristic;
        this.balanceDelta = balanceDelta;
        this.section = section;
    }
}