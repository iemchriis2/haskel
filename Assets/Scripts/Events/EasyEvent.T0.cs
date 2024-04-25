//Michael Revit

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// A simple event class.
/// </summary>
public class EasyEvent {

	//Script Variables
	private string _eventName;
	private List<KeyValuePair<object, Action>> _collection = new List<KeyValuePair<object, Action>>();



	#region Constructor

	public EasyEvent(string eventName) {
		_eventName = eventName;
	}

	#endregion



	#region Subscribe Unsubscribe

	public void Subscribe(object obj, Action action) {
		_collection.Add(new KeyValuePair<object, Action>(obj, action));
	}



	public void Unsubscribe(object obj) {
		for (int i = 0; i < _collection.Count; i++)
			if (_collection[i].Key == obj)
				_collection.RemoveAt(i--);
	}



	public void Unsubscribe(Action action) {
		for (int i = 0; i < _collection.Count; i++)
			if (_collection[i].Value == action) {
				_collection.RemoveAt(i);
				break;
			}
	}

	#endregion



	#region Invoke

	public void Invoke() {
		for (int i = 0; i < _collection.Count; i++) {
			try {
				_collection[i].Value();
			} catch (Exception e) {
				Debug.LogError("Exception in Event [" + _eventName + "] :" + e.Message + "\n---\n" + e.StackTrace + "\n---");
			}
		}
	}

	#endregion

}
