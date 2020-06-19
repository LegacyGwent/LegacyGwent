using System;
using System.Threading.Tasks;

namespace AITest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //AI战斗
            await Tools.AIBattle();

            //退出提示
            await Tools.ConfirmExit();
        }
    }
}
