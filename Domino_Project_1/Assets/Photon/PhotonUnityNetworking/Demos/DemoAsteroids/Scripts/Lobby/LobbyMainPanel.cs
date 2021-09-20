using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Asteroids
{
    public class LobbyMainPanel : MonoBehaviourPunCallbacks
    {
        [Header("Login Panel")]
        public GameObject LoginPanel;

        public InputField PlayerNameInput;
        public Text LoginType;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;

        public GameObject GetLoginType;

        public InputField RoomNameInputField;
        public InputField MaxPlayersInputField;

        //[Header("Join Random Room Panel")]
        //public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;
        public GameObject DifferentLoginPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Instructions Panel")]
        public GameObject InstructionsPanel;

        [Header("Pieces Menu Panel")]
        public GameObject PiecesMenuPanel;

        [Header("About Panel")]
        public GameObject AboutPanel;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;

                private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;

        #region UNITY

        public void Awake()
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListEntries = new Dictionary<string, GameObject>();
            
            PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();

            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        /*public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000) + LoginType.text;

            RoomOptions options = new RoomOptions {MaxPlayers = 4};

            PhotonNetwork.CreateRoom(roomName, options, null);
        }*/

        public string RemoveLoginCode(string name)
        {
            int found = name.IndexOf("[");
            name = name.Substring(0, found);

            return name;
        }

        public string ExtractLoginCode(string name)
        {
            int found = name.IndexOf("[");
            name = name.Substring(found + 1, name.Length - 2 - found);

            return name;
        }

        public override void OnJoinedRoom()
        {
            SetActivePanel(InsideRoomPanel.name);

            if(ExtractLoginCode(PhotonNetwork.CurrentRoom.Name) != ExtractLoginCode(PhotonNetwork.LocalPlayer.NickName))
            {
                PhotonNetwork.LeaveRoom();
                DifferentLoginPanel.SetActive(true);
                return;
            }

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
                entry.transform.SetParent(InsideRoomPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, RemoveLoginCode(p.NickName));

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
                }

                if (playerListEntries.ContainsKey(p.ActorNumber))
                {
                    foreach (GameObject players in playerListEntries.Values)
                    {
                        Destroy(players.gameObject);
                    }

                    playerListEntries.Clear();
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (ExtractLoginCode(PhotonNetwork.CurrentRoom.Name) != ExtractLoginCode(PhotonNetwork.LocalPlayer.NickName))
            {
                PhotonNetwork.LeaveRoom();
                DifferentLoginPanel.SetActive(true);
                return;
            }

            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber,RemoveLoginCode(newPlayer.NickName));

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if(otherPlayer.IsMasterClient)
            {
                foreach (GameObject entry in playerListEntries.Values)
                {
                    Destroy(entry.gameObject);
                }

                playerListEntries.Clear();
            }
            else
            {
                if (playerListEntries.ContainsKey(otherPlayer.ActorNumber))
                {
                    Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
                    playerListEntries.Remove(otherPlayer.ActorNumber);
                }

                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool) isPlayerReady);
                }
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
        }

        public void OnBackInstructionsOrPiecesMenuButtonClicked()
        {

            SetActivePanel(LoginPanel.name);
        }

        public void OnCreateRoomButtonClicked()
        {
            int found = RoomNameInputField.text.IndexOf("[");

            string roomName = RoomNameInputField.text;

            if (found == -1)
            {
                roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

                byte maxPlayers;
                byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
                maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 4);

                RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

                PhotonNetwork.CreateRoom(roomName + LoginType.text, options, null);
            }
            else
            {
                Debug.LogError("Room Name is invalid.");
            }
            
        }

        /*public void OnJoinRandomRoomButtonClicked()
        {
            SetActivePanel(JoinRandomRoomPanel.name);

            PhotonNetwork.JoinRandomRoom();
        }*/

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void OnLoginButtonClicked()
        {
            int found = PlayerNameInput.text.IndexOf("[");
            string playerName = PlayerNameInput.text + LoginType.text;

            if (!playerName.Equals("") && found == -1)
            {
                PhotonNetwork.LocalPlayer.NickName = playerName;
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
        }

        public void OnInstructionsButtonClicked()
        {
            SetActivePanel(InstructionsPanel.name);
        }

        public void OnPiecesMenuButtonClicked()
        {
            SetActivePanel(PiecesMenuPanel.name);
        }

        public void OnAboutButtonClicked()
        {
            SetActivePanel(AboutPanel.name);
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("Domino - GameScene");
        }

        #endregion

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            int count = 0;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                count++;
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool) isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            if (count < 2)
                return false;

            return true;
        }
        
        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            //JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InstructionsPanel.SetActive(activePanel.Equals(InstructionsPanel.name));
            PiecesMenuPanel.SetActive(activePanel.Equals(PiecesMenuPanel.name));
            AboutPanel.SetActive(activePanel.Equals(AboutPanel.name));
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(info.Name))
                    {
                        cachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                // Update cached room info
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    cachedRoomList.Add(info.Name, info);
                }
            }
        }

        private void UpdateRoomListView()
        {
            foreach (RoomInfo info in cachedRoomList.Values)
            {
                GameObject entry = Instantiate(RoomListEntryPrefab);
                entry.transform.SetParent(RoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }
    }
}