using Godot;
using System;

public static class Localization
{
	private static FileManager fm = new FileManager();
	
	private static string SelectedLanguage;
	private static string OnStartDefaulLanguage = "English";
	private static string OnStartDefaulHeader = "Dialogue";
	#if TOOLS
	public static void Init()	// Only Editor Use
	{
		var Languages = GetAllLanguages();
		if(Languages.Count > 0)
		{
			SelectedLanguage = Languages[0];
		}
		else
		{
			AddLanguage(OnStartDefaulLanguage);
			SelectedLanguage = OnStartDefaulLanguage;
		}
		
		var Headers = GetAllHeaders();
		if(Headers.Count < 1)
		{
			AddHeader(OnStartDefaulHeader);
		}
	}
	#endif
	
	public static string GetSelectedLanguage()
	{
		return SelectedLanguage;
	}
	
	public static void SetSelectedLanguage(string Language)
	{
		SelectedLanguage = Language;
	}
	
	public static string GetStringByKey(string _Key)
	{
		if (Engine.IsEditorHint()) //If Editor
		{
			Speech speech = fm.GetSpeechByKeyAndLanguage(_Key, SelectedLanguage);
			if(speech != null)
			{
				return speech.Text;
			}
			
			return "";
			
		}
		else
		{
			
			return fm.GetTextByKeyAndLanguage(_Key, SelectedLanguage);
		}
	}
	
	public static Godot.Collections.Array<string> GetPackagedLanguages()
	{
		Godot.Collections.Array<string> PackagedLanguages = fm.GetPackagedLanguages();
		
		return PackagedLanguages;
	}
	
	// Only Editor Use
	#if TOOLS
	public static void AddValue(string Language, string Header, string Value)
	{
		string _key = fm.GenerateKey();
		string _language = Language;
		string _header = Header;
		string _value = Value;
		
		Speech NewSpeech = new Speech();
		fm.AddSpeech(NewSpeech);
	}
	
	
	
	
	// Get
	public static Godot.Collections.Array<Speech> GetAllSpeeches()
	{
		return fm.GetAllSpeeches();
	}
	
	public static Godot.Collections.Array<string> GetAllHeaders()
	{
		return fm.GetAllHeaders();
	}
	
	public static Godot.Collections.Array<string> GetAllLanguages()
	{
		return fm.GetAllLocalizations();
	}
	
	// Add
	public static void AddSpeech(string Header, string Text)
	{
		Speech NewSpeech = new Speech(fm.GenerateKey(), Header, SelectedLanguage, Text);
		
		fm.AddSpeech(NewSpeech);
	}
	
	public static void AddHeader(string Header)
	{
		fm.AddHeader(Header);
	}
	
	public static void AddLanguage(string Language)
	{
		fm.AddLocalization(Language);
	}
	
	// Edit
	public static void EditSpeech(Speech OldSpeech, Speech NewSpeech)
	{
		OldSpeech.Localization = SelectedLanguage;
		NewSpeech.Localization = SelectedLanguage;
		
		if(OldSpeech.Text != NewSpeech.Text)
		{
			fm.EditSpeechText(OldSpeech, NewSpeech);
		}
		else if(OldSpeech.Header != NewSpeech.Header)
		{
			fm.EditSpeechHeader(OldSpeech, NewSpeech);
		}
	}
	
	public static void EditHeader(string OldHeader, string NewHeader)
	{
		fm.EditHeader(OldHeader, NewHeader);
	}
	
	public static void EditLanguage(string OldLanguage, string NewLanguage)
	{
		fm.EditLanguage(OldLanguage, NewLanguage);
	}
	
	// Delete
	public static void RemoveSpeech(Speech Speech)
	{
		fm.RemoveSpeech(Speech);
	}
	
	public static void RemoveHeader(string Header)
	{
		fm.RemoveHeader(Header);
	}
	
	public static void RemoveLanguage(string Language)
	{
		fm.RemoveLocalization(Language);
	}
	
	// Get First
	public static string GetFirstHeader()
	{
		var Headers = GetAllHeaders();
		
		return Headers[0];
	}
	
	public static string GetFirstLanguage()
	{
		var Languages = GetAllLanguages();
		
		return Languages[0];
	}
	
	// Can Remove
	public static bool CanRemoveHeader()
	{
		var Headers = GetAllHeaders();
		if(Headers.Count < 1)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public static bool CanRemoveLanguage()
	{
		var Languages = GetAllLanguages();
		
		if(Languages.Count < 1)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	public static bool IsHeaderExists(string Header)
	{
		return fm.IsHeaderExist(Header);
	}
	
	
	public static bool IsLanguageExists(string Language)
	{
		return fm.IsLocalizationExist(Language);
	}
	
	public static void Compile(string ExeGameFilePath)
	{
		fm.Compile(ExeGameFilePath);
	}
	
	#endif
}
