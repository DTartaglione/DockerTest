-- New Stored Procedure to insert crossing times into dynamicds for NITTEC

CREATE OR REPLACE FUNCTION db_objects.sp_insertcrossingdata(_facility_id int4, _ca_wait_time_in_mins int8, _us_wait_time_in_mins int8,  
_last_update timestamp with time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$ 

   
    begin
	    insert into dynamicds.tblcrossingdata (facility_id, ca_wait_time_in_mins, us_wait_time_in_mins, last_update) values 
		 (_facility_id, _ca_wait_time_in_mins, _us_wait_time_in_mins, _last_update );

    end;
$function$
;
