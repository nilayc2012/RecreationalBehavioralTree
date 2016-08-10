using UnityEngine;
using System.Collections;

namespace BehaviorTrees {
	/* A condition class type to define preconditions and post-conditions
	 * for the affordances. A condition has the actor name, condition name,
	 * and the state of the condition.
	 */

	//TODO: Change all the code to follow Condition ENUM instead of string
	public class Condition : object{
		protected string actor;
		protected string condName;
		protected bool status;
		protected string relActor = "";

		protected bool isLocation = false;


		// Getters
		public string getActor () {
		
			return this.actor;
		}

		public string getRealtedActor () {

			return this.relActor;
		}

		public string getConditionName () {

			return this.condName;
		}

		public bool getStatus () {

			return this.status;
		}

		public bool IsLocation () {

			return isLocation;
		}

		// Constructors
		public Condition () {}
		public Condition(string actor, string cond, bool status) {

			this.actor = actor;
			this.condName = cond;
			this.status = status;
		}

		public Condition(string actor, Constants.ConditionType cond, bool status) {

			this.actor = actor;
			this.condName = cond.ToString ();
			this.status = status;
		}

		public Condition(string actor, Constants.ConditionType cond, string relName, bool status = true) {

			this.actor = actor;
			this.condName = cond.ToString ();
			this.relActor = relName;
			this.status = status;
		}

		// Operators
		public bool IsNegation (Condition cond) {

			return ((actor.Equals (cond.getActor ())) && (condName.Equals (cond.getConditionName ())) && (relActor.Equals (cond.getRealtedActor ())) && (!status.Equals (cond.getStatus ())));

		}

		public bool IsLocationContradiction (Condition cond) {

			return ((this.IsLocation () && cond.IsLocation ()) && ((actor.Equals (cond.getActor ())) && (!relActor.Equals (cond.getRealtedActor ()))));
		}

		public bool IsContradicts(Condition cond) {

			return (this.IsNegation (cond) || this.IsLocationContradiction (cond));
		}

		public string asString () {

			/*if (status == false)
				cond = "not-" + cond;
			if (isInit == true)
				return actor + " \\ is \\ " + cond + "\n";
			else
				return actor + " \\ thus changes \\ " + cond + "\n";*/

			return actor + " " + condName + " " + relActor + " " + status.ToString ();
		}

		public override bool Equals(System.Object obj)
		{
			// If parameter cannot be cast to ThreeDPoint return false:
			Condition c = obj as Condition;
			if ((object)c == null)
			{
				return false;
			}

			// Return true if the fields match:
			return asString().Equals(c.asString());
		}

		public bool Equals (Condition cond) {

			return (this.asString ().Equals (cond.asString ()));
		}
	}

	class Location : Condition {

		public Location(string actor, string location) : base (actor, Constants.ConditionType.AT, location) { 
		
			isLocation = true;
		}

		public Location(string actor, Vector3 location) : base(actor, Constants.ConditionType.AT, location.ToString()) {

			isLocation = true;
		}
	}
}
