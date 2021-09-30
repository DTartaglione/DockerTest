CREATE OR REPLACE FUNCTION db_objects.bulkupdatelinks(_file_id character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

BEGIN
	--first update the links we will be updating in the production table (set status = 0)
	update dynamicds.tbllinktrafficdata set status = 0 where  
    id in (select id from dynamicds.tbllinktrafficdata_temp where file_id = _file_id);
    
    --now copy the newer links into the prod table
    insert into dynamicds.tbllinktrafficdata 
    select 
    id, agency_id, speed_in_kph, travel_time_seconds, freeflow_in_kph, volume, occupancy, data_type, last_update, status
    from dynamicds.tbllinktrafficdata_temp where file_id = _file_id;
   
    --remove older link records with status 0
    delete from dynamicds.tbllinktrafficdata where status = 0;
   
    --copy the HERE data into the historic data set 
   /* INSERT INTO historicds.tbllinktrafficdata 
    SELECT id, agency_id, speed_in_kph, travel_time_seconds, volume, occupancy, data_type, last_update, freeflow_in_kph 
    from
    (select 
    id,
    agency_id,
    speed_in_kph,
    travel_time_seconds, 
    volume,
    occupancy,
    data_type,
    last_update,
    freeflow_in_kph
    FROM dynamicds.tbllinktrafficdata_temp WHERE
    agency_id = 3 and 
    id in
    (SELECT id from public.tblboundingboxlinks)
    and file_id = _file_id) as data_table;
   
    --copy the XCM data into the historic data set 
    INSERT INTO historicds.tbllinktrafficdata_XCM 
    (link_id, agency_id, speed_in_kph, travel_time_seconds, volume, occupancy, data_type, last_update, freeflow_in_kph)
    SELECT id, agency_id, speed_in_kph, travel_time_seconds, volume, occupancy, data_type, last_update, freeflow_in_kph 
    from
    (select 
    id,
    agency_id,
    speed_in_kph,
    travel_time_seconds, 
    volume,
    occupancy,
    data_type,
    last_update,
    freeflow_in_kph
    FROM dynamicds.tbllinktrafficdata_temp WHERE
    agency_id = 2 and 
    id in
    (SELECT id from public.tblboundingboxlinks)
    and file_id = _file_id) as data_table;*/

    --clear out the data from the temp table no error
    delete from dynamicds.tbllinktrafficdata_temp where file_id = _file_id;
   
    EXCEPTION WHEN OTHERS then
    --todo - raise error here for alerting that we failed somewhere
     --clear out the data from the temp table if error
    delete from dynamicds.tbllinktrafficdata_temp where file_id = _file_id;
    
end;

$function$
;
