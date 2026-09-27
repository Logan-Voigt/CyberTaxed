using Godot;
using System;
using System.Collections.Generic;
using System.Dynamic;

public struct Dialog
{
	public string who;
	public string message;
	public float waitDelay;

	public Dialog(string who, string message, float waitDelay)
	{
		this.who = who;
		this.message = message;
		this.waitDelay = waitDelay;
	}
}
public partial class DialogManager : Node2D
{
	[Export] private Martin martin;
	[Export] private CharacterController player;
	[Export] public PackedScene NextScene;
	
	private List<Dialog> _dialogList = new List<Dialog>()
	{
		new Dialog("Martin", "Hey kid, you're late! Ah whatever, let's get started.", 4f),
		new Dialog("Martin", "The City has announced that the Destination " +
							 "for tax free movement has been decided. Those fools chose to " +
							 "keep the location hidden behind an encrypted dataset located " +
							 "deeper in the city.", 8f),
		new Dialog("Martin", " I need you to make your way to these locations"  +
							 " to find out what the destination is, I only have a few years left" +
							 "in these old bones, I could die in peace knowing I got to walk outside" +
							 " without paying those assholes a single penny!", 9f),
		new Dialog("Jordy", "You're only giving me [i]this[/i] much for such an important mission?", 5.5f),
		new Dialog("Martin", "Look kid, I don't have much left for years or coin, " +
							 "if I want to spend it on booze and VR sessions that is my right!", 7f),
		new Dialog("Jordy", "Fine, fine. I'll be back with the message, just don't keel over before then!", 7)
	};

	public override void _Ready()
	{
		StartConvo();
	}
	public async void StartConvo()
	{
		foreach (Dialog dialog in _dialogList)
		{
			if (dialog.who == "Martin")
				martin.SayLine(dialog.message);
			else
				player.SayLine(dialog.message);
			
			await ToSignal(GetTree().CreateTimer(dialog.waitDelay), SceneTreeTimer.SignalName.Timeout);
		}
		SceneManager.Instance.ChangeScene(NextScene);
	}
}
