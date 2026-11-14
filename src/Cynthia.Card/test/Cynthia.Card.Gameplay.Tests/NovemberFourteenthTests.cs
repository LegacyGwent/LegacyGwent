using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace Cynthia.Card.Gameplay.Tests
{
 public class NovemberFourteenthTests
 {
  [Fact]
  public async Task OffboardCountdownPublishesFreshCemeteryAndPrivateDeckSnapshots()
  {
   var f=new HeadlessGameFixture();
   var c=f.AddCard(f.Game.Player1Index,CardId.Cerys,RowPosition.MyCemetery);
   await f.SynchronizeClientsAsync();
   await f.Game.SetCemeteryInfo();
   var before=f.FirstPlayer.ReceivedSnapshots.Last(x=>x.Type==ServerOperationType.SetMyCemetery).Json;
   f.FirstPlayer.ReceivedSnapshots.Clear();f.SecondPlayer.ReceivedSnapshots.Clear();
   await c.Effect.SetCountdown(value:3);
   var own=f.FirstPlayer.ReceivedSnapshots.Last(x=>x.Type==ServerOperationType.SetMyCemetery).Json;
   var enemy=f.SecondPlayer.ReceivedSnapshots.Last(x=>x.Type==ServerOperationType.SetEnemyCemetery).Json;
   Assert.Contains("\"Countdown\":4",before);
   Assert.Contains("\"Countdown\":3",own);Assert.Equal(own,enemy);
   var d=f.AddCard(f.Game.Player1Index,CardId.Cerys,RowPosition.MyDeck);
   f.FirstPlayer.ReceivedSnapshots.Clear();f.SecondPlayer.ReceivedSnapshots.Clear();
   await d.Effect.SetCountdown(value:2);
   Assert.Contains(f.FirstPlayer.ReceivedSnapshots,x=>x.Type==ServerOperationType.SetMyDeck && x.Json.Contains("\"Countdown\":2"));
   Assert.DoesNotContain(f.SecondPlayer.ReceivedSnapshots,x=>x.Type==ServerOperationType.SetMyDeck || x.Type==ServerOperationType.SetEnemyDeck);
  }
  [Fact]
  public async Task CerysWaitsAtZeroWhenBoardFullThenRetriesOnNextResurrection()
  {
   var f=new HeadlessGameFixture();
   var c=f.AddCard(f.Game.Player1Index,CardId.Cerys,RowPosition.MyCemetery);
   c.Status.Countdown=1;
   GameCard blocker=null;
   foreach(var row in new[]{RowPosition.MyRow1,RowPosition.MyRow2,RowPosition.MyRow3})
    for(var n=0;n<f.Game.RowMaxCount;n++) blocker=f.AddCard(f.Game.Player1Index,CardId.Wolf,row);
   var bone=f.AddCard(f.Game.Player1Index,CardId.BoneTalisman,RowPosition.MyStay);
   f.AddCard(f.Game.Player1Index,CardId.Wolf,RowPosition.MyCemetery);
   await f.SynchronizeClientsAsync();
   await f.Game.AddTask(async()=>{await bone.Effect.CardUseEffect();});
   Assert.Equal(0,c.Status.Countdown);
   Assert.Equal(RowPosition.MyCemetery,c.Status.CardRow);
   await blocker.Effect.Banish();
   f.AddCard(f.Game.Player1Index,CardId.Wolf,RowPosition.MyCemetery);
   await f.SynchronizeClientsAsync();
   await f.Game.AddTask(async()=>{await bone.Effect.CardUseEffect();});
   Assert.True(c.Status.CardRow.IsOnPlace());
   Assert.Equal(4,c.Status.Countdown);
  }
  [Theory]
  [InlineData(false, 4)]
  [InlineData(true, 1)]
  public async Task CerysFinalResurrectionTickRespectsLock(bool locked, int expectedCount)
  {
   var f=new HeadlessGameFixture();
   var c=f.AddCard(f.Game.Player1Index,CardId.Cerys,RowPosition.MyCemetery);
   c.Status.Countdown=1;c.Status.IsLock=locked;
   var bone=f.AddCard(f.Game.Player1Index,CardId.BoneTalisman,RowPosition.MyStay);
   f.AddCard(f.Game.Player1Index,CardId.Wolf,RowPosition.MyCemetery);
   await f.SynchronizeClientsAsync();
   await f.Game.AddTask(async()=>{await bone.Effect.CardUseEffect();});
   Assert.Equal(expectedCount,c.Status.Countdown);
   Assert.Equal(!locked,c.Status.CardRow.IsOnPlace());
   Assert.Equal(GwentMap.CardMap[CardId.Cerys].Strength+(locked?0:1),c.Status.Strength);
  }
  [Fact]
  public async Task BoneTalismanResurrectionReducesCerysCountdown()
  {
   var f=new HeadlessGameFixture();
   var c=f.AddCard(f.Game.Player1Index,CardId.Cerys,RowPosition.MyCemetery);
   var bone=f.AddCard(f.Game.Player1Index,CardId.BoneTalisman,RowPosition.MyStay);
   var beast=f.AddCard(f.Game.Player1Index,CardId.Wolf,RowPosition.MyCemetery);
   await f.SynchronizeClientsAsync();
   await f.Game.AddTask(async()=>{await bone.Effect.CardUseEffect();});
   Assert.Equal(3,c.Status.Countdown);
   Assert.Equal(RowPosition.MyStay,beast.Status.CardRow);
  }
  [Fact]
  public async Task MatronBoostsOneChosenHandUnitByTotalStrengtheningIncludingSpy()
  {
   var f=new HeadlessGameFixture();
   var matron=f.AddCard(f.Game.Player1Index,CardId.DryadMatron,RowPosition.MyRow1);
   var first=f.AddCard(f.Game.Player1Index,CardId.Braenn,RowPosition.MyRow1);
   var second=f.AddCard(f.Game.Player1Index,CardId.BowDryad,RowPosition.MyRow1);
   var hand=f.AddCard(f.Game.Player1Index,CardId.Wolf,RowPosition.MyHand);
   hand.Status.IsSpying=true;
   await f.SynchronizeClientsAsync();
   await matron.Effect.CardPlayEffect(false,false);
   Assert.Equal(2,hand.Status.HealthStatus);
   Assert.Equal(GwentMap.CardMap[CardId.Braenn].Strength+1,first.Status.Strength);
   Assert.Single(f.FirstPlayer.MenuRequests);
  }
  [Fact]
  public async Task MaidenIgnoresStrongerSameNameAndCopiesOriginalNotCurrentState()
  {
   var f=new HeadlessGameFixture();
   var girl=f.AddCard(f.Game.Player1Index,CardId.GirlWhoDrankBrokilonWater,RowPosition.MyRow1);
   var copy=f.AddCard(f.Game.Player1Index,CardId.GirlWhoDrankBrokilonWater,RowPosition.MyRow1,99);
   copy.Status.IsLock=true;
   var dryad=f.AddCard(f.Game.Player1Index,CardId.DryadMatron,RowPosition.MyRow1,30);
   dryad.Status.HealthStatus=20;dryad.Status.IsLock=true;
   await f.SynchronizeClientsAsync();
   await f.Game.SendEvent(new AfterTurnStart(f.Game.Player2Index));
   Assert.Equal(CardId.DryadMatron,girl.Status.CardId);
   Assert.Equal(GwentMap.CardMap[CardId.DryadMatron].Strength,girl.Status.Strength);
   Assert.Equal(0,girl.Status.HealthStatus);
   Assert.False(girl.Status.IsLock);
  }
 }
}
