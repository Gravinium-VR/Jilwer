using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using Org.Gravinium.Jilwer.Runtime.Core.Collections;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ArrayListTester : UdonSharpBehaviour
{
    private ObjectArrayList list = new ObjectArrayList();
    private int counter = 1;

    public override void Interact()
    {
        list.Add(counter);
        counter++;

        string msg = "[";
        for (int i = 0; i < list.Length(); i++)
        {
            msg += list.Get(i) + (list.Length() - 1 < i ? ", " : "");
        }
        msg += "]";
        Debug.Log("[ArrayListTester] ");
    }
}
