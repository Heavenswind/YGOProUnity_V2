using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using YGOSharp.Network.Enums;

public class RoomList : WindowServantSP
{
    UIselectableList superScrollView = null;
    List<string[]> listOfRooms = new List<string[]>();
    bool hideStarted;
    UILabel roomPSWLabel;

    UIInput LP;
    UIInput ST;
    UIInput DR;
    UIInput TM;
    UIInput ID;

    UIToggle NC;
    UIToggle NS;
    UIToggle PR;

    UIPopupList NF;
    UIPopupList FMT;
    UIPopupList MR;
    UIPopupList MD;
    
    public override void initialize()
    {
        createWindow(Program.I().new_ui_RoomList);
        UIHelper.registEvent(gameObject, "exit_", OnClickExit);
        UIHelper.registEvent(gameObject, "refresh_", onRefresh);
        roomPSWLabel = UIHelper.getByName<UILabel>(gameObject, "roomNameLabel");
        hideStarted = UIHelper.getByName<UIToggle>(gameObject, "hideStarted_").value = UIHelper.fromStringToBool(Config.Get("hideStarted_", "1"));
        UIHelper.registEvent(gameObject, "hideStarted_", save);
        UIHelper.registEvent(gameObject, "normal_", NormalGame);
        UIHelper.registEvent(gameObject, "ranked_", RankedGame);
        UIHelper.registEvent(gameObject, "create_", MakeRoom);

        LP = UIHelper.getByName<UIInput>(gameObject, "lp_");
        ST = UIHelper.getByName<UIInput>(gameObject, "st_");
        DR = UIHelper.getByName<UIInput>(gameObject, "dr_");
        TM = UIHelper.getByName<UIInput>(gameObject, "tm_");
        ID = UIHelper.getByName<UIInput>(gameObject, "id_");

        NC = UIHelper.getByName<UIToggle>(gameObject, "NC");
        NS = UIHelper.getByName<UIToggle>(gameObject, "NS");
        PR = UIHelper.getByName<UIToggle>(gameObject, "PR");

        NF = UIHelper.getByName<UIPopupList>(gameObject, "NF");
        MR = UIHelper.getByName<UIPopupList>(gameObject, "MR");
        MD = UIHelper.getByName<UIPopupList>(gameObject, "MD");
        FMT = UIHelper.getByName<UIPopupList>(gameObject, "FMT");

        NF.Clear();
        foreach(YGOSharp.Banlist bl in YGOSharp.BanlistManager.Banlists)
            NF.AddItem(bl.Name);


        superScrollView = gameObject.GetComponentInChildren<UIselectableList>();
        superScrollView.selectedAction = OnSelected;
        superScrollView.install();
        SetActiveFalse();
    }

    void MakeRoom()
    {
        TcpHelper.CtosMessage_CreateGame(SetRulesPacket(CtosMessage.CreateGame));
    }

    public Package SetRulesPacket(CtosMessage kind)
    {
        Package message = new Package
        {
            Fuction = (int)kind
        };
        uint lflist = 0;
        byte region = 2;
        byte masterrule = 3;
        byte mode = 0;
        bool priority = PR.value;
        bool nocheckdeck = NC.value;
        bool noshuffledeck = NS.value;
        int lifepoints = 8000;
        byte starthand = 5;
        byte drawcount = 1;
        short timer = 300;
        int roomId = -1;

        int itmp;
        byte btmp;
        short stmp;

        if (int.TryParse(LP.value, out itmp))
            lifepoints = itmp;
        if (byte.TryParse(ST.value, out btmp))
            starthand = btmp;
        if (byte.TryParse(DR.value, out btmp))
            drawcount = btmp;
        if (short.TryParse(TM.value, out stmp))
            timer = stmp;
        if (int.TryParse(ID.value, out itmp))
            roomId = itmp;


        for(byte b = 0; b < MD.items.Count; b++)
        {
            if (MD.value == MD.items[b])
                mode = b;
        }

        for(byte b = 0; b < FMT.items.Count; b++)
        {
            if (FMT.value == FMT.items[b])
                region = b;
        }

        foreach(YGOSharp.Banlist bl in YGOSharp.BanlistManager.Banlists)
        {
            if(NF.value == bl.Name)
            {
                lflist = bl.Hash;
            }
        }

        for (byte b = 0; b < MR.items.Count; b++)
        {
            if (MR.value == MR.items[b])
                masterrule = (byte)(b + 3);
        }

        message.Data.writer.Write(roomId);
        message.Data.writer.Write(lflist);
        message.Data.writer.Write(region);
        message.Data.writer.Write(masterrule);
        message.Data.writer.Write(mode);
        message.Data.writer.Write(priority);
        message.Data.writer.Write(nocheckdeck);
        message.Data.writer.Write(noshuffledeck);
        message.Data.writer.Write(lifepoints);
        message.Data.writer.Write(starthand);
        message.Data.writer.Write(drawcount);
        message.Data.writer.Write(timer);

        return message;
    }

    private void NormalGame()
    {

    }

    private void RankedGame()
    {

    }

    void JoinRoom(string RoomID)
    {
        (new Thread(() => { TcpHelper.Join(RoomID); })).Start();
    }

    private void save()
    {
        hideStarted = UIHelper.getByName<UIToggle>(gameObject, "hideStarted_").value;
        Config.Set("hideStarted_", UIHelper.fromBoolToString(UIHelper.getByName<UIToggle>(gameObject, "hideStarted_").value));
    }

    private void onRefresh()
    {
        Program.I().selectServer.OnClickRoomList();
    }

    public void UpdateList(List<string[]> roomList)
    {
        show();
        listOfRooms.Clear();
        listOfRooms.AddRange(roomList);
        printFile();
    }
    public void OnClickExit()
    {
        Program.I().shiftToServant(Program.I().menu);
        if (TcpHelper.tcpClient != null)
        {
            if (TcpHelper.tcpClient.Connected)
            {
                TcpHelper.tcpClient.Close();
            }
        }
        hide();
    }
    public override void hide()
    {
        roomPSWLabel.text = "";
        base.hide();
    }

    private void printFile()
    {
        superScrollView.clear();
        superScrollView.toTop();
        if (hideStarted)
        {
            listOfRooms.RemoveAll(s => Convert.ToInt32(s[10]) != 0);
        }
        listOfRooms.TrimExcess();
        listOfRooms = listOfRooms.OrderBy(s => s[3]).ToList();
        foreach (string[] room in listOfRooms)
        {
            superScrollView.add(room[9]);
        }
    }

    string selectedString = string.Empty;
    void OnSelected()
    {
        if (!isShowed)
        {
            return;
        }
        string roomPsw;
        if (selectedString == superScrollView.selectedString)
        {
            roomPsw = listOfRooms.Find(s => s[9] == selectedString)[2];
            JoinRoom(roomPsw);
            return;
        }
        selectedString = superScrollView.selectedString;
        roomPsw = listOfRooms.Find(s => s[9] == selectedString)[2];
        if (roomPsw != null)
        {
            roomPSWLabel.text = roomPsw;
        }
        else
        {
            roomPSWLabel.text = "";
        }
    }
}