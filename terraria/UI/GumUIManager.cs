using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Gum;
using Gum.Wireframe;
using MonoGameLibrary.Input;
using terraria.Players;
using Gum.Forms.Controls;

using static terraria.Constants;


namespace terraria.UI;

public enum UIScene
{
    StartMenu,
    Playing,    
    EscapeMenu,  
    OptionsMenu 
}

public class GumUIManager
{
    private readonly Game _game;
    private readonly PlayerInput _playerInput;
    private readonly Action _onExitGame;

    public UIScene CurrentScene { get; private set; } = UIScene.StartMenu;
    private UIScene _previousScene = UIScene.StartMenu;

    private FrameworkElement _startMenuRoot;
    private FrameworkElement _escapeMenuRoot;
    private FrameworkElement _optionsMenuRoot;

    private KeyAction? _actionWaitingForRebind = null;
    private Button _buttonWaitingForRebind = null;
    private readonly Dictionary<KeyAction, Button> _keybindButtons = new();

    public bool IsGamePaused => CurrentScene != UIScene.Playing;

    public GumUIManager(Game game, PlayerInput playerInput, Action onExitGame)
    {
        _game = game;
        _playerInput = playerInput;
        _onExitGame = onExitGame;
    }

    public void Initialize()
    {
        GumService.Default.Initialize(_game);

        _startMenuRoot = BuildStartMenu();
        _escapeMenuRoot = BuildEscapeMenu();
        _optionsMenuRoot = BuildOptionsMenu();

        ChangeScene(UIScene.StartMenu);
    }

    public void ChangeScene(UIScene newScene)
    {
        // cancel any active keybind listening if switching menus
        CancelPendingRebind();

        if (CurrentScene != UIScene.OptionsMenu)
        {
            _previousScene = CurrentScene;
        }

        CurrentScene = newScene;

        _startMenuRoot.IsVisible = CurrentScene == UIScene.StartMenu;
        _escapeMenuRoot.IsVisible = CurrentScene == UIScene.EscapeMenu;
        _optionsMenuRoot.IsVisible = CurrentScene == UIScene.OptionsMenu;
    }

    private FrameworkElement BuildStartMenu()
    {
        var panel = CreateCenteredColumnPanel();

        var title = new Label { Text =  AppName};
        panel.AddChild(title);

        var startBtn = new Button { Text = "Start", Width = 220, Height = 40 };
        startBtn.Click += (_, _) => ChangeScene(UIScene.Playing);
        panel.AddChild(startBtn);

        var optionsBtn = new Button { Text = "Options", Width = 220, Height = 40 };
        optionsBtn.Click += (_, _) => ChangeScene(UIScene.OptionsMenu);
        panel.AddChild(optionsBtn);

        var quitBtn = new Button { Text = "Quit", Width = 220, Height = 40 };
        quitBtn.Click += (_, _) => _onExitGame?.Invoke();
        panel.AddChild(quitBtn);

        return panel;
    }

    private FrameworkElement BuildEscapeMenu()
    {
        var panel = CreateCenteredColumnPanel();

        var title = new Label { Text = "PAUSED" };
        panel.AddChild(title);

        var resumeBtn = new Button { Text = "Resume", Width = 220, Height = 40 };
        resumeBtn.Click += (_, _) => ChangeScene(UIScene.Playing);
        panel.AddChild(resumeBtn);

        var optionsBtn = new Button { Text = "Options", Width = 220, Height = 40 };
        optionsBtn.Click += (_, _) => ChangeScene(UIScene.OptionsMenu);
        panel.AddChild(optionsBtn);

        var mainMenuBtn = new Button { Text = "Quit to Main Menu", Width = 220, Height = 40 };
        mainMenuBtn.Click += (_, _) => ChangeScene(UIScene.StartMenu);
        panel.AddChild(mainMenuBtn);

        return panel;
    }

    private FrameworkElement BuildOptionsMenu()
    {
        var panel = CreateCenteredColumnPanel();

        var title = new Label { Text = "KEYBINDINGS (Click to Rebind)" };
        panel.AddChild(title);


        var scrollViewer = new ScrollViewer();

        scrollViewer.Width = 260;
        scrollViewer.Height = 300;
        panel.AddChild(scrollViewer);

        scrollViewer.InnerPanel.StackSpacing = 2;//spacing in between buttons

        

        foreach (KeyAction action in Enum.GetValues<KeyAction>())
        {
            if (action == KeyAction.Exit) continue; // keep Escape reserved for menu navigation

            var bindBtn = new Button
            {
                Text = $"{action}: {_playerInput.GetBindText(action)}",
                Width = 240,
                Height = 34
            };

            bindBtn.Click += (_, _) => StartListeningForRebind(action, bindBtn);

            _keybindButtons[action] = bindBtn;
            scrollViewer.AddChild(bindBtn);
        }

        var backBtn = new Button { Text = "Back", Width = 220, Height = 40 };
        backBtn.Click += (_, _) => ChangeScene(_previousScene);
        panel.AddChild(backBtn);

        return panel;
    }

    private StackPanel CreateCenteredColumnPanel()
    {
        var panel = new StackPanel
        {
            Spacing = 12
        };
        panel.AddToRoot();
        panel.Visual.Anchor(Anchor.Center);
        return panel;
    }

    private void StartListeningForRebind(KeyAction action, Button button)
    {
        CancelPendingRebind();
        _actionWaitingForRebind = action;
        _buttonWaitingForRebind = button;
        _buttonWaitingForRebind.Text = $"{action}: [Press Any Key/Button...]";
    }

    private void CancelPendingRebind()
    {
        if (_actionWaitingForRebind.HasValue && _buttonWaitingForRebind != null)
        {
            var act = _actionWaitingForRebind.Value;
            _buttonWaitingForRebind.Text = $"{act}: {_playerInput.GetBindText(act)}";
        }
        _actionWaitingForRebind = null;
        _buttonWaitingForRebind = null;
    }

    public void Update(GameTime gameTime, InputManager inputManager)
    {
        // 1. If we are waiting for the user to press a new key in the Options menu, intercept it first
        if (CurrentScene == UIScene.OptionsMenu && _actionWaitingForRebind.HasValue)
        {
            if (TryCaptureNewBinding(inputManager))
            {
                GumService.Default.Update(gameTime);
                return;
            }
        }

        // 2. Toggle Escape Menu when Escape is pressed
        if (_playerInput.IsNewPress(KeyAction.Exit))
        {
            if (CurrentScene == UIScene.Playing)
                ChangeScene(UIScene.EscapeMenu);
            else if (CurrentScene == UIScene.EscapeMenu)
                ChangeScene(UIScene.Playing);
            else if (CurrentScene == UIScene.OptionsMenu)
                ChangeScene(_previousScene);
        }

        // 3. Update Gum UI
        GumService.Default.Update(gameTime);
    }

    private bool TryCaptureNewBinding(InputManager inputManager)
    {
        //cancel rebind when esc
        if (inputManager.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            CancelPendingRebind();
            return true;
        }


        Keys[] pressedKeys = inputManager.Keyboard.CurrentState.GetPressedKeys();
        foreach (Keys key in pressedKeys)
        {
            if (inputManager.Keyboard.WasKeyJustPressed(key))
            {
                ApplyRebind(new TriggerKey(key));
                return true;
            }
        }

        foreach (MouseButton mb in Enum.GetValues<MouseButton>())
        {
            if (inputManager.Mouse.WasButtonJustPressed(mb))
            {
                ApplyRebind(new TriggerKey(mb));
                return true;
            }
        }

        var pad = inputManager.GamePads[(int)PlayerIndex.One];
        foreach (Buttons btn in Enum.GetValues<Buttons>())
        {
            if (pad.IsButtonDown(btn))
            {
                ApplyRebind(new TriggerKey(btn));
                return true;
            }
        }

        return false;
    }

    private void ApplyRebind(TriggerKey newTrigger)
    {
        KeyAction action = _actionWaitingForRebind.Value;
        _playerInput.RebindPrimary(action, newTrigger);
        _buttonWaitingForRebind.Text = $"{action}: {_playerInput.GetBindText(action)}";

        _actionWaitingForRebind = null;
        _buttonWaitingForRebind = null;
    }

    public void Draw()
    {
        GumService.Default.Draw();
    }
}