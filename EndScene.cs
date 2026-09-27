using Godot;
using System;

public partial class EndScene : Control
{
	[Export] private TextureRect continueScreen;
	[Export] private PackedScene MainMenu;

	private float _timerDelay = 3;
	private float _timer;

	private bool _startScroll;
	private bool _scrolled = false;
	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		_timer += (float)delta;

		if (_timer >= _timerDelay)
		{
			_startScroll = true; 
		}

		if (_startScroll && !_scrolled)
		{
			RunCredits();
			_scrolled = true;
		}
	}

	public async void RunCredits()
	{
		Vector2 targetPosition = new Vector2(continueScreen.Position.X, continueScreen.Position.Y-1200);
		Tween tween = CreateTween();
		tween.TweenProperty(continueScreen, "global_position", targetPosition, 10);
		await ToSignal(tween, Tween.SignalName.Finished);
		
		SceneManager.Instance.ChangeScene(MainMenu);
	}
}
