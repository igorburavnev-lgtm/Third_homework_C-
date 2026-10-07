using System;
using System.Collections.Generic;

[Task(1)]
public class BuravnyovSelfish() : Player(
    traits: new STraits(0, 4, 2, 4, 0), // def 4, luck 2, charm 4 — как у топ-ботов
    "SelfishTFT",
    "Буравнёв Игорь Евгеньевич")
{
    private readonly HashSet<string> toPunish = new();

    public override SMove MakeMove(string enemyUID, IReadOnlyList<SMove> enemyHistory, STraits enemyTraits)
    {
        // Классический Tit-for-Tat с наказанием и злопамятностью.
        //   1. Если этот соперник крал у нас ЛИЧНО — SKIP навсегда.
        //   2. Если его последний публичный ход — STEAL — SKIP (наказание).
        //   3. Первая встреча — SPLIT 50.
        //   4. Он поделился — зеркалим ставку.
        //   5. Он SKIP — отвечаем SPLIT 50 (показываем готовность).
        if (toPunish.Contains(enemyUID))
        {
            toPunish.Remove(enemyUID);
            return new SMove(uid, 1, EMove.SKIP);
        }

        if (enemyHistory.Count == 0)
            return new SMove(uid, 50, EMove.SPLIT);

        var last = enemyHistory[^1];
        if (last.move == EMove.STEAL)
            return new SMove(uid, 1, EMove.SKIP);

        if (last.move == EMove.SPLIT)
            return new SMove(uid, Math.Clamp(last.bet, 1, 100), EMove.SPLIT);

        return new SMove(uid, 50, EMove.SPLIT);
    }

    public override void OnResult(string enemyUID, SMove myMove, SMove enemyMove, double score)
    {
        if (enemyMove.move == EMove.STEAL)
            toPunish.Add(enemyUID);
    }
}