using Godot;
using System;
using System.Runtime.InteropServices;

public partial class KillBot : Node2D
{
	[Export] private CharacterController player;
	[Export] private Area2D killArea;
	[Export] private AnimatedSprite2D anomationSprite;
	[Export] private int speed;
	

	private bool _chase;
	private bool _closeEnough;
	public override void _Ready()
	{
		player.Broke += StartChase;
		killArea.BodyEntered += OnBodyEntered;
		anomationSprite.Play("Attack");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		
		if (player == null) return;
		
		if (_chase && !_closeEnough)
		{
			Vector2 dir = (player.Position - GlobalPosition).Normalized();
			anomationSprite.FlipH = dir <= Vector2.Zero;
			GlobalPosition += dir * speed * (float)delta;
		}
	}

	private void OnBodyEntered(Node body)
	{
		if (body is CharacterController player)
		{
			player.Die();
			_closeEnough = true;
		}
	}

	private void OnBodyExited(Node body)
	{
		if (body is CharacterController player)
		{
			
			_closeEnough = false;
		}
	}

	public void StartChase()
	{
		_chase = true;
	}
}
