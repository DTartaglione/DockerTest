
-- 001 - add stop time update id to tblstoptimeupdate so we can easily relate it to tbltripupdate
-- DROP TABLE transitds.tblstoptimeupdate;
CREATE TABLE transitds.tblstoptimeupdate (
	id serial NOT NULL,
	stop_time_update_id int not null,
	stop_sequence int4 NULL,
	stop_id varchar(64) NULL,
	arrival_stop_time_event_id int4 NULL,
	departure_stop_time_event_id int4 NULL,
	schedule_relationship_id int4 NULL,
	CONSTRAINT pkey_transitds_tblstoptimeupdate PRIMARY KEY (id)
);

-- 002 - Create table for storage in historicds schema
-- DROP TABLE historicds.tblstoptimeupdate;
CREATE TABLE historicds.tblstoptimeupdate (
	id serial NOT NULL,
	stop_time_update_id int not null,
	stop_sequence int4 NULL,
	stop_id varchar(64) NULL,
	arrival_stop_time_event_id int4 NULL,
	departure_stop_time_event_id int4 NULL,
	schedule_relationship_id int4 NULL,
	CONSTRAINT pkey_historicds_tblstoptimeupdate PRIMARY KEY (id)
);

-- 003 - Create table for storage in historicds schema
--DROP TABLE historicds.tblstoptimeevent;
CREATE TABLE historicds.tblstoptimeevent (
	id serial NOT NULL,
	delay int4 NULL,
	"time" int4 NULL,
	uncertainty int4 NULL,
	CONSTRAINT pkey_historicdsds_tblstoptimeevent PRIMARY KEY (id)
);

-- 004 - sp_updatevehicleposition
DROP FUNCTION IF EXISTS transitds.sp_updatevehicleposition(character varying, integer, character varying, character varying, double precision, double precision, double precision, 
double precision, double precision, character varying, character varying, timestamp without time zone);

CREATE OR REPLACE FUNCTION transitds.sp_updatevehicleposition(_trip_id character varying, _route_id character varying, 
_stop_id integer, _stop_code character varying, _vehicle_id character varying, _direction_id integer, _latitude float,
_longitude float, _heading float, _speed_km_hr float, _odometer float, _license_plate character varying,
_vehicle_label character varying, _start_date character varying, _last_update timestamp without time zone)
RETURNS void
 LANGUAGE plpgsql
AS $function$

      declare _vehicle_descriptor_id int := nextval('transitds.tblvehicledescriptor_id_seq');
      declare _position_id int := nextval('transitds.tblposition_id_seq');
      declare _trip_descriptor_id int := nextval('transitds.tbltripdescriptor_id_seq');

	  begin
		  
		  if not exists (select id from transitds.tbltripdescriptor where trip_id = _trip_id and 
		  route_id = _route_id) then
		      begin 
			      insert into transitds.tbltripdescriptor (id, trip_id, route_id, direction_id, start_time, 
			      start_date, schedule_relationship_id)
		          values 
		          (_trip_descriptor_id, _trip_id, _route_id, _direction_id, null, _start_date, null);
		      end;
		  else 
		       _trip_descriptor_id = id from transitds.tbltripdescriptor where trip_id = _trip_id 
		       and route_id = _route_id;
		  end if;
		  
		  if not exists (select id from transitds.tblvehicledescriptor where label = _vehicle_id) then
		      begin
		          insert into transitds.tblvehicledescriptor (id, label, license_plate) 
	   	          values 
	   	          (_vehicle_descriptor_id, _vehicle_id, null);
	   	      end;
	   	  else 
		       _vehicle_descriptor_id = id from transitds.tblvehicledescriptor where label = _vehicle_id;
	   	  end if;
	   	  
	   	  insert into transitds.tblposition (id, latitude, longitude, bearing, odometer, speed)
	   	  values
	   	  (_position_id, _latitude, _longitude, _heading, _odometer, _speed_km_hr);
	   	  
	   	  insert into transitds.tblvehicleposition (trip_descriptor_id, vehicle_descriptor_id, position_id, 
	   	  current_stop_sequence, stop_id, vehicle_stop_status_id, "timestamp", congestion_level_id, 
	   	  occupancy_status_id)
	   	  values
	   	  (_trip_descriptor_id, _vehicle_descriptor_id, _position_id, null, _stop_id, null, _last_update, null, null);
		  
       
      end;
$function$
;

-- 005 - sp_updatetrips
DROP FUNCTION IF EXISTS transitds.sp_updatetrips(character varying, character varying, integer, character varying, 
character varying, timestamp without time zone, integer, character varying, integer, integer, integer);

CREATE OR REPLACE FUNCTION transitds.sp_updatetrips(_trip_id character varying, _route_id character varying, 
_direction_id integer, _vehicle_id character varying, _start_date character varying, _last_update timestamp without time zone, 
_delay integer, _stop_id character varying, _stop_sequence integer, _arrival integer, _departure integer)
--RETURNS integer
returns table (_stop_time_update_id integer, _stop_time_arr_event_id integer, _stop_time_dep_event_id integer)
 LANGUAGE plpgsql
AS $function$

      declare _vehicle_descriptor_id int := 0;
      declare _trip_descriptor_id int := 0;
      declare _stop_time_arr_event_id int := 1;
      declare _stop_time_dep_event_id int := 1; 
      declare _stop_time_update_id int := 1;
      declare _trip_update_id int := 0;
      declare _stoptimecount int := 0;
     
	  begin
		  
		  if not exists (select id from transitds.tbltripdescriptor where trip_id = _trip_id and 
		  route_id = _route_id) then
		      begin 
			      _trip_descriptor_id =  nextval('transitds.tbltripdescriptor_id_seq');
			      
			      insert into transitds.tbltripdescriptor (id, trip_id, route_id, direction_id, start_time, 
			      start_date, schedule_relationship_id)
		          values 
		          (_trip_descriptor_id, _trip_id, _route_id, _direction_id, null, _start_date, null);
		      end;
		  else 
		       _trip_descriptor_id = id from transitds.tbltripdescriptor where trip_id = _trip_id 
		       and route_id = _route_id;
		  end if;
		  
		  if not exists (select id from transitds.tblvehicledescriptor where label = _vehicle_id) then
		      begin
			      _vehicle_descriptor_id =  nextval('transitds.tblvehicledescriptor_id_seq');
			      
		          insert into transitds.tblvehicledescriptor (id, label, license_plate) 
	   	          values 
	   	          (_vehicle_descriptor_id, _vehicle_id, null);
	   	      end;
	   	  else 
		       _vehicle_descriptor_id = id from transitds.tblvehicledescriptor where label = _vehicle_id;
	   	  end if;
	   	 
	   	  _stop_time_arr_event_id = nextval('transitds.tblstoptimeevent_id_seq');
	   	  _stop_time_dep_event_id = nextval('transitds.tblstoptimeevent_id_seq');
	   	   
	   	  if exists (select 1 from transitds.tbltripupdate where trip_descriptor_id = _trip_descriptor_id and 
	   	  vehicle_descriptor_id = _vehicle_descriptor_id) then
	   	      _stop_time_update_id = stop_time_update_id from transitds.tbltripupdate where trip_descriptor_id = _trip_descriptor_id and 
	   	      vehicle_descriptor_id = _vehicle_descriptor_id;
 
	   	      --_stop_time_arr_event_id = arrival_stop_time_event_id from transitds.tblstoptimeupdate where stop_time_update_id = _stop_time_update_id
	   	      --and stop_sequence = _stop_sequence and stop_id = _stop_id;
	   	     
	   	      --_stop_time_dep_event_id = departure_stop_time_event_id from transitds.tblstoptimeupdate where stop_time_update_id = _stop_time_update_id
	   	      --and stop_sequence = _stop_sequence and stop_id = _stop_id;
	   	  else 

	   	    _stop_time_update_id = nextval('transitds.tblstoptimeupdate_id_seq');    
	   	    --_stop_time_arr_event_id = nextval('transitds.tblstoptimeevent_id_seq');
	   	    --_stop_time_dep_event_id = nextval('transitds.tblstoptimeevent_id_seq');
	   	    _trip_update_id = nextval('transitds.tbltripupdate_id_seq');
	   	   
	   	     insert into transitds.tbltripupdate (id, trip_descriptor_id, vehicle_descriptor_id, stop_time_update_id, "timestamp", delay)
	   	     values (_trip_update_id, _trip_descriptor_id, _vehicle_descriptor_id, _stop_time_update_id, _last_update, _delay);
	   	    
	      end if;
	   	 
	      return query select _stop_time_update_id, _stop_time_arr_event_id, _stop_time_dep_event_id;
      end;
$function$
;

-- 006 - new SP sp_getgtfsvehiclepositions
CREATE OR REPLACE FUNCTION transitds.sp_getgtfsvehiclepositions()
 RETURNS TABLE(_id integer, _vehicle_label character varying, _trip_id character varying, _route_id character varying, 
 _stop_id character varying, _last_update timestamp without time zone)
 LANGUAGE plpgsql
AS $function$

 declare _end_int timestamp;
 declare _begin_int timestamp;

 begin 
	 
	 _end_int = max("timestamp") 
	  from transitds.tblvehicleposition vp 
      inner join transitds.tblvehicledescriptor vd 
      on vp.vehicle_descriptor_id = vd.id
      left join transitds.tbltripdescriptor td 
      on td.id = vp.trip_descriptor_id
      left join transitds.tblstops s 
      on s.stop_id = vp.stop_id;
    
     _begin_int = (date_part('year', _end_int) || '-' || 
      date_part('month', _end_int)  || '-' ||
      date_part('day', _end_int))::timestamp - INTERVAL '24 hours';
	 
      return query select vp.id, vd."label", td.trip_id, td.route_id, vp.stop_id, vp."timestamp"
      from transitds.tblvehicleposition vp 
      inner join transitds.tblvehicledescriptor vd 
      on vp.vehicle_descriptor_id = vd.id
      left join transitds.tbltripdescriptor td 
      on td.id = vp.trip_descriptor_id
      left join transitds.tblstops s 
      on s.stop_id = vp.stop_id
      where vp."timestamp" >= _begin_int and 
      vp."timestamp" <= _end_int;

 end;

$function$
;

-- 007 - new SP sp_getgtfstripupdates
CREATE OR REPLACE FUNCTION transitds.sp_getgtfstripupdates()
 RETURNS TABLE(_id integer, _vehicle_label character varying, _trip_id character varying, _route_id character varying, _stop_id character varying,
 _stop_time_update_id integer, _last_update timestamp without time zone)
 LANGUAGE plpgsql
AS $function$

 declare _end_int timestamp;
 declare _begin_int timestamp;

 begin 
	 
	  _end_int = max("timestamp") 
      from transitds.tbltripupdate tu 
      inner join transitds.tblstoptimeupdate st 
      on st.stop_time_update_id = tu.stop_time_update_id      
      inner join transitds.tblvehicledescriptor vd 
      on tu.vehicle_descriptor_id = vd.id
      left join transitds.tbltripdescriptor td 
      on td.id = tu.trip_descriptor_id;
    
     _begin_int = (date_part('year', _end_int) || '-' || 
      date_part('month', _end_int)  || '-' ||
      date_part('day', _end_int))::timestamp - INTERVAL '24 hours';
	 
      return query select tu.id, vd."label", td.trip_id, td.route_id, 
      st.stop_id, tu.stop_time_update_id, tu."timestamp"
      from transitds.tbltripupdate tu 
      inner join transitds.tblstoptimeupdate st 
      on st.stop_time_update_id = tu.stop_time_update_id      
      inner join transitds.tblvehicledescriptor vd 
      on tu.vehicle_descriptor_id = vd.id
      left join transitds.tbltripdescriptor td 
      on td.id = tu.trip_descriptor_id
      where tu."timestamp" >= _begin_int and 
      tu."timestamp" <= _end_int;

 end;

$function$
;


