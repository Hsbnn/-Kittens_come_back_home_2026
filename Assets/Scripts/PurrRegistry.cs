using System.Collections.Generic;
using UnityEngine;

public static class PurrRegistry
{
    private static readonly List<KittenRescue> Kittens = new List<KittenRescue>();

    public static void Register(KittenRescue kitten)
    {
        if (kitten != null && !Kittens.Contains(kitten))
        {
            Kittens.Add(kitten);
        }
    }

    public static void Unregister(KittenRescue kitten)
    {
        Kittens.Remove(kitten);
    }

    public static void BroadcastPurr(Vector3 source, float range, float beatScore)
    {
        for (int i = Kittens.Count - 1; i >= 0; i--)
        {
            if (Kittens[i] == null)
            {
                Kittens.RemoveAt(i);
                continue;
            }

            Kittens[i].ReceivePurr(source, range, beatScore);
        }
    }
}
