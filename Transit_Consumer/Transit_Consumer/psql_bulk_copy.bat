setlocal ENABLEDELAYEDEXPANSION
ECHO on

IF "%1%" == "stoptimeupdates" GOTO do_stoptimeupdate_insert

IF "%1%" == "stoptimeevents" GOTO do_stoptimeevent_insert

:do_stoptimeupdate_insert
set PGPASSWORD=mindhoppass
psql -h localhost -d db_PA_MOD -U mindhop -c "insert into historicds.tblstoptimeupdate select * from transitds.tblstoptimeupdate;"
set PGPASSWORD=mindhoppass
psql -h localhost -d db_PA_MOD -U mindhop -c "delete from transitds.tblstoptimeupdate;"
set PGPASSWORD=mindhoppass
psql -h localhost -d db_PA_MOD -U mindhop -c "\copy transitds.tblstoptimeupdate (stop_time_update_id, stop_sequence, stop_id, arrival_stop_time_event_id, departure_stop_time_event_id) from '%2%' (format csv, DELIMITER ',', NULL '')"

:do_stoptimeevent_insert
set PGPASSWORD=mindhoppass
psql -h localhost -d db_PA_MOD -U mindhop -c "insert into historicds.tblstoptimeevent select * from transitds.tblstoptimeevent;"
set PGPASSWORD=mindhoppass
psql -h localhost -d db_PA_MOD -U mindhop -c "delete from transitds.tblstoptimeevent;"
set PGPASSWORD=mindhoppass
psql -h localhost -d db_PA_MOD -U mindhop -c "\copy transitds.tblstoptimeevent (id, delay, time, uncertainty) from '%2%' (format csv, DELIMITER ',', NULL '')"


GOTO END
:END
