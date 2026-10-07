using System;
using System.Collections.Generic;

[Task(2)]
public class BuravnyovSociety() : Player(
    traits: new STraits(0, 5, 0, 5, 0),
    "SocietyCoop",
    "Буравнёв Игорь Евгеньевич")
{
    private readonly HashSet<string> toPunish = new();

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
        {
            toPunish.Remove(enemyUID);
            return new SMove(uid, 1, EMove.SKIP);
        }

        if (enemyHistory.Count > 0 && enemyHistory[^1].move == EMove.STEAL)
            return new SMove(uid, 1, EMove.SKIP);

        return new SMove(uid, 100, EMove.SPLIT);
    }

    public override void OnResult(string enemyUID, SMove myMove, SMove enemyMove, double score)
    {
        if (enemyMove.move == EMove.STEAL)
            toPunish.Add(enemyUID);
    }
}