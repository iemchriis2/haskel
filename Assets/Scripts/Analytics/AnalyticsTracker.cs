//Thomas Pereira and Micheal Revit

using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.Analytics;



public class AnalyticsTracker : MonoBehaviour
{
    //These aren't used but I don't want to break anything so I'm going to leave them here. (Thomas)
    public static string PlayerName;
    public static string SceneName;
    public static string TimeToComplete;
    public static int CorrectAnswers;
    public static int IncorrectAnswers;
    public static int MissedHazards;
    public static int NeutralAnswers;
    public static float OverallScore;

    private int _updatedUnanswered;

    public static EasyEvent<float> OverallScoreCalculatedEvent = new EasyEvent<float>("Overall Score Calculated");

    private AnalyticsResult _analyticsResult;

    private void Awake()
    {
        EndMenu.GameEndEvent.Subscribe(this, EV_SendAnalytics);
    }

    private void EV_SendAnalytics()
    {
        
        HazardObject.Calculate();

        if(HazardObject.Correct == 0 && HazardObject.Incorrect == 0){
            OverallScore = 0;
        }
        else{
            OverallScore = (1f* (HazardObject.Correct)/(HazardObject.Correct + HazardObject.Incorrect)*100f);
            OverallScore = Mathf.Round(OverallScore);
        }


        Analytics.EnableCustomEvent("GameStats", true);
        //Sends analytics to Unity.
        _analyticsResult = Analytics.CustomEvent("GameStats", new Dictionary<string, object>
        {
            {"Player Name", KeyboardMenu.Name },
            {"Scene Name", "Stage " + (MainMenu.StageIndex + 1).ToString() },
            {"Time To Complete", EndMenu.TimeString },
            {"Correct Answers", HazardObject.Correct.ToString() },
            {"Incorrect Answers", HazardObject.Incorrect.ToString() },
            {"Unanswered Items in Scene", HazardObject.Unanswered.ToString() },
            {"Overall Score as a percentage", OverallScore.ToString() + "%" }
        });
         
        Analytics.FlushEvents();
        Debug.Log("Analytics result: " + _analyticsResult.ToString());
        Debug.Log("Should've sent analytics.");
        
        OverallScoreCalculatedEvent?.Invoke(OverallScore);

        //Quicker way to test analytics
        //WriteAnalyticsToTxtFile();
    }

    private void WriteAnalyticsToTxtFile()
    {
        string _path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + @"\Hero3Output.txt";

        StreamWriter _streamWriter = new StreamWriter(_path, true);
        _streamWriter.WriteLine("Player Name: " + KeyboardMenu.Name);
        _streamWriter.WriteLine("Stage: " + (MainMenu.StageIndex + 1).ToString());
        _streamWriter.WriteLine("Time To Complete: " + EndMenu.TimeString);
        _streamWriter.WriteLine("Correct Answers: " + HazardObject.Correct.ToString());
        _streamWriter.WriteLine("Incorrect Answers: " + HazardObject.Incorrect.ToString());
        _streamWriter.WriteLine("Unanswered Items in Scene: " + HazardObject.Unanswered.ToString());
        
        //if(HazardObject.Correct != 0 || HazardObject.Incorrect != 0){
        //}
        //else{
        //    _streamWriter.WriteLine("Unanswered Items in Scene: " + HazardObject.HazardList.Count);
        //}
        //_streamWriter.WriteLine("Percentage of Items Looked at: " + (1f*HazardObject.LookedAtHazardList.Count/HazardObject.HazardList.Count)*100f);
        _streamWriter.WriteLine("Overall Score as a percentage: " + OverallScore.ToString() + "%");
        _streamWriter.WriteLine("Hazard items looked at: " + HazardObject.LookedAtHazardList.Count);
        foreach(var hazard in HazardObject.LookedAtHazardList)
        {
            _streamWriter.WriteLine(hazard.Name);
        }
        _streamWriter.WriteLine();
        Debug.Log("After streamwriter");
        _streamWriter.Close();
    }
}



//Log overall score((correct+incorrect)/overall looked at only) for each player; this includes:
//o Player’s name
//o Scene number
//o number of correct answers
//o number of incorrect answers (identified hazards in the wrong category)
//o number of missed hazards
//o number of neutral answers (objects that are not hazards but were assigned one)
//o time to complete

        // Analytics.CustomEvent("GameStats", new Dictionary<string, object>
        // {
        //     {"Player Name", KeyboardMenu.Name },
        //     {"Scene Name", (MainMenu.StageIndex + 1).ToString() },
        //     {"Time To Complete", EndMenu.TimeString },
        //     {"Correct Answers", HazardObject.Correct.ToString() },
        //     {"Incorrect Answers", HazardObject.Incorrect.ToString() },
        //     {"Neutral Answers", HazardObject.AnsweredNeutrals.ToString() },
        //     {"Unanswered Items in Scene", _updatedUnanswered.ToString() },
        //     {"Percetage of Items Looked At", (1f * HazardObject.LookedAtHazardList.Count/HazardObject.HazardList.Count) * 100 },
        //     {"Overall Score as a percentage", OverallScore.ToString() + "%" }
        // });