--001 - sp_updatetblroutes
CREATE OR REPLACE FUNCTION transitds.sp_updatetblroutes(_route_id character varying, 
_agency_id character varying, _route_short_name character varying(64), _route_long_name character varying(256),
_route_desc character varying, _route_type int, _route_url character varying(512), _route_color character varying(16),
_route_text_color character varying(16), _route_sort_order int)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$

      declare _local_route_id integer; 
     
	  begin
	   	  
          IF EXISTS (select route_id from transitds.tblroutes where route_id = _route_id) THEN
              begin
	              --existing route, do update
                  update transitds.tblroutes set 
                  agency_id = _agency_id,
                  route_short_name = _route_short_name,
                  route_long_name = _route_long_name,
                  route_desc = _route_desc,
                  route_type = _route_type,
                  route_url = _route_url,
                  route_color = _route_color,
                  route_text_color = _route_text_color,
                  route_sort_order = _route_sort_order
                  where route_id = _route_id;
              end;
          ELSE
              begin
                  --new route, do insert
                  insert into transitds.tblroutes (route_id, agency_id, route_short_name, route_long_name,
                  route_desc, route_type, route_url, route_color, route_text_color, route_sort_order) 
                  values
                  (_route_id, _agency_id, _route_short_name, _route_long_name,
                  _route_desc, _route_type, _route_url, _route_color, _route_text_color, _route_sort_order);
	              
	          end;
          END if;

          select id into _local_route_id from transitds.tblroutes where route_id = _route_id;
          return _local_route_id;
        
      end;
$function$
;

--002 - sp_updatetbltransitshapes
CREATE OR REPLACE FUNCTION transitds.sp_updatetbltransitshapes(_route_id character varying, 
_shape_id int, _shape_linestring character varying, _shape_length int)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
     
	  begin
	   	  
		  if (_shape_id = 0) then
		      begin
			     _shape_id = _route_id; 
			  end;
	      end if;
		  
          IF EXISTS (select shape_id from transitds.tbltransitshapes where shape_id = _shape_id) THEN
              begin
	              --existing shape, do update
                  update transitds.tbltransitshapes set 
                  shape_linestring = _shape_linestring,
                  shape_length = _shape_length
                  where shape_id = _shape_id;
              end;
          ELSE
              begin
                  --new route, do insert
                  insert into transitds.tbltransitshapes (shape_id, shape_linestring, shape_length) 
                  values
                  (_shape_id, _shape_linestring, _shape_length);
	              
	          end;
          END if;
         
      end;
$function$
;

-- 003 - sp_updatetblstops
CREATE OR REPLACE FUNCTION transitds.sp_updatetblstops(_stop_id integer, 
_stop_code character varying (128), _stop_name character varying (512), _stop_desc character varying (512),
_stop_lat float, _stop_lon float, _zone_id integer, _stop_url character varying(512), 
_location_type int, _parent_station int, _stop_timezone character varying (128), _wheelchair_boarding int,
_level_id int, _platform_code character varying (128))
 RETURNS integer
 LANGUAGE plpgsql
AS $function$

      DECLARE _local_stop_id integer;
     
	  begin
	   	  
		  IF EXISTS (select stop_id from transitds.tblstops where stop_id = _stop_id) THEN
              begin
	              --existing stop, do update
                  update transitds.tblstops set 
                  stop_code = _stop_code,
                  stop_name = _stop_name,
                  stop_desc = _stop_desc,
                  stop_lat = _stop_lat,
                  stop_lon = _stop_lon,
                  zone_id = _zone_id, 
                  stop_url = _stop_url, 
                  location_type = _location_type,
                  parent_station = _parent_station,
                  stop_timezone = _stop_timezone,
                  wheelchair_boarding = _wheelchair_boarding,
                  level_id = _level_id, 
                  platform_code = _platform_code
                  where stop_id = _stop_id;

              end;
          ELSE
              begin
                  --new route, do insert
                  insert into transitds.tblstops (stop_id, stop_code, stop_name, stop_desc,
                  stop_lat, stop_lon, zone_id, stop_url, location_type, parent_station,
                  stop_timezone, wheelchair_boarding, level_id, platform_code) 
                  values
                  (_stop_id, _stop_code, _stop_name, _stop_desc,
                  _stop_lat, _stop_lon, _zone_id, _stop_url, _location_type, _parent_station,
                  _stop_timezone, _wheelchair_boarding, _level_id, _platform_code);
	              
	          end;
          END if;
         
         select id into _local_stop_id from transitds.tblstops where stop_id = _stop_id;
         return _local_stop_id;
         
      end;
$function$
;

-- 004 - sp_updatevehicleposition
CREATE OR REPLACE FUNCTION transitds.sp_updatevehicleposition(_route_id character varying, 
_stop_id integer, _stop_code character varying, _vehicle_id character varying, _latitude float,
_longitude float, _heading float, _speed_km_hr float, _odometer float, _license_plate character varying,
_vehicle_label character varying, _last_update timestamp without time zone)
RETURNS void
 LANGUAGE plpgsql
AS $function$

      declare _vehicle_descriptor_id int := nextval('transitds.tblvehicledescriptor_id_seq');
      declare _position_id int := nextval('transitds.tblposition_id_seq');
      

	  begin
		  
		  if not exists (select id from transitds.tblvehicledescriptor where label = _vehicle_label
		  and license_plate = _license_plate) then
		      begin
		          insert into transitds.tblvehicledescriptor (id, label, license_plate) 
	   	          values 
	   	          (_vehicle_descriptor_id, _vehicle_label, null);
	   	      end;
	   	  end if;
	   	  
	   	  insert into transitds.tblposition (id, latitude, longitude, bearing, odometer, speed)
	   	  values
	   	  (_position_id, _latitude, _longitude, _heading, _odometer, _speed_km_hr);
	   	  
	   	  insert into transitds.tblvehicleposition (trip_descriptor_id, vehicle_descriptor_id, position_id, 
	   	  current_stop_sequence, stop_id, vehicle_stop_status_id, "timestamp", congestion_level_id, 
	   	  occupancy_status_id)
	   	  values
	   	  (null, _vehicle_descriptor_id, _position_id, null, _stop_id, null, _last_update, null, null);
		  
       
      end;
$function$
;


-- 005 - sp_updatetransitprediction
CREATE OR REPLACE FUNCTION transitds.sp_updatetransitprediction(_route_id character varying, 
_stop_code character varying, _is_departure boolean, _minutes integer, _seconds integer, _vehicle_id character varying,
_block_id character varying, _direction character varying, _event_date_time timestamp without time zone)
RETURNS void
 LANGUAGE plpgsql
AS $function$

      declare _stoptime_event_id int := nextval('transitds.tblstoptimeevent_id_seq');
      declare _stoptime_update_id int := nextval('transitds.tblstoptimeupdate_id_seq');
      declare _stop_id character varying;
      declare _arrival_event_id int;
      declare _departure_event_id int;

	  begin
		  
		  select stop_id into _stop_id from transitds.tblstops where stop_code = _stop_code;
		 
		  if (_is_departure = true) then
		      begin
			     _departure_event_id = _stoptime_event_id;
			     _arrival_event_id = null;
		      end;
		  else
		      begin
			      _departure_event_id = null;
			     _arrival_event_id = _stoptime_event_id;
			  end;
		  end if;
		  
		 --insert into tblstoptimeevent
		  insert into transitds.tblstoptimeevent (id, delay, time, uncertainty) values
		  (_stoptime_event_id, 0, _minutes, 0);
	   	  
          --insert into tblstoptimeupdate
          insert into transitds.tblstoptimeupdate (id, stop_sequence, stop_id, arrival_stop_time_event_id, departure_stop_time_event_id, schedule_relationship_id) values 
          (_stoptime_update_id, null, _stop_id, _arrival_event_id, _departure_event_id, null);
      end;
$function$
;

-- 006 - sp_updatetransitmessages
CREATE OR REPLACE FUNCTION transitds.sp_updatetransitmessages(_agency_id character varying, _route_id character varying, 
_route_type character varying, _stop_code character varying,_start_epoch bigint, _end_epoch bigint, _message character varying 
)
RETURNS void
 LANGUAGE plpgsql
AS $function$

      declare _time_range_id int := nextval('transitds.tbltimerange_id_seq');
      declare _stop_id int;
      
      
     
	  begin
		  
		  select stop_id into _stop_id from transitds.tblstops where stop_code = _stop_code;
		 
		  
      end;
$function$
;




