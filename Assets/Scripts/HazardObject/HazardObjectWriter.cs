//Bonga Maswanganye

using System.Collections;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.IO;

using UnityEngine;


[DataContract]
public class HazardObjectWriter 
{
    [DataMember]
    public string Name;
    [DataMember]
    public bool ExistInScene;






    public HazardObjectWriter(HazardObject x)
    {
        Name = x.Name;
        ExistInScene = x.ExistInScene;

    }

}
