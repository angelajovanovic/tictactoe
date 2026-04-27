using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class StatisticsManager : MonoBehaviour
{

    public static int TotalGames;
    public static int Player1Wins;
    public static int Player2Wins;
    public static int Draws;
    public TextMeshProUGUI totalGamesText;
    public TextMeshProUGUI p1WinsText;
    public TextMeshProUGUI p2WinsText;
    public TextMeshProUGUI drawsText;
    public static float TotalDuration;
    public TextMeshProUGUI avgDurationText;

    private static readonly string NotebookFileName = "game_stats.txt";

    private void Awake()
    {
        TotalGames = PlayerPrefs.GetInt("TotalGames", 0);
        Player1Wins = PlayerPrefs.GetInt("Player1Wins", 0);
        Player2Wins = PlayerPrefs.GetInt("Player2Wins", 0);
        Draws = PlayerPrefs.GetInt("Draws", 0);
        TotalDuration = PlayerPrefs.GetFloat("TotalDuration", 0f);

        // ensure notebook file exists
        try
        {
            string path = Path.Combine(Application.persistentDataPath, NotebookFileName);
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "Timestamp,Winner,Duration(s),TotalGames,Player1Wins,Player2Wins,Draws\n");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Failed to create notebook file: " + ex.Message);
        }
    }

    public static void SaveGame(string winner, float matchTime)
    {
        TotalGames++;
        PlayerPrefs.SetInt("TotalGames", TotalGames);

        TotalDuration += matchTime;
        PlayerPrefs.SetFloat("TotalDuration", TotalDuration);

        if (winner == "X")
        {
            Player1Wins++;
            PlayerPrefs.SetInt("Player1Wins", Player1Wins);
        }
        else if (winner == "O")
        {
            Player2Wins++;
            PlayerPrefs.SetInt("Player2Wins", Player2Wins);
        }
        else if (winner == "Draw")
        {
            Draws++;
            PlayerPrefs.SetInt("Draws", Draws);
        }

        PlayerPrefs.Save(); 

        // append to notebook file
        try
        {
            string path = Path.Combine(Application.persistentDataPath, NotebookFileName);
            string line = string.Format("{0},{1},{2:F1},{3},{4},{5},{6}\n",
                System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                winner,
                matchTime,
                TotalGames,
                Player1Wins,
                Player2Wins,
                Draws);
            File.AppendAllText(path, line);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Failed to write to notebook file: " + ex.Message);
        }
    }

    public static string ReadNotebook()
    {
        try
        {
            string path = Path.Combine(Application.persistentDataPath, NotebookFileName);
            if (File.Exists(path))
                return File.ReadAllText(path);
            return string.Empty;
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Failed to read notebook file: " + ex.Message);
            return string.Empty;
        }
    }

    public void DisplayStats()
    {
        
        int actualTotal = Player1Wins + Player2Wins + Draws;

        if (totalGamesText != null)
            totalGamesText.text = "Total Games: " + actualTotal; // Koristi zbir

        if (p1WinsText != null)
            p1WinsText.text = "Player 1 Wins: " + Player1Wins;

        if (p2WinsText != null)
            p2WinsText.text = "Player 2 Wins: " + Player2Wins;

        if (drawsText != null)
            drawsText.text = "Draws: " + Draws;

        float average = 0f;
        if (actualTotal > 0) average = TotalDuration / actualTotal;

        if (avgDurationText != null)
            avgDurationText.text = "Avg Duration: " + average.ToString("F1") + "s";
    }
}
