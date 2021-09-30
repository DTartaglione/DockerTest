
IF "%1%" == "delete" GOTO do_delete

IF "%1%" == "insert" GOTO do_insert

:do_delete
GOTO END


:do_insert
set PGPASSWORD=sap@$$
psql -h localhost -d db_PA_MOD -U postgres -c "\copy historicds.tbllinktrafficdata_2019 from '%2%' with delimiter as ','"
GOTO END

:END
