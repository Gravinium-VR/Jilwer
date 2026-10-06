using UdonSharp;
using UnityEngine;
using Gravinium.Jilwer.Core;
using Gravinium.Jilwer.Collections;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ArrayListTester : UdonSharpBehaviour
{
    private ArrayList _list;
    private int _counter = 1;

    private void OnEnable()
    {
        var err = ArrayList.New(out _list);
        if (err != Error.None)
        {
            Debug.LogError($"Failed to create ArrayList! Error: 0x{err:X2}");
            enabled = false;
        }
    }

    public override void Interact()
    {
        _list.Add(_counter);
        _counter++;

        string msg = "[";
        for (int i = 0; i < _list.Length(); i++)
        {
            if (_list.Get(i, out object num) != Error.None) return;
            msg += num + (i < _list.Length() - 1 ? ", " : "]");
        }
        Debug.Log("[ArrayListTester] " + msg);
    }
}