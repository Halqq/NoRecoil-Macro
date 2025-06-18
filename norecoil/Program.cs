
using System.Runtime.InteropServices;
using ClickableTransparentOverlay;

namespace norecoil {    public class Program : Overlay
    {
        private bool _showMenu = false;
        
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);
        
        private const int VK_RSHIFT = 0xA1;
        
        private const int VK_LBUTTON = 0x01;
        private const int VK_RBUTTON = 0x02;         
        private readonly No_recoil_r.MouseMover _mouseMover = new No_recoil_r.MouseMover();
        private readonly ConfigManager _configManager = new ConfigManager();
        private readonly GuiManager _guiManager;
        private readonly KeyManager _keyManager;
        
        private bool _wasRightShiftPressed = false;
        private int _recoilDownForce = 5;
        private int _recoilLeftForce = 2;
        private int _recoilRightForce = 3;
        private bool _macroEnabled = false;
        
        private string[] _attackers = {
            "Sledge", "Thatcher", "Ash", "Thermite", "Twitch", "Montagne", "Glaz", "Fuze", "IQ", "Blitz",
            "Buck", "Blackbeard", "Capitão", "Hibana", "Jackal", "Ying", "Zofia", "Dokkaebi", "Lion",
            "Finka", "Maverick", "Nomad", "Gridlock", "Nokk", "Amaru", "Kali", "Iana", "Ace", "Zero",
            "Flores", "Osa", "Sens", "Grim", "Brava", "Ram"
        };
        
        private string[] _defenders = {
            "Smoke", "Mute", "Castle", "Pulse", "Doc", "Rook", "Kapkan", "Tachanka", "Jäger", "Bandit",
            "Frost", "Valkyrie", "Caveira", "Echo", "Mira", "Lesion", "Ela", "Vigil", "Maestro", "Alibi",
            "Clash", "Kaid", "Mozzie", "Warden", "Goyo", "Wamai", "Oryx", "Melusi", "Aruni", "Thunderbird",
            "Thorn", "Azami", "Solis", "Fenrir", "Tubarão"        };
        
        private int _selectedAttacker = 0;
        private int _selectedDefender = 0;
        
        private int _lastSelectedAttacker = -1;
        private int _lastSelectedDefender = -1;
        private bool _usingAttacker = true; 

        public Program()
        {
            _configManager.Initialize(_attackers, _defenders);
            
            _keyManager = new KeyManager(_configManager);
            
            _guiManager = new GuiManager(_configManager, _attackers, _defenders, _keyManager);
            
            LoadConfigurations();
            
            _lastSelectedAttacker = _selectedAttacker;
            _lastSelectedDefender = _selectedDefender;
            
            _usingAttacker = true;
        }protected override void Render()
        {
            CheckKeyBinds();
            
            CheckMouseButtons();
              if (_showMenu)
            {
                _guiManager.RenderMainMenu(_macroEnabled, _selectedAttacker, _selectedDefender, _usingAttacker,
                    ref _recoilDownForce, ref _recoilLeftForce, ref _recoilRightForce,
                    out bool attackerClicked, out int newSelectedAttacker,
                    out bool defenderClicked, out int newSelectedDefender);
                
                if (attackerClicked)
                {
#if DEBUG
                    Console.WriteLine($"Clique detectado em atacante: {_attackers[newSelectedAttacker]}");
#endif
                    if (newSelectedAttacker != _selectedAttacker)
                    {
                        SaveCurrentOperatorConfig();
                    }

                    _selectedAttacker = newSelectedAttacker;
                    _usingAttacker = true;

                    LoadCurrentOperatorConfig();
                    _configManager.UpdateSelectedOperators(_selectedAttacker, _selectedDefender);
                }
                
                if (defenderClicked)
                {
#if DEBUG
                    Console.WriteLine($"Clique detectado em defensor: {_defenders[newSelectedDefender]}");
#endif
                    if (newSelectedDefender != _selectedDefender)
                    {
                        SaveCurrentOperatorConfig();
                    }

                    _selectedDefender = newSelectedDefender;
                    _usingAttacker = false;

                    LoadCurrentOperatorConfig();
                    _configManager.UpdateSelectedOperators(_selectedAttacker, _selectedDefender);                }
            }
        }        private void CheckKeyBinds()
        {
            if (_keyManager.CheckForKeyBinding())
            {
                return; //Dont process other keys while waiting for bind
            }
            
            if (_keyManager.IsSpecificKeyPressed(_keyManager.ToggleGuiKey))
            {
                _showMenu = !_showMenu;
            }
            
            if (_keyManager.IsSpecificKeyPressed(_keyManager.ToggleMacroKey))
            {
                _macroEnabled = !_macroEnabled;
                _configManager.UpdateMacroEnabled(_macroEnabled);
            }        }
          private void CheckMouseButtons()
        {
            if (!_macroEnabled)
                return;
                
            bool leftPressed = _keyManager.IsMouseButtonPressed(VK_LBUTTON);
            bool rightPressed = _keyManager.IsMouseButtonPressed(VK_RBUTTON);
            
            if (leftPressed && rightPressed)
            {
                _mouseMover.MoveMouseWithForce(_recoilDownForce, _recoilLeftForce, _recoilRightForce);
                
                Thread.Sleep(10);
            }
        }        
        private void LoadConfigurations()
        {
            var macroConfig = _configManager.GetMacroConfig();
            var appConfig = _configManager.GetAppConfig();
            
            _guiManager.GuiColor = _configManager.GetGuiColor();
            _keyManager.LoadKeybinds(appConfig.ToggleGuiKey, appConfig.ToggleMacroKey);
            
            _macroEnabled = macroConfig.MacroEnabled;
            _selectedAttacker = macroConfig.SelectedAttacker;
            _selectedDefender = macroConfig.SelectedDefender;
            
#if DEBUG
            Console.WriteLine($"Configurações carregadas - Cor: R={_guiManager.GuiColor.X}, G={_guiManager.GuiColor.Y}, B={_guiManager.GuiColor.Z}");
            Console.WriteLine($"Keybinds carregados - GUI: {_keyManager.ToggleGuiKey}, Macro: {_keyManager.ToggleMacroKey}");
#endif
            
            LoadCurrentOperatorConfig();
        }

        private void LoadCurrentOperatorConfig()
        {
            string operatorName;
            bool isAttacker;
            
            if (_usingAttacker)
            {
                operatorName = _attackers[_selectedAttacker];
                isAttacker = true;
            }
            else
            {
                operatorName = _defenders[_selectedDefender];
                isAttacker = false;
            }
            
            var config = _configManager.GetOperatorConfig(operatorName, isAttacker);
            
            _recoilDownForce = config.RecoilDownForce;
            _recoilLeftForce = config.RecoilLeftForce;
            _recoilRightForce = config.RecoilRightForce;
            
#if DEBUG
            Console.WriteLine($"Carregando configuração: {operatorName} ({(isAttacker ? "Atacante" : "Defensor")}) - Down:{_recoilDownForce}, Left:{_recoilLeftForce}, Right:{_recoilRightForce}");
#endif
        }

        private void SaveCurrentOperatorConfig()
        {
            string operatorName;
            bool isAttacker;
            
            if (_usingAttacker)
            {
                operatorName = _attackers[_selectedAttacker];
                isAttacker = true;
            }
            else
            {
                operatorName = _defenders[_selectedDefender];
                isAttacker = false;
            }
            
#if DEBUG
            Console.WriteLine($"Salvando configuração: {operatorName} ({(isAttacker ? "Atacante" : "Defensor")}) - Down:{_recoilDownForce}, Left:{_recoilLeftForce}, Right:{_recoilRightForce}");
#endif
            _configManager.UpdateOperatorConfig(operatorName, isAttacker, _recoilDownForce, _recoilLeftForce, _recoilRightForce);
        }        private void CheckOperatorChanges()
        {
            LoadCurrentOperatorConfig();
            
            _lastSelectedAttacker = _selectedAttacker;
            _lastSelectedDefender = _selectedDefender;
            
            _configManager.UpdateSelectedOperators(_selectedAttacker, _selectedDefender);
        }

        public static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("No Recoil Overlay started");
#endif
            
            Program p = new Program();
            p.Start().Wait();
        }
    }
    

}