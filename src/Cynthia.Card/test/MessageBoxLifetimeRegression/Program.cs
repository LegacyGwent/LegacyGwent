using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
var box = new MessageBox
{
    TitleText = new Text(), MessageText = new Text(), YesText = new Text(), NoText = new Text(),
    YesButton = new GameObject(), NoButton = new GameObject(), Buttons = new GameObject(), Context = new RectTransform()
};
typeof(MessageBox).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(box, null);

var first = box.Show("confirm A", "A");
var second = box.Show("confirm B", "B");
Check(!await first && !second.IsCompleted, "Replacing a confirmation affirmed it or completed the new owner");
box.YesClick();
Check(await second && !box.gameObject.activeSelf, "Current confirmation did not own its click");
Console.WriteLine("PASS: overlapping confirmations decline old owner and click only resolves current owner");

var quitConfirmation = box.Show("quit", "quit?");
var quit = false;
var quitOwner = Task.Run(async () => { if (await quitConfirmation) quit = true; });
box.ShowAutoClose("notice", "opponent disconnected", 1);
await quitOwner.WaitAsync(TimeSpan.FromSeconds(2));
Check(!quit && !await quitConfirmation && !box.Buttons.activeSelf, "Notification silently confirmed an existing action");
var oldCountdown = box.Coroutines.Last();
var replacement = box.Show("replacement", "new?");
Check(!oldCountdown.MoveNext() && box.gameObject.activeSelf && !replacement.IsCompleted,
    "Old notification countdown closed the replacement dialog");
box.NoClick();
Check(!await replacement, "Notification's prior waiter stole the replacement click");
Console.WriteLine("PASS: notification never authorizes a pending action or lets old countdown/waiter affect replacement");

var waiting = box.Show("confirm", "before wait");
box.Wait("wait", "loading");
Check(!await waiting, "Wait replacement retained a confirmation");
box.Close();
var closed = box.Show("confirm", "before close");
box.Close();
Check(!await closed, "Close retained a confirmation");
var disabled = box.Show("confirm", "before disable");
typeof(MessageBox).GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(box, null);
Check(!await disabled, "Scene disable retained a confirmation");
box.YesClick();
var afterStrayClick = box.Show("confirm", "after stray click");
Check(!afterStrayClick.IsCompleted, "A click without an owner leaked into the next confirmation");
box.NoClick();
Check(!await afterStrayClick, "Fresh confirmation did not receive own No click");
Console.WriteLine("PASS: wait/close/scene disable expire answers; stray clicks cannot leak into a fresh display");
Console.WriteLine("Unity APIs are stubs; production MessageBox lifecycle executed, native layout/timer rendering unverified.");
