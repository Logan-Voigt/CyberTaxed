using Godot;
using System;

public partial class GameOverScene : Control
{
	[Export] private Button mainMenu;
	[Export] private PackedScene mainMenuScene;

	public override void _Ready()
	{
		mainMenu.Pressed += GoToMainMenu;
	}
	
	
	private void GoToMainMenu()
	{
		SceneManager.Instance.ChangeScene(mainMenuScene);
	}
}
