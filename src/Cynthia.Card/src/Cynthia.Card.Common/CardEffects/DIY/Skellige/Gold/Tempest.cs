using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70093")]//暴风雨Tempest
    public class Tempest : CardEffect
    {
        public Tempest(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var rowlist = new List<RowPosition>() { RowPosition.EnemyRow1, RowPosition.EnemyRow2, RowPosition.EnemyRow3, RowPosition.MyRow1, RowPosition.MyRow2, RowPosition.MyRow3 };
            var rainlist = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x => x.Value.RowStatus.IsHazard())
                .Select(x => x.Key);

            for(var i = 0; i<4;i++)
            {
                
                var result = await Game.GetSelectRow(Card.PlayerIndex, Card, rowlist);
               
                if(result == RowPosition.EnemyRow1 || result == RowPosition.EnemyRow2 || result == RowPosition.EnemyRow3)
                {
                    var rows = result.Mirror().MyRowToIndex();
                    if (Game.GameRowEffect[AnotherPlayer][rows].RowStatus == RowStatus.TorrentialRain)
                    {
                        await Game.GameRowEffect[AnotherPlayer][rows].SetStatus<SkelligeStormStatus>();
                    }
                    else
                    {
                        await Game.GameRowEffect[AnotherPlayer][rows].SetStatus<TorrentialRainStatus>();
                    }
                    
                }
                if(result == RowPosition.MyRow1 || result == RowPosition.MyRow2 || result == RowPosition.MyRow3)
                {
                    var rows = result.MyRowToIndex();
                    if(Game.GameRowEffect[PlayerIndex][rows].RowStatus == RowStatus.TorrentialRain)
                    {
                        await Game.GameRowEffect[PlayerIndex][rows].SetStatus<SkelligeStormStatus>();
                    }
                    else
                    {
                        await Game.GameRowEffect[PlayerIndex][rows].SetStatus<TorrentialRainStatus>();
                    }
                }
                rowlist.Remove(result);
            }
            
            return 0;
        }
    }
}
