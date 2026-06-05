using UdonSharp;
using UnityEngine;
using Gravinium.Jilwer.Core;
using Gravinium.Jilwer.Core.Collections;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ArrayListTester : UdonSharpBehaviour
{
    [HideInInspector] public JilwerRuntime jilwer;

    private ArrayList _list;
    private int _counter = 1;

    private void Start()
    {
        var err = ArrayList.New(jilwer, out _list);
        if (err != Error.None)
        {
            Debug.LogError($"Failed to create ArrayList! Error: 0x{err:X2}");
            Destroy(this);
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