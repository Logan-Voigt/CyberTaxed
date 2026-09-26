using System.Collections.Generic;
using Godot;

public enum PlayerStates
{
	IDLE,
	WALKING,
	RUNNING,
	CROUCHING,
}
public partial class CharacterControler : CharacterBody3D
{
	[ExportCategory("Movement")]
	[Export] public float WalkSpeed = 10f;
	[Export] public float RunSpeed = 18f;
	[Export] public float CrouchSped = 6;
	[Export] public float JumpVelocity = 9f;

	[ExportCategory("Taxes & Money")] 
	
	[Export] public float Cash = 1000;
	[Export] public Label3D CashLabel;
	[Export] public Node3D SpawnTransform;
	[Export] public PackedScene TaxPopupScene;
	[Export] public float MovingTaxTime = 2;
	[Export] public float CrouchTax = 1f;
	[Export] public float WalkingTax = 2;
	[Export] public float RunningTax = 4;
	[Export] public float JumpTax = 5;
	[Export] public Color positiveCash = Colors.Green;
	[Export] public Color negativeCash = Colors.Red;
	
	private PlayerStates _currentState = PlayerStates.IDLE;
	private float _movingTimer;
	private Dictionary<PlayerStates, float> _speeds;
	
	
	private const float speedMultiplier = 10f;
	public override void _Ready()
	{
		UpdateCash();
		_speeds = new Dictionary<PlayerStates, float>()
		{
			{PlayerStates.IDLE, 0f},
			{ PlayerStates.WALKING, WalkSpeed },
			{ PlayerStates.RUNNING, RunSpeed },
			{ PlayerStates.CROUCHING, CrouchSped }
		};

	}
	public override void _PhysicsProcess(double delta)
	{
		
		Vector3 vel = Velocity;

		
		if (!IsOnFloor())
			vel.Y -= 9.8f * (float)delta;
		
		else if (Input.IsActionJustPressed("Jump"))
		{
			vel.Y = JumpVelocity;
			ApplyTax(JumpTax, "Jump Tax", vel.Y);
		}


		if (Input.IsActionPressed("Crouch"))
			_currentState = PlayerStates.CROUCHING;
		else if (Input.IsActionPressed("Sprint"))
			_currentState = PlayerStates.RUNNING;
		else if (Input.IsActionPressed("Left") || Input.IsActionPressed("Right"))
			_currentState = PlayerStates.WALKING;
		else 
			_currentState = PlayerStates.IDLE;


		float taxAmount = 0f;
		string taxReason = "";

		switch (_currentState)
		{
			case PlayerStates.CROUCHING:
				taxAmount = CrouchTax;
				taxReason = "Crouching Tax";
				break;
			case PlayerStates.RUNNING:
				taxAmount = RunningTax;
				taxReason = "Running Tax";
				break;
			case PlayerStates.WALKING:
				taxAmount = WalkingTax;
				taxReason = "Walking Tax";
				break;
		}
		
		vel.X = Input.GetAxis("Left", "Right") * _speeds[_currentState] * (float)delta * speedMultiplier;
		vel.Z = 0;

		if (_currentState != PlayerStates.IDLE  && vel.X != 0)
		{
			_movingTimer += (float)delta;
		}

		if (_movingTimer > MovingTaxTime && vel.X != 0)
		{
			_movingTimer = 0;
			ApplyTax(taxAmount, taxReason);
		}
		
		
		

		Velocity = vel;
		MoveAndSlide();
		Position = new Vector3(Position.X, Position.Y, 0);
	}

	private void ApplyTax(float taxAmount, string taxReason, float upVelocity = 0f)
	{
		Cash -= taxAmount;
		string taxString = $"{taxReason} -{taxAmount}$";
		SpawnTaxPopup(taxString, upVelocity);
		UpdateCash();
	}

	private void SpawnTaxPopup(string taxString, float upVelocity = 0f)
	{
		TaxPopup popup = TaxPopupScene.Instantiate<TaxPopup>();
		popup.Position = SpawnTransform.Position + new Vector3(0, upVelocity, 0) * .1f;
		AddChild(popup);
		popup.EditLabel(taxString);
	}
	
	private void UpdateCash()
	{
		if (Cash >= 0)
			CashLabel.Modulate = positiveCash;
		
		else if (Cash < 0)
			CashLabel.Modulate = negativeCash;
		
		CashLabel.Text = $"Cash: {Cash}";
	}
}
