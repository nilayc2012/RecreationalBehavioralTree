using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class Constants {

	public static string PBT_Trace = "";
	public static string traceFilesPath = Application.dataPath + "/traces/";
	public static bool StartSimulation = false;
	public static bool MergeMemories = false;
	public static bool ShowMergePane = false;
	public static List<string> objsWithMemories = new List<string> ();
	public static List<string> objsMergeMemories = new List<string> ();
	public static Dictionary<string, List<TraceMessage>> agentMemories = new Dictionary<string, List<TraceMessage>> ();

	public enum AFF_TAGS {
        move_to,
		walks_to,
		looks_at,
		buys,
		accept_apology,
		amazed,
		amazing,
		angry,
		apologize,
		bad,
		bye,
		cheer,
		cheerful,
		compliment,
		congratulate,
		good,
		goodbye,
		greet,
		greet_smile,
		insult,
		joke,
		laugh,
		meets,
		sad,
		shakehands,
		shy,
		smile,
		tell_news,
		thanks,
		wave,
		wicked,
		wicked_smile,
		go_to_corner,
		go_to_theatre,
        steal_idol
		
		
	};

	public static Dictionary<AFF_TAGS, System.Type> affordanceMap = new Dictionary<AFF_TAGS, System.Type> {

		{ AFF_TAGS.walks_to, typeof(GoTo) },
		{ AFF_TAGS.looks_at, typeof(LookAt) },
		{ AFF_TAGS.buys, typeof(Buy) },
		{ AFF_TAGS.accept_apology, typeof(AcceptApology) },
		{ AFF_TAGS.amazed, typeof(Amazed) },
		{ AFF_TAGS.amazing, typeof(Amazing) },
		{ AFF_TAGS.angry, typeof(Angry) },
		{ AFF_TAGS.apologize, typeof(Apologize) },
		{ AFF_TAGS.bad, typeof(Bad) },
		{ AFF_TAGS.bye, typeof(Bye) },
		{ AFF_TAGS.cheer, typeof(Cheer) },
		{ AFF_TAGS.cheerful, typeof(Cheerful) },
		{ AFF_TAGS.compliment, typeof(Compliment) },
		{ AFF_TAGS.congratulate, typeof(Congratulate) },
		{ AFF_TAGS.good, typeof(Good) },
		{ AFF_TAGS.goodbye, typeof(GoodBye) },
		{ AFF_TAGS.greet, typeof(Greet) },
		{ AFF_TAGS.greet_smile, typeof(GreetSmile) },
		{ AFF_TAGS.insult, typeof(Insult) },
		{ AFF_TAGS.joke, typeof(Joke) },
		{ AFF_TAGS.laugh, typeof(Laugh) },
		{ AFF_TAGS.meets, typeof(Meet) },
		{ AFF_TAGS.sad, typeof(Sad) },
		{ AFF_TAGS.shakehands, typeof(ShakeHands) },
		{ AFF_TAGS.shy, typeof(Shy) },
		{ AFF_TAGS.smile, typeof(Smile) },
		{ AFF_TAGS.tell_news, typeof(TellNews) },
		{ AFF_TAGS.thanks, typeof(Thanks) },
		{ AFF_TAGS.wave, typeof(Wave) },
		{ AFF_TAGS.wicked, typeof(Wicked) },
        { AFF_TAGS.wicked_smile, typeof(WickedSmile) },
		{ AFF_TAGS.go_to_corner, typeof(GoToCorner) },
		{ AFF_TAGS.go_to_theatre, typeof(GoToTheatre) },
        { AFF_TAGS.steal_idol, typeof(GoToIdol) },
        { AFF_TAGS.move_to, typeof(MoveTo) }

    };

	public enum ConditionType {
		AT,
		IS_ALIVE
	};

	public static List<string> visualizableAttributes = new List<string> {
		ConditionType.AT.ToString ()
	};

	public static Dictionary<string, string> smartObjToGameObjMap = new Dictionary<string, string>();
}
