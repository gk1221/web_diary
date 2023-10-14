on error resume next		'寧無作用,不可出錯,以免hung住
'--------------------------------資料庫連結--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
'SVexe="SOS-VM18"
'strConn="Driver={SQL SERVER};Server=" & SVexe & ";Trusted_Connection=True;Database=" 
strConn="Driver={ODBC Driver 17 for SQL Server};Server=(local);Trusted_Connection=yes;DataTypeCompatibility=80;Database="
Dim connDiary    : Set connDiary=CreateObject("ADODB.Connection")   : connDiary.Open strConn & "Diary"
Dim connSclass : Set connSclass=CreateObject("ADODB.Connection") : connSclass.Open strConn & "WebSclass"
set rs=createobject("ADODB.Recordset")
'小鬧鐘--------------------------------------------------------------------------------
Dim MI	'以5分鐘為1單位
MI=DT(now,"mi")
if cint(mid(MI,2,1))>4 then
  MI=mid(MI,1,1) & "5"
else
  MI=mid(MI,1,1) & "0"
end if
'目前班別
dim yymmdd : dim yymmdp : dim hhmi
hhmi=cint(DT(now,"hhmi")) : yymmdd=DT(now,"yymmdd") : yymmdp=DT(now-1,"yymmdd")
if hhmi<0730 then
  c=yymmdp & "3"
elseif hhmi<1530 then
  c=yymmdd & "1"
elseif hhmi<1930 then
  c=yymmdd & "2"
else
  c=yymmdd & "3"
end if
'取得當班者P1 P2
rs.open "select * from ClassTable where YYMM='20" & mid(c,1,4) & "' and DD='" & mid(c,5,2) & "'",connSclass
if not rs.eof then
  select case mid(c,7,1)
  case "1"
    P1=rs("P1") : P2=rs("P2")
  case "2"
    P1=rs("P3") : P2=rs("P4")
  case "3"
    P1=rs("P5") : P2=rs("P6")
  end select
end if
rs.close
'取得符合現在的小鬧鐘設定
rs.open "select * from Clock where WorkYN='Y' and BeepYN='Y'" _
	& " and (YY='"  & DT(now,"yy") & "' or YY='**')" _
	& " and (MM='"  & DT(now,"mm") & "' or MM='**')" _
        & " and (DD='"  & DT(now,"dd") & "' or DD='**' " _
        & "   or DD='*" & P1           & "' or DD='*" & P2 & "'" _
        & "   or DD='*" & Weekday(now,vbMonDay) & "')" _
	& " and (HH='"  & DT(now,"hh") & "' or HH='**'" _
        & "   or HH='*" & mid(DT(now,"hh"),2,1) & "')" _
	& " and (MI='"  & MI           & "' or MI='**'" _
        & "   or MI='*" & mid(MI,2,1)  & "'" _
        & "   or MI='#1' and (" & MI & "=00  or " & MI & "=15  or " & MI & "=30  or " & MI & "=45)" _
        & "   or MI='#2' and (" & MI & "=00  or " & MI & "=20  or " & MI & "=40)" _
        & "   or MI='#3' and (" & MI & "=00  or " & MI & "=30))" ,connDiary ,3,3
while not rs.eof
  MsgCode=rs("MsgCode")   ': if rs("RoutYN")="Y" then MsgCode="Please." '例行工作不可重複記錄(Please.的訊息無法存檔)
  call InsWarn(MsgCode,rs("MsgText") & "(" & SVexe & ",diary.vbs,op)")
  
  '僅warn 1次者,將啟用改為停用
  if replace(rs("YY") & rs("MM") & rs("DD") & rs("HH") & rs("MI"),"*","a") <= mid(DT(now,"yymmddhhmi"),1,9) & "a" _
	and rs("RoutYN")<>"Y" then 
    rs("WorkYN")="N" : rs.update
  end if
  rs.movenext
wend
rs.close

'交接班清除Suspend------------------------------------------------------------------------------------
if hhmi>=0730 and hhmi<0735 or hhmi>=1130 and hhmi<1135 or hhmi>=1530 and hhmi<1535 or hhmi>=1930 and hhmi<1935 _
               or hhmi>=2330 and hhmi<2335 or hhmi>=0330 and hhmi<0335 then
  connDiary.execute "update warnlog set Status='R' where Status='S'"
end if
'------------------------------------------------------------------------------------
set rs=nothing
set connDiary=nothing
set connSclass=nothing
'----------------------------------------Insert into WarnLog函數----------------------------------------------------------------------------------------------------------------------------
Function InsWarn(error_code,warnmsg)
  dim rs1 : set rs1=createobject("ADODB.Recordset")
  rs1.open "select max(Serial) from warnlog",connDiary
  Serial=1 : if not rs1.eof then Serial=rs1(0)+1
  rs1.close

  connDiary.execute "insert into warnlog values('" & DT(now,"yyyy/mm/dd hh:mi") & "'," & Serial & ",'" & error_code & "','" & warnmsg & "','W')" 
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

