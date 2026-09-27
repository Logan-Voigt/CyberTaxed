using Godot;
using System;

public partial class EndSign : Area2D
{
	[Export] private KillBot killBot;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is CharacterController player)
		{
			SceneManager.Instance.Ending = "Ending";
			killBot.StartChase();
		}
	}
}
