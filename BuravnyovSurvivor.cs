using System;
using System.Collections.Generic;
using System.Linq;

[Task(3)]
public class BuravnyovSurvivor() : Player(
    traits: new STraits(0, 4, 2, 4, 0),
    "SurvivorFaction",
    "Буравнёв Игорь Евгеньевич")
{
    private readonly HashSet<string> toPunish = new();
    private readonly Dictionary<string, int> meetings = new();

    public override SMove MakeMove(string enemyUID, IReadOnlyList<SMove> enemyHistory, STraits enemyTraits)
    {
        if (enemyTraits.Equals(traits))
            return new SMove(uid, 100, EMove.SPLIT);

        bool thief = enemyTraits.damage == 5 && enemyTraits.stealth == 5;
        if (thief) return new SMove(uid, 1, EMove.SPLIT);

        if (toPunish.Contains(enemyUID))
            return new SMove(uid, 1, EMove.SKIP);

        if (enemyHistory.Count > 0 && enemyHistory[^1].move == EMove.STEAL)
            return new SMove(uid, 1, EMove.SKIP);

        int k = meetings.GetValueOrDefault(enemyUID);
        if (k == 0) return new SMove(uid, 100, EMove.SPLIT);
        if (k == 1) return new SMove(uid, 1, EMove.SPLIT);

        if (enemyHistory.Count >= 3)
        {
            var splits = enemyHistory.Where(m => m.move == EMove.SPLIT).ToList();
            if (splits.Count >= 2)
            {
                double avg = splits.Average(m => m.bet);
                if (avg >= 60) return new SMove(uid, 100, EMove.SPLIT);
                if (avg >= 20) return new SMove(uid, (int)avg, EMove.SPLIT);
                return new SMove(uid, 1, EMove.SPLIT);
            }
        }
        return new SMove(uid, 50, EMove.SPLIT);
    }

    public override void OnResult(string enemyUID, SMove myMove, SMove enemyMove, double score)
    {
        if (enemyMove.move == EMove.STEAL) toPunish.Add(enemyUID);
        meetings[enemyUID] = meetings.GetValueOrDefault(enemyUID) + 1;
    }
}
