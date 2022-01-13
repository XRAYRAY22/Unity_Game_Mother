using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class cPuzzle : MonoBehaviour
{
    [SerializeField] private UnityEvent successEvent;
    [SerializeField] private cMatActive[] targets; // can't expose interface lists??

    public void Press(int index)
    {
        bool correct = true;

        for (int i = 0; i < index; i++)
        {
            if (!targets[i].isActive()) correct = false; // confirm all previous targets are lit
        }

        if (correct) targets[index].Activate();
        else Clear(); // reset if incorrect order

        if (correct && index == targets.Length - 1) successEvent.Invoke();
    }

    void Clear()
    {
        foreach (IActivate target in targets)
        {
            target.Deactivate();
        }
    }
}
