on error resume next		'寧無作用,不可出錯,以免hung住
'--------------------------------物件設定------------------------------------------------------------------------------------------------------
'strConn="Driver={SQL SERVER};Server=10.6.1.4;Trusted_Connection=True;Database=" 
strConn="Driver={ODBC Driver 17 for SQL Server};Server=(local);Trusted_Connection=yes;DataTypeCompatibility=80;Database="
Dim conn    : Set conn=CreateObject("ADODB.Connection")   : conn.Open strConn & "Diary"
set rs=createobject("ADODB.Recordset")
set rs1=createobject("ADODB.Recordset")

dim MailTag  : MailTag="#{mail}:" : dim MailOkTag : MailOkTag="#{mail ok!}:"	'mail標籤：隔天通知一次
dim SqlMail  : SqlMail="SaveDT >='" & DT(now-7,"yyyy/mm/dd 00:00") & "'"	    'Mail & Trace Check 7天，否則資料量太大
dim LeafTag  : LeafTag="#{已離職}："						                    '離職即不用通知
dim TraceTag : TraceTag="#{Trace}:"  					                    	'追蹤標籤：每週一通知
dim NewTag   : NewTag="#{New}:"       					                    	'更新標籤：更新隔天
dim w : w=Weekday(now,vbMonDay)							                        '現在星期幾
dim PreDay : PreDay=DT(now-1,"yyyy/mm/dd hh:mi")

'----------------Logging ...... ----------------------------------------------------------------------------------------------------------------
set fs=createobject("scripting.filesystemobject")
if DT(now,"dd")="01" then
  set ff=fs.OpenTextFile("TraceMail_" & DT(now,"mm") & ".log",2,true) 
else
  set ff=fs.OpenTextFile("TraceMail_" & DT(now,"mm") & ".log",8,true)
end if
ff.write DT(now,"yyyy/mm/dd hh:mi:ss")

'-----------------------------依姓名取其該屬日誌後mail出去--------------------------------------------------------------------------------------
rs1.open "select [成員],[備註] from [View_組織架構] where [性質]='員工' and [備註] like '%" & MailTag & "%' and [備註] not like '%" & LeafTag & "%'",conn
while not rs1.eof 
  whoProText="" : tour=""	'取得該員需通知的日誌內容

  rs.open "select distinct Diary.tour,Diary.DiaryNo from Diary,Process where Diary.tour=Process.tour and Diary.DiaryNo=Process.DiaryNo" _
    & " and (" & GetGroupSQL(rs1(0)) & ") order by Diary.tour,Diary.DiaryNo",conn
  while not rs.eof
    if rs(0)<>tour then
      tour=rs(0)
      whoProText=whoProText  & vbcrlf & vbcrlf & "[" & tour & "]"  & vbcrlf
    end if

    whoProText=whoProText & GetMsg(tour,rs(1)) & GetDiary(tour,rs(1)) & vbcrlf
    rs.movenext
  wend
  rs.close

  if whoProText<>"" then call SendMail(rs1(0),GetMail(rs1(0),rs1(1)),whoProText)	'mail出去

  rs1.movenext
wend
rs1.close
ff.write " [Mail]：" & DT(now,"hh:mi:ss")

'----------------修改 MailTag -> MailOkTag----------------------------------------------------------------------------------------------------
rs.open "select ProText from Process where ProText like '%" & MailTag & "%' and " & SqlMail,conn,3,3
while not rs.eof
  rs(0)=replace(rs("ProText"),MailTag,MailOkTag)	'僅通知一次
  rs.update
  rs.movenext
wend
rs.close

'----------------異常處理異動記錄--------------------------------------------------------------------------------------------------------------
LifeNo=1
rs.open "select max([記錄編號]) from [異動記錄]",conn
if not rs.eof then LifeNo=rs(0)+1
rs.close

rs.open "select * from [異動記錄] where [表格名稱]='異常處理' and ([異動記錄] like '刪除：原本：%' or [異動記錄] like '新增：%') and substring([主鍵編號],1,7) not in (select distinct [訊息代碼] from [View_SOP])",conn
while not rs.eof
  if GetValue("select [異動記錄] from [異動記錄] where [異動記錄] ='刪除：" & replace(rs(3),"'","''") & "'")="" then
    conn.execute "Insert into [異動記錄] values(" & LifeNo & ",'異常處理','" & rs(2) & "','刪除：" & replace(rs(3),"'","''") & "','機房OP','" & DT(now,"yyyy/mm/dd 00:00") & "','0.0.0.0')"
    LifeNo=LifeNo+1
  end if
  rs.movenext
wend
rs.close

rs.open "select * from [View_SOP] where [訊息代碼] in (select substring([主鍵編號],1,7) from [異動記錄] where [表格名稱]='異常處理') and [更新日]='" & year(now-1) & "/" & month(now-1) & "/" & day(now-1) & "'",conn
while not rs.eof
  conn.execute "Insert into [異動記錄] values(" & LifeNo & ",'異常處理','" & rs(0) & "." & rs(1) & "','修改：" & replace(rs(2),"'","''") & "','機房OP','" & DT(now,"yyyy/mm/dd 00:00") & "','0.0.0.0')"
  LifeNo=LifeNo+1
  rs.movenext
wend
rs.close

rs.open "select * from [View_SOP] where [訊息代碼] not in (select substring([主鍵編號],1,7) from [異動記錄] where [表格名稱]='異常處理')",conn
while not rs.eof
  conn.execute "Insert into [異動記錄] values(" & LifeNo & ",'異常處理','" & rs(0) & "." & rs(1) & "','新增：" & replace(rs(2),"'","''") & "','機房OP','" & DT(now,"yyyy/mm/dd 00:00") & "','0.0.0.0')"
  LifeNo=LifeNo+1
  rs.movenext
wend
rs.close

'----------------結束--------------------------------------------------------------------------------------------------------------------------
ff.write " [End]：" & DT(now,"hh:mi:ss") & vbcrlf
ff.close
'msgbox "ok"
'----------------取得mail address--------------------------------------------------------------------------------------------------------------
Function GetMail(who,Memo)	
  dim pos : GetMail=""

  pos=instr(1,Memo,MailTag)	
  if pos>0 then GetMail=mid(Memo,pos+8,instr(pos+8,Memo,"#")-pos-8)      
End Function
'----------------日誌--------------------------------------------------------------------------------------------------------------
Function GetGroupSQL(who)   'mail標籤：隔天通知，追蹤標籤：每週一通知
  if who="機房OP" then
    GetGroupSQL="ProText like '%" & MailTag & "%' and Process." & SqlMail _
        & " or " & w & "=1 and (Status=1 or Status=3) and ProText like '%" & TraceTag & "%'" _
        & " or Process.SaveDT >='" & PreDay & "' and ProText like '%" & NewTag & "%'"  '(1) for operator
  else
    dim rs : set rs=createobject("ADODB.Recordset")
    GetGroupSQL="ProText like '%" & MailTag & who & "%' and Process." & SqlMail _
        & " or " & w & "=1 and (Status=1 or Status=3) and ProText like '%" & TraceTag & who & "%'" _
        & " or Process.SaveDT >='" & PreDay & "' and ProText like '%" & NewTag & who & "%'"  '(1) for 員工

    rs.open "select [課別] from [View_組織架構] where [性質]='員工' and [成員]='" & who & "'",conn
    while not rs.eof
      GetGroupSQL=GetGroupSQL & " or ProText like '%" & MailTag & rs(0) & "%' and Process." & SqlMail _
        & " or " & w & "=1 and (Status=1 or Status=3) and ProText like '%" & TraceTag & rs(0) & "%'" _
        & " or Process.SaveDT >='" & PreDay & "' and ProText like '%" & NewTag & rs(0) & "%'"  '(1) for 課別
      rs.movenext
    wend
    rs.close

    rs.open "select [群組] from [View_維護群組] where [成員]='" & who & "'",conn
    while not rs.eof
      GetGroupSQL=GetGroupSQL & " or ProText like '%" & MailTag & rs(0) & "%' and Process." & SqlMail _
        & " or " & w & "=1 and (Status=1 or Status=3) and ProText like '%" & TraceTag & rs(0) & "%'" _
        & " or Process.SaveDT >='" & PreDay & "' and ProText like '%" & NewTag & rs(0) & "%'"  '(1) for 群組
      rs.movenext
    wend
    rs.close
  end if
End Function
'----------------日誌--------------------------------------------------------------------------------------------------------------
Function GetMsg(tour,DiaryNo)
  dim rs : set rs=createobject("ADODB.Recordset")
  dim i : i=1 : GetMsg=""
  rs.open "select * from Msg where tour='" & tour & "' and DiaryNo=" & DiaryNo & " order by MsgDT",conn
  while not rs.eof
    GetMsg=GetMsg & "# " & mid(rs("MsgDT"),12) & " " & rs("MsgCode") & " "  & rs("MsgText")  & vbcrlf
    i=i+1
    rs.movenext
  wend
  rs.close
End Function

Function GetDiary(tour,DiaryNo)
  dim rs : set rs=createobject("ADODB.Recordset")
  dim i : i=1 : GetDiary=""
  rs.open "select * from Process where tour='" & tour & "' and DiaryNo=" & DiaryNo & " and ProText not like '@@%' order by ProcessDT",conn
  while not rs.eof
    GetDiary=GetDiary & "(" & i & ") " & rs("UserName") & " " & mid(rs("ProcessDT"),6) & " "  & rs("ProText")  & vbcrlf     
    i=i+1
    rs.movenext
  wend
  rs.close
End Function
'----------------Send Mail--------------------------------------------------------------------------------------------------------------
Sub SendMail(who,email,whoProText)	
  'if who<>"機房OP" then exit sub '***********程式test用************
  if email="" then exit sub

  whoProText=who & " 長官您好：" & vbcrlf & vbcrlf _
    & "以下是你所負責的系統於昨日發生異常事件，值班人員已登錄於工作日誌中，請自行或通知值班人員於該日誌記錄(http://10.6.1.11:8888)做補充說明，謝謝！" & vbcrlf _
    & "(說明檔路徑：工作日誌 -> 系統設定 -> 使用手冊)" & vbcrlf & vbcrlf _
    & whoProText

  sch = "http://schemas.microsoft.com/cdo/configuration/" 
  Set cdoConfig=CreateObject("CDO.Configuration") 
  With cdoConfig.Fields 
	.Item(sch & "sendusing") = 2 ' cdoSendUsingPort 
	.Item(sch & "smtpserverport") = 25
	.Item(sch & "smtpconnectiontimeout") = 60
	.Item(sch & "smtpserver") = "ms1.cwb.gov.tw"  'your smtp
	.update 
  End With
 
  Set cdoMessage=CreateObject("CDO.Message") 
  With cdoMessage 
	.Configuration = cdoConfig 
	.From     ="sosop@cwb.gov.tw"
	.To       =email
	.Subject  ="機房工作日誌之異常事件記錄完整性通知(" & DT(now,"mm/dd") & ")"
	'.HtmlBody =whoProText
	.TextBody =whoProText
	.BodyPart.charset = "UTF-8" 
	.Send
  End With 

  Set cdoConfig  =nothing
  Set cdoMessage =nothing

  ff.write " " & who
End Sub
'----------------------------------------------------------------------------------------------------------------------------------------
Function GetValue(byval SQL)
  dim dr : set dr=createobject("ADODB.Recordset")
  dr.open SQL,conn

  GetValue=""
  if not dr.eof then GetValue=dr(0)

  dr.close
End Function
'--------------------------------時間格式函數---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
Function DT(byval sDT,byval fDT)
  dim YY0,MM0,DD0,HH0,MI0,SS0
  DT=lcase(fDT)
  YY0=year(sDT)   : if instr(1,DT,"yy",1)>0 and instr(1,DT,"yyyy",1)=0 then YY0=mid(YY0,3)
  DT=replace(replace(DT,"yyyy","yy"),"yy",YY0)
  MM0=month(sDT)  : if MM0<10 then MM0="0" & MM0
  DT=replace(DT,"mm",MM0)
  DD0=day(sDT)    : if DD0<10 then DD0="0" & DD0
  DT=replace(DT,"dd",DD0)
  HH0=hour(sDT)   : if HH0<10 then HH0="0" & HH0
  DT=replace(DT,"hh",HH0)
  MI0=minute(sDT) : if MI0<10 then MI0="0" & MI0
  DT=replace(DT,"mi",MI0)
  SS0=second(sDT) : if SS0<10 then SS0="0" & SS0
  DT=replace(DT,"ss",SS0) 
End Function

