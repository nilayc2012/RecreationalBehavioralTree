using UnityEngine;
using System.Collections;
using BehaviorTrees;

public class SmartCharacter : SmartObject{

	GoTo goTo;
	public GameObject startloc;

	void Awake () {
		
		name = this.gameObject.name;
		startloc.transform.position=this.gameObject.transform.position;
		startloc.transform.rotation=this.gameObject.transform.rotation;
		
	}
	
	void Start() {
		
		name = this.gameObject.name;
	}	

	public bool Equals(SmartCharacter obj) {
		
		if (name == obj.name)
			return true;

		return false;
	}
}
