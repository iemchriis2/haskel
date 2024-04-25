//Bonga Maswanganye and Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;


using UnityEngine;




public class ObjectSpawner : MonoBehaviour {

	#region Variables

	//Preference Variables
	// arrays containing all the hazard objects in a scene
	[SerializeField]
	private HazardObject[] _hazards_Housekeeping_PPE;
	[SerializeField]
	private HazardObject[] _hazards_Elevated_Platforms;
	[SerializeField]
	private HazardObject[] _hazards_Electrical;
	[SerializeField]
	private HazardObject[] _hazards_Fire_Protection;
	[SerializeField]
	private HazardObject[] _hazards_Trenching;
	[SerializeField]
	private HazardObject[] _hazards_Guarding;
	[SerializeField]
	private HazardObject[] _hazards_Welding;
	[SerializeField]
	private HazardObject[] _hazards_Cranes;
	[SerializeField]
	private HazardObject[] _hazards_Neutral;


	//Script Variables
	private System.Random random = new System.Random();
	private string _dataPath;
	//public Dictionary<bool, string> Scene0, Scene1, Scene2;
	public HazardData Dictionaries = new HazardData();
	public string[] XMLStringHolder = new string[3];
	public int CurrentScene = 0;
	public static ObjectSpawner spawner;

	#endregion



	#region MonoBehaviour Implementation

	void Awake()
	{
		spawner = this;
		_dataPath = Application.persistentDataPath + @"\HeroVR\Data";
		
	}
	
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.X))
		{
			SaveAllArrays();
			WriteDictionaryToXML(Dictionaries.Scenes[CurrentScene]);
		}
		if (Input.GetKeyUp(KeyCode.Y))
		{
			LoadObjectsInScene(XMLStringHolder[CurrentScene],CurrentScene);
		}
		if (Input.GetKeyUp(KeyCode.Alpha0))
		{
			LoadObjectsInScene(XMLStringHolder[0], 0);
		}
		if (Input.GetKeyUp(KeyCode.Alpha1))
		{
			LoadObjectsInScene(XMLStringHolder[1], 1);
		}
		if (Input.GetKeyUp(KeyCode.Alpha2))
		{
			LoadObjectsInScene(XMLStringHolder[2], 2);
		}
		if (Input.GetKeyUp(KeyCode.Alpha3))
		{
			ObjectSpawner.LoadScene(0);
		}
	}
	private void OnValidate() {

		CheckAllArrays();

	}
	[ContextMenu("Switch To Scene 0")]
	void SwitchScene0()
	{
		CurrentScene = 0;
	}
	[ContextMenu("Switch To Scene 1")]
	void SwitchScene1()
	{
		CurrentScene = 1;

	}
	[ContextMenu("Switch To Scene 2")]
	 void SwitchScene2()
	{
		CurrentScene = 2;
	}
	[ContextMenu("Save Current Scene")]
	public void SaveCurrentScene()
	{
		CheckAllArrays();
		SaveAllArrays();
		WriteDictionaryToXML(Dictionaries.Scenes[CurrentScene]);

	}
	#endregion



	#region Spawn

	static public void SelectScene(int SelectedScene)
	{
		spawner.CurrentScene = SelectedScene;
	}
	static public void LoadScene(int SelectedScene)
	{
		LoadObjectsInScene(spawner.XMLStringHolder[SelectedScene], SelectedScene);
	}
	public void AddToDictionary(HazardObject[] hazards)
	{
		foreach (HazardObject i in hazards)
		{
			Dictionaries.Scenes[CurrentScene].Add(i.Name, i.ExistInScene);
		}

	}
	public void WriteDictionaryToXML(Dictionary<string,bool> Scene)
	{
		DataContractSerializer serializer = new DataContractSerializer(Scene.GetType());
		StringWriter sw = new StringWriter();
		XmlTextWriter writer = new XmlTextWriter(sw);
		writer.Formatting = Formatting.Indented;
		serializer.WriteObject(writer, Scene);
		XMLStringHolder[CurrentScene] = sw.GetStringBuilder().ToString();
		Dictionaries.XMLData[spawner.CurrentScene] = sw.GetStringBuilder().ToString();
		sw.Close();
	}

	 public static void LoadObjectsInScene(string XMLStringHolder, int SceneToLoad)
	{
		StringReader stringReader = new StringReader(XMLStringHolder);
		XmlReader xmlReader = XmlReader.Create(stringReader);
		DataContractSerializer deserializer = new DataContractSerializer(typeof(Dictionary<string,bool>));
		Dictionary<string,bool> holder = (Dictionary<string,bool>)deserializer.ReadObject(xmlReader);
		SetLoadedScene(holder);
		xmlReader.Close();
		stringReader.Close();

	}
	 public static void SetLoadedScene(Dictionary<string,bool> keys)
	{
		GameObject holder;
		GameObject[] HazardsInScene = GameObject.FindGameObjectsWithTag("Hazard");
		foreach(KeyValuePair<string,bool> i in keys)
		{
			for(int z = 0; z < HazardsInScene.Length; z++)
			{
				HazardObject ho = HazardsInScene[z].GetComponent<HazardObject>();
				ho.Clean();
				if (ho.Name == i.Key)
				{
					if (i.Value == false)
					{
						holder = HazardsInScene[z];
						holder.SetActive(false);

					}
					if (i.Value == true)
					{
						holder = HazardsInScene[z];
						holder.SetActive(true);
					}
				}
			}

		}
	}



	#endregion



	#region Internal

	private void CheckArraySolidarity(HazardObject[] array, HazardObject.HazardType type) {
		for (int i = 0; i < array.Length; i++) {
			HazardObject h = array[i].GetComponent<HazardObject>();
			if (h == null) {
				Debug.LogWarning("Object in " + type + " has no HazardObject component.");
				
			} else if (h.Type != type) {
				Debug.LogWarning("Object in " + type + " has incorrect HazardType.");
				
			}
		}

	}
	private void CheckAllArrays()
	{
		Dictionaries.Scenes[CurrentScene].Clear();
		CheckArraySolidarity(_hazards_Housekeeping_PPE, HazardObject.HazardType.HOUSEKEEPING_PPE);
		CheckArraySolidarity(_hazards_Elevated_Platforms, HazardObject.HazardType.ELEVATED_PLATFORMS);
		CheckArraySolidarity(_hazards_Electrical, HazardObject.HazardType.ELECTRICAL);
		CheckArraySolidarity(_hazards_Fire_Protection, HazardObject.HazardType.FIRE_PROTECTION);
		CheckArraySolidarity(_hazards_Trenching, HazardObject.HazardType.TRENCHING);
		CheckArraySolidarity(_hazards_Guarding, HazardObject.HazardType.GUARDING);
		CheckArraySolidarity(_hazards_Welding, HazardObject.HazardType.WELDING);
		CheckArraySolidarity(_hazards_Cranes, HazardObject.HazardType.CRANES);
	}
	private void SaveAllArrays()
	{
		AddToDictionary(_hazards_Housekeeping_PPE);
		AddToDictionary(_hazards_Elevated_Platforms);
		AddToDictionary(_hazards_Electrical);
		AddToDictionary(_hazards_Fire_Protection);
		AddToDictionary(_hazards_Trenching);
		AddToDictionary(_hazards_Guarding);
		AddToDictionary(_hazards_Welding);
		AddToDictionary(_hazards_Cranes);


	}




	#endregion

}

