#if TOOLS
using Godot;
using System;

[Tool]
public partial class LocalizationSettings : Control
{
	// UI Paths
	private string SpeechElement = "res://addons/PublicLocalization/Scenes/SpeechElement.tscn";
	private string LangElement = "res://addons/PublicLocalization/Scenes/Element.tscn";
	
	private string GameExeFilePath = "";
	private string FileDialogNodeName = "/Control/FileDialog";
	private string SpeechScreenNodeName = "/Control/SpeechScreen";
	private string ValuesContainerNodeName = "/Control/SpeechScreen/TabContainer/Values/VBC_ValuesContainer";
	private string HeadersContainerNodeName = "/Control/SpeechScreen/TabContainer/Headers/VBoxContainer";
	private string LanguagesContainerNodeName = "/Control/SpeechScreen/TabContainer/Languages/VBoxContainer";
	private string DeleteConfirmationDialogPath = "/Control/DeleteConfirmationDialog";
	private string LanguageOptionsBox = "/Control/SpeechScreen/LanguageOptionButton";
	
	// UI Handlers
	private FileDialog FDialog;
	private Window SpeechScreen;
	private VBoxContainer ValuesContainer;
	private VBoxContainer HeadersContainer;
	private VBoxContainer LanguagesContainer;
	private ConfirmationDialog DeleteConfirmationDialog;
	private OptionButton LanguageOptionBox;
	
	// Cached Variables
	private Speech CachedSpeechToDelete;
	private Speech CachedSpeechToEdit;
	
	// Other
	long CurrentTab = 0;
	
	public override void _Ready()
	{
		base._Ready();
		
		FDialog = GetNode<FileDialog>(GetParent().GetPath() + FileDialogNodeName);
		SpeechScreen = GetNode<Window>(GetParent().GetPath() + SpeechScreenNodeName);
		ValuesContainer = GetNode<VBoxContainer>(GetParent().GetPath() + ValuesContainerNodeName);
		HeadersContainer = GetNode<VBoxContainer>(GetParent().GetPath() + HeadersContainerNodeName);
		LanguagesContainer = GetNode<VBoxContainer>(GetParent().GetPath() + LanguagesContainerNodeName);
		DeleteConfirmationDialog = GetNode<ConfirmationDialog>(GetParent().GetPath() + DeleteConfirmationDialogPath);
		LanguageOptionBox = GetNode<OptionButton>(GetParent().GetPath() + LanguageOptionsBox);
		
		
		FDialog.Hide(); 
		SpeechScreen.Hide();
		DeleteConfirmationDialog.Hide();
		
		DrawLanguageOptionBox();
		LanguageOptionBox.ItemSelected += OnLanguageSelected;
		
		DrawValues();
	}
	
	private void DrawLanguageOptionBox()
	{
		LanguageOptionBox.Clear();
		Godot.Collections.Array<string> Languages = Localization.GetAllLanguages();
		
		foreach(string Language in Languages)
		{
			LanguageOptionBox.AddItem(Language);
		}
		
		string CurrentLanguage = Localization.GetSelectedLanguage();
		if(!Localization.IsLanguageExists(CurrentLanguage) 
			&& Languages.Count > 0)
		{
			string ItemText = LanguageOptionBox.GetItemText(0);
			LanguageOptionBox.Select(0);
			Localization.SetSelectedLanguage(ItemText);
			DrawValues();
			return;
		}
		
		for(int ItemIndex = 0; ItemIndex < LanguageOptionBox.ItemCount; ItemIndex++)
		{
			string ItemText = LanguageOptionBox.GetItemText(ItemIndex);
			if(CurrentLanguage.ToLower() == ItemText.ToLower())
			{
				LanguageOptionBox.Select(ItemIndex);
			}
		}
		
	}
	
	private void OnLanguageSelected(long Index)
	{
		// Durty hack bc godot
		int indx = (Index.ToString()).ToInt();
		 
		string CurrentItem = LanguageOptionBox.GetItemText(indx);
		Localization.SetSelectedLanguage(CurrentItem);
		
		DrawValues();
	}
	
	#region UI
	
	private void _on_lang_values_screen_close_requested()
	{
		SpeechScreen.Hide();
	}
	
	private void _on_tab_container_tab_selected(long tab)
	{	
		CurrentTab = tab;
		
		if(CurrentTab == 0)
		{
			DrawValues();
		}
		else if (CurrentTab == 1)
		{
			DrawHeaders();
		}
		else if (CurrentTab == 2)
		{
			DrawLanguages();
		}
		
	}
	
	public void DrawValues()
	{
		string CurrentLanguage = Localization.GetSelectedLanguage();

		// Clear children
		foreach(var child in ValuesContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		var Speeches = Localization.GetAllSpeeches();
		foreach(Speech speech in Speeches)
		{
			if(speech.Localization.ToLower() != CurrentLanguage.ToLower())
			{
				continue;
			}
			
			var scene = GD.Load<PackedScene>(SpeechElement);
			var Value = scene.Instantiate();
			
			ValuesContainer.AddChild(Value);
			
			LineEdit KeyField = (LineEdit)GetNode("SpeechScreen/TabContainer/Values/VBC_ValuesContainer/" + Value.Name + "/KeyField");
			LineEdit HeaderField = (LineEdit)GetNode("SpeechScreen/TabContainer/Values/VBC_ValuesContainer/" + Value.Name + "/HeaderField");
			LineEdit ValueField = (LineEdit)GetNode("SpeechScreen/TabContainer/Values/VBC_ValuesContainer/" + Value.Name + "/ValueField");
			
			SpeechElement CastedNode = (SpeechElement)GetNode("SpeechScreen/TabContainer/Values/VBC_ValuesContainer/" + Value.Name); 
			
			
			string Path = GetParent().GetPath() + ValuesContainerNodeName;
			CastedNode.SetPath(Path);
			CastedNode.SetupValues(speech.Key, speech.Header, speech.Text);
			CastedNode.OnSpeechDeletePress += OnSpeechDeletePressed;
			CastedNode.OnSpeechEditPress += OnSpeechEdit;
		}
	}
	
	private void OnSpeechEdit(Speech OldSpeech, Speech NewSpeech)
	{
		Localization.EditSpeech(OldSpeech, NewSpeech);
	}
	
	
	// Headers
	private void DrawHeaders()
	{
		foreach(var child in HeadersContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		var Headers = Localization.GetAllHeaders();
		
		foreach(string Header in Headers)
		{
			var scene = GD.Load<PackedScene>(LangElement);
			var HeaderInstance = scene.Instantiate();
			HeadersContainer.AddChild(HeaderInstance);
			
			Element CastedInstance = (Element)GetNode("SpeechScreen/TabContainer/Headers/VBoxContainer/" + HeaderInstance.Name);
			CastedInstance.SetupValue(Header);
			CastedInstance.OnValueEdit += OnHeaderEdited;
			CastedInstance.OnValueDeletePress += OnHeaderRemoved;
		}
	}
	
	private void OnHeaderEdited(string OldValue, string NewValue)
	{
		Localization.EditHeader(OldValue, NewValue);
		DrawHeaders();
	}
	
	private void OnHeaderRemoved(string Value)
	{
		Localization.RemoveHeader(Value);
		DrawHeaders();
	}
	
	// Languages
	private void DrawLanguages()
	{
		foreach(var child in LanguagesContainer.GetChildren())
		{
			child.QueueFree();
		}
		
		var Languages = Localization.GetAllLanguages();
		
		foreach(string Language in Languages)
		{
			var scene = GD.Load<PackedScene>(LangElement);
			var LanguageInstance = scene.Instantiate();
			LanguagesContainer.AddChild(LanguageInstance);
			
			Element CastedInstance = (Element)GetNode("SpeechScreen/TabContainer/Languages/VBoxContainer/" + LanguageInstance.Name);
			
			CastedInstance.SetupValue(Language);
			CastedInstance.OnValueEdit += OnLanguageEdited;
			CastedInstance.OnValueDeletePress += OnLanguageRemoved;
		}
	}
	
	private void OnLanguageEdited(string OldValue, string NewValue)
	{
		Localization.EditLanguage(OldValue, NewValue);
		
		string CurrentLanguage = Localization.GetSelectedLanguage();
		if(CurrentLanguage.ToLower() == OldValue.ToLower())
		{
			Localization.SetSelectedLanguage(NewValue);
		}
		
		DrawLanguages();
		DrawLanguageOptionBox();
	}
	
	private void OnLanguageRemoved(string Value)
	{
		Localization.RemoveLanguage(Value);
		DrawLanguages();
		DrawLanguageOptionBox();
	}
	
	
	private void _on_btn_add_value_pressed()
	{
		if(CurrentTab == 0) // Speech
		{
			Localization.AddSpeech(Localization.GetFirstHeader(), "...");
			DrawValues();
		}
		else if (CurrentTab == 1) // Headers
		{
			Localization.AddHeader("New Header");
			DrawHeaders();
		}
		else if (CurrentTab == 2) // Languages
		{
			Localization.AddLanguage("New Language");
			DrawLanguages();
			DrawLanguageOptionBox();
		}
	}
	
	
	
	
	
	#endregion UI
	
	#region DeletedConfirmation
	
	private void OnSpeechDeletePressed(Speech speech)
	{
		// TODO: Remove, this is debug
		// UPD: Is this realy better?
		Localization.RemoveSpeech(speech);
		DrawValues();
		// end TODO: Remove
		
		/*
		// TODO: Add
		DeleteConfirmationDialog.Show();
		CachedSpeechToDelete = speech;
		*/
	}
	
	private void _on_delete_confirmation_dialog_confirmed()
	{
		if(CachedSpeechToDelete == null)
		{
			return;
		}
		Localization.RemoveSpeech(CachedSpeechToDelete);
		DrawValues();
		DeleteConfirmationDialog.Hide();
		CachedSpeechToDelete = null;
	}
	
	private void _on_delete_confirmation_dialog_canceled()
	{
		CachedSpeechToDelete = null;
		DeleteConfirmationDialog.Hide();
	}
	
	private void _on_delete_confirmation_dialog_close_requested()
	{
		CachedSpeechToDelete = null;
		DeleteConfirmationDialog.Hide();
	}
	
	#endregion DeleteConfirmation
	
	
	
	#region MainUI
	
	private void _on_load_btn_pressed()
	{
		FDialog.Show();
	}

	private void _on_file_dialog_file_selected(string path)
	{
		GameExeFilePath = path;
		Localization.Compile(path);
	}
	
	private void _on_btn_open_settings_pressed()
	{
		SpeechScreen.Show();
		DrawValues();
	}
	
	#endregion MainUI
}
#endif












