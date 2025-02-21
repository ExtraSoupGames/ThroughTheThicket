using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameState
{
    public void Open();
    public void Close();
    public void Initialize(GameManager manager);
}
