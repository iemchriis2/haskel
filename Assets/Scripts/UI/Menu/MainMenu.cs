//Michael Reivt/

#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;



/// <summary>
/// This class allows the user to transition to the stage (or tutorial) of their choosing.
/// </summary>
public class MainMenu : Menu {

	#region Variables

	//Singleton Variable
	public static MainMenu Singleton;

	//Event Variables
	public static EasyEvent GameStartEvent = new EasyEvent("Game Start");
	public static string LoadedSceneName { get { return Singleton._loadedSceneName; } }

	//Preference Variables
	[SerializeField]
	private PhysicalButton _stageButton0, _stageButton1, _stageButton2, _tutorialButton, _quitButton;
	[SerializeField]
	private float _fadeExtraTime = 1.5f;

	//Script Variables
	private string _loadedSceneName;

	public GameObject[] HazardVariations;
	public static int StageIndex;
	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		Singleton = this;
	}



    [SerializeField]
    private int _editorAutoPlayLevelIndex = -1;

    protected override void Start()
    {
        _stageButton0.ButtonUpEvent.Subscribe(this, () => { EV_PlayButton(0); });
        _stageButton1.ButtonUpEvent.Subscribe(this, () => { EV_PlayButton(1); });
        _stageButton2.ButtonUpEvent.Subscribe(this, () => { EV_PlayButton(2); });
		_quitButton.ButtonUpEvent.Subscribe(this, EV_QuitButton);
        _tutorialButton.ButtonUpEvent.Subscribe(this, EV_TutorialButton);
        EndMenu.GameEndEvent.Subscribe(this, EV_GameEnd);
        ScoreDoneMenu.RequestEvent.Subscribe(this, EV_DoneLookingAtScoreEvent);
		NameConfirmationMenu.RequestEvent.Subscribe(this, EV_DoneLookingAtScoreEvent);

        // if (_editorAutoPlayLevelIndex != -1)
        //     StartCoroutine(Coroutine());

		foreach(GameObject hazard in HazardVariations)
		{
			hazard.gameObject.SetActive(false);
		}

		gameObject.SetActive(false);
    }

    // private IEnumerator Coroutine()
    // {
    //     yield return null;
    //     yield return null;
    //     yield return null;
    //     yield return new WaitForSeconds(.5f);

    //     EV_PlayButton(_editorAutoPlayLevelIndex);
    // }

    #endregion



    #region Event Callbacks

    public void EV_PlayButton(int index) {
		StageIndex = index;
		//StartCoroutine(LoadScene("Stage" + index, () => { GameStartEvent.Invoke(); }, index));
		for (int i = 0; i < HazardVariations.Length; i++)
		{
			if (i == index)
			{
				HazardVariations[i].gameObject.SetActive(true);
			}
			else
			{
				HazardVariations[i].gameObject.SetActive(false);
			}
		}
		GameStartEvent.Invoke();
	}



	public void EV_TutorialButton() {
		//StartCoroutine(LoadScene("Tutorial", () => { Tutorial.StartTutorial(); }));
		Tutorial.StartTutorial();
	}



	public void EV_GameEnd() {
		gameObject.SetActive(true);

		//StartCoroutine(UnloadScene());
	}



	private void EV_TutorialEnd() {
		gameObject.SetActive(true);

		//StartCoroutine(UnloadScene());
	}



	private void EV_DoneLookingAtScoreEvent() {
		gameObject.SetActive(true);
	}



	public void EV_QuitButton() {
		Debug.LogWarning("quit");
		Application.Quit();
	}

	#endregion



	#region Coroutines

	private IEnumerator LoadScene(string sceneName, Action action, int index = -1) {
		_loadedSceneName = sceneName;

		Fade.FadeToBlack(() => {}, 1, true);

		if (index != -1)
			ObjectSpawner.LoadScene(index);

		AsyncOperation a = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		while (!a.isDone)
			yield return null;

		action?.Invoke();

		Fade.Unlock();

		//gameObject.SetActive(false);
	}



	private IEnumerator UnloadScene() {
		AsyncOperation a = SceneManager.UnloadSceneAsync(_loadedSceneName, UnloadSceneOptions.None);
		while (!a.isDone)
			yield return null;
	}

	#endregion

}
