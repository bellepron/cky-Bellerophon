using UnityEngine;
using Zenject;
using System;

public class UIManager : IInitializable, ITickable, IDisposable
{
    [Inject] readonly GameStateController _gameState;
    [Inject] readonly CursorController _cursor;

    public void Initialize()
    {
        _gameState.OnStateChanged += HandleStateChanged;

        _cursor.SetConfined();
        _gameState.SetState(GameState.Gameplay);
    }

    public void Tick()
    {
        // ESC → toggle
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_gameState.CurrentState == GameState.Gameplay)
                _gameState.SetState(GameState.UI);
            else if (_gameState.CurrentState == GameState.UI)
                _gameState.SetState(GameState.Gameplay);
        }
    }

    void HandleStateChanged(GameState state)
    {

    }

    public void Dispose()
    {
        _gameState.OnStateChanged -= HandleStateChanged;
    }
}