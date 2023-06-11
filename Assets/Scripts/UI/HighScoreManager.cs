using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HighScoreManager : SingletonMonobehaviour<HighScoreManager>
{
    private HighScores highScores = new HighScores();

    protected override void Awake()
    {
        base.Awake();

        LoadScores();
    }


    //Load Scores
    private void LoadScores()
    {
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log("loaded=");
        if (File.Exists(Application.persistentDataPath + "/GameHighScores.dat"))
        {
            ClearScoreList();

            FileStream file = File.OpenRead(Application.persistentDataPath + "/GameHighScores.dat");
            Debug.Log(Application.persistentDataPath);

            highScores = (HighScores)bf.Deserialize(file);

            file.Close();
            

        }
    }


    //Clear All Scores
    private void ClearScoreList()
    {
        highScores.scoreList.Clear();
    }

 
    
    
    //Add score to high scores list
    public void AddScore(Score score, int rank)
    {
        highScores.scoreList.Insert(rank - 1, score);

        //Maintain the maximum number of scores to save
        if (highScores.scoreList.Count > Settings.numberOfHighScoresToSave)
        {
            //remove lowest score
            highScores.scoreList.RemoveAt(Settings.numberOfHighScoresToSave);
        }

        SaveScores();
    }


    //Save Scores
    private void SaveScores()
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + "/GameHighScores.dat");

        bf.Serialize(file, highScores);

        file.Close();
    }


    //Get highscores
    public HighScores GetHighScores()
    {
        return highScores;
    }


    
    //Return the rank of the playerScore compared to the other high scores
    public int GetRank(long playerScore)
    {
        // If there are no scores currently in the list - then this score must be ranked 1 - then return
        if (highScores.scoreList.Count == 0) return 1;

        int index = 0;

        // Loop through scores in list to find the rank of this score
        for (int i = 0; i < highScores.scoreList.Count; i++)
        {
            index++;

            if (playerScore >= highScores.scoreList[i].playerScore)
            {
                return index;
            }
        }

        if (highScores.scoreList.Count < Settings.numberOfHighScoresToSave)
            return (index + 1);

        //return 0 if not higher than other highscores
        return 0;
    }

    
}