<%@ Page Title="��Z��x" Language="C#" MasterPageFile="../MasterPage.master"
AutoEventWireup="true" Debug="true" MaintainScrollPositionOnPostback="true"
CodeFile="diary.aspx.cs" Inherits="Diary_diary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content
  ID="Content2"
  ContentPlaceHolderID="ContentPlaceHolder1"
  runat="Server"
>
  <div id="divBody" align="center" style="width: 100%; overflow: auto">
    <% MainDiary(); //��Z��x MainSave(); //��Z��� MainTrace(); //�l�ܤ��i %>
  </div>
  <script type="text/javascript">
    var qryArea = "Diary",
      qryTbl,
      RowNo = 0,
      qryTour,
      DiaryNo,
      MsgNo,
      MsgDT,
      ProcessNo,
      MsgCode,
      FontColor;

    function trClick(
      tmpArea,
      tmpQryTbl,
      tmpNo,
      tmpQryTour,
      tmpDiaryNo,
      tmpMsgNo,
      tmpMsgDT,
      tmpProcessNo,
      tmpMsgCode,
      tmpColor
    ) {
      //�I��C
      FontColor = "";
      document.getElementById("tbl" + qryArea).rows[RowNo].style.background =
        FontColor;
      FontColor = tmpColor;
      document.getElementById("tbl" + tmpArea).rows[tmpNo].style.background =
        "white";

      qryArea = tmpArea;
      qryTbl = tmpQryTbl;
      RowNo = tmpNo;
      qryTour = tmpQryTour;
      DiaryNo = tmpDiaryNo;
      MsgNo = tmpMsgNo;
      MsgDT = tmpMsgDT;
      ProcessNo = tmpProcessNo;
      MsgCode = tmpMsgCode;
    }

    function TourClick(qryTour) {
      //����qryTour��Z����x
      window.open("Diary.aspx?tour=" + qryTour, "_self");
    }

    function MsgDT_Click(qryTour, MsgNo, MsgCode, qryMsgDT) {
      var MsgDT = "";
      MsgDT = prompt(
        " �п�J�n�ק諸�T���ɶ��A�榡���Gyyyy/mm/dd hh:mi",
        qryMsgDT
      ).toString();
      if (MsgDT != "")
        window.open(
          "Diary.aspx?DBaction=t&qryTour=" +
            qryTour +
            "&MsgNo=" +
            MsgNo +
            "&MsgDT=" +
            MsgDT,
          "_self"
        );
    }

    function Cmd_KeyDown(KeyCode) {
      //�s�W�R������
      if (KeyCode == 45) Cmd_Click(qryArea, "a"); //Ins
      else if (KeyCode == 46) Cmd_Click(qryArea, "d"); //Del
    }

    function trDblClick() {
      //�����C�ק�
      Cmd_Click(qryArea, "u");
    }

    function Cmd_Click(tmpArea, DBaction) {
      //�s�W�R���ק�
      if (qryArea != tmpArea)
        alert("�Х��I��z�����ʤ��O�� !"); //�惡qryArea�o�I��qryArea��Cmd
      else {
        var tmpText = "";
        if (document.getElementById("tbl" + qryArea).textContent != undefined)
          tmpText = document.getElementById("tbl" + qryArea).rows[RowNo]
            .cells[2].textContent;
        else
          tmpText = document
            .getElementById("tbl" + qryArea)
            .rows[RowNo].cells(2).innerText;

        switch (DBaction) {
          case "a":
          case "u": {
            if ((qryTbl == "Msg") & (DBaction == "a"))
              window.open(
                "Process.aspx?qryTour=" + qryTour + "&DiaryNo=" + DiaryNo,
                "_self"
              );
            //�s�WMsg
            else {
              if ((qryTbl == "Msg") & (DBaction == "u"))
                MsgDT_Click(qryTour, MsgNo, MsgCode, MsgDT); //�ק�Msg
              //�s�W�έק�Process
              else {
                if (DBaction == "a")
                  window.open(
                    "Process.aspx?qryTour=" + qryTour + "&DiaryNo=" + DiaryNo,
                    "_self"
                  );
                else
                  window.open(
                    "Process.aspx?qryTour=" +
                      qryTour +
                      "&DiaryNo=" +
                      DiaryNo +
                      "&ProcessNo=" +
                      ProcessNo,
                    "_self"
                  );
              }
            }

            break;
          }
          case "d": {
            //�R��Msg��Process
            if (
              confirm(
                "<<�`�N>>�I�R���ӵ���x��̫�Y�u�Ѥ@���o�ͰT���γB�z�L�{����A�㵧��x�O���A�]�t�Ҧ��ݩʳ]�w�A�o�ͰT���P�B�z�L�{�A���N�@�֧R���I��ĳ�H���s�W�A�R�����ާ@�覡�A�i�H�קK�~�R��ơC\n\n�z�T�w�R���U�C��ơH\n\n " +
                  tmpText
              )
            ) {
              if (qryTbl == "Msg")
                window.open(
                  "Diary.aspx?DBaction=d&qryTour=" +
                    qryTour +
                    "&DiaryNo=" +
                    DiaryNo +
                    "&MsgNo=" +
                    MsgNo,
                  "_self"
                );
              if (qryTbl == "Process")
                window.open(
                  "Diary.aspx?DBaction=d&qryTour=" +
                    qryTour +
                    "&DiaryNo=" +
                    DiaryNo +
                    "&ProcessNo=" +
                    ProcessNo,
                  "_self"
                );
            }

            break;
          }
        }
      }
    }
  </script>
</asp:Content>
