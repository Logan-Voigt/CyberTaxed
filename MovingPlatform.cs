using Godot;
using System;

public partial class MovingPlatform : AnimatableBody2D
{
	[Export] private RichTextLabel PayLabel;
	[Export] private float Cost;
	[Export] private Area2D PayArea;

	[ExportCategory("Moving")]
	[Export] public Vector2 Offset;
	[Export] private float Pause;
	[Export] private float Duration;

	private bool _purchased;
	private Vector2 _start;
	private Tween _tween;
	public override void _Ready()
	{
		_start = Position;
		PayLabel.Text = $"[color=red][b]FEE:[/b] -{Cost}$[/color]";
		PayArea.BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is CharacterController controller)
		{
			if (!_purchased && controller.Cash >= Cost)
			{
				controller.Cash -= Cost;
				controller.UpdateCash();
				_purchased = true;
				PayLabel.Text = $"[color=green][b]$Purchased$[/b][/color]";
			}
			
			if (_purchased)
				MovePlatformTowardOffset();
		}
	}


	private void MovePlatformTowardOffset()
	{
		_tween = CreateTween();

		_tween.SetProcessMode(Tween.TweenProcessMode.Physics);
		_tween.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		
		_tween.TweenProperty(this, "position", _start + Offset, Duration);
		_tween.TweenInterval(Pause);
		_tween.TweenProperty(this, "position", _start, Duration);
	}

}
