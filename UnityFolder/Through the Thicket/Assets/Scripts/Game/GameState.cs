using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IGameState : MonoBehaviour
{
    protected bool isOpen;
    public virtual void Open()
    {
        isOpen = true;
    }
    public virtual void Close()
    {
        isOpen = false;
    }
    public abstract void Initialize(GameManager manager);
    public abstract void UpdateWhenOpen();
    public bool IsOpen()
    {
        return isOpen;
    }
}
public abstract class IWorldState : IGameState
{
    protected bool isPaused;
    public virtual void Pause()
    {
        isPaused = true;
    }
    public virtual void Play()
    {
        isPaused = false;
    }
    public override void Open()
    {
        Play();
        base.Open();
    }
    public override void Close()
    {
        Pause();
        base.Close();
    }
    public bool IsActiveAndOpen()
    {
        return !isPaused && IsOpen();
    }
    public abstract void TakeInput(int input);

    public abstract void SetTile(int x, int y, Layers layer, ITileSegment segment);
}
public abstract class IUIState : IGameState
{

}