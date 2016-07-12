﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Parse;
using System.IO;
using Newtonsoft.Json;
using System;
using UnityEngine.SceneManagement;
using Assets.Mangers;

public class LevelManager : MonoBehaviour
{
    public Player playerLeft;
    public Player playerRight;
    public Text endGameText;
    public Arrow arrow;
    public Canvas endGameCanvas;
    public Score score;
    public Canvas worldCanvas;
    public AimLine aimLine;
    public RawImage fader;

    private Color replayColor = new Color(0, 0, 0, 0.5f);
    private Color playingColor = new Color(0, 0, 0, 0f);


    // For creating the wall
    public GameObject Brick;

    public const float MaxPlayerDistance = 45.0f;
    public const float MinPlayerDistance = 5.0f;

    public List<GameObject> bricks = new List<GameObject>();
    
    public void OnLevelWasLoaded()
    {
        startPlaying();
    }

    // Use this for initialization
    void Start()
    {
        startPlaying(); // TODO remove this, it is just for easy testing in the unity editor
    }

    void Update()
    {
        //if (LevelDefinition.gameState == GameState.ShowLastMove)
        //{
        //    // Lerp the colour of the texture between itself and black.
        //    fader.color = Color.Lerp(fader.color, replayColor, 1.5f * Time.deltaTime);
        //}
        //else
        //{
        //    fader.color = Color.Lerp(fader.color, playingColor, 1.5f * Time.deltaTime);
        //}
    }

    private void AddWalls(float baseYPosition, int numBricks)
    {
        for (int y = 0; y < numBricks; y++)
        {
            bricks.Add((GameObject)Instantiate(Brick, new Vector3(0, baseYPosition + y * Brick.GetComponent<BoxCollider2D>().size.y, 0), Quaternion.identity));
        }
    }
    private void RemoveWalls()
    {
        foreach (GameObject brick in bricks)
        {
            DestroyObject(brick);
        }
    }

    public enum EndGameState
    {
        LeftWins = 0,
        RightWins,
        Tie
    }

    public void EndGame(EndGameState endGameState)
    {
        LevelDefinition.gameState = GameState.GameOver;
        StartCoroutine(EndGameOverTime(endGameState, 2.0f));
    }

    IEnumerator EndGameOverTime(EndGameState endGameState, float delayInSeconds)
    {
        if (endGameState == EndGameState.LeftWins)
        {
            score.ScoreLeft++;
            endGameText.text = "Left Player Wins!";

        }
        else if (endGameState == EndGameState.RightWins)
        {
            score.ScoreRight++;
            endGameText.text = "Right Player Wins!";
        }
        else if (endGameState == EndGameState.Tie)
        {
            endGameText.text = "Tie game!";
        }

        // Show who won for X seconds
        endGameText.enabled = true;
        yield return new WaitForSeconds(delayInSeconds);

        endGameCanvas.enabled = true;
    }

    public void startPlaying()
    {
        LevelDefinition.LevelDefinitionSetDefault();

        // Set the game state to show the last player's move
        if (LevelDefinition.ShotArrows.Count > 0)
        {
            LevelDefinition.gameState = GameState.ShowLastMove;
        }
        else
        {
            LevelDefinition.gameState = GameState.Playing;
        }

        // Remove the old wall and set up the current ones.
        RemoveWalls();
        AddWalls(LevelDefinition.WallPosition, LevelDefinition.WallHeight);

        // Set the player distance
        SetPlayerWidth(LevelDefinition.PlayerDistanceFromCenter);

        // Reset the aimline (including the computer's new aim start point)
        aimLine.SetupMatch();

        // Set the world canvas (instructions above the left player)
        //worldCanvas.transform.position = new Vector3(playerLeft.transform.position.x + 6.0f, worldCanvas.transform.position.y, worldCanvas.transform.position.z);
     
        // Remove all the shot arrows,
        // Add the current ones
        // Then set the arrow's position
        arrow.RemoveAllShotArrows();
        arrow.AddShotArrows(LevelDefinition.ShotArrows);
        arrow.ResetPosition(LevelDefinition.IsPlayerLeftTurn);

        // Set the player's healths
        playerLeft.SetHealth(LevelDefinition.PlayerLeftHealth);
        playerRight.SetHealth(LevelDefinition.PlayerRightHealth);

        // Check if the game is already over.
        // Setup the rebuttle text
        RebuttalText.setEnabled(LevelDefinition.RebuttleTextEnabled);
        if (LevelDefinition.PlayerLeftHealth <= 0 && LevelDefinition.PlayerRightHealth <= 0)
        {
            EndGame(EndGameState.Tie);
        }
        else if(LevelDefinition.PlayerLeftHealth <= 0)
        {
            EndGame(EndGameState.RightWins);
        }
        else if (LevelDefinition.PlayerRightHealth <= 0 && !LevelDefinition.RebuttleTextEnabled)
        {
            EndGame(EndGameState.LeftWins);
        }
        else
        {
            // Remove the end game text
            endGameText.enabled = false;

            // Remove the endgame menu
            endGameCanvas.enabled = false;
        }
    }
    public static void quitPlaying()
    {
        SceneManager.LoadScene("StartMenu");
    }
    public void quitPlayingLevel()
    {
        SceneManager.LoadScene("StartMenu");
    }

    public void SetPlayerWidth(float distance)
    {
        //float distanceFromCenter = (Random.value * MaxPlayerDistance) + MinPlayerDistance;
        playerLeft.transform.position = new Vector3(-distance, playerLeft.transform.position.y, playerLeft.transform.position.z);
        playerRight.transform.position = new Vector3(distance, playerRight.transform.position.y, playerRight.transform.position.z);
    }

    public void startNextGame()
    {
        throw new NotImplementedException();
    }
}
