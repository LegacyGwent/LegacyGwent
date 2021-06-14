using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("130210")]//杜度
	public class DuduPro : CardEffect
	{//选择一个单位，变为它的复制。
		public DuduPro(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			var result = await Game.GetSelectPlaceCards(Card);
            if (!result.TrySingle(out var target))
            {
                return 0;
            }
			//var targrt = result.Single();
            await Card.Effect.Transform(target.CardInfo().CardId ,Card,isForce:true);
			//锁定
			if(target.Status.IsLock)
			{
				await Card.Effect.Lock(Card);
			}
			//倒数
			if(target.Status.IsCountdown)
			{
				await Card.Effect.SetCountdown(target.Status.Countdown);
			}
			//白字
			if(target.Status.Strength<Card.Status.Strength)
            {
                await Card.Effect.Weaken(Card.Status.Strength-target.Status.Strength, Card);//削弱
            }
            else if(target.Status.Strength>Card.Status.Strength)
            {
                await Card.Effect.Strengthen(target.Status.Strength-Card.Status.Strength, Card);//强化
            }
			//绿字，红字
			if(target.CardPoint()<Card.CardPoint())
            {
                await Card.Effect.Damage(Card.CardPoint()-target.CardPoint(), Card, BulletType.RedLight, true);//造成穿透伤害
            }
            else if(target.CardPoint()>Card.CardPoint())
            {
                await Card.Effect.Boost(target.CardPoint()-Card.CardPoint(), Card);//增益
            }
			//护甲
			if(target.Status.Armor>0)
			{
				await Card.Effect.Armor(target.Status.Armor, Card);
			}
			//间谍
			if(target.Status.IsSpying != Card.Status.IsSpying)
			{
				await Card.Effect.Spying(Card);
			}
			//护盾
			if(target.Status.IsShield)
			{
				Card.Status.IsShield = true;
			}
			//佚亡
			if(target.Status.IsDoomed)
			{
				Card.Status.IsDoomed = true;
			}
			//坚韧
			if(target.Status.IsResilience)
			{
				Card.Status.IsResilience = true;
			}
			return 0;
		}
	}
}