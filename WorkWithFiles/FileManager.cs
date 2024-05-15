using Godot;
using System;

public partial class Speech : GodotObject
{
	public string Key;
	public string Header;
	public string Localization;
	public string Text;
	
	public Speech() {}
	
	public Speech(string key, string header, string localization, string text)
	{
		Key = key;
		Header = header;
		Localization = localization;
		Text = text;
	}
}

public partial class ProviderValue : GodotObject
{
	public string Key;
	public string Header;
	public int NumberOfString;
	
	public ProviderValue() {}
	
	public ProviderValue(string key, string header, int numOfString)
	{
		Key = key;
		Header = header;
		NumberOfString = numOfString;
	}
}

public class FileManager
{
	private static string HeadersFilePath = "res://addons/PublicLocalization/Config/Headers.cfg";
	private static string LocalizationsFilePath = "res://addons/PublicLocalization/Config/Languages.cfg";
	private static string SpeechFilePath = "res://addons/PublicLocalization/Config/Speeches.cfg";
	
	private static string PluginGeneralFolderName = "Localization";
	
	public string GenerateKey()
	{
		while(true)
		{			
			var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			string key = "";
			var n_char = characters.Length-1;
			for(int i = 0; i < 10; i++)
			{
				int rndValue = (int)(GD.Randi() % n_char);
				key += characters[rndValue];
			}
			
			if(!IsKeyExisting(key))
			{
				return key;
			}
		}
	}
	
	#region PreCompile
	
	#region Headers
	
	public Godot.Collections.Array<string> GetAllHeaders()
	{
		return GetAllCsvElements(HeadersFilePath);
	}
	
	public void AddHeader(string HeaderToAdd)
	{
		AddCsvItem(HeaderToAdd, HeadersFilePath);
	}
	
	public void RemoveHeader(string HeaderToRemove)
	{
		RemoveCsvItem(HeaderToRemove, HeadersFilePath);
	}
	
	public void EditHeader(string OldHeader, string NewHeader)
	{
		RemoveCsvItem(OldHeader, HeadersFilePath);
		AddCsvItem(NewHeader, HeadersFilePath);
		EditAllSpeechesHeaders(OldHeader, NewHeader);
	}
	
	public bool IsHeaderExist(string Header)
	{
		return IsHeaderOrLocalizationExist(Header, HeadersFilePath);
	}
	
	
	#endregion Headers
	
	#region Localizations
	
	public Godot.Collections.Array<string> GetAllLocalizations()
	{
		return GetAllCsvElements(LocalizationsFilePath);
	}
	
	public void AddLocalization(string LocalizationToAdd)
	{
		AddCsvItem(LocalizationToAdd, LocalizationsFilePath);
		
		// Create new speeches for new localization

		var Speeches = GetAllSpeeches();
		if(Speeches.Count < 1)
		{
			return;
		}
		string UniteLanguage = Speeches[0].Localization;
		
		Godot.Collections.Array<Speech> NewSpeechList = new Godot.Collections.Array<Speech>();
		
		foreach(Speech speech in Speeches)
		{
			NewSpeechList.Add(speech);
			if(speech.Localization.ToLower() != UniteLanguage.ToLower())
			{
				continue;
			}
			Speech NewSpeech = new Speech(speech.Key, speech.Header, LocalizationToAdd, "...");
			NewSpeechList.Add(NewSpeech);
		}
		
		WriteAllSpeeches(NewSpeechList);
	}
	
	public void RemoveLocalization(string LocalizationToRemove)
	{
		RemoveCsvItem(LocalizationToRemove, LocalizationsFilePath);
		// Remove speeches for deleted localization
		
		var Speeches = GetAllSpeeches();
		if(Speeches.Count < 1)
		{
			return;
		}
		
		Godot.Collections.Array<Speech> NewSpeechList = new Godot.Collections.Array<Speech>();
		
		foreach(Speech speech in Speeches)
		{
			
			if(speech.Localization.ToLower() != LocalizationToRemove.ToLower())
			{
				NewSpeechList.Add(speech);
				continue;
			}
		}
		
		WriteAllSpeeches(NewSpeechList);
	}	
	
	public void EditLanguage(string OldLanguage, string NewLanguage)
	{
		RemoveCsvItem(OldLanguage, LocalizationsFilePath);
		AddCsvItem(NewLanguage, LocalizationsFilePath);
		EditAllSpeechesLanguages(OldLanguage, NewLanguage);
	}
	
	public bool IsLocalizationExist(string Localization)
	{
		return IsHeaderOrLocalizationExist(Localization, LocalizationsFilePath);
	}
	
	#endregion Localizations
	
	#region Speeches
	
	public Godot.Collections.Array<Speech> GetAllSpeeches()
	{
		var FileToRead = FileAccess.Open(SpeechFilePath, FileAccess.ModeFlags.Read);
		var Content = FileToRead.GetAsText();
		FileToRead.Close();
		
		Godot.Collections.Array<Speech> Speeches = new Godot.Collections.Array<Speech>();
		
		if(Content == "")
		{
			return Speeches;
		}
		
		var strings = Content.Split("\r\n");
		
		foreach(string str in strings)
		{
			var SpeechParts = str.Split(";");
			
			if(SpeechParts.Length < 3)
			{
				continue;
			}
			
			Speech NewSpeech = new Speech
			(
				SpeechParts[0],
				SpeechParts[1],
				SpeechParts[2],
				SpeechParts[3]
			);
			Speeches.Add(NewSpeech);
			
		}
		
		return Speeches;
	}
	
	public void AddSpeech(Speech NewSpeech) // add NEW speech
	{
		
		Godot.Collections.Array<Speech> Speeches = GetAllSpeeches();
		
		// Check if key exist
		
		foreach(Speech speech in Speeches)
		{
			if(speech.Key == NewSpeech.Key
			&& speech.Localization == NewSpeech.Localization)
			{
				return; // Speech with same key exist in this localization
			}
		}
		
		// End Check if key exist
		
		// Check if header exist // TODO: Move in UI
		
		if(!IsHeaderExist(NewSpeech.Header))
		{
			return; // Header does not exist
		}
		
		// End Check if header exist // TODO: Move in UI
		
		/*
		// Check if language exist // TODO: Move in UI
		
		if(!IsLocalizationExist(NewSpeech.Localization))
		{
			return; // language does not exist
		}
		
		// End Check if language exist // TODO: Move in UI
		*/
		
		// Check for localization, if more than 1 - create new empty speech for it with same key
		
		var Localizations = GetAllLocalizations();
		
		foreach(string Localization in Localizations)
		{			
			Speech NewLocalizationSpeech = new Speech(NewSpeech.Key, NewSpeech.Header, Localization, NewSpeech.Text);
			Speeches.Add(NewLocalizationSpeech);
			
		}
		
		// End Check for localization, if more than 1 - create new empty speech for it with same key
		
		WriteAllSpeeches(Speeches);
	}
	
	public void EditSpeechText(Speech OldSpeech, Speech NewSpeech)
	{
		var Speeches = GetAllSpeeches();
		for(int i = 0; i < Speeches.Count; i++)
		{
			if(IsSpeechesIquals(Speeches[i], OldSpeech))
			{
				Speeches[i].Text = NewSpeech.Text;
			}
		}
		WriteAllSpeeches(Speeches);
	}
	
	public void EditSpeechHeader(Speech OldSpeech, Speech NewSpeech)
	{
		var Speeches = GetAllSpeeches();
		
		for(int i = 0; i < Speeches.Count; i++)
		{
			if(IsSpeechesIquals(Speeches[i], OldSpeech))
			{
				Speeches[i].Header = NewSpeech.Header;
			}
		}
		
		WriteAllSpeeches(Speeches);
	}
	
	public void EditAllSpeechesHeaders(string OldHeader, string NewHeader)
	{
		var Speeches = GetAllSpeeches();
		
		for(int i = 0; i < Speeches.Count; i++)
		{
			if(Speeches[i].Header == OldHeader)
			{
				Speeches[i].Header = NewHeader;
			}
		}
		
		WriteAllSpeeches(Speeches);
	}
	
	public void EditAllSpeechesLanguages(string OldLanguage, string NewLanguage)
	{
		var Speeches = GetAllSpeeches();
		
		for(int i = 0; i < Speeches.Count; i++)
		{
			if(Speeches[i].Localization.ToLower() == OldLanguage.ToLower())
			{
				Speeches[i].Localization = NewLanguage;
			}
		}
		
		WriteAllSpeeches(Speeches);
	}
	
	public bool IsSpeechesIquals(Speech A, Speech B)
	{
		if(A.Key.ToLower() == B.Key.ToLower() &&
			A.Header.ToLower() == B.Header.ToLower() &&
			A.Localization.ToLower() == B.Localization.ToLower() &&
			A.Text.ToLower() == B.Text.ToLower())
		{
			return true;
		}
		
		return false;
	}
	
	public void RemoveSpeech(Speech SpeechToRemove)
	{
		Godot.Collections.Array<Speech> Speeches = GetAllSpeeches();
		
		for(int i = Speeches.Count-1; i > -1; i--)
		{
			if(Speeches[i].Key.ToLower() == SpeechToRemove.Key.ToLower())
			{
				Speeches.RemoveAt(i);
			}
		}
		
		WriteAllSpeeches(Speeches);
	}
	
	public bool IsSpeechExist(Speech SpeechToFind)
	{
		var Speeches = GetAllSpeeches();
		foreach(Speech speech in Speeches)
		{
			if(speech == SpeechToFind)
			{
				return true;
			}
		}
		
		return false;
	}
	
	private void WriteAllSpeeches(Godot.Collections.Array<Speech> Speeches)
	{
		ClearFile(SpeechFilePath);
		
		var Content = "";
		
		foreach(Speech speech in Speeches)
		{
			var FileToRead2 = FileAccess.Open(SpeechFilePath, FileAccess.ModeFlags.Read);
			Content = FileToRead2.GetAsText();
			FileToRead2.Close();
			
			string StringToAdd = $"{speech.Key};{speech.Header};{speech.Localization};{speech.Text}\r\n";
			var FileToWrite = FileAccess.Open(SpeechFilePath, FileAccess.ModeFlags.ReadWrite);
			FileToWrite.StoreString(Content+StringToAdd);
			FileToWrite.Close();
		}
	}
	
	public bool IsKeyExisting(string Key)
	{
		Godot.Collections.Array<Speech> Speeches = GetAllSpeeches();
		
		// Check if key exist
		foreach(Speech speech in Speeches)
		{
			if(speech.Key == Key)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public Speech GetSpeechByKeyAndLanguage(string Key, string Language)
	{
		var Speeches = GetAllSpeeches();
		
		foreach(Speech speech in Speeches)
		{
			if(speech.Key.ToLower() == Key.ToLower()
			&& speech.Localization.ToLower() == Language.ToLower())
			{
				return speech;
			}
		}
		
		GD.Print("Error: Speech doesnt found by key and language");
		
		return null;
	}
	
	#endregion Speeches
	
	#endregion PreCompile
	
	#region Compile
	
	public void Compile(string GameExeFilePath)
	{
		if(!GameExeFilePath.IsAbsolutePath())
		{
			return; // Not Absolute Path
		}
		
		
		string CommonPath = GameExeFilePath.GetBaseDir();
		
		string PluginFolderPath = CommonPath + "\\" + PluginGeneralFolderName;
		
		DirAccess.MakeDirAbsolute(PluginFolderPath);
		
		//Create all localization from data
		
		var Languages = GetAllLocalizations();
		
		// Create all languange folders
		
		foreach(string Language in Languages)
		{
			Godot.Collections.Array<ProviderValue> ProviderValues = new Godot.Collections.Array<ProviderValue>();
			
			string LanguagePath = PluginFolderPath + "\\" + Language;
			DirAccess.MakeDirAbsolute(LanguagePath);
			
			// Create all headers files
			
			var Headers = GetAllUsedHeaders();
			
			foreach(string Header in Headers)
			{
				
				var Keys = GetKeysOfHeader(Header);
				
				string StringToWrite = "";
				
				for(int i = 0; i < Keys.Count; i++)
				{
					StringToWrite += GetSpeechText(Keys[i], Header, Language) + ";\r\n";
					ProviderValue PlaceInFile = new ProviderValue(Keys[i], Header, i);
					ProviderValues.Add(PlaceInFile);
				}
				
				// Write texts to file (and remember position)
				
				string FilePath = LanguagePath + "\\" + Header + ".txt";
				
				var HeaderFileToWrite = FileAccess.Open(FilePath, FileAccess.ModeFlags.Write);
				HeaderFileToWrite.StoreString(StringToWrite);
				HeaderFileToWrite.Close();
			}
			// Create provider file
			string StringToWriteInProviderFile = "";
			foreach(ProviderValue providerValue in ProviderValues)
			{
				StringToWriteInProviderFile += $"{providerValue.Key};{providerValue.Header};{providerValue.NumberOfString};\r\n";
			}
			
			string ProviderFilePath = PluginFolderPath + "\\" + "Provider" + ".txt";
			var ProviderFileToWrite = FileAccess.Open(ProviderFilePath, FileAccess.ModeFlags.Write);
			ProviderFileToWrite.StoreString(StringToWriteInProviderFile);
			ProviderFileToWrite.Close();
		}
		
		


		//End create all localization from data
		
	}
	
	
	private Godot.Collections.Array<string> GetKeysOfHeader(string Header)
	{
		Godot.Collections.Array<string> Keys = new Godot.Collections.Array<string>();
		
		var Speeches = GetAllSpeeches();
		
		foreach(Speech speech in Speeches)
		{
			if(!Keys.Contains(speech.Key) &&
				speech.Header == Header)
			{
				Keys.Add(speech.Key);
			}
		}
		return Keys;
	}
	
	private string GetSpeechText(string Key, string Header, string Localization)
	{
		var Speeches = GetAllSpeeches();
		
		foreach(Speech speech in Speeches)
		{
			if(speech.Key.ToLower() == Key.ToLower() &&
				speech.Header.ToLower() == Header.ToLower() &&
				speech.Localization.ToLower() == Localization.ToLower()
			)
			{
				return speech.Text;
			}
		}
		
		GD.Print("ERROR: GetSpeechText doesnt found speech by key");
		
		return "";
	}
	
	private Godot.Collections.Array<string> GetAllUsedHeaders()
	{
		Godot.Collections.Array<string> Headers = new Godot.Collections.Array<string>();
		
		var Speeches = GetAllSpeeches();
		
		foreach(Speech speech in Speeches)
		{
			if(!ArrayContainsItem(Headers, speech.Header))
			{
				Headers.Add(speech.Header);
			}
		}
		
		return Headers;
	}
	
	private bool ArrayContainsItem(Godot.Collections.Array<string> Array, string obj)
	{
		foreach(var item in Array)
		{
			if(item == obj)
			{
				return true;
			}
		}
		return false;
	}
	
	#endregion Compile
	
	#region PostCompile
	
	public string GetTextByKeyAndLanguage(string Key, string Language)
	{
		#if TOOLS
		if(!IsKeyExisting(Key))
		{
			GD.Print($"ERROR: Key {Key} doesnt exists");
			return "";
		}
		#endif
		
		string ExeFilePath = GetAbsouluteExeFilePath();
		string PluginFolderPath = ExeFilePath + "\\" + PluginGeneralFolderName;
		string LanguagePath = PluginFolderPath + "\\" + Language;
		
		if(!DirAccess.DirExistsAbsolute(LanguagePath))
		{
			string ErrorStringToWrite = $"Error: {Language} - Language doesnt exist";
			WriteErrorLog(ErrorStringToWrite);
		}
		
		// read provider
		var ProviderValues = GetProviderValues(Language);
		ProviderValue PointerToText = FindProviderValueByKey(ProviderValues, Key);
		
		// read file
		string FileWithTextPath = LanguagePath + "\\" + PointerToText.Header + ".txt";
		return GetStringFromFile(FileWithTextPath, PointerToText.NumberOfString);
	}
	
	private string GetStringFromFile(string FilePath, int NumOfString)
	{
		var ProviderToRead = FileAccess.Open(FilePath, FileAccess.ModeFlags.Read);
		var Content = ProviderToRead.GetAsText();
		ProviderToRead.Close();
		
		var Strings = Content.Split("\r\n");
	
		if(Strings.Length < NumOfString)
		{
			WriteErrorLog("Error: String Count Less than number of string");
		}
		
		
		for(int i = 0; i < Strings.Length; i++)
		{
			if(i == NumOfString)
			{
				return Strings[i].Substr(0, Strings[i].Length-1);
			}
		}
	
		return "";
		
	}
	
	private ProviderValue FindProviderValueByKey(Godot.Collections.Array<ProviderValue> ProviderValues, string Key)
	{
		foreach(ProviderValue providerValue in ProviderValues)
		{
			if(providerValue.Key.ToLower() == Key.ToLower())
			{
				return providerValue;
			}
		}
		
		WriteErrorLog("ProviderValue doesnt founded by key");
		
		return new ProviderValue();
	}
	
	private Godot.Collections.Array<ProviderValue> GetProviderValues(string Language)
	{
		Godot.Collections.Array<ProviderValue> ProviderValues = new Godot.Collections.Array<ProviderValue>();
		
		string ExeFilePath = GetAbsouluteExeFilePath();
		string PluginFolderPath = ExeFilePath + "\\" + PluginGeneralFolderName;
		string ProviderFilePath = PluginFolderPath + "\\" + "Provider" + ".txt";
		
		var ProviderToRead = FileAccess.Open(ProviderFilePath, FileAccess.ModeFlags.Read);
		var Content = ProviderToRead.GetAsText();
		ProviderToRead.Close();
		
		var Strings = Content.Split("\r\n");
		
		foreach(string Str in Strings)
		{
			if(Str == "")
			{
				continue;
			}
			
			var ProviderValueParts = Str.Split(";");
			
			ProviderValue ProviderValueToAdd = new ProviderValue(ProviderValueParts[0],
																ProviderValueParts[1],
																StringExtensions.ToInt(ProviderValueParts[2]));
			
			
			ProviderValues.Add(ProviderValueToAdd);
		}
		return ProviderValues;
		
	}
	
	private void WriteErrorLog(string ErrorLog)
	{
		string ExeFilePath = GetAbsouluteExeFilePath();
		string TimeString1 = Time.GetTimeStringFromSystem();
		//TimeString.Replace(':', '-'); // BE CURSED ******* GODOT!!!
	
		string TimeString = "";
		for(int i = 0; i < TimeString1.Length; i++)
		{
			if(TimeString1[i] == ':')
			{
				TimeString += '-';
			}
			else
			{
				TimeString += TimeString1[i];
			}
		}
		
		
		string ErrorFilePath = ExeFilePath + "\\" + "Error_log_" + TimeString + ".txt";		
		var ErrorFileToWrite = FileAccess.Open(ErrorFilePath, FileAccess.ModeFlags.Write);
		ErrorFileToWrite.StoreString(ErrorLog);
		ErrorFileToWrite.Close();
		
		
		
	}
	
	public Godot.Collections.Array<string> GetPackagedLanguages()
	{
		Godot.Collections.Array<string> PackagedLanguages = new Godot.Collections.Array<string>();
		
		string GamePath = GetAbsouluteExeFilePath();
		string LocalizationPath = GamePath + "\\" + "Localization";
		
		var Languages = DirAccess.GetDirectoriesAt(LocalizationPath);
		
		foreach(string Language in Languages)
		{
			PackagedLanguages.Add(Language);
		}
		
		return PackagedLanguages;
	}
	
	#endregion PostCompile
	
	#region Private

	private void ClearFile(string _Path)
	{
		using var file = FileAccess.Open(_Path, FileAccess.ModeFlags.Write);
		file.Close();
	}
	
	public Godot.Collections.Array<string> GetAllCsvElements(string Path)
	{
		var Elements = new Godot.Collections.Array<string>{};
		using var file = FileAccess.Open(Path, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		var strings = Content.Split(';');
		
		foreach(var str in strings)
		{
			string castedString = (string)str;
			if(castedString != ""
			&& castedString != "\r"
			&& castedString != "\n"
			&& castedString != "\r\n"
			&& castedString != "\n\r")
			{
				Elements.Add(castedString);
			}
		}
		
		return Elements;
	}
	
	public void AddCsvItem(string ItemToAdd, string Path)
	{
		var FileToRead = FileAccess.Open(Path, FileAccess.ModeFlags.Read);
		var Content = FileToRead.GetAsText();
		FileToRead.Close();
		
		if(Content == "")
		{
			var FileToWrite = FileAccess.Open(Path, FileAccess.ModeFlags.ReadWrite);
			// Add Item
			FileToWrite.StoreString(ItemToAdd+';');
			FileToWrite.Close();
			return;
		}
		
		if(IsHeaderOrLocalizationExist(ItemToAdd, Path))
		{
			return;
		}
		
		var FileToWrite2 = FileAccess.Open(Path, FileAccess.ModeFlags.ReadWrite);
		// Add Item
		FileToWrite2.StoreString(Content + ItemToAdd+';');
		FileToWrite2.Close();
	}
	
	public void RemoveCsvItem(string ItemToRemove, string Path)
	{
		using var file = FileAccess.Open(Path, FileAccess.ModeFlags.ReadWrite);
		string Content = file.GetAsText();
		file.Close();
		
		var strings = Content.Split(';');
		ClearFile(Path);
		
		foreach(var str in strings)
		{
			string castedString = (string)str;
			if(castedString.ToLower() != ItemToRemove.ToLower() &&
			   castedString != "")
			{
				AddCsvItem(castedString, Path);
			}
		}
		
	}
	
	private bool IsHeaderOrLocalizationExist(string HeaderOrLocalization, string FilePath)
	{
		
		using var file = FileAccess.Open(FilePath, FileAccess.ModeFlags.Read);
		var Content = file.GetAsText();
		var strings = file.GetCsvLine(";");
		file.Close();

		if(Content == "")
		{
			return false;
		}
		
		foreach(var str in strings)
		{
			
			
			string castedString = (string)str;
			
			string A = castedString.ToLower();
			string B = HeaderOrLocalization.ToLower();
			
			if(A == B)
			{
				return true;
			}
		}
		
		return false;
	}
	
	public string GetAbsouluteExeFilePath()
	{
		
#if TOOLS
		return Testing.GetDataFromTestConfig().GetBaseDir();
		
#else
		string ExeFilePath = OS.GetExecutablePath().GetBaseDir();
		return ExeFilePath;
#endif
		
	}
	
	#endregion Private
}
