using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game properties manager
/// </summary>
public class GameManager : GameSingleton<GameManager>
{
    /// <summary>
    /// Whether game is in debug mode
    /// </summary>
    public bool DebugMode = true;

    [Header("Game Objects")]
    /// <summary>
    /// Player instance
    /// </summary>
    public Player Player;
}
