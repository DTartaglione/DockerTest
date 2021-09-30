/*
Create the followingc collections in MongoDB:
aoc_status_code
pax_volume

*/

--INSERT INTO staticds.tblagency VALUES ('6','Port Authority');

CREATE SCHEMA pads;

--DROP TABLE pads.airport_operations_center;
CREATE TABLE pads.airport_operations_center(
	id integer,
	aoc_name character varying,
	geom geometry,
	CONSTRAINT airport_operations_center_pkey PRIMARY KEY (id)
);
CREATE SEQUENCE pads.airport_operations_center_id_seq;

INSERT INTO pads.airport_operations_center VALUES ('1','LaGuardia AOC',ST_GeomFromText('POINT (-73.885073 40.772069)',4326));

--DROP TABLE pads.aoc_status_code;
CREATE TABLE pads.aoc_status_code(
	id integer,
	aoc_id integer,
	code_id integer,				--1 = Green, 2 = Yellow, 3 = Amber, 4 = Red (internal ids)
	code_name character varying,
	last_update timestamp without time zone,
	CONSTRAINT aoc_status_code_pkey PRIMARY KEY (id)
);
CREATE SEQUENCE pads.aoc_status_code_id_seq;

CREATE TABLE pads.pa_alerts(
	id integer,
	aoc_id integer,
	alert_message character varying,
	create_timestamp timestamp without time zone,	
	status int4,
	geom geometry,
	last_update timestamp without time zone,
	CONSTRAINT pa_alerts_pkey PRIMARY KEY (id)
);
CREATE SEQUENCE pads.pa_alerts_id_seq;

CREATE TABLE pads.pax_volume(
	id integer,
	aoc_id integer,
	airline_code character varying,
	hour_timestamp timestamp without time zone,
	arrv_volume integer,
	dept_volume integer,
	last_update timestamp without time zone,
	CONSTRAINT pax_volume_pkey PRIMARY KEY (id)
);
CREATE SEQUENCE pads.pax_volume_id_seq;

CREATE OR REPLACE FUNCTION pads.sp_getaocinfo()
 RETURNS TABLE(_id int, _aoc_name character varying)
 LANGUAGE sql
AS $function$ 

   SELECT id, aoc_name FROM pads.airport_operations_center;

$function$
;

CREATE OR REPLACE FUNCTION pads.sp_updateaoctrafficstatus(_aoc_id int, _code_id int, _code_name character varying, _last_update timestamp without time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

begin
	if exists (select 1 from pads.aoc_status_code where aoc_id = _aoc_id)
	then
		if exists (select 1 from pads.aoc_status_code where aoc_id = _aoc_id and last_update < _last_update)
		then
			update pads.aoc_status_code
			set code_id = _code_id,
				code_name = _code_name,
				last_update = _last_update
			where aoc_id = _aoc_id;
		end if;

	else
	insert into pads.aoc_status_code
	values (nextval('pads.aoc_status_code_id_seq'),_aoc_id,_code_id,_code_name,_last_update);

	end if;
			 
end;
$function$
;

CREATE OR REPLACE FUNCTION pads.sp_getactivealerts()
 RETURNS TABLE(_id int, _last_update timestamp without time zone)
 LANGUAGE sql
AS $function$ 

   SELECT id, last_update FROM pads.pa_alerts WHERE status = 1;

$function$
;

CREATE OR REPLACE FUNCTION pads.sp_insertalert(_aoc_id int, _alert_message character varying, _create_timestamp timestamp without time zone, _last_update timestamp without time zone, _geom geometry)
 RETURNS int
 LANGUAGE plpgsql
AS $function$

declare

	_id int := nextval('pads.pa_alerts_id_seq');

begin
	
	insert into pads.pa_alerts
	values (_id,_aoc_id,_alert_message,_create_timestamp,1,_geom,_last_update);

	return _id;
			 
end;
$function$
;

CREATE OR REPLACE FUNCTION pads.sp_deactivatealert(_alert_id integer, _disable_older_than_value_hours integer)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

begin
	
	--If no alerts are set to disable, we will pass a -1 to check if any alerts have been manually edited and disable them
	if(_alert_id = -1)
	then
	update pads.pa_alerts
	set status = 0, last_update = current_timestamp
	where status = 1 AND last_update < current_timestamp + interval '1h' * _disable_older_than_value_hours * -1;
	
	else
	update pads.pa_alerts
	set status = 0, last_update = current_timestamp
	where id = _alert_id;
	
	end if;

end;
$function$
;

CREATE OR REPLACE FUNCTION pads.sp_getlatestpaxupdateinfo()
 RETURNS TABLE(_airline_code character varying, _last_update timestamp without time zone)
 LANGUAGE sql
AS $function$ 

	SELECT airline_code,last_update
	FROM
	   (SELECT row_number() over(partition by airline_code order by last_update desc), airline_code, last_update
	   FROM pads.pax_volume) as x
	WHERE row_number = 1;

$function$
;

CREATE OR REPLACE FUNCTION pads.sp_updatepaxvolume( _aoc_id int, _airline_code character varying, _hour_timestamp timestamp without time zone, _arrv_volume int, _dept_volume int, _last_update timestamp without time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

begin
	if exists (select 1 from pads.pax_volume where aoc_id = _aoc_id and _airline_code = airline_code and _hour_timestamp = hour_timestamp)
	then
		update pads.pax_volume
		set hour_timestamp = _hour_timestamp,
			arrv_volume = _arrv_volume,
			dept_volume = _dept_volume,
			last_update = _last_update
		where aoc_id = _aoc_id and _airline_code = airline_code and _hour_timestamp = hour_timestamp;
			
	else
	insert into pads.pax_volume
	values (nextval('pads.pax_volume_id_seq'), _aoc_id, _airline_code, _hour_timestamp, _arrv_volume, _dept_volume, _last_update);

	end if;
			 
end;
$function$
;

SELECT last_update + interval '1h' * 24 * -1 from pads.pa_alerts
--SELECT * FROM pads.aoc_status_code
--SELECT * FROM pads.pa_alerts

--UPDATE pads.pa_alerts SET status = 1, last_update = last_update + interval '1h' * 25 * -1;

