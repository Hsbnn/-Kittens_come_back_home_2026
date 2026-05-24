using System.Collections.Generic;
using UnityEngine;

public static class BellRegistry
{
    private static readonly List<ResonanceBell> Bells = new List<ResonanceBell>();
    private static int nextSignalId;

    public static void Register(ResonanceBell bell)
    {
        if (bell != null && !Bells.Contains(bell))
        {
            Bells.Add(bell);
        }
    }

    public static void Unregister(ResonanceBell bell)
    {
        Bells.Remove(bell);
    }

    public static void BroadcastMeow(Vector3 source, int note, float range)
    {
        BroadcastMeow(source, note, range, 1f);
    }

    public static void BroadcastMeow(Vector3 source, int note, float range, float intensity)
    {
        int signalId = ++nextSignalId;
        BroadcastMeow(source, note, range, intensity, signalId);
    }

    public static void BroadcastMeow(Vector3 source, int note, float range, float intensity, int signalId)
    {
        for (int i = Bells.Count - 1; i >= 0; i--)
        {
            if (Bells[i] == null)
            {
                Bells.RemoveAt(i);
                continue;
            }

            Bells[i].ReceiveMeow(source, note, range, intensity, signalId);
        }

        HarmonyGate.ReportMeow(source, note, range);
    }
}
