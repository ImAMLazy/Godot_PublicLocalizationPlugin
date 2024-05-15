#if TOOLS
using Godot;
using System;

[Tool]
public partial class SpeechElement : HBoxContainer
{
	private string OverridedPath = "";
	private string LangValueNodePath = "";
	private string KeyFieldName = "KeyField";
	private string HeaderFieldName = "HeaderField";
	private string ValueFieldName = "ValueField";
	
	private LineEdit KeyField;
	private LineEdit HeaderField;
	private LineEdit ValueField;
	
	private Speech CurrentSpeech;
	
	[Signal] public delegate void OnSpeechDeletePressEventHandler(Speech speech);
	[Signal] public delegate void OnSpeechEditPressEventHandler(Speech OldSpeech, Speech NewSpeech);
	
	private bool IsHeaderTextChanged = false;
	private bool IsValueTextChanged = false;
	
	public SpeechElement()
	{
		
	}
	
	public void SetupValues(string Key, string Header, string Text)
	{
		CurrentSpeech = new Speech(Key, Header, "Lang Not Needed", Text);
		
		KeyField.Text = CurrentSpeech.Key;
		HeaderField.Text = CurrentSpeech.Header;
		ValueField.Text = CurrentSpeech.Text;
	}
	
	public void SetPath(string path)
	{
		LangValueNodePath = path;
		
		KeyField = GetNode<LineEdit>(LangValueNodePath +"/" +Name + "/" + KeyFieldName);
		HeaderField = GetNode<LineEdit>(LangValueNodePath +"/" +Name + "/" + HeaderFieldName);
		ValueField = GetNode<LineEdit>(LangValueNodePath +"/" +Name + "/" + ValueFieldName);
	}
	
	private void _on_btn_remove_pressed()
	{
		if(Localization.CanRemoveHeader())
		{
			EmitSignal(SignalName.OnSpeechDeletePress, CurrentSpeech);
		}
	}
	
	private void _on_header_field_text_changed(string new_text)
	{
		IsHeaderTextChanged = true;
	}
	
	private void _on_header_field_text_submitted(string new_text)
	{
	 	if(IsHeaderTextChanged)
		{
			SaveHeader(new_text);
		}
		
		IsHeaderTextChanged = false;
	}
	
	private void _on_header_field_focus_exited()
	{
		if(IsHeaderTextChanged)
		{
			SaveHeader();
		}
		
		IsHeaderTextChanged = false;
	}
	
	private void SaveHeader(string str = null) // Save Header field in Speech
	{
		Speech OldSpeech = new Speech(
			CurrentSpeech.Key,
			CurrentSpeech.Header,
			CurrentSpeech.Localization,
			CurrentSpeech.Text); 
		
		bool IsHeaderChanged = false;
		
		if(str == null)
		{
			
			
			string NewHeader = HeaderField.Text;
			if(Localization.IsHeaderExists(NewHeader) && NewHeader != "")
			{
				CurrentSpeech.Header = NewHeader;
				IsHeaderChanged = true;
			}
		}
		else
		{
			string NewHeader = str;
			if(Localization.IsHeaderExists(NewHeader) && NewHeader != "")
			{
				CurrentSpeech.Header = NewHeader;
				IsHeaderChanged = true;
			}
		}
		
		if(IsHeaderChanged)
		{
			EmitSignal(SignalName.OnSpeechEditPress, OldSpeech, CurrentSpeech);
		}
		else
		{
			HeaderField.Text = OldSpeech.Header;
		}
	}
	
	private void _on_value_field_text_changed(string new_text)
	{
		IsValueTextChanged = true;
	}

	private void _on_value_field_text_submitted(string new_text)
	{
		if(IsValueTextChanged)
		{
			SaveValue(new_text);
		}
			
		IsValueTextChanged = false;
	}

	private void _on_value_field_focus_exited()
	{
		if(IsValueTextChanged)
		{
			SaveValue();
		}
			
		IsValueTextChanged = false;
	}

	private void SaveValue(string str = null)
	{
		Speech OldSpeech = new Speech(
			CurrentSpeech.Key,
			CurrentSpeech.Header,
			CurrentSpeech.Localization,
			CurrentSpeech.Text); 
		
		bool IsValueChanged = false;
		
		if(str == null)
		{
			string NewValue = ValueField.Text;
			if(NewValue != "")
			{
				CurrentSpeech.Text = NewValue;
				IsValueChanged = true;
			}
		}
		else
		{
			string NewValue = str;
			if(NewValue != "")
			{
				CurrentSpeech.Text = NewValue;
				IsValueChanged = true;
			}
		}
		
		if(IsValueChanged)
		{
			EmitSignal(SignalName.OnSpeechEditPress, OldSpeech, CurrentSpeech);
		}
		else
		{
			ValueField.Text = OldSpeech.Text;
		}
		
	}
}
#endif






