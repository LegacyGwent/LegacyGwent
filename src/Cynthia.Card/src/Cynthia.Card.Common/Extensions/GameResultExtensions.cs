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
            if (gameResult.BlueWinCount > gameResult.RedWinCount)
            {
                return GameStatus.Win;
            }
            if (gameResult.BlueWinCount < gameResult.RedWinCount)
            {
                return GameStatus.Lose;
            }
            return GameStatus.Draw;
        }

        public static bool IsEffective(this GameResult gameResult)
        {
            if (gameResult.RedWinCount != 2 && gameResult.BlueWinCount != 2)
            {
                return false;
            }
            return true;
        }

        public static GameStatus RedPlayerStatus(this GameResult gameResult)
        {
            if (gameResult.BlueWinCount < gameResult.RedWinCount)
            {
                return GameStatus.Win;
            }
            if (gameResult.BlueWinCount > gameResult.RedWinCount)
            {
                return GameStatus.Lose;
            }
            return GameStatus.Draw;
        }
    }
}