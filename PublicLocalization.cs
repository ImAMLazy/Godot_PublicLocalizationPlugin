#if TOOLS
using Godot;
using System;

[Tool]
public partial class PublicLocalization : EditorPlugin
{
	
	string LocalizationSettingsScreenPath = "res://addons/PublicLocalization/Scenes/LocalizationSettings.tscn";
	
	Control _dock;
	public override void _EnterTree()
	{
		Localization.Init();
		
		_dock = GD.Load<PackedScene>(LocalizationSettingsScreenPath).Instantiate<Control>();
		AddControlToDock(DockSlot.LeftUl, _dock);
	}
	
	public override bool _Build()
	{
		return true;
	}
	
	public override void _ExitTree()
	{
		RemoveControlFromDocks(_dock);
		_dock.Free();
	}
	
	
	
}
#endif
