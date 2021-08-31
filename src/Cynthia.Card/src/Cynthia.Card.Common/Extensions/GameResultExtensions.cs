using System;
using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card
{
    public static class GameResultExtensions
    {
        public static GameStatus BluePlayerStatus(this GameResult gameResult)
        {
            if (!gameResult.isSurrender && gameResult.BlueWinCount == gameResult.RedWinCount)
            {
                return GameStatus.Draw;
            }
            if (gameResult.RedPlayerGameResultStatus == GameStatus.Win)
            {
                return GameStatus.Lose;
            }
            else
            {
                return GameStatus.Win;
            }
        }

        public static bool IsEffective(this GameResult gameResult)
        {
            if (!gameResult.isSurrender && gameResult.RedWinCount != 2 && gameResult.BlueWinCount != 2)
            {
                return false;
            }
            return true;
        }

        public static GameStatus RedPlayerStatus(this GameResult gameResult)
        {
            if (!gameResult.isSurrender && gameResult.BlueWinCount == gameResult.RedWinCount)
            {
                return GameStatus.Draw;
            }
            return gameResult.RedPlayerGameResultStatus;
        }
    }
}