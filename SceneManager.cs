using Godot;
using System;

public partial class SceneManager : CanvasLayer
{
	
	public static SceneManager Instance { get; private set; }
	
	[Export] public PackedScene EndingScene;
	[Export] public PackedScene GameOverScene;
	
	private bool _busy = false;
	private ColorRect _fade;

	public string Ending = "Death";
	public override void _Ready()
	{
		Instance = this;
		Layer = 100;
		
		_fade = new ColorRect();
		_fade.MouseFilter = Control.MouseFilterEnum.Ignore;
		_fade.Color = Colors.NavyBlue;
		_fade.Modulate = new Color(_fade.Color, 0);
		
		_fade.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		AddChild(_fade);
	}

	public void Reset()
	{
		Ending = "Death";
	}
	public async void ChangeScene(PackedScene  path)
	{
		if (_busy) return;
		_busy = true;

		await Fade(1f, 1);
		GetTree().ChangeSceneToPacked(path);
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged);
		await Fade(0f, 1);
		
		_busy = false;
	}

	public SignalAwaiter Fade(float to, float time)
	{
		var tween = CreateTween();
		tween.TweenProperty(_fade, "modulate:a", to, time);
		return ToSignal(tween, Tween.SignalName.Finished);
	}

	public void GameEnd()
	{
		if (Ending == "Ending")
		{
			ChangeScene(EndingScene);
		}
		else if (Ending == "Death")
		{
			ChangeScene(GameOverScene);
		}
	}
}
