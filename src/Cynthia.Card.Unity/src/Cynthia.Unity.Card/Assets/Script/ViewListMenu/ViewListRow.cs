using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Autofac;
using Cynthia.Card.Client;
using Microsoft.AspNetCore.SignalR.Client;

public class ViewListRow : MonoBehaviour
{
    public Text RoomNumber;
    public Text RoomName;
    public Button ViewButton;
    private string RoomId;

    public void SetViewListRow(int roomNumber, string roomName, string roomId)
    {
        RoomNumber.text = "Room " + roomNumber;
        RoomName.text = roomName;
        RoomId = roomId;
    }
    public void SetRViewListRowString(string roomNumber, string roomName, string roomId)
    {
        RoomNumber.text = "Room " + roomNumber;
        RoomName.text = roomName;
        RoomId = roomId;
    }
    public void SetTransparency()
    {
        RoomNumber.color = Color.clear;
        RoomName.color = Color.clear;
        ViewButton.image.color = Color.clear;
        ViewButton.GetComponentInChildren<Text>().color = Color.clear;
        ViewButton.interactable = false;
    }
    public async void OnClickViewButton()
    {
        if (await DependencyResolver.Container.Resolve<GwentClientService>().HubConnection.InvokeAsync<bool>("JoinViewList", RoomId))
        {
            ClientGlobalInfo.ViewingRoomId = RoomId;
            SceneManager.LoadScene("GamePlay");
        }
    }
}
