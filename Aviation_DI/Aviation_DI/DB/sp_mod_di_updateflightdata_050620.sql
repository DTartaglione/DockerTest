drop function db_objects.sp_mod_di_updateflightdata;

CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_updateflightdata(_source_flight_id character varying, _agency_id integer, _airline_code character varying, 
_airline_name character varying, _flight_number character varying, _origin_airport_code character varying, _destination_airport_code character varying, 
_aircraft_type character varying, _current_speed double precision, _current_altitude double precision, _flight_status character varying, 
_scheduled_departure_datetime character varying, _estimated_departure_datetime character varying, _actual_departure_datetime character varying, 
_departure_gate character varying, _scheduled_arrival_datetime character varying, _estimated_arrival_datetime character varying, 
_actual_arrival_datetime character varying, _arrival_gate character varying, _latitude double precision, _longitude double precision, 
_direction double precision, _departure_delay integer, _arrival_delay integer, _num_seats integer, _last_update timestamp without time zone, 
_flight_type_id integer, _code_shares character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

      DECLARE _return bigint;
      DECLARE _geom geometry := null;
      
	begin
	    _geom = ST_GeomFromText('point (' || _longitude || ' ' || _latitude || ')', 4326);

            IF _aircraft_type = '' THEN
		_aircraft_type = NULL;
	    END IF;

if _scheduled_departure_datetime = '' then
	       _scheduled_departure_datetime = null;
	    end if;
	    
	    if _estimated_departure_datetime = '' then
	       _estimated_departure_datetime = null;
	    end if;
	    
	    if _actual_departure_datetime = '' then
	       _actual_departure_datetime = null;
	    end if;
	    
	    if _scheduled_arrival_datetime = '' then
	       _scheduled_arrival_datetime = null;
	    end if;
	   
	    if _estimated_arrival_datetime = '' then
	       _estimated_arrival_datetime = null;
	    end if;
	    
	    if _actual_arrival_datetime = '' then
	       _actual_arrival_datetime = null;
	    end if;
		
	    IF NOT EXISTS (SELECT id FROM dynamicds.tblflightstatus WHERE source_flight_id = _source_flight_id AND DATE(scheduled_departure_datetime) = DATE(_scheduled_departure_datetime)) THEN
        --IF NOT EXISTS (SELECT id FROM dynamicds.tblflightstatus WHERE flight_number = _flight_number AND DATE(scheduled_departure_datetime) = DATE(_scheduled_departure_datetime)) THEN
       --IF NOT EXISTS (SELECT id FROM dynamicds.tblflightstatus WHERE source_flight_id = _source_flight_id AND DATE(scheduled_departure_datetime) = DATE(_scheduled_departure_datetime)) THEN      
	   BEGIN
		    INSERT INTO dynamicds.tblflightstatus (source_flight_id, agency_id, airline_code, airline_name, flight_number, origin_airport_code,
		    destination_airport_code, aircraft_type, current_speed, current_altitude, flight_status,
		    scheduled_departure_datetime, estimated_departure_datetime, actual_departure_datetime, departure_gate, 
		    scheduled_arrival_datetime, estimated_arrival_datetime, actual_arrival_datetime, arrival_gate, latitude, longitude,
		    direction, departure_delay, arrival_delay, number_seats, geom, last_update, flight_type_id, code_shares)
		    VALUES
	            (_source_flight_id, _agency_id, _airline_code, _airline_name, _flight_number, _origin_airport_code,
		    _destination_airport_code, _aircraft_type, _current_speed, _current_altitude, _flight_status,
		    _scheduled_departure_datetime::timestamp without time zone, _estimated_departure_datetime::timestamp without time zone, 
		    _actual_departure_datetime::timestamp without time zone, _departure_gate, 
		    _scheduled_arrival_datetime::timestamp without time zone, _estimated_arrival_datetime::timestamp without time zone, 
		    _actual_arrival_datetime::timestamp without time zone, _arrival_gate, _latitude, _longitude,
		    _direction, _departure_delay, _arrival_delay, _num_seats, _geom, _last_update, _flight_type_id, _code_shares);
	       END;
	   ELSE 
	       BEGIN
		    UPDATE dynamicds.tblflightstatus SET
                    airline_code = _airline_code, 
                    airline_name = _airline_name, 
                    origin_airport_code = _origin_airport_code,
		    destination_airport_code = _destination_airport_code, 
		    aircraft_type = _aircraft_type, 
		    current_speed = _current_speed, 
		    current_altitude = _current_altitude, 
		    flight_status = _flight_status,
		    scheduled_departure_datetime = _scheduled_departure_datetime::timestamp without time zone, 
		    estimated_departure_datetime = _estimated_departure_datetime::timestamp without time zone, 
		    actual_departure_datetime = _actual_departure_datetime::timestamp without time zone, 
		    departure_gate = _departure_gate, 
		    scheduled_arrival_datetime = _scheduled_arrival_datetime::timestamp without time zone, 
		    estimated_arrival_datetime = _estimated_arrival_datetime::timestamp without time zone, 
		    actual_arrival_datetime = _actual_arrival_datetime::timestamp without time zone, 
		    arrival_gate = _arrival_gate, 
		    latitude = _latitude, 
		    longitude = _longitude,
		    direction = _direction, 
		    departure_delay = _departure_delay, 
		    arrival_delay = _arrival_delay, 
		    number_seats = _num_seats,
		    geom = _geom, 
		    last_update = _last_update,
		    flight_type_id = _flight_type_id,
		    code_shares = _code_shares
		    WHERE source_flight_id = _source_flight_id AND agency_id = _agency_id
		    --WHERE flight_number = _flight_number AND agency_id = _agency_id 
		   -- where source_flight_id = _source_flight_id and agency_id = _agency_id
			AND DATE(scheduled_departure_datetime) = DATE(_scheduled_departure_datetime);
	       END;
	   END IF;

end;
$function$
;
