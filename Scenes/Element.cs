#if TOOLS
using Godot;
using System;

[Tool]
public partial class Element : HBoxContainer
{
	private string ValueFieldPath = "/LineEdit";
	
	private LineEdit ValueField;
	
	private string BaseValue;
	
	private bool IsValueChanged = false;
	
	[Signal] public delegate void OnValueDeletePressEventHandler(string Value);
	[Signal] public delegate void OnValueEditEventHandler(string OldValue, string NewValue);
	
	public override void _Ready()
	{
		ValueField = GetNode<LineEdit>(GetParent().GetPath() + "/" + Name + "/" + ValueFieldPath);
	}
	
	public void SetupValue(string v)
	{
		BaseValue = v;
		ValueField.Text = v;
	}
	
	private void SetValue()
	{
		ValueField.Text = BaseValue;
	}
	
	private void _on_line_edit_text_changed(string new_text)
	{
		IsValueChanged = true;
	}

	private void _on_line_edit_text_submitted(string new_text)
	{
		if(IsValueChanged)
		{
			SaveValue(new_text);
		}
			
		IsValueChanged = false;
	}

	private void _on_line_edit_focus_exited()
	{
		if(IsValueChanged)
		{
			SaveValue();
		}
			
		IsValueChanged = false;
	}
	
	private void SaveValue(string NewValue = null)
	{
		if(NewValue == null)
		{
			NewValue = ValueField.Text;
		}
		
		EmitSignal(SignalName.OnValueEdit, BaseValue, NewValue);
	}
	
	private void _on_button_pressed()
	{
		EmitSignal(SignalName.OnValueDeletePress, BaseValue);
	}
}
#endif
