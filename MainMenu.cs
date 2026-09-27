using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] private Button Start;
	[Export] private PackedScene LoreScene;

	public override void _Ready()
	{
		
		GD.Print("Starting Menu...");
		Start.Pressed += StartGame;
	}

	public void StartGame()
	{
		GD.Print("Starting game...");
		SceneManager.Instance.ChangeScene(LoreScene);
	}
}
