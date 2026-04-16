using System;

public enum GameState
{
    Gameplay,
    UI
}

public class GameStateController
{
    public GameState CurrentState { get; private set; }

    public event Action<GameState> OnStateChanged;

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}