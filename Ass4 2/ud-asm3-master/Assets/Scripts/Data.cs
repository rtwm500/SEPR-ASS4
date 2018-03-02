using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;


public static class Data
{

    private static bool isDemo;
    private static int realPlayers;
    private static bool gameFromLoaded;

    public static bool IsDemo
    {
        get
        {
            return isDemo;
        }
        set
        {
            isDemo = value;
        }
    }

    public static int RealPlayers {
        get {
            return realPlayers;
        }
        set {
            realPlayers = value;
        }
    }

    public static bool GameFromLoaded {
        get {
            return gameFromLoaded;
        }
        set {
            gameFromLoaded = value;
        }
    }
}
