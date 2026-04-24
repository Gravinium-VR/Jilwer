using UdonSharp;
using UnityEngine;
using Gravinium.Jilwer.Core;
using Gravinium.Jilwer.Core.Collections;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ArrayListTester : UdonSharpBehaviour
{
    public JilwerRuntime jilwer;

    private ObjectArrayList _list;
    private int _counter = 1;

    private void Start()
    {
        _list = ObjectArrayList.New(jilwer);
    }

    public override void Interact()
    {
        _list.Add(_counter);
        _counter++;

        string msg = "[";
        for (int i = 0; i < _list.Length(); i++)
        {
            msg += _list.Get(i) + (i < _list.Length() - 1 ? ", " : "]");
        }
        Debug.Log("[ArrayListTester] " + msg);
    }
}
