using System.Collections.Generic;
using System.Net.Mime;
using Godot;
using Godot.Collections;

public enum PlayerStates
{
	IDLE,
	WALKING,
	RUNNING,
	SLOW_WALKING,
	LOOTING
}

public partial class CharacterController : CharacterBody2D
{
	[ExportCategory("Movement")] [Export] public float WalkSpeed = 256f;
	[Export] public float RunSpeed = 512f;
	[Export] public float CrouchSped = 196f;
	[Export] public float JumpVelocity = 400f;
	[Export] public AnimatedSprite2D CharacterAnimations;


	[ExportCategory("Dialog")] [Export] public RichTextLabel TextBox;
	[Export] public Node2D LeftAnchor;
	[Export] public Node2D RightAnchor;


	[ExportCategory("looting")] 
	[Export] public float LootDelay = 1;
	[Export] public int CorpsesOnMap = 12;
	[Export] public Label CorpsesLooted;
	
	
	[ExportCategory("Taxes & Money")] [Export]
	public bool Taxable;

	[Export] public Label CashLabel;
	[Export] public float Cash = 1000;
	[Export] public Node2D SpawnTransform;
	[Export] public PackedScene TaxPopupScene;
	[Export] public float MovingTaxTime = 2;
	[Export] public float CrouchTax = 1f;
	[Export] public float WalkingTax = 2;
	[Export] public float RunningTax = 4;
	[Export] public float JumpTax = 5;
	[Export] public float LootingTax = 10;
	[Export] public Color positiveCash = Colors.Green;
	[Export] public Color negativeCash = Colors.Red;

	private PlayerStates _currentState = PlayerStates.IDLE;
	private float _movingTimer;
	private float _lootingTimer;
	private System.Collections.Generic.Dictionary<PlayerStates, float> _speeds;

	private bool _facingLeft;
	private const float _facingLeftOffset = -20;
	private const float speedMultiplier = 32f;

	public bool OnCorpus; // Able to loot.
	public Corpus ActiveCorpus;

	public bool OnGroundLastFrame;
	private bool _dead;

	private int _corpsesLooted;

	private Array<string> RichLines = new Array<string>()
	{
		"Score! Thanks for the help, pal. Hopefully there's fresher air in the next life.",
		"I thought they discontinued these implants! I think I know how he died...",
		"This is gonna catch a pretty penny, hopefully it doesn't have an active tracking chip..."
	};

	private Array<string> MediumLines = new Array<string>()
	{
		"Every penny counts, as Martin would say.",
		"This should keep me going for a while.",
		"I didn't realize these extensions came in this colour!"
	};

	private Array<string> CheapLines = new Array<string>()
	{
		"Was it really worth this, to download a car, dude?",
		"Another Flame Chip? The Smiling Corp really overproduced these..",
		"Another corpse, another dollar."
	};
	

	[Signal] public delegate void BrokeEventHandler();
	public override void _Ready()
	{
		
		UpdateCash();
		_speeds = new System.Collections.Generic.Dictionary<PlayerStates, float>()
		{
			{PlayerStates.IDLE, 0f},
			{ PlayerStates.WALKING, WalkSpeed },
			{ PlayerStates.RUNNING, RunSpeed },
			{ PlayerStates.SLOW_WALKING, CrouchSped }
		};
		OnGroundLastFrame = IsOnFloor();
		UpdateCorpseCounter();

	}
	public override void _PhysicsProcess(double delta)
	{
		if (_dead) return;
		
		if (Cash < 0) EmitSignal(SignalName.Broke);
		if (_facingLeft)
			TextBox.Position = LeftAnchor.Position;
		else
			TextBox.Position = RightAnchor.Position;


		if (_currentState != PlayerStates.LOOTING)
		{
			if (Input.IsActionPressed("Crouch"))
				_currentState = PlayerStates.SLOW_WALKING;
			else if (Input.IsActionPressed("Sprint"))
				_currentState = PlayerStates.RUNNING;
			else if (Input.IsActionPressed("Left") || Input.IsActionPressed("Right"))
				_currentState = PlayerStates.WALKING;
			else
				_currentState = PlayerStates.IDLE;
		}

		float value = 0;
		if (Input.IsActionJustPressed("Interact"))
		{
			if (ActiveCorpus != null && !ActiveCorpus.Looted)
			{
				value = ActiveCorpus.LootingCorpus();
				_currentState = PlayerStates.LOOTING;
				CharacterAnimations.Play("Going_Down");
				Cash += value;
				ApplyTax(LootingTax, "Looting Dead Person Tax");
				GotMoneyDialog(value);
				UpdateCash();
				_corpsesLooted++;
				UpdateCorpseCounter();
			}
		}
		float taxAmount = 0f;
		string taxReason = "";
		string animation = "";

		switch (_currentState)
		{
			case PlayerStates.SLOW_WALKING:
				taxAmount = CrouchTax;
				taxReason = "Slow Walk Tax";
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
		if (Input.IsActionPressed("Left"))
			_facingLeft = true;
		if  (Input.IsActionPressed("Right"))
			_facingLeft = false;


		
		
		if (_currentState != PlayerStates.LOOTING)
			MovementHandler(Velocity, delta, taxAmount, taxReason);


		if (_currentState == PlayerStates.LOOTING)
			_lootingTimer += (float)delta;
			
		if (_lootingTimer >= LootDelay)
		{
			_currentState = PlayerStates.IDLE;
			_lootingTimer = 0;
		}

		if (_currentState != PlayerStates.LOOTING)
			AnimationHandler(IsOnFloor());
		

		OnGroundLastFrame = IsOnFloor();
	}

	private Vector2 MovementHandler(Vector2 velocity, double delta,float taxAmount, string taxReason)
	{
		Vector2 vel = velocity;
		
		if (!IsOnFloor())
			vel.Y += 900f * (float)delta;
		
		else if (Input.IsActionJustPressed("Jump"))
		{
			vel.Y = -JumpVelocity;
			ApplyTax(JumpTax, "Jump Tax", vel.Y*.75f);
		}
		
		if (_currentState != PlayerStates.IDLE  && vel.X != 0)
		{
			_movingTimer += (float)delta;
		}

		if (_movingTimer > MovingTaxTime && vel.X != 0)
		{
			_movingTimer = 0;
			ApplyTax(taxAmount, taxReason);
		}
		
		
		vel.X = Input.GetAxis("Left", "Right") * _speeds[_currentState];
		
		Velocity = vel;
		MoveAndSlide();
		Position = new Vector2(Position.X, Position.Y);
		return vel;
	}
	

	private void AnimationHandler(bool isOnFloor)
	{
		CharacterAnimations.FlipH = _facingLeft;
		CharacterAnimations.Offset = new Vector2(_facingLeft ? _facingLeftOffset : 0, 0);
		
		if (Input.IsActionJustPressed("Jump") && OnGroundLastFrame)
			CharacterAnimations.Play("Jumping");
		else if (_currentState == PlayerStates.WALKING && isOnFloor)
			CharacterAnimations.Play("Walking");
		else if (_currentState == PlayerStates.SLOW_WALKING && isOnFloor && Velocity.X != 0)
			CharacterAnimations.Play("Slow_Walking");
		else if (_currentState == PlayerStates.RUNNING && isOnFloor && Velocity.X != 0)
			CharacterAnimations.Play("Running");
		else if (_currentState == PlayerStates.IDLE && isOnFloor)
			CharacterAnimations.Play("Idle");
	}

	private void ApplyTax(float taxAmount, string taxReason, float upVelocity = 0f)
	{
		if (!Taxable) return;
		Cash -= taxAmount;
		string taxString = $"{taxReason} -{taxAmount}$";
		SpawnTaxPopup(taxString, upVelocity);
		UpdateCash();
	}

	private void SpawnTaxPopup(string taxString, float upVelocity = 0f)
	{
		TaxPopup popup = TaxPopupScene.Instantiate<TaxPopup>();
		popup.Position = SpawnTransform.Position + new Vector2(0, upVelocity) * .1f;
		AddChild(popup);
		popup.EditLabel(taxString);
	}

	public void UpdateCash()
	{
		if (Cash >= 0)
			CashLabel.Modulate = positiveCash;
		else
			CashLabel.Modulate = negativeCash;
		
		CashLabel.Text = $"Cash: {Cash}$";
	}

	public void SayLine(string text)
	{
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

	private void GotMoneyDialog(float value)
	{
		if (value <= 100)
			SayLine(CheapLines.PickRandom());
		else if (value <= 175)
			SayLine(MediumLines.PickRandom());
		else
			SayLine(RichLines.PickRandom());
	}

	public async void Die()
	{
		_dead = true;
		CharacterAnimations.Play("Die");
		await ToSignal(CharacterAnimations, AnimatedSprite2D.SignalName.AnimationFinished);
		SceneManager.Instance.GameEnd();
	}

	private void UpdateCorpseCounter()
	{
		CorpsesLooted.Text = $"Corpses: {_corpsesLooted}/{CorpsesOnMap}";
	}

}
