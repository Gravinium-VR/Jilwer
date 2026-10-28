// ReSharper disable Unity.IncorrectMonoBehaviourInstantiation
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
        _list = new ArrayList();
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