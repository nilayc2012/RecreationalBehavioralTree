using UnityEngine;
using System.Collections;
using BehaviorTrees;

public class SmartCharacter : SmartObject{

	GoTo goTo;

	void Awake () {
		
		name = this.gameObject.name;
	}
	
	public bool Equals(SmartCharacter obj) {
		
		if (name == obj.name)
			return true;

		return false;
	}
}
