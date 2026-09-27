using Godot;
using System;
using System.Drawing;
using Color = Godot.Color;

public partial class ItemPopup : Node2D
{
	[Export] public Label itemLabel;
	[Export] public Sprite2D itemSprite;
	[Export] public Sprite2D itemTexture;
	[Export] public PointLight2D itemLight;
	[Export] public float duration;

	private Tween _moveTween;
	public override void _Ready()
	{
		float xOffset = (float)GD.RandRange(-128, 128);
		Vector2 offset = new Vector2(xOffset, -128f);
		
		float angle = (float)GD.RandRange(-25.0, 25.0);
		
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

	public void UpdateItem(string itemText, Texture2D texture, Color color)
	{
		itemLabel.Text = $"+{itemText}$";
		itemSprite.Texture = texture;
		GradientTexture2D tex = (GradientTexture2D)itemTexture.Texture.Duplicate();
		tex.Gradient = (Gradient)tex.Gradient.Duplicate();
		tex.Gradient.SetColor(0, color);  
		tex.Gradient.SetColor(1, new Color(color, 0f));
		itemTexture.Texture = tex;
		itemLight.Color = color;
	}
	
}
