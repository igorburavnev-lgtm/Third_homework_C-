using System;
using System.Collections.Generic;
using System.Linq;

[Task(3)]
public class BuravnyovSurvivor() : Player(
    traits: new STraits(1, 4, 1, 3, 1), 
    "SurvivorKatya",
    "Буравнёв Игорь Евгеньевич")
{
    private readonly HashSet<string> toPunish = new();
    private readonly Queue<bool> recentSteals = new();

    public override SMove MakeMove(string enemyUID, IReadOnlyList<SMove> enemyHistory, STraits enemyTraits)
    {
        bool isThief = enemyTraits.damage == 5
                       && enemyTraits.defense == 0
                       && enemyTraits.luck == 0
                       && enemyTraits.charm == 0
                       && enemyTraits.stealth == 5;
        if (isThief)
            return new SMove(uid, 1, EMove.SPLIT);

        if (toPunish.Contains(enemyUID))
            return new SMove(uid, 1, EMove.SKIP);
        if (enemyHistory.Count > 0 && enemyHistory[^1].move == EMove.STEAL)
            return new SMove(uid, 1, EMove.SKIP);

        int stealsUsed = recentSteals.Count(x => x);
        var splits = enemyHistory.Where(m => m.move == EMove.SPLIT).ToList();
        bool naiveRich = splits.Count >= 5
                         && enemyHistory.All(m => m.move != EMove.STEAL)
                         && splits.Average(m => m.bet) >= 80;

        if (naiveRich && stealsUsed < 2 && random.NextDouble() < 0.15)
            return new SMove(uid, 100, EMove.STEAL);

        // Иначе — честная сделка на 100.
        return new SMove(uid, 100, EMove.SPLIT);
    }

    public override void OnResult(string enemyUID, SMove myMove, SMove enemyMove, double score)
    {
        if (enemyMove.move == EMove.STEAL)
            toPunish.Add(enemyUID);

        recentSteals.Enqueue(myMove.move == EMove.STEAL);
        if (recentSteals.Count > 10) recentSteals.Dequeue();
    }
}