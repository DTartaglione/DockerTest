--Please remove all old table and sp instances of Weather DI: drop dynamicds.tblweather and sp_mod_di_updateweather

--DROP TABLE dynamicds.tblweatherhourly;
CREATE TABLE dynamicds.tblweatherhourly(
	id int NOT NULL,
	source_id character varying,		--NWS = period_id
	location_id int,	
	period_name character varying,
	start_time timestamp without time zone,
	end_time timestamp without time zone,
	temperature int,
	wind_speed character varying,
	wind_direction character varying,
	icon character varying,
	short_forecast character varying,
	detailed_forecast character varying,
	last_update timestamp,
	CONSTRAINT tblweatherhourly_pkey PRIMARY KEY (id)
);
--DROP SEQUENCE dynamicds.tblweatherhourly_id_seq;
CREATE SEQUENCE dynamicds.tblweatherhourly_id_seq;

--DROP TABLE dynamicds.tblweather;
CREATE TABLE dynamicds.tblweather(
	id int NOT NULL,
	source_id character varying,		--NWS = period_id
	location_id int,	
	period_name character varying,
	start_time timestamp without time zone,
	end_time timestamp without time zone,
	temperature int,
	wind_speed character varying,
	wind_direction character varying,
	icon character varying,
	short_forecast character varying,
	detailed_forecast character varying,
	last_update timestamp,
	CONSTRAINT tblweather_pkey PRIMARY KEY (id)
);
--DROP SEQUENCE dynamicds.tblweather_id_seq;
CREATE SEQUENCE dynamicds.tblweather_id_seq;

CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_updateweatherhourly(_source_id character varying,_location_id int,_period_name character varying,_start_time timestamp without time zone,_end_time timestamp without time zone, _temperature int, _wind_speed character varying, _wind_direction character varying, _icon character varying, _short_forecast character varying, _detailed_forecast character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
      
	BEGIN

		    IF NOT EXISTS (SELECT 1 FROM dynamicds.tblweatherhourly WHERE source_id = _source_id AND location_id = _location_id)
		    THEN
		    INSERT INTO dynamicds.tblweatherhourly (id,source_id,location_id,period_name,start_time,end_time,temperature,wind_speed,wind_direction,icon,short_forecast,detailed_forecast,last_update)
		    VALUES (nextval('dynamicds.tblweatherhourly_id_seq'),_source_id,_location_id,_period_name,_start_time,_end_time,_temperature,_wind_speed,_wind_direction,_icon,_short_forecast,_detailed_forecast,current_timestamp);
		   
		   
		    ELSE
		    UPDATE dynamicds.tblweatherhourly
		    SET
		    period_name = _period_name,
		    start_time = _start_time,
		    end_time = _end_time,
		    temperature = _temperature,
		    wind_speed = _wind_speed,
		    wind_direction = _wind_direction,
		    icon = _icon,
		    short_forecast = _short_forecast,
		    detailed_forecast = _detailed_forecast,
		    last_update = current_timestamp
		    WHERE source_id = _source_id and location_id = _location_id;
		   
		    END IF;

end;
$function$
;

CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_updateweather(_source_id character varying,_location_id int,_period_name character varying,_start_time timestamp without time zone,_end_time timestamp without time zone, _temperature int, _wind_speed character varying, _wind_direction character varying, _icon character varying, _short_forecast character varying, _detailed_forecast character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
      
	BEGIN

		    IF NOT EXISTS (SELECT 1 FROM dynamicds.tblweather WHERE source_id = _source_id AND location_id = _location_id)
		    THEN
		    INSERT INTO dynamicds.tblweather (id,source_id,location_id,period_name,start_time,end_time,temperature,wind_speed,wind_direction,icon,short_forecast,detailed_forecast,last_update)
		    VALUES (nextval('dynamicds.tblweather_id_seq'),_source_id,_location_id,_period_name,_start_time,_end_time,_temperature,_wind_speed,_wind_direction,_icon,_short_forecast,_detailed_forecast,current_timestamp);
		   
		   
		    ELSE
		    UPDATE dynamicds.tblweather
		    SET
		    period_name = _period_name,
		    start_time = _start_time,
		    end_time = _end_time,
		    temperature = _temperature,
		    wind_speed = _wind_speed,
		    wind_direction = _wind_direction,
		    icon = _icon,
		    short_forecast = _short_forecast,
		    detailed_forecast = _detailed_forecast,
		    last_update = current_timestamp
		    WHERE source_id = _source_id and location_id = _location_id;
		   
		    END IF;

end;
$function$
;


















