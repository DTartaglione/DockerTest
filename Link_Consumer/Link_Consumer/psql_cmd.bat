
IF "%1%" == "delete" GOTO do_delete

IF "%1%" == "insert" GOTO do_insert

IF "%1%" == "insert_historic" GOTO do_insert_historic
:do_delete
GOTO END


:do_insert
REM set PGPASSWORD=mindhoppass&& psql -h mindhopdb-11-4-2.cugngvuqz5eh.us-east-2.rds.amazonaws.com -d db_PA_MOD -U mindhop -c "\copy dynamicds.tbllinktrafficdata_temp from '%2%' with delimiter as ','"
set PGPASSWORD=mindhoppass
psql -h localhost -d db_PA_MOD -U mindhop -c "\copy dynamicds.tbllinktrafficdata_temp from '%2%' with delimiter as ','"
GOTO END

:do_insert_historic

IF "%3%" == "2" SET tablename=historicds.tbllinktrafficdata_xcm
IF "%3%" == "3" SET tablename=historicds.tbllinktrafficdata

IF "%3%" == "2" SET collist=link_id,agency_id,speed_in_kph,travel_time_seconds,freeflow_in_kph,volume,occupancy,data_type,last_update
IF "%3%" == "3" SET collist=id,agency_id,speed_in_kph,travel_time_seconds,freeflow_in_kph,volume,occupancy,data_type,last_update

REM set PGPASSWORD=mindhoppass&& psql -h mindhopdb-11-4-2.cugngvuqz5eh.us-east-2.rds.amazonaws.com -d db_PA_MOD -U mindhop -c "\copy dynamicds.tbllinktrafficdata_temp from '%2%' with delimiter as ','"
set PGPASSWORD=mindhoppass
psql -h localhost -d db_PA_MOD -U mindhop -c "\copy %tablename% (%collist%) from '%2%' with delimiter as ','"
GOTO END

:END
