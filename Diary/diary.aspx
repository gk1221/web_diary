<%@ Page Title="當班日誌" Language="C#" MasterPageFile="../MasterPage.master" AutoEventWireup="true"
    Debug="true" MaintainScrollPositionOnPostback="true" CodeFile="diary.aspx.cs"
    Inherits="Diary_diary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div id="divBody" align="center" style="width: 100%; overflow: auto;">
        <% 
            MainDiary();    //當班日誌
            MainSave();     //當班更改
            MainTrace();    //追蹤公告
        %>
    </div>
    <script type="text/javascript">
        var qryArea="Diary",qryTbl,RowNo=0,qryTour,DiaryNo,MsgNo,MsgDT,ProcessNo,MsgCode,FontColor ;

        function trClick(tmpArea,tmpQryTbl,tmpNo,tmpQryTour,tmpDiaryNo,tmpMsgNo,tmpMsgDT,tmpProcessNo,tmpMsgCode,tmpColor)	//點選列 
        {
            FontColor="";                        
            document.getElementById("tbl" + qryArea).rows[RowNo].style.background = FontColor;
            FontColor = tmpColor;
            document.getElementById("tbl" + tmpArea).rows[tmpNo].style.background = "white";

            qryArea=tmpArea ; qryTbl=tmpQryTbl ; RowNo=tmpNo ; qryTour=tmpQryTour ; DiaryNo=tmpDiaryNo ; MsgNo=tmpMsgNo ; MsgDT=tmpMsgDT ; ProcessNo=tmpProcessNo ; MsgCode=tmpMsgCode ;            
        }

        function TourClick(qryTour) //移到qryTour當班的日誌
        {
            window.open("Diary.aspx?tour=" + qryTour, "_self");
        } 

        function MsgDT_Click(qryTour,MsgNo,MsgCode,qryMsgDT)
        {
            var MsgDT="";
            MsgDT = prompt("請輸入要修改的訊息時間，格式為：yyyy/mm/dd hh:mi", qryMsgDT).toString();            
            if(MsgDT != "") window.open("Diary.aspx?DBaction=t&qryTour=" + qryTour + "&MsgNo=" + MsgNo + "&MsgDT=" + MsgDT,"_self");
        }
        
        function Cmd_KeyDown(KeyCode)    //新增刪除按鍵
        {
            if (KeyCode == 45) Cmd_Click(qryArea,"a");	//Ins
            else if (KeyCode == 46) Cmd_Click(qryArea,"d")	//Del
        }       

        function trDblClick()	//雙擊列修改 
        {
            Cmd_Click(qryArea,"u") ;            
        } 

        function Cmd_Click(tmpArea,DBaction)	//新增刪除修改
        {
            if(qryArea != tmpArea) alert("請先點選您欲異動之記錄 !");	//選此qryArea卻點彼qryArea的Cmd
            else
            {
                var tmpText = "";                
                if (document.getElementById("tbl" + qryArea).textContent != undefined) tmpText = document.getElementById("tbl" + qryArea).rows[RowNo].cells[2].textContent;
                else tmpText = document.getElementById("tbl" + qryArea).rows[RowNo].cells(2).innerText;                
                
                switch(DBaction)
                {
                    case "a": case "u":
                    {
                        if (qryTbl=="Msg" & DBaction=="a") window.open("Process.aspx?qryTour=" + qryTour + "&DiaryNo=" + DiaryNo,"_self");  //新增Msg                            
                        else
                        {				 
                            if (qryTbl=="Msg" & DBaction=="u") MsgDT_Click(qryTour,MsgNo,MsgCode,MsgDT);	//修改Msg                                
                            else 	//新增或修改Process
                            {
                                if (DBaction=="a") window.open("Process.aspx?qryTour=" + qryTour + "&DiaryNo=" + DiaryNo,"_self");
                                else window.open("Process.aspx?qryTour=" + qryTour + "&DiaryNo=" + DiaryNo + "&ProcessNo=" + ProcessNo,"_self");
                            }
                        }
                     
                        break;
                    }
                case "d": //刪除Msg或Process   
                    {
                        if (confirm("<<注意>>！刪除該筆日誌到最後若只剩一筆發生訊息或處理過程之後，整筆日誌記錄，包含所有屬性設定，發生訊息與處理過程，均將一併刪除！建議以先新增再刪除的操作方式，可以避免誤刪資料。\n\n您確定刪除下列資料？\n\n " + tmpText)) 
                        {
                            if (qryTbl == "Msg") window.open("Diary.aspx?DBaction=d&qryTour=" + qryTour + "&DiaryNo=" + DiaryNo + "&MsgNo=" + MsgNo, "_self");
                            if (qryTbl == "Process") window.open("Diary.aspx?DBaction=d&qryTour=" + qryTour + "&DiaryNo=" + DiaryNo + "&ProcessNo=" + ProcessNo, "_self");
                        }

                        break;
                    }
                }
            }
        }
    </script>
</asp:Content>
