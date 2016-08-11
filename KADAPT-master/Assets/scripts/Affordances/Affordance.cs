using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;

namespace BehaviorTrees {
	public class Affordance {

		public string name = "Action";
		public SmartObject affordant;
		public SmartObject affordee;
		protected List<Condition> preconditions = new List<Condition>();
		protected List<Condition> effects = new List<Condition>();
		protected string tag = "";
		protected Node root = null;

		public string asString () {

			return this.name;
		}

		protected void initialize() {

			tag = Constants.affordanceMap.FirstOrDefault (t => t.Value == this.GetType ()).Key.ToString ();
			name = affordant.name + " " + tag + " " + affordee.name;
		}

		/*
		public string actionSummary () {

			string summary = this.asString ();
			foreach (Condition cond in effects) {
				summary = summary + cond.asString ();
			}
			return summary;	
		}
*/
		public List<Condition> GetEffects () {

			return effects;
		}

		public List<Condition> GetPreConditions () {

			return preconditions;
		}

		public Node GetPBTRoot () {

			return root;
		}

		//TODO : Redundant, Ridiculous, REMOVE!!
		public void AddSummaryToTrace () {

//			Constants.PBT_Trace = Constants.PBT_Trace + this.actionSummary ();
		}

		public void publishMessage () {
			//TraceMessage msg = new TraceMessage (this.asString (), affordant.name, affordee.name);
			float time = Time.realtimeSinceStartup;//.ToString("n2");// + " " + this.asString();
			MessageBus.PublishMessage (new TraceMessage(time, this.asString(), affordant.name, affordee.name));
		}

		public Node PublisMsgNode () {

			return new LeafInvoke (
				() => this.publishMessage());
		}


		public Node UpdateNarrativeStateNode () {

			return new LeafInvoke (
				() => NSM.UpdateNarrativeState (effects));
		}

		public Node UpdateAndPublish () {

			return new Sequence (this.PublisMsgNode (), this.UpdateNarrativeStateNode ());
		}

		public bool Equals(Affordance aff) {

			if (aff == null)
				return false;
			else if (this.asString () == aff.asString ())
				return true;
			else
				return false;
		}
	}
}
