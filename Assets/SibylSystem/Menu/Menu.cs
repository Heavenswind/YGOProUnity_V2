using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class Menu : WindowServantSP
{
    //GameObject screen;
    public override void initialize()
    {
        string hint = File.ReadAllText("config/hint.conf");
        createWindow(Program.I().new_ui_menu);
        UIHelper.registEvent(gameObject, "setting_", OnClickSetting);
        UIHelper.registEvent(gameObject, "deck_", OnClickSelectDeck);
        UIHelper.registEvent(gameObject, "online_", OnClickOnline);
        UIHelper.registEvent(gameObject, "replay_", OnClickReplay);
        UIHelper.registEvent(gameObject, "single_", Program.gugugu);
        UIHelper.registEvent(gameObject, "ai_", Program.gugugu);
        UIHelper.registEvent(gameObject, "exit_", OnClickExit);
    }

    public override void show()
    {
        base.show();
        Program.charge();
    }

    public override void hide()
    {
        base.hide();
    }

    static int Version = 0;
    string upurl = "";

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        if (hashCode == "RMSshow_onlyYes")
        {
            Application.OpenURL(upurl);
        }
    }

    bool outed = false;
    public override void preFrameFunction()
    {
        base.preFrameFunction();
        if (upurl != "" && outed == false)
        {
            outed = true;
            RMSshow_onlyYes("RMSshow_onlyYes", InterString.Get("发现更新!@n你可以免费下载"), null);
        }
    }

    void OnClickExit()
    {
        Program.I().quit();
        Program.Running = false;
        TcpHelper.SaveRecord();
        Process.GetCurrentProcess().Kill();
    }

    void OnClickOnline()
    {
        Program.I().shiftToServant(Program.I().selectServer);
    }

    void OnClickAI()
    {
        Program.I().shiftToServant(Program.I().aiRoom);
    }

    void OnClickPuzzle()
    {
        Program.I().shiftToServant(Program.I().puzzleMode);
    }

    void OnClickReplay()
    {
        Program.I().shiftToServant(Program.I().selectReplay);
    }

    void OnClickSetting()
    {
        Program.I().setting.show();
    }

    void OnClickSelectDeck()
    {
        Program.I().shiftToServant(Program.I().selectDeck);
    }
}
