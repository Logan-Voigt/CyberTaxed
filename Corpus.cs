using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum CorpusBodies
{
	ONE = 1,
	TWO = 2,
	THREE = 3,
	SIX = 6,
}

public struct Item
{
	public Texture2D Image;
	public float value;
	public Color glowColor;

	public Item(Texture2D Image, float value, Color glowColor)
	{
		this.Image = Image;
		this.value = value;
		this.glowColor = glowColor;
	}
}

public partial class Corpus : Node2D
{
	[Export] private CorpusBodies ActiveBodyIndex = CorpusBodies.ONE;
	[Export] private AnimatedSprite2D CurrentCorpus;
	[Export] private Area2D CorpusArea;
	[Export] private PackedScene ItemPopupScene;
	[Export] private Node2D SpawnPoint;
	[Export] private Godot.Collections.Dictionary<string, Texture2D> _itemTextures = new();
	[Export] private Godot.Collections.Dictionary<string, float> _value;
	[Export] private Godot.Collections.Dictionary<string, Color> _glowColors = new();
	private Dictionary<string, Item> _items = new Dictionary<string, Item>();

	public bool Looted;
	public override void _Ready()
	{
		CorpusArea.BodyEntered += PlayerEnteredArea;
		CorpusArea.BodyExited += PlayerExitedArea;
		
		string bodyIndex = ((int)ActiveBodyIndex).ToString();
		CurrentCorpus.Play(bodyIndex);
		
		foreach (string key in _itemTextures.Keys)
		{
			Color glowColor = _glowColors[key];
			Texture2D texture = _itemTextures[key];
			float value =  _value[key];
			
			Item tempItem = new Item(texture, value, glowColor);
			
			_items.Add(key, tempItem);
		}
	}

	private void PlayerEnteredArea(Node2D body)
	{
		if (body is CharacterController controller)
		{
			controller.OnCorpus = true;
			controller.ActiveCorpus = this;
		}
	}

	public float LootingCorpus()
	{
		if (Looted) return 0;
		
		int randomNumberOfItemSPawns = GD.RandRange(1,3);

		float dollarTotal = 0;
		
		for(int i = 0; i < randomNumberOfItemSPawns; i++)
			dollarTotal += SpawnItem();

		Looted = true;
			
		return dollarTotal;
	}

	public float SpawnItem()
	{
		Item reward = _items.ElementAt(GD.RandRange(0, _items.Count - 1)).Value;
		
		ItemPopup popup = ItemPopupScene.Instantiate<ItemPopup>();
		popup.Position = SpawnPoint.Position;
		AddChild(popup);
		popup.UpdateItem(reward.value.ToString(), reward.Image, reward.glowColor);
		
		return reward.value;
	}
	

	private void PlayerExitedArea(Node2D body)
	{
		if (body is CharacterController controller)
		{
			controller.OnCorpus = false;
			controller.ActiveCorpus = null;
		}
	}
}
				
