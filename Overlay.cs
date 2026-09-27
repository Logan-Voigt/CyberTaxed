using Godot;
using System;
using System.Runtime.Intrinsics;

public partial class Overlay : TextureRect
{
	private float _t;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		_t += (float)delta;
		Position = new Vector2(Mathf.Sin(_t * 0.2f), Mathf.Cos(_t * 0.15f)) * 120f;
		Scale = Vector2.One * 1.5f;
	}
}
