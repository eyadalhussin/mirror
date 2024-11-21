using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum MinionType
{
    Minion,
    PlayerMinion
}

public class MinionCore : NetworkBehaviour
{
    //Type and ID
    private int _id = 999;
    private MinionType _minionType;

    [SyncVar(hook = nameof(OnMinionColorChanged))]
    private Color minionColor = Color.black;

    private void Start()
    {
        if (isServer)
        {
            ChangeColor();
        }
    }

    public void SetMinionType(MinionType type)
    {
        _minionType = type;
    }

    public MinionType GetMinionType()
    {
        return _minionType;
    }

    public void SetID(int identification)
    {
        _id = identification;
    }

    public int GetID()
    {
        return _id;
    }

    [Server]
    private void ChangeColor()
    {
        if (_minionType == MinionType.PlayerMinion)
        {
            MinionPlayerAI mpai = GetComponent<MinionPlayerAI>();
            if (mpai)
            {
                // Directly get the color from some server-side logic.
                // This could be a stored color on the player object that the server is aware of.
                Color playerColor = mpai.GetControllingPlayerColor();
                minionColor = playerColor;
            }
        }
    }

    private void OnMinionColorChanged(Color oldColor, Color newColor)
    {
        // This will be called on clients when the SyncVar is updated from the server
        UpdateRendererColor(newColor);
    }

    [Client]
    private void UpdateRendererColor(Color color)
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend)
        {
            rend.material.color = color;
        }
        else
        {
            Debug.LogError("Renderer component not found on minion.");
        }
    }


}
