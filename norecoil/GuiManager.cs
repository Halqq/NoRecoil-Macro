using ImGuiNET;
using System.Numerics;

namespace norecoil;

public class GuiManager
{    
    private int _selectedTab = 0; // 0 = Macro, 1 = Config
    
    private Vector3 _guiColor = new Vector3(0.4f, 0.4f, 0.8f);
    private ConfigManager _configManager;
    private string[] _attackers;
    private string[] _defenders;
    private KeyManager _keyManager;
    
    public GuiManager(ConfigManager configManager, string[] attackers, string[] defenders, KeyManager keyManager)
    {
        _configManager = configManager;
        _attackers = attackers;
        _defenders = defenders;
        _keyManager = keyManager;
        
        _guiColor = _configManager.GetGuiColor();
    }
      public Vector3 GuiColor 
    { 
        get => _guiColor; 
        set => _guiColor = value; 
    }
      public void RenderMainMenu(bool macroEnabled, int selectedAttacker, int selectedDefender, bool usingAttacker,
        ref int recoilDownForce, ref int recoilLeftForce, ref int recoilRightForce,
        out bool attackerClicked, out int newSelectedAttacker,
        out bool defenderClicked, out int newSelectedDefender)
    {
        attackerClicked = false;
        defenderClicked = false;
        newSelectedAttacker = selectedAttacker;
        newSelectedDefender = selectedDefender;
        
        ApplyColorTheme();
        
        ImGui.SetNextWindowPos(new Vector2(10, 10), ImGuiCond.Always);
        
        ImGui.SetNextWindowSize(new Vector2(1000, 500), ImGuiCond.Always);
        
        bool showMenu = true;
        if (ImGui.Begin("No Recoil Control", ref showMenu, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize))
        {
            if (ImGui.BeginTabBar("MainTabs"))
            {
                if (ImGui.BeginTabItem("Macro"))
                {
                    RenderMacroTab(macroEnabled, selectedAttacker, selectedDefender, usingAttacker,
                        ref recoilDownForce, ref recoilLeftForce, ref recoilRightForce,
                        out attackerClicked, out newSelectedAttacker,
                        out defenderClicked, out newSelectedDefender);
                    ImGui.EndTabItem();
                }
                  if (ImGui.BeginTabItem("Config"))
                {
                    RenderConfigTab();
                    ImGui.EndTabItem();
                }
                
                ImGui.EndTabBar();
            }
            
            ImGui.End();
        }
    }
    
    private void RenderMacroTab(bool macroEnabled, int selectedAttacker, int selectedDefender, bool usingAttacker,
        ref int recoilDownForce, ref int recoilLeftForce, ref int recoilRightForce,
        out bool attackerClicked, out int newSelectedAttacker,
        out bool defenderClicked, out int newSelectedDefender)
    {
        attackerClicked = false;
        defenderClicked = false;
        newSelectedAttacker = selectedAttacker;
        newSelectedDefender = selectedDefender;
        
        ImGui.Text($"Macro Status: {(macroEnabled ? "ENABLED" : "DISABLED")}");
        
        string currentOperator = usingAttacker ? _attackers[selectedAttacker] : _defenders[selectedDefender];
        string operatorType = usingAttacker ? "ATTACKER" : "DEFENDER";
        
        ImGui.Spacing();
        
        int oldDownForce = recoilDownForce;
        int oldLeftForce = recoilLeftForce;
        int oldRightForce = recoilRightForce;
        
        ImGui.Text($"Recoil Down Force: {recoilDownForce}");
        ImGui.SliderInt("##recoil_down", ref recoilDownForce, 0, 50);
        ImGui.Spacing();
        
        ImGui.Text($"Recoil Left Force: {recoilLeftForce}");
        ImGui.SliderInt("##recoil_left", ref recoilLeftForce, 0, 50);
        ImGui.Spacing();
        
        ImGui.Text($"Recoil Right Force: {recoilRightForce}");
        ImGui.SliderInt("##recoil_right", ref recoilRightForce, 0, 50);
        
        ImGui.Separator();
        ImGui.Spacing();
        
        ImGui.Columns(2, "OperatorColumns", true);
        ImGui.SetColumnWidth(0, 400);
        ImGui.SetColumnWidth(1, 400);
        
        if (usingAttacker)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 1.0f, 0.0f, 1.0f)); 
            ImGui.Text("Attackers (ACTIVE):");
            ImGui.PopStyleColor();
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1.0f)); 
            ImGui.Text("Attackers:");
            ImGui.PopStyleColor();
        }
        
        ImGui.SetNextItemWidth(-1); 
        
        int previousSelectedAttacker = selectedAttacker;
        
        int tempSelectedAttacker = selectedAttacker;
        bool attackerListClicked = ImGui.ListBox("##attackers", ref tempSelectedAttacker, _attackers, _attackers.Length, 15);
        
        if (attackerListClicked)
        {
            Console.WriteLine($"Clique detectado em atacante: {_attackers[tempSelectedAttacker]}");
            attackerClicked = true;
            newSelectedAttacker = tempSelectedAttacker;
        }

        ImGui.NextColumn();

        if (!usingAttacker)
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.0f, 1.0f, 0.0f, 1.0f)); 
            ImGui.Text("Defenders (ACTIVE):");
            ImGui.PopStyleColor();
        }
        else
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1.0f));
            ImGui.Text("Defenders:");
            ImGui.PopStyleColor();
        }
        
        ImGui.SetNextItemWidth(-1); 
        
        int previousSelectedDefender = selectedDefender;
        
        int tempSelectedDefender = selectedDefender;
        bool defenderListClicked = ImGui.ListBox("##defenders", ref tempSelectedDefender, _defenders, _defenders.Length, 15);
        
        if (defenderListClicked)
        {
            Console.WriteLine($"Clique detectado em defensor: {_defenders[tempSelectedDefender]}");
            defenderClicked = true;
            newSelectedDefender = tempSelectedDefender;
        }
        
        ImGui.Columns(1);
    }
      private void RenderConfigTab()
    {
        ImGui.Text("GUI Color:");
        ImGui.SetNextItemWidth(200);
        
        Vector3 oldColor = _guiColor;
        ImGui.ColorPicker3("Interface Color", ref _guiColor);
        
        if (oldColor != _guiColor)
        {
            _configManager.UpdateGuiColor(_guiColor);
        }
        
        ImGui.Separator();
        
        ImGui.Text("Key Binds:");
        
        ImGui.Text("Toggle GUI:");
        ImGui.SameLine();
        if (_keyManager.WaitingForGuiKeyBind)
        {
            ImGui.Text("Press any key...");
        }
        else
        {
            string guiKeyName = _keyManager.GetKeyName(_keyManager.ToggleGuiKey);
            if (ImGui.Button($"{guiKeyName}##gui_bind"))
            {
                _keyManager.WaitingForGuiKeyBind = true;
            }
        }
        
        ImGui.Text("Toggle Macro:");
        ImGui.SameLine();
        if (_keyManager.WaitingForMacroKeyBind)
        {
            ImGui.Text("Press any key...");
        }
        else
        {
            string macroKeyName = _keyManager.GetKeyName(_keyManager.ToggleMacroKey);
            if (ImGui.Button($"{macroKeyName}##macro_bind"))
            {
                _keyManager.WaitingForMacroKeyBind = true;
            }
        }
    }
    
    public void ApplyColorTheme()
    {
        var style = ImGui.GetStyle();
        
        style.Colors[(int)ImGuiCol.Button] = new Vector4(_guiColor.X, _guiColor.Y, _guiColor.Z, 1.0f);
        style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(_guiColor.X * 1.2f, _guiColor.Y * 1.2f, _guiColor.Z * 1.2f, 1.0f);
        style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(_guiColor.X * 0.8f, _guiColor.Y * 0.8f, _guiColor.Z * 0.8f, 1.0f);
        
        style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(_guiColor.X, _guiColor.Y, _guiColor.Z, 1.0f);
        style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(_guiColor.X * 1.2f, _guiColor.Y * 1.2f, _guiColor.Z * 1.2f, 1.0f);
        style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(_guiColor.X * 0.4f, _guiColor.Y * 0.4f, _guiColor.Z * 0.4f, 1.0f);
        style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(_guiColor.X * 0.6f, _guiColor.Y * 0.6f, _guiColor.Z * 0.6f, 1.0f);
        style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(_guiColor.X * 0.8f, _guiColor.Y * 0.8f, _guiColor.Z * 0.8f, 1.0f);
        
        style.Colors[(int)ImGuiCol.Tab] = new Vector4(_guiColor.X * 0.6f, _guiColor.Y * 0.6f, _guiColor.Z * 0.6f, 1.0f);
        style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(_guiColor.X, _guiColor.Y, _guiColor.Z, 1.0f);
        style.Colors[(int)ImGuiCol.TabSelected] = new Vector4(_guiColor.X * 0.8f, _guiColor.Y * 0.8f, _guiColor.Z * 0.8f, 1.0f);

        style.Colors[(int)ImGuiCol.Header] = new Vector4(_guiColor.X * 0.6f, _guiColor.Y * 0.6f, _guiColor.Z * 0.6f, 1.0f);
        style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(_guiColor.X * 0.8f, _guiColor.Y * 0.8f, _guiColor.Z * 0.8f, 1.0f);
        style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(_guiColor.X, _guiColor.Y, _guiColor.Z, 1.0f);
        
        style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(_guiColor.X, _guiColor.Y, _guiColor.Z, 1.0f);
        
        style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(_guiColor.X * 0.2f, _guiColor.Y * 0.2f, _guiColor.Z * 0.2f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(_guiColor.X * 0.6f, _guiColor.Y * 0.6f, _guiColor.Z * 0.6f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(_guiColor.X * 0.8f, _guiColor.Y * 0.8f, _guiColor.Z * 0.8f, 1.0f);
        style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(_guiColor.X, _guiColor.Y, _guiColor.Z, 1.0f);
        
        style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(_guiColor.X * 0.4f, _guiColor.Y * 0.4f, _guiColor.Z * 0.4f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(_guiColor.X * 0.6f, _guiColor.Y * 0.6f, _guiColor.Z * 0.6f, 1.0f);
        style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(_guiColor.X * 0.3f, _guiColor.Y * 0.3f, _guiColor.Z * 0.3f, 1.0f);
        
        style.Colors[(int)ImGuiCol.Separator] = new Vector4(_guiColor.X * 0.6f, _guiColor.Y * 0.6f, _guiColor.Z * 0.6f, 1.0f);
        style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(_guiColor.X * 0.8f, _guiColor.Y * 0.8f, _guiColor.Z * 0.8f, 1.0f);
        style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(_guiColor.X, _guiColor.Y, _guiColor.Z, 1.0f);
        
        style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(_guiColor.X * 0.6f, _guiColor.Y * 0.6f, _guiColor.Z * 0.6f, 1.0f);        style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(_guiColor.X * 0.8f, _guiColor.Y * 0.8f, _guiColor.Z * 0.8f, 1.0f);
        style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(_guiColor.X, _guiColor.Y, _guiColor.Z, 1.0f);
    }
}