
CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_updateevent(_event_id character varying, _source_event_id character varying, _owner_org_id bigint, _event_status bigint, _event_category_id integer, _event_category character varying, _event_type character varying, _last_update timestamp without time zone, _facility_id bigint, _facility_name character varying, _event_direction bigint, _article character varying, _from_node_id bigint, _from_node_name character varying, _to_node_id bigint, _to_node_name character varying, _event_description character varying, _event_severity bigint, _from_latitude double precision, _from_longitude double precision, _to_latitude double precision, _to_longitude double precision, _estimated_duration bigint, _extra_info character varying, _city character varying, _county character varying, _state character varying, _num_affected_lanes bigint, _num_total_lanes bigint, _lane_type character varying, _lane_status character varying, _start_date_time timestamp without time zone, _end_date_time timestamp without time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

      DECLARE _return bigint;
      DECLARE _evtgeom geometry := null;
      DECLARE _update_count int := 0;
      
	begin
		    _evtgeom = ST_GeomFromText('point (' || _from_longitude || ' ' || _from_latitude || ')', 4326);
		    _update_count = (update_count + 1) FROM dynamicds.tblevent WHERE event_id = _event_id;
		    
		    --IF _county = '' or _county is null THEN
			--	select _county_name into _county from db_objects.sp_getcountystatedata()
			--    where st_contains(ST_AsText(_geom), ST_GeomFromText('point (' || _from_longitude || ' ' || _from_latitude || ')'));
			--end if; 
		   
		    UPDATE dynamicds.tblevent SET 
		        event_status = _event_status, 
		        event_category_id = _event_category_id,
		        event_category = _event_category,
		        event_type = _event_type,
			last_update = _last_update,
			facility_id = _facility_id,
			facility_name = _facility_name,
			event_direction = _event_direction,
			article = _article,
			from_node_id = _from_node_id,
			from_node_name = _from_node_name,
			to_node_id = _to_node_id,
			to_node_name = _to_node_name,
			event_description = _event_description,
			event_severity = _event_severity,
			from_latitude = _from_latitude, 
			from_longitude = _from_longitude,
			to_latitude = _to_latitude,
			to_longitude = _to_longitude,
			estimated_duration = _estimated_duration, 
			extra_info = _extra_info,
			city = _city,
			county = _county,
			state = _state,
			num_affected_lanes = _num_affected_lanes,
			num_total_lanes = _num_total_lanes,
			lane_type = _lane_type,
			lane_status = _lane_status,
			start_date_time = _start_date_time,
			end_date_time = _end_date_time,
			update_count = _update_count,
			geom = _evtgeom
                        WHERE event_id = _event_id;


end;
$function$
;
