--001 - add email type ID column to pa_alerts table
ALTER TABLE pads.pa_alerts
ADD COLUMN email_type_id int4 NULL;

--002 - set email type ID to default 0
update pads.pa_alerts set email_type_id =
(select id from alertsproducerds.tblemailtype where type_name = 'PA_Alerts');

--003 - Verify the new Type ID column is populated with an ID that should match the ID of the PA_Alerts email type.
select * from pads.pa_alerts;

--004 - set a foreign key constraint between alertsproducerds.tblemailtype and pads.pa_alerts
ALTER TABLE pads.pa_alerts ADD CONSTRAINT pa_alerts_tblemailtype FOREIGN KEY (email_type_id) REFERENCES alertsproducerds.tblemailtype(id);

--005 - New SP change to include email_type_id in insert.
CREATE OR REPLACE FUNCTION pads.sp_insertalert(_aoc_id integer, _alert_message character varying, _create_timestamp timestamp without time zone, _last_update timestamp without time zone, _geom geometry,
_email_type_id integer)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$

declare

	_id int := nextval('pads.pa_alerts_id_seq');

begin
	
	insert into pads.pa_alerts
	values (_id,_aoc_id,_alert_message,_create_timestamp,1,_geom,_last_update, _email_type_id);

	return _id;
			 
end;
$function$
;
