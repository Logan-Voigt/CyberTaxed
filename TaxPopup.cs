using Godot;
using System;

public partial class TaxPopup : Node2D
{
	[Export] public Label taxLabel;
	[Export] public float duration;

	private Tween _moveTween;
	public override void _Ready()
	{
		float xOffset = (float)GD.RandRange(-16, 16);
		Vector2 offset = new Vector2(xOffset, -16f);
		
		
		float angle = (float)GD.RandRange(-10.0, 10.0);
		
		_moveTween = CreateTween();
		_moveTween.TweenProperty(this, "global_position",offset, duration)
			.AsRelative()
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		_moveTween.Parallel().TweenProperty(this, "rotation_degrees", angle, duration)
			.AsRelative()
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);
		_moveTween.TweenCallback(Callable.From(QueueFree));
	}

	public void EditLabel(string taxText)
	{
		taxLabel.Text = taxText;
	}
}
