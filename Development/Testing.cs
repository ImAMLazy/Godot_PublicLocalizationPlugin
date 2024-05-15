using Godot;
using System;

public static class Testing
{
	
	private static string HeadersFilePath = "res://addons/PublicLocalization/Config/Headers.cfg";
	private static string LocalizationsFilePath = "res://addons/PublicLocalization/Config/Languages.cfg";
	private static string SpeechFilePath = "res://addons/PublicLocalization/Config/Speeches.cfg";
	
	private static string TestConfigFilePath = "res://addons/PublicLocalization/Config/GameExePath.cfg";
	
	private static bool IsLogsEnabledForHeaders = false;
	private static bool IsLogsEnabledForLocalizations = false;
	private static bool IsLogsEnabledForSpeeches = true;
	
	private static FileManager fm = new FileManager();

	public static string GetDataFromTestConfig()
	{
		using var file = FileAccess.Open(TestConfigFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		return Content;
	}
	
	#region Headers
	
	public static void Test_Headers()
	{
		ClearHeadersFile();
		
		bool Test1 = AddHeaderTest();
		if(!Test1)
		{
			PrintHeaderLogs("Test 1 " + (Test1 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Headers Tests Failed !!!");
			return;
		}
		ClearHeadersFile();
		
		bool Test2 = MultiAddHeaderTest();
		if(!Test2)
		{
			PrintHeaderLogs("Test 2 " + (Test2 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Headers Tests Failed !!!");
			return;
		}
		ClearHeadersFile();
		
		bool Test3 = RemoveHeaderTest();
		if(!Test3)
		{
			PrintHeaderLogs("Test 3 " + (Test3 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Headers Tests Failed !!!");
			return;
		}
		ClearHeadersFile();
		
		if(Test1 && Test2 && Test3)
		{
			DevLog.Log("Passed - Headers Tests");
		}
		else
		{
			DevLog.Log("ERROR: Headers Tests Failed !!!");
		}
		
		ClearHeadersFile();
		
	}
	
	
	private static bool AddHeaderTest()
	{
		string RightResult = "123;";
		
		fm.AddHeader("123");
		
		using var file = FileAccess.Open(HeadersFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private static bool MultiAddHeaderTest()
	{
		string RightResult = "321;123;";
		
		fm.AddHeader("321");
		fm.AddHeader("123");
		fm.AddHeader("123");
		fm.AddHeader("123");
		fm.AddHeader("123");
		fm.AddHeader("123");
		
		using var file = FileAccess.Open(HeadersFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private static bool RemoveHeaderTest()
	{
		string RightResult = "321;";
		
		fm.AddHeader("123");
		fm.AddHeader("321");
		fm.RemoveHeader("123");
		
		using var file = FileAccess.Open(HeadersFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	
	private static void ClearHeadersFile()
	{
		ClearFile(HeadersFilePath);
	}
	
	public static string Test_Headers_old()
	{
		string Header = "987-----";
		
		if(IsLogsEnabledForHeaders)
		{
			PrintAllHeaders();
		}
		PrintHeaderLogs("1");
		
	 	fm.AddHeader(Header);
		fm.AddHeader(Header);
		fm.AddHeader(Header);
		fm.AddHeader(Header);
		fm.AddHeader(Header);
		fm.AddHeader(Header);
		fm.AddHeader("123");
		fm.AddHeader(Header);
		fm.AddHeader(Header);
		fm.AddHeader(Header);
		fm.AddHeader(Header);
		fm.AddHeader(Header);
		if(IsLogsEnabledForHeaders)
		{
			PrintHeaderLogs("6");
		}
		
		PrintAllHeaders();
		if(IsLogsEnabledForHeaders)
		{
			PrintHeaderLogs("7");
		}
		/*
		fm.RemoveHeader(Header);
		DevLog.Log("8");
		
		PrintAllHeaders();
		DevLog.Log("9");
		*/
		
		
		
		//string Value = (string)Headers[0];
		return "";
	}
	
	private static void PrintHeaderLogs(string log)
	{
		if(IsLogsEnabledForHeaders)
		{
			DevLog.Log(log);
		}
	}
	
	private static void PrintAllHeaders()
	{
		Godot.Collections.Array<string> Headers = fm.GetAllHeaders();
		foreach(string header in Headers)
		{
			GD.Print(header);
		}
		GD.Print("-----");
	}
	
	#endregion Headers
	
	#region Localizations
	
	public static void Test_Localizations()
	{
		ClearLocalizationsFile();
		
		bool Test1 = AddLocalizationTest();
		if(!Test1)
		{
			PrintLocalizationLogs("Test 1 " + (Test1 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Localizations Tests Failed !!!");
			return;
		}
		ClearLocalizationsFile();
		
		bool Test2 = MultiAddLocalizationTest();
		if(!Test2)
		{
			PrintLocalizationLogs("Test 2 " + (Test2 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Localizations Tests Failed !!!");
			return;
		}
		ClearLocalizationsFile();
		
		bool Test3 = RemoveLocalizationTest();
		if(!Test3)
		{
			PrintLocalizationLogs("Test 3 " + (Test3 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Localizations Tests Failed !!!");
			return;
		}
		ClearLocalizationsFile();
		
		if(Test1 && Test2 && Test3)
		{
			DevLog.Log("Passed - Localizations Tests");
		}
		else
		{
			DevLog.Log("ERROR: Localizations Tests Failed !!!");
		}
		
		ClearLocalizationsFile();
		
	}
	
	
	private static bool AddLocalizationTest()
	{
		string RightResult = "123;";
		
		fm.AddLocalization("123");
		
		using var file = FileAccess.Open(LocalizationsFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private static bool MultiAddLocalizationTest()
	{
		string RightResult = "321;123;";
		
		fm.AddLocalization("321");
		fm.AddLocalization("123");
		fm.AddLocalization("123");
		fm.AddLocalization("123");
		fm.AddLocalization("123");
		fm.AddLocalization("123");
		
		using var file = FileAccess.Open(LocalizationsFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private static bool RemoveLocalizationTest()
	{
		string RightResult = "321;";
		
		fm.AddLocalization("123");
		fm.AddLocalization("321");
		fm.RemoveLocalization("123");
		
		using var file = FileAccess.Open(LocalizationsFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	
	private static void ClearLocalizationsFile()
	{
		ClearFile(LocalizationsFilePath);
	}
	
	
	private static void PrintLocalizationLogs(string log)
	{
		if(IsLogsEnabledForLocalizations)
		{
			DevLog.Log(log);
		}
	}
	
	private static void PrintAllLocalizations()
	{
		Godot.Collections.Array<string> Localizations = fm.GetAllLocalizations();
		foreach(string Localization in Localizations)
		{
			GD.Print(Localization);
		}
		GD.Print("-----");
	}
	
	#endregion Localizations
	
	#region Speech
	public static void Test_Speeches()
	{
		ClearAllFiles();
		
		bool Test1 = AddSpeechTest();
		//PrintAllSpeeches();
		if(!Test1)
		{
			PrintSpeechesLogs("Test 1 " + (Test1 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Speeches Tests Failed !!!");
			return;
		}
		ClearAllFiles();
		
		
		bool Test2 = EditSpeechTest();
		if(!Test2)
		{
			PrintSpeechesLogs("Test 2 " + (Test2 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Speeches Tests Failed !!!");
			return;
		}
		ClearAllFiles();
		
		bool Test3 = RemoveSpeechTest();
		if(!Test3)
		{
			PrintSpeechesLogs("Test 3 " + (Test3 ? "Passed" : "Failed"));
			DevLog.Log("ERROR: Speeches Tests Failed !!!");
			return;
		}
		ClearAllFiles();
		
		
		if(Test1 && Test2 && Test3)
		{
			DevLog.Log("Passed - Speeches Tests");
		}
		else
		{
			DevLog.Log("ERROR: Speeches Tests Failed !!!");
		}
		
		ClearAllFiles();
	}
	
	private static void SpeechTestPrepare()
	{
		fm.AddHeader("Header1");
		fm.AddHeader("Header2");
		
		fm.AddLocalization("Lang1");
		fm.AddLocalization("Lang2");
	}
	
	private static bool AddSpeechTest()
	{
		SpeechTestPrepare();
		
		string RightResult = "123;Header1;Lang1;000\r\n" + "123;Header1;Lang2;000\r\n" +
							 "321;Header2;Lang1;000\r\n" + "321;Header2;Lang2;000\r\n";
		
		Speech SpeechToAdd = new Speech("123", "Header1", "Lang1", "000");
		Speech SpeechToAdd1 = new Speech("321", "Header2", "Lang1", "000");
		
		// Shouldnt draw
		Speech SpeechToAdd2 = new Speech("321", "Header2", "Lang431", "000");
		Speech SpeechToAdd3 = new Speech("321", "Header5325", "Lang1", "000");
		Speech SpeechToAdd4 = new Speech("321", "Header1", "Lang1", "000");
		
		fm.AddSpeech(SpeechToAdd);
		fm.AddSpeech(SpeechToAdd1);
		fm.AddSpeech(SpeechToAdd2);
		fm.AddSpeech(SpeechToAdd3);
		fm.AddSpeech(SpeechToAdd4);
		
		using var file = FileAccess.Open(SpeechFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	
	private static bool EditSpeechTest()
	{
		SpeechTestPrepare();
		
		string RightResult = "123;Header1;Lang1;000\r\n" + 
							 "123;Header1;Lang2;111\r\n" + 
							 "321;Header2;Lang1;000\r\n" +
							 "321;Header2;Lang2;000\r\n" 
							 ;
		
		Speech SpeechToAdd = new Speech("123", "Header1", "Lang1", "000");
		Speech SpeechToAdd2 = new Speech("321", "Header2", "Lang1", "000");
		
		
		Speech BeforeEditSpeech = new Speech("123", "Header1", "Lang2", "000");
		Speech AfterEditSpeech = new Speech("123", "Header1", "Lang2", "111");
		
		fm.AddSpeech(SpeechToAdd);
		fm.AddSpeech(SpeechToAdd2);
		
		fm.EditSpeechText(BeforeEditSpeech, AfterEditSpeech);
		
		using var file = FileAccess.Open(SpeechFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private static bool RemoveSpeechTest()
	{
		SpeechTestPrepare();
		
		string RightResult = "321;Header2;Lang1;000\r\n" +
							 "321;Header2;Lang2;000\r\n" 
							 ;
		
		Speech SpeechToAdd = new Speech("123", "Header1", "Lang1", "000");
		Speech SpeechToAdd2 = new Speech("321", "Header2", "Lang1", "000");
		
		fm.AddSpeech(SpeechToAdd);
		fm.AddSpeech(SpeechToAdd2);
		
		fm.RemoveSpeech(SpeechToAdd);
		
		using var file = FileAccess.Open(SpeechFilePath, FileAccess.ModeFlags.Read);
		string Content = file.GetAsText();
		file.Close();
		
		if(Content == RightResult)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private static void PrintSpeechesLogs(string Log)
	{
		if(IsLogsEnabledForSpeeches)
		{
			DevLog.Log(Log);
		}
	}
	
	private static void ClearSpeechesFile()
	{
		ClearFile(SpeechFilePath);
	}
	
	private static void ClearAllFiles()
	{
		ClearFile(HeadersFilePath);
		ClearFile(LocalizationsFilePath);
		ClearFile(SpeechFilePath);
	}
	
	private static void PrintAllSpeeches()
	{
		var Speeches = fm.GetAllSpeeches();
		foreach(var speech in Speeches)
		{
			Speech CastedItem = (Speech)speech;
			GD.Print(CastedItem.Key);
			GD.Print(CastedItem.Header);
			GD.Print(CastedItem.Localization);
			GD.Print(CastedItem.Text);
			GD.Print("");
		}
		GD.Print("-----");
	}
	
	#endregion Speech
	
	#region Compile
	
	public static void CompilePrepare()
	{
		fm.AddHeader("Header1");
		fm.AddHeader("Header2");
		
		fm.AddLocalization("Lang1");
		fm.AddLocalization("Lang2");
		
		Speech SpeechToAdd = new Speech("123", "Header1", "Lang1", "000");
		Speech SpeechToAdd2 = new Speech("321", "Header2", "Lang1", "111");
		
		Speech BeforeSpeech = new Speech("321", "Header2", "Lang2", "111");
		Speech EditedSpeech = new Speech("321", "Header2", "Lang2", "333");
		
		fm.AddSpeech(SpeechToAdd);
		fm.AddSpeech(SpeechToAdd2);
		fm.EditSpeechText(BeforeSpeech, EditedSpeech);
	}
	
	#endregion Compile
	
	#region Private
	
	private static void ClearFile(string _Path)
	{
		using var file = FileAccess.Open(_Path, FileAccess.ModeFlags.Write);
		file.Close();
	}
	
	#endregion Private
}
