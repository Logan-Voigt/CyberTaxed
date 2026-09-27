using Godot;
using System;

public partial class Martin : Node2D
{
	[Export] private Area2D TalkBox;
	[Export] private RichTextLabel TextBox;
	[Export] private AnimatedSprite2D AnimatedSprite;

	public override void _Ready()
	{
		AnimatedSprite.Play("Idle");
	}
	public void SayLine(string text)
	{
		AnimatedSprite.Play("Talking");
		TextBox.Show();
		
		TextBox.Modulate = new Color(TextBox.Modulate, 1f);
		TextBox.Text = text;
		TextBox.VisibleRatio = 0f;

		float timeToType = text.Length * 0.03f;

		Tween tween = CreateTween();
		tween.TweenProperty(TextBox, "visible_ratio", 1f, timeToType);
		tween.TweenInterval(1.5);
		tween.TweenProperty(TextBox, "modulate:a", 0, 0.25f);
		tween.TweenCallback(Callable.From(() => TextBox.Hide()));


	}
	
	
	
	
}
