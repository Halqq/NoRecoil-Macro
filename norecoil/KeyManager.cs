using System.Runtime.InteropServices;

namespace norecoil;

public class KeyManager
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vKey);
    
    private Dictionary<int, bool> _keyStates = new Dictionary<int, bool>();
    
    private bool _waitingForGuiKeyBind = false;
    private bool _waitingForMacroKeyBind = false;
    
    private int _toggleGuiKey;
    private int _toggleMacroKey;
    
    private ConfigManager _configManager;
    
    public KeyManager(ConfigManager configManager)
    {
        _configManager = configManager;
    }
    
    public bool WaitingForGuiKeyBind 
    { 
        get => _waitingForGuiKeyBind; 
        set => _waitingForGuiKeyBind = value; 
    }
    
    public bool WaitingForMacroKeyBind 
    { 
        get => _waitingForMacroKeyBind; 
        set => _waitingForMacroKeyBind = value; 
    }
    
    public int ToggleGuiKey 
    { 
        get => _toggleGuiKey; 
        set => _toggleGuiKey = value; 
    }
    
    public int ToggleMacroKey 
    { 
        get => _toggleMacroKey; 
        set => _toggleMacroKey = value; 
    }
    
    public bool IsKeyPressed(int key)
    {
        bool isPressed = (GetAsyncKeyState(key) & 0x8000) != 0;
        bool wasPressed = _keyStates.ContainsKey(key) && _keyStates[key];
        
        _keyStates[key] = isPressed;
        
        return isPressed && !wasPressed; 
    }
    
    public bool IsSpecificKeyPressed(int key)
    {
        if (key == 0xA0 || key == 0xA1 || key == 0xA2 || key == 0xA3 || 
            key == 0xA4 || key == 0xA5 || key == 0x5B || key == 0x5C)
        {
            bool isPressed = (GetAsyncKeyState(key) & 0x8000) != 0;
            bool wasPressed = _keyStates.ContainsKey(key) && _keyStates[key];
            
            _keyStates[key] = isPressed;
            
            return isPressed && !wasPressed; 
        }
        else
        {
            return IsKeyPressed(key);
        }
    }
    
    public bool IsMouseButtonPressed(int button)
    {
        return (GetAsyncKeyState(button) & 0x8000) != 0;
    }
    
    public bool CheckForKeyBinding()
    {
        if (!_waitingForGuiKeyBind && !_waitingForMacroKeyBind)
            return false;
            
        int[] specificKeys = { 
            0xA0, 0xA1,
            0xA2, 0xA3, 
            0xA4, 0xA5, 
            0x5B, 0x5C  
        };
        
        foreach (int key in specificKeys)
        {
            if (IsKeyPressed(key))
            {
                if (_waitingForGuiKeyBind)
                {
                    _toggleGuiKey = key;
                    _waitingForGuiKeyBind = false;
                    _configManager.UpdateKeybinds(_toggleGuiKey, _toggleMacroKey);
                    Console.WriteLine($"GUI key bound to: {GetKeyName(key)}");
                }
                else if (_waitingForMacroKeyBind)
                {
                    _toggleMacroKey = key;
                    _waitingForMacroKeyBind = false;
                    _configManager.UpdateKeybinds(_toggleGuiKey, _toggleMacroKey);
                    Console.WriteLine($"Macro key bound to: {GetKeyName(key)}");
                }
                return true;
            }
        }
        
        for (int key = 8; key <= 255; key++)
        {
            if (key == 0x10 || key == 0x11 || key == 0x12) 
                continue;
                
            if (specificKeys.Contains(key))
                continue;
                
            if (IsKeyPressed(key))
            {
                if (_waitingForGuiKeyBind)
                {
                    _toggleGuiKey = key;
                    _waitingForGuiKeyBind = false;
                    _configManager.UpdateKeybinds(_toggleGuiKey, _toggleMacroKey);
                    Console.WriteLine($"GUI key bound to: {GetKeyName(key)}");
                }
                else if (_waitingForMacroKeyBind)
                {
                    _toggleMacroKey = key;
                    _waitingForMacroKeyBind = false;
                    _configManager.UpdateKeybinds(_toggleGuiKey, _toggleMacroKey);
                    Console.WriteLine($"Macro key bound to: {GetKeyName(key)}");
                }
                return true;
            }
        }
        
        return false;
    }
    
    public void LoadKeybinds(int toggleGuiKey, int toggleMacroKey)
    {
        _toggleGuiKey = toggleGuiKey;
        _toggleMacroKey = toggleMacroKey;
    }
    
    public string GetKeyName(int keyCode)
    {
        return keyCode switch
        {
            0x41 => "A", 0x42 => "B", 0x43 => "C", 0x44 => "D", 0x45 => "E", 0x46 => "F",
            0x47 => "G", 0x48 => "H", 0x49 => "I", 0x4A => "J", 0x4B => "K", 0x4C => "L",
            0x4D => "M", 0x4E => "N", 0x4F => "O", 0x50 => "P", 0x51 => "Q", 0x52 => "R",
            0x53 => "S", 0x54 => "T", 0x55 => "U", 0x56 => "V", 0x57 => "W", 0x58 => "X",
            0x59 => "Y", 0x5A => "Z",
            0x30 => "0", 0x31 => "1", 0x32 => "2", 0x33 => "3", 0x34 => "4",
            0x35 => "5", 0x36 => "6", 0x37 => "7", 0x38 => "8", 0x39 => "9",
            0x70 => "F1", 0x71 => "F2", 0x72 => "F3", 0x73 => "F4", 0x74 => "F5", 0x75 => "F6",
            0x76 => "F7", 0x77 => "F8", 0x78 => "F9", 0x79 => "F10", 0x7A => "F11", 0x7B => "F12",
            0x25 => "Left Arrow", 0x26 => "Up Arrow", 0x27 => "Right Arrow", 0x28 => "Down Arrow",
            0x60 => "Numpad 0", 0x61 => "Numpad 1", 0x62 => "Numpad 2", 0x63 => "Numpad 3",
            0x64 => "Numpad 4", 0x65 => "Numpad 5", 0x66 => "Numpad 6", 0x67 => "Numpad 7",
            0x68 => "Numpad 8", 0x69 => "Numpad 9",
            0x6A => "Numpad *", 0x6B => "Numpad +", 0x6D => "Numpad -", 0x6E => "Numpad .", 0x6F => "Numpad /",
            0xA0 => "Left Shift", 0xA1 => "Right Shift",
            0xA2 => "Left Ctrl", 0xA3 => "Right Ctrl", 
            0xA4 => "Left Alt", 0xA5 => "Right Alt",
            0x5B => "Left Windows", 0x5C => "Right Windows",
            0x10 => "Shift (Generic)", 0x11 => "Ctrl (Generic)", 0x12 => "Alt (Generic)",
            0x08 => "Backspace", 0x09 => "Tab", 0x0D => "Enter", 0x14 => "Caps Lock",
            0x1B => "Escape", 0x20 => "Space", 0x21 => "Page Up", 0x22 => "Page Down",
            0x23 => "End", 0x24 => "Home", 0x2C => "Print Screen", 0x2D => "Insert", 0x2E => "Delete",
            0x01 => "Left Mouse", 0x02 => "Right Mouse", 0x04 => "Middle Mouse",
            0x05 => "Mouse 4", 0x06 => "Mouse 5",
            0xBA => "Semicolon (;)", 0xBB => "Equals (=)", 0xBC => "Comma (,)", 0xBD => "Minus (-)",
            0xBE => "Period (.)", 0xBF => "Slash (/)", 0xC0 => "Grave (`)", 0xDB => "Left Bracket ([)",
            0xDC => "Backslash (\\)", 0xDD => "Right Bracket (])", 0xDE => "Quote (')",
            0x13 => "Pause", 0x91 => "Scroll Lock", 0x90 => "Num Lock",
            
            _ => $"Key {keyCode}"
        };
    }
}