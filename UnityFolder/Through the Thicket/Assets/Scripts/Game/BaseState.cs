using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState : IWorldState
{
    public override void Open()
    {
        
    }
    public override void Close() 
    { 

    }
    public override void Initialize(GameManager manager)
    {

    }
    public override void UpdateWhenOpen()
    {

    }

    public override void Pause()
    {
    }

    public override void Play()
    {
    }

    public override void TakeInput(Inputs input)
    {
    }

    public override void SetTile(int x, int y, Layers layer, ITileSegment segment)
    {
    }
}
