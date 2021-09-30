asfd
--Update to sp_insertpaxdata to use numeric instead of float so load factor correctly inserts as decimal

DROP FUNCTION IF EXISTS db_objects.sp_insertpaxdata(_data_time_type integer, _airline_id integer, _airport_id integer, _pax_type_id integer, _pax_timestamp timestamp without time zone, _volume integer, _flight_type_id integer, _flight_count integer, _load_factor_percentage double, _extra_info character varying, _last_update timestamp without time zone);
DROP FUNCTION IF EXISTS db_objects.sp_insertpaxdata(_data_time_type integer, _airline_id integer, _airport_id integer, _pax_type_id integer, _pax_timestamp timestamp without time zone, _volume integer, _flight_type_id integer, _flight_count integer, _load_factor_percentage numeric, _extra_info character varying, _last_update timestamp without time zone);
CREATE OR REPLACE FUNCTION db_objects.sp_insertpaxdata(_data_time_type integer, _airline_id integer, _airport_id integer, _pax_type_id integer, _pax_timestamp timestamp without time zone, _volume integer, _flight_type_id integer, _flight_count integer, _load_factor_percentage numeric, _extra_info character varying, _last_update timestamp without time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

	begin
	if (_data_time_type = 0)
	then
	insert into dynamicds.tblflightvolumehourly
	(airline_id, airport_id, pax_type_id, pax_hour_timestamp, volume, flight_type_id, flight_count, load_factor_percentage, extra_info, last_update)
	values (_airline_id, _airport_id, _pax_type_id, _pax_timestamp, _volume, _flight_type_id, _flight_count, _load_factor_percentage, _extra_info,_last_update);

	else
	insert into dynamicds.tblflightvolumedaily
	(airline_id, airport_id, pax_type_id, pax_date, volume, flight_type_id, flight_count, load_factor_percentage, extra_info, last_update)
	values (_airline_id, _airport_id, _pax_type_id, _pax_timestamp::date, _volume, _flight_type_id, _flight_count, _load_factor_percentage, _extra_info,_last_update);

	end if;		  

end;
$function$
;




--Rollback
/*
DROP FUNCTION IF EXISTS db_objects.sp_insertpaxdata(_data_time_type integer, _airline_id integer, _airport_id integer, _pax_type_id integer, _pax_timestamp timestamp without time zone, _volume integer, _flight_type_id integer, _flight_count integer, _load_factor_percentage numeric, _extra_info character varying, _last_update timestamp without time zone);

CREATE OR REPLACE FUNCTION db_objects.sp_insertpaxdata(_data_time_type integer, _airline_id integer, _airport_id integer, _pax_type_id integer, _pax_timestamp timestamp without time zone, _volume integer, _flight_type_id integer, _flight_count integer, _load_factor_percentage double precision, _extra_info character varying, _last_update timestamp without time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

	begin
	if (_data_time_type = 0)
	then
	insert into dynamicds.tblflightvolumehourly
	(airline_id, airport_id, pax_type_id, pax_hour_timestamp, volume, flight_type_id, flight_count, load_factor_percentage, extra_info, last_update)
	values (_airline_id, _airport_id, _pax_type_id, _pax_timestamp, _volume, _flight_type_id, _flight_count, _load_factor_percentage, _extra_info,_last_update);

	else
	insert into dynamicds.tblflightvolumedaily
	(airline_id, airport_id, pax_type_id, pax_date, volume, flight_type_id, flight_count, load_factor_percentage, extra_info, last_update)
	values (_airline_id, _airport_id, _pax_type_id, _pax_timestamp::date, _volume, _flight_type_id, _flight_count, _load_factor_percentage, _extra_info,_last_update);

	end if;		  

end;
$function$
;



 */


