using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Numerics;
using System;

namespace norecoil;

public class ConfigManager
{
    private const string ConfigFolderName = "NoRecoil";
    private const string MacroConfigFile = "macro_config.json";
    private const string AppConfigFile = "app_config.json";
    
    private string GetConfigDirectory()
    {
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(documentsPath, ConfigFolderName);
    }
    
    private string GetMacroConfigPath() => Path.Combine(GetConfigDirectory(), MacroConfigFile);
    private string GetAppConfigPath() => Path.Combine(GetConfigDirectory(), AppConfigFile);
    
    private void EnsureConfigDirectoryExists()
    {
        string configDir = GetConfigDirectory();
        if (!Directory.Exists(configDir))
        {
            Directory.CreateDirectory(configDir);
        }
    }
    
    public class OperatorMacroConfig
    {
        public int RecoilDownForce { get; set; } = 0;
        public int RecoilLeftForce { get; set; } = 0;
        public int RecoilRightForce { get; set; } = 0;
    }
    
    public class MacroConfig
    {
        public Dictionary<string, OperatorMacroConfig> Attackers { get; set; } = new();
        public Dictionary<string, OperatorMacroConfig> Defenders { get; set; } = new();
        public int SelectedAttacker { get; set; } = 0;
        public int SelectedDefender { get; set; } = 0;
        public bool MacroEnabled { get; set; } = false;
    }
    
    public class Vector3Config
    {
        public float X { get; set; } = 0.4f;
        public float Y { get; set; } = 0.4f;
        public float Z { get; set; } = 0.8f;
        
        public Vector3 ToVector3() => new Vector3(X, Y, Z);
        
        public static Vector3Config FromVector3(Vector3 vector) => new Vector3Config
        {
            X = vector.X,
            Y = vector.Y,
            Z = vector.Z
        };
    }
    
    public class AppConfig
    {
        public Vector3Config GuiColor { get; set; } = new Vector3Config();

        //Default keybinds
        
        public int ToggleGuiKey { get; set; } = 0xA1; 
        public int ToggleMacroKey { get; set; } = 0x2E; 
    }
    
    private MacroConfig _macroConfig = new();
    private AppConfig _appConfig = new();
    
    public MacroConfig GetMacroConfig() => _macroConfig;
    public AppConfig GetAppConfig() => _appConfig;
    
    public void Initialize(string[] attackers, string[] defenders)
    {
        LoadConfigs();
        
        foreach (var attacker in attackers)
        {
            if (!_macroConfig.Attackers.ContainsKey(attacker))
            {
                _macroConfig.Attackers[attacker] = new OperatorMacroConfig();
            }
        }
        
        foreach (var defender in defenders)
        {
            if (!_macroConfig.Defenders.ContainsKey(defender))
            {
                _macroConfig.Defenders[defender] = new OperatorMacroConfig();
            }
        }
        
        SaveMacroConfig();
    }
    
    private void LoadConfigs()
    {
        LoadMacroConfig();
        LoadAppConfig();
    }
    
    private void LoadMacroConfig()
    {
        try
        {
            string configPath = GetMacroConfigPath();
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<MacroConfig>(json);
                if (config != null)
                {
                    _macroConfig = config;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar configuração de macro: {ex.Message}");
        }
    }
    
    private void LoadAppConfig()
    {
        try
        {
            string configPath = GetAppConfigPath();
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<AppConfig>(json);
                if (config != null)
                {
                    _appConfig = config;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar configuração da aplicação: {ex.Message}");
        }
    }
    
    public void SaveMacroConfig()
    {
        try
        {
            EnsureConfigDirectoryExists();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_macroConfig, options);
            File.WriteAllText(GetMacroConfigPath(), json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar configuração de macro: {ex.Message}");
        }
    }
    
    public void SaveAppConfig()
    {
        try
        {
            EnsureConfigDirectoryExists();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_appConfig, options);
            File.WriteAllText(GetAppConfigPath(), json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar configuração da aplicação: {ex.Message}");
        }
    }
    public void UpdateOperatorConfig(string operatorName, bool isAttacker, int downForce, int leftForce, int rightForce)
    {
        var config = new OperatorMacroConfig
        {
            RecoilDownForce = downForce,
            RecoilLeftForce = leftForce,
            RecoilRightForce = rightForce
        };
        
        if (isAttacker)
        {
            _macroConfig.Attackers[operatorName] = config;
            Console.WriteLine($"Configuração de ATACANTE salva: {operatorName} - Down:{downForce}, Left:{leftForce}, Right:{rightForce}");
        }
        else
        {
            _macroConfig.Defenders[operatorName] = config;
            Console.WriteLine($"Configuração de DEFENSOR salva: {operatorName} - Down:{downForce}, Left:{leftForce}, Right:{rightForce}");
        }
        
        SaveMacroConfig();
    }
    public OperatorMacroConfig GetOperatorConfig(string operatorName, bool isAttacker)
    {
        if (isAttacker && _macroConfig.Attackers.ContainsKey(operatorName))
        {
            var config = _macroConfig.Attackers[operatorName];
            Console.WriteLine($"Configuração de ATACANTE carregada: {operatorName} - Down:{config.RecoilDownForce}, Left:{config.RecoilLeftForce}, Right:{config.RecoilRightForce}");
            return config;
        }
        else if (!isAttacker && _macroConfig.Defenders.ContainsKey(operatorName))
        {
            var config = _macroConfig.Defenders[operatorName];
            Console.WriteLine($"Configuração de DEFENSOR carregada: {operatorName} - Down:{config.RecoilDownForce}, Left:{config.RecoilLeftForce}, Right:{config.RecoilRightForce}");
            return config;
        }
        
        Console.WriteLine($"Configuração padrão usada para: {operatorName} ({(isAttacker ? "ATACANTE" : "DEFENSOR")})");
        return new OperatorMacroConfig();
    }
    
    public void UpdateSelectedOperators(int attackerIndex, int defenderIndex)
    {
        _macroConfig.SelectedAttacker = attackerIndex;
        _macroConfig.SelectedDefender = defenderIndex;
        SaveMacroConfig();
    }
    
    public void UpdateMacroEnabled(bool enabled)
    {
        _macroConfig.MacroEnabled = enabled;
        SaveMacroConfig();
    }
    public void UpdateGuiColor(Vector3 color)
    {
        _appConfig.GuiColor = Vector3Config.FromVector3(color);
        SaveAppConfig();
        Console.WriteLine($"Cor salva: R={color.X}, G={color.Y}, B={color.Z}");
    }
    
    public Vector3 GetGuiColor()
    {
        return _appConfig.GuiColor.ToVector3();
    }
    
    public void UpdateKeybinds(int guiKey, int macroKey)
    {
        _appConfig.ToggleGuiKey = guiKey;
        _appConfig.ToggleMacroKey = macroKey;
        SaveAppConfig();
        Console.WriteLine($"Keybinds salvos: GUI={guiKey}, Macro={macroKey}");
    }
}