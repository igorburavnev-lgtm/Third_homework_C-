using System;
using System.Collections.Generic;

[Task(1)]
public class BuravnyovSelfish() : Player(
    traits: new STraits(0, 4, 2, 4, 0),
    "SelfishProbe",
    "Буравнёв Игорь Евгеньевич")
{
    private readonly HashSet<string> toPunish = new();
    private readonly Dictionary<string, int> meetings = new();
    private readonly HashSet<string> generous = new();

    public override SMove MakeMove(string enemyUID, IReadOnlyList<SMove> enemyHistory, STraits enemyTraits)
    {
        if (toPunish.Contains(enemyUID))
        {
            toPunish.Remove(enemyUID);
            return new SMove(uid, 1, EMove.SKIP);
        }
        if (enemyHistory.Count > 0 && enemyHistory[^1].move == EMove.STEAL)
            return new SMove(uid, 1, EMove.SKIP);

        int k = meetings.GetValueOrDefault(enemyUID);

        if (k == 0) return new SMove(uid, 50, EMove.SPLIT);
        if (k == 1) return new SMove(uid, 1, EMove.SPLIT);

        if (generous.Contains(enemyUID)) return new SMove(uid, 1, EMove.SPLIT);

        if (enemyHistory.Count > 0 && enemyHistory[^1].move == EMove.SPLIT)
            return new SMove(uid, Math.Clamp(enemyHistory[^1].bet, 1, 100), EMove.SPLIT);

        return new SMove(uid, 1, EMove.SPLIT);
    }

    public override void OnResult(string enemyUID, SMove myMove, SMove enemyMove, double score)
    {
        if (enemyMove.move == EMove.STEAL) { toPunish.Add(enemyUID); return; }

        int k = meetings.GetValueOrDefault(enemyUID);
        if (k == 1 && enemyMove.move == EMove.SPLIT && enemyMove.bet >= 50)
            generous.Add(enemyUID);

        meetings[enemyUID] = k + 1;
    }
}
