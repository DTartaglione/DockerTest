CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_insertevent(_event_id character varying, _source_event_id character varying, _owner_org_id bigint, _event_status bigint, _event_category_id integer, _event_category character varying, _event_type character varying, _create_time timestamp without time zone, _last_update timestamp without time zone, _facility_id bigint, _facility_name character varying, _event_direction bigint, _article character varying, _from_node_id bigint, _from_node_name character varying, _to_node_id bigint, _to_node_name character varying, _event_description character varying, _event_severity bigint, _from_latitude double precision, _from_longitude double precision, _to_latitude double precision, _to_longitude double precision, _estimated_duration bigint, _extra_info character varying, _city character varying, _county character varying, _state character varying, _num_affected_lanes bigint, _num_total_lanes bigint, _lane_type character varying, _lane_status character varying, _start_date_time timestamp without time zone, _end_date_time timestamp without time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

      DECLARE _return bigint;
      DECLARE _evtgeom geometry := null;
      DECLARE _update_count int := 0;
     
	begin
		    
			 _evtgeom = ST_GeomFromText('point (' || _from_longitude || ' ' || _from_latitude || ')', 4326);
			
			--IF _county = '' or _county is null THEN
			--	select _county_name into _county from db_objects.sp_getcountystatedata()
			--   where st_contains(ST_AsText(_geom), ST_GeomFromText('point (' || _from_longitude || ' ' || _from_latitude || ')'));
			--end if; 
			
		    INSERT INTO dynamicds.tblevent (event_id,source_event_id,owner_org_id, event_status, event_category_id, event_category,event_type,create_time,
			last_update,facility_id,facility_name,event_direction,article,from_node_id,from_node_name,
			to_node_id,to_node_name,event_description,event_severity,from_latitude, from_longitude,to_latitude,
			to_longitude,estimated_duration, extra_info,city,county,state,num_affected_lanes,num_total_lanes,lane_type,lane_status,
			start_date_time,end_date_time,update_count,geom)
			VALUES
			(_event_id, _source_event_id, _owner_org_id, _event_status, _event_category_id, _event_category, _event_type, _create_time, _last_update, _facility_id
			,_facility_name, _event_direction, _article, _from_node_id, _from_node_name, _to_node_id, _to_node_name
			,_event_description, _event_severity, _from_latitude, _from_longitude, _to_latitude, _to_longitude, _estimated_duration, _extra_info
			,_city, _county, _state, _num_affected_lanes, _num_total_lanes, _lane_type, _lane_status, _start_date_time, _end_date_time
			,_update_count, _evtgeom);
	   

		--	_return = 1;



			
		    
		--EXCEPTION WHEN others THEN
		--	_return = -1;
		
		--END;

	
	--RETURN _return;

end;
$function$
;
