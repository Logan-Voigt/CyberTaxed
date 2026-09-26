using Godot;
using System;

public partial class TaxPopup : Node3D
{
	[Export] public Label3D taxLabel;
	[Export] public float duration;

	private Tween _moveTween;
	public override void _Ready()
	{
		float xOffset = (float)GD.RandRange(-1.0, 1.0);
		Vector3 offset = new Vector3(xOffset, .75f, 0f);
		
		_moveTween = CreateTween();
		_moveTween.TweenProperty(this, "global_position",offset, duration)
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
