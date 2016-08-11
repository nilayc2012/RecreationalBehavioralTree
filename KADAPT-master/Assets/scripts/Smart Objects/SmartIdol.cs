using UnityEngine;
using System.Collections;
using BehaviorTrees;

public class SmartIdol : SmartObject{

	GoTo goTo;

	void Awake () {
		
		name = this.gameObject.name;
	}

	void Start() {
		
		name = this.gameObject.name;
	}
	
	public bool Equals(SmartIdol obj) {
		
		if (name == obj.name)
			return true;

		return false;
	}
}
