on error resume next '寧無作用,不可出錯,以免hung住
'資料庫連結-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
set objWShell = CreateObject("WScript.Shell")
'SVexe="sos-vm16"
'strConn="Driver={SQL SERVER};Server=" & SVexe & ";Trusted_Connection=True;Database=" 
strConn="Driver={ODBC Driver 17 for SQL Server};Server=(local);Trusted_Connection=yes;DataTypeCompatibility=80;Database="
Dim connDiary    : Set connDiary=CreateObject("ADODB.Connection")   : connDiary.Open strConn & "Diary"
Dim connControl  : Set connControl=CreateObject("ADODB.Connection") : connControl.Open strConn & "Control"
set rs=createobject("ADODB.Recordset")
'--------------------------------
dim nowDT,hhmm : nowDT=DT(now,"yyyy/mm/dd hh:mi") : hhmm=mid(nowDT,12)
dim hhmi,yymmdd,yymmdp,yymmpp : hhmi=cint(DT(now,"hhmi")) : yymmdd=DT(now,"yyyymmdd") : yymmdp=DT(now-1,"yyyymmdd") : yymmpp=DT(now-2,"yyyymmdd")

SysCode="991"
PeoCode="2991f02"  : PeoText="門禁(全): 門禁管制人員進出紀錄之導入"
DevCode="2991f00" : DevText="門禁(全): 門禁管制設備異動紀錄之導入"
'存入日誌--------------------------------------------------------------------------------------------------------------
for i=0 to 3    '往前檢查三個班
  tour=TourC(i)  
  '-------------------------人員進出-----------------------------------------------------------------------------
  ProText=replace(replace(GetPeoText(tour),"'","''"),"""","""""")
  if ProText<>"人員進出：" then 
    rs.open "select * from Process where tour='" & tour & "' and ProText like '人員進出：%'",connDiary,3,3
    if rs.eof then
      call InsControl(tour,PeoCode,PeoText,ProText)    
    else
      rs("ProText")=ProText : rs("SaveDT")=nowDT
      rs.update      
    end if
    rs.close
  else
    'WScript. Echo "人員進出"
    call DelControl(tour,GetDiaryNo(tour,PeoCode))
  end if
  '-------------------------設備異動---------------------------------------------------------------------------
  ProText=replace(replace(GetDevText(tour),"'","''"),"""","""""")
  'WScript.Echo ProText
  'if len(ProText)>240 then ProText=mid(ProText,1,240) 
  if ProText<>"設備異動：" then   
    rs.open "select * from Process where tour='" & tour & "' and ProText like '設備異動：%'",connDiary,3,3
    if rs.eof then
      call InsControl(tour,DevCode,DevText,ProText)
    else
      rs("ProText")=ProText : rs("SaveDT")=nowDT
      rs.update
    end if
    rs.close
  else
    'WScript. Echo "設備進出" 
    call DelControl(tour,GetDiaryNo(tour,DevCode))
  end if
next
'-----------------------------門禁未登出之檢查---------------------------------------------------------------------------
if mid(hhmm,1,2)="07" or mid(hhmm,1,2)="19" then
  rs.open "select * from people where 離開日期='' or 離開時間=''",connControl
  if not rs.eof then InsWarn "(" & SVexe & ",control.vbs,op)Please. 有人員進入機房尚未登出，請檢查是否需登出門禁！"
  rs.close
end if
'----------------------------------------Insert into WarnLog函數------------------------------------------------------------
Function InsWarn(warnmsg)
  set objCmd = objWShell.Exec("WScript.exe d:\SSM_WEB\Public\WarnOP.vbs " & warnmsg) 
End Function
'--------------------------------時間格式函數---------------------------------------------------------------------------------
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
'---------------取得可新增記錄之序號---------------------------------------------------------
function GetSerial(qryTour,tbl,maxC,whereC,valC)
	dim rs : set rs=createobject("ADODB.Recordset")
	GetSerial=1
	if whereC<>"" then
		rs.open "select max(" & maxC & ") from " & tbl & " where tour='" & qryTour & "' and " & whereC & "=" & valC,connDiary
	else
		rs.open "select max(" & maxC & ") from " & tbl & " where tour='" & qryTour & "'",connDiary
	end if
	if not rs.eof then GetSerial=rs(0)+1
	rs.close
end function
'---------------取得前3班的班別---------------------------------------------------------
Function TourC(pre)  
  select case pre Mod 4
  case 0
    if hhmi<0730 then
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "3" 'yymmdp=DT(now-1,"yyyymmdd")
    elseif hhmi<1530 then
      TourC=DT(now-pre\4,"yyyymmdd") & "1"
    elseif hhmi<1930 then
      TourC=DT(now-pre\4,"yyyymmdd") & "2"
    else
      TourC=DT(now-pre\4,"yyyymmdd") & "3"
    end if   
  case 1
    if hhmi<0730 then
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "2"
    elseif hhmi<1530 then
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "3"
    elseif hhmi<1930 then
      TourC=DT(now-pre\4,"yyyymmdd") & "1"
    else
      TourC=DT(now-pre\4,"yyyymmdd") & "2"
    end if 
  case 2
    if hhmi<0730 then
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "1"
    elseif hhmi<1530 then
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "2"
    elseif hhmi<1930 then
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "3"
    else
      TourC=DT(now-pre\4,"yyyymmdd") & "1"
    end if
  case 3
    if hhmi<0730 then
      TourC=DT(now-(2+pre\4),"yyyymmdd") & "3"
    elseif hhmi<1530 then
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "1"
    elseif hhmi<1930 then
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "2"
    else
      TourC=DT(now-(1+pre\4),"yyyymmdd") & "3"
    end if
  end select
End Function
'---------------取得SQL---------------------------------------------------------
Function GetSQL(qryTour,which)
  dim nextYMD : nextYMD=DT(DateSerial(mid(qryTour,1,4),mid(qryTour,5,2),mid(qryTour,7,2))+1,"yyyy/mm/dd")
  'WScript.Echo "nextymd = " & nextYMD
  dim SQLp : SQLp="select distinct 申請單位,目的,處理過程 from people"
  'WScript.Echo SQLp & " where (進入日期='" & FormatTour(qryTour) & "' and 進入時間 between '19:30:00' and '23:59:59') or (進入日期='" & nextYMD & "' and 進入時間 between '00:00:00' and '07:29:59')"
  
  if which="p" then
    select case mid(qryTour,9,1)
    case "1" : GetSQL=SQLp & " where 進入日期='" & FormatTour(qryTour) & "' and 進入時間 between '07:30:00' and '15:29:59'"
    case "2" : GetSQL=SQLp & " where 進入日期='" & FormatTour(qryTour) & "' and 進入時間 between '15:30:00' and '19:29:59'"
    case "3" : GetSQL=SQLp & " where (進入日期='" & FormatTour(qryTour) & "' and 進入時間 between '19:30:00' and '23:59:59') or (進入日期='" & nextYMD & "' and 進入時間 between '00:00:00' and '07:29:59')"
    end select
  else
    select case mid(qryTour,9,1)
    case "1" : GetSQL="select * from Device2 where CreateDate between ' " & mid(qryTour,1,8) & " 07:30 " & "' and '" & mid(qryTour,1,8) & " 15:29'"
    case "2" : GetSQL="select * from Device2 where CreateDate between ' " & mid(qryTour,1,8) & " 15:30 " & "' and '" & mid(qryTour,1,8) & " 19:29 '"
    case "3" : GetSQL="select * from Device2 where CreateDate between ' " & mid(qryTour,1,8) & " 19:30 " & "' and '" & nextYMD & " 07:29 '"
    end select
  end if
End Function
'---------------格式化日期---------------------------------------------------------
function FormatTour(qryTour)
	FormatTour=mid(qryTour,1,4) & "/" & mid(qryTour,5,2) & "/" & mid(qryTour,7,2)
end function
'---------------取得門禁(人員進出)資料---------------------------------------------------------
function GetPeoText(qryTour)
  dim rs : set rs=createobject("ADODB.Recordset")
  dim tmpUnit,tmpProc
  rs.open GetSQL(qryTour,"p"),connControl
  while not rs.eof
    if tmpUnit<>Trim(rs("申請單位")) then
      GetPeoText=GetPeoText & tmpProc
      tmpUnit=Trim(rs("申請單位")) : tmpProc=tmpUnit & "-"
    end if
    if len(Trim(rs("處理過程"))) <50 and not Trim(rs("處理過程"))="" then
      if instr(1,tmpProc,Trim(rs("處理過程")))=0 or mid(tmpProc,len(tmpProc))="-" then tmpProc=tmpProc & Trim(rs("處理過程")) & " "   
    else
      if instr(1,tmpProc,Trim(rs("目的")))=0 or mid(tmpProc,len(tmpProc))="-" then tmpProc=tmpProc & Trim(rs("目的")) & " " 
    end if          
    rs.movenext
  wend
  rs.close
  GetPeoText="人員進出：" & GetPeoText & tmpProc
end function
'---------------取得門禁(設備異動)資料---------------------------------------------------------
function GetDevText(qryTour)
  dim io,rs : set rs=createobject("ADODB.Recordset")
  'WScript.Echo GetSQL(qryTour,"d")
  rs.open GetSQL(qryTour,"d"),connControl
  while not rs.eof
    io=rs("io")
    select case io
    case "I"  io="移入"
    case "O"  io="移出"
    case "N"  io="更換"
    case else io=""
    end select
    GetDevText=GetDevText & Trim(rs("staffname")) & io & "-" & Trim(rs("hostname"))&"   " 
    rs.movenext
  wend
  rs.close
  GetDevText="設備異動：" & GetDevText
end function
'---------------取得門禁DiaryNo---------------------------------------------------------
function GetDiaryNo(qryTour,MsgCode)
  dim rs : set rs=createobject("ADODB.Recordset")
  rs.open "select * from Msg,Process where Msg.tour=Process.tour and Msg.DiaryNo=Process.DiaryNo and Msg.tour='" & qryTour & "' and MsgCode='" & MsgCode & "' and ProText like '人員進出：%'",connDiary
  if not rs.eof then GetDiaryNo=rs(1)
  rs.close
end function
'---------------刪除門禁資料---------------------------------------------------------
function DelControl(qryTour,DiaryNo)
  'WScript.Echo "delete from Process where tour='" & qryTour & "' and DiaryNo='" & DiaryNo & "'"
  connDiary.execute "delete from Process where tour='" & qryTour & "' and DiaryNo='" & DiaryNo & "'"
  connDiary.execute "delete from Msg where tour='" & qryTour & "' and DiaryNo=" & DiaryNo
  connDiary.execute "delete from Diary where tour='" & qryTour & "' and DiaryNo=" & DiaryNo
end function
'---------------新增門禁資料---------------------------------------------------------
function InsControl(qryTour,MsgCode,MsgText,ProText)
  dim DiaryNo
  DiaryNo=GetSerial(qryTour,"Diary","DiaryNo","",0)
  MsgNo=GetSerial(qryTour,"Msg","MsgNo","",0)
  ProcessNo=GetSerial(qryTour,"Process","ProcessNo","",0)
  connDiary.execute "insert into Diary values('" & qryTour & "'," & DiaryNo & ",'" & nowDT & "','" & nowDT & "',0,'',0,'" & mid(MsgCode,2,3) & "','" & mid(MsgCode,5,1) & "','','','')"
  connDiary.execute "insert into Msg values('" & qryTour & "'," & DiaryNo & "," & MsgNo & ",'" & nowDT & "','" & nowDT & "','" & MsgCode & "','" & MsgText & "',0)"
  connDiary.execute "insert into Process values('" & qryTour & "'," & DiaryNo & "," & ProcessNo & ",'" & nowDT & "','" & nowDT & "','" & ProText & "','SOS','operator','電腦操作課','機房OP','')" 
end function
