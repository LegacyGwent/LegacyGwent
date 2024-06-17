using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card.Common.CardEffects.Neutral.Derive
{
    public abstract class Choosespell3 : CardEffect
    {
        protected Choosespell3(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardUseEffect()
        {
            InitDict();
            var list = baseChoiceList.ToList();
            var switchCard = await Card.GetMenuSwitch
            (
                (string.Empty, methodDesDict[list[0]]),
                (string.Empty, methodDesDict[list[1]]),
                (string.Empty, methodDesDict[list[2]])
            );
            return await UseMethodByChoice(list[switchCard]);
        }

        protected abstract Task<int> UseMethodByChoice(int switchCard);

        private List<int> baseChoiceList = new List<int> {1, 2, 3};

        protected Dictionary<int, string> methodDesDict;

        private void InitDict()
        {
            if (methodDesDict == null)
            {
                RealInitDict();
            }
        }

        protected abstract void RealInitDict();
    }
}