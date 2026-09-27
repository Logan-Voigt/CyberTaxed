using Godot;
using System;

public partial class InfoSign : Node2D
{
	[Export(PropertyHint.MultilineText)] public string InfoText;
	[Export] public Area2D InfoArea;
	[Export] public RichTextLabel InfoLabel;

	private Tween _tween;
	public override void _Ready()
	{
		InfoLabel.Text = InfoText;
		InfoLabel.VisibleRatio = 0f;
		
		InfoArea.BodyEntered += OnBodyEntered;
		InfoArea.BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (!(body is CharacterController player)) return;
		InfoLabel.Modulate = new Color(InfoLabel.Modulate, 1f);
		_tween?.Kill();
		InfoLabel.VisibleRatio = 0f;
		float duration = InfoText.Length * 0.015f;
		_tween = CreateTween();
		_tween.TweenProperty(InfoLabel, "visible_ratio", 1f, duration);
	}

	private void OnBodyExited(Node body)
	{
		if (!(body is CharacterController player)) return;
		_tween?.Kill();
		
		_tween = CreateTween();
		
		_tween.TweenProperty(InfoLabel, "modulate:a", 0f, .5);
	}
	
}
