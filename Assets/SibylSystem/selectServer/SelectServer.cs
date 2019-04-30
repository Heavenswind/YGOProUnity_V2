using System.Threading;

public class SelectServer : WindowServantSP
{
    UIPopupList serversList;

    UIInput inputPsw;

    public override void initialize()
    {
        createWindow(Program.I().new_ui_selectServer);
        UIHelper.registEvent(gameObject, "exit_", OnClickExit);
        UIHelper.registEvent(gameObject, "face_", OnClickFace);
        UIHelper.registEvent(gameObject, "join_", OnLogin);
        serversList = UIHelper.getByName<UIPopupList>(gameObject, "server");
        serversList.fontSize = 20;
        serversList.value = Config.Get("serversPicker", "Chronos");
        UIHelper.registEvent(gameObject, "server", PickServer);
        UIHelper.getByName<UIInput>(gameObject, "name_").value = Config.Get("name", "");
        UIHelper.getByName<UIInput>(gameObject, "name_").defaultText = "";
        name = Config.Get("name", "");
        inputPsw = UIHelper.getByName<UIInput>(gameObject, "psw_");
        inputPsw.defaultText = "";
        SetActiveFalse();
    }

    public static string ServerIP = "chronos.tk";
    public static string ServerPort = "7911";

    private void PickServer()
    {
        string server = serversList.value;
        switch (server)
        {
            case "Chronos":
                {
                    ServerIP = "chronos.tk";
                    ServerPort = "7911";
                    Config.Set("serversPicker", "Chronos");
                    break;
                }
            default:
                Config.Set("serversPicker", "Chronos");
                break;

        }

    }

    public void OnLogin()
    {
        string username = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        string password = UIHelper.getByName<UIInput>(gameObject, "psw_").value;
        TcpHelper.Authenticate(username, password);
    }

    public void OnClickRoomList()
    {
        if (!isShowed)
        {
            return;
        }
        TcpHelper.roomListChecking = true;
        TcpHelper.CtosMessage_RoomList();
    }

    public override void show()
    {
        base.show();
        Program.I().room.RMSshow_clear();
        Program.charge();
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
    }

    void OnClickExit()
    {
        Program.I().shiftToServant(Program.I().menu);
        if (TcpHelper.tcpClient != null)
        {
            if (TcpHelper.tcpClient.Connected)
            {
                TcpHelper.tcpClient.Close();
            }
        }
    }

    public void KF_onlineGame(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            RMSshow_onlyYes("", InterString.Get("昵称不能为空。"), null);
            return;
        }
        name = username;
        Config.Set("name", username);
        (new Thread(() => { TcpHelper.Join("0"); })).Start();
    }

    public string name = "";

    void OnClickFace()
    {
        name = UIHelper.getByName<UIInput>(gameObject, "name_").value;
        RMSshow_face("showFace", name);
        Config.Set("name", name);
    }

}
