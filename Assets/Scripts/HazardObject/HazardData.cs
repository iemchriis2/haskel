using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;


using UnityEngine;

[DataContract]
public class HazardData
    //contains the XML data for loading each scene and 3 dictonaries containing all the objects in the hazard array for each of the 3 scenes
{
    [DataMember,SerializeField]
    public Dictionary<string, bool>[] Scenes = new Dictionary<string, bool>[]
    {
        
        new Dictionary<string, bool>(),
        new Dictionary<string, bool>(),
        new Dictionary<string, bool>()
    };
    [SerializeField]
    public string[] XMLData = new string[3];


}

