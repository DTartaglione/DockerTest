CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_closeevent(_event_id character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

 	   declare _update_count int := 0;
 	   DECLARE _last_update timestamp := CURRENT_TIMESTAMP;
--begin

    

	   
		BEGIN 
		   _update_count = (update_count + 1) FROM dynamicds.tblevent WHERE event_id = _event_id;

			UPDATE dynamicds.tblevent
			SET last_update = _last_update
			,close_time = _last_update
			,event_status = 11
			,update_count = _update_count
			WHERE event_id = _event_id;
			--SET _return = 1;
		
		    insert into historicds.tbleventarchive 
		    select * from dynamicds.tblevent WHERE event_id = _event_id; 
		    
		    delete from dynamicds.tblevent WHERE event_id = _event_id; 
		--EXCEPTION WHEN others THEN
		--	_return = -1;
		
		END;

	
	--select _return;

$function$
;
