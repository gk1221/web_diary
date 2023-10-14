'1. 僅清diary，其它資料庫需手動執行
'2. 每次均需確認程式碼是否需調整

'********************************************
DbYear="2013"	'要清除何年(不包含)以前資料，追蹤、未定、重大異常、資安事故不予刪除
'********************************************

strConn="Driver={SQL SERVER};Server=sos-vm16;Trusted_Connection=True;Database=" 
Dim conn    : Set conn=CreateObject("ADODB.Connection")   : conn.Open strConn & "Diary"
set rs=createobject("ADODB.Recordset")
'------------------------------------------------------------------------------------
rs.open "select tour,DiaryNo,Status from Diary where tour<'" & DbYear & "01011' and Degree<>2 and Degree<>3",conn   '排除重大事故與資安等級
while not rs.eof
  tour=rs(0) : DiaryNo=rs(1) : Status=rs(2)
  if Status<>1 and Status<>3 then  '排除追蹤與未定  
    conn.execute "delete from Msg     where tour='" & tour & "' and DiaryNo=" & DiaryNo
    conn.execute "delete from Process where tour='" & tour & "' and DiaryNo=" & DiaryNo
  end if
  rs.movenext
wend
rs.close

conn.execute "delete from Diary where tour<'" & DbYear & "01011' and Status<>1 and Status<>3 and Degree<>2 and Degree<>3" '排除追蹤、未定、重大異常、資安事故

conn.execute "delete from Sign where tour not in (select distinct tour from Diary)"
conn.execute "delete from Look where LookDate<'" & DbYear & "/01/01'"
conn.execute "delete from WarnLog where WarnDT<'" & DbYear & "/01/01 00:00'"

msgbox "ok!"


