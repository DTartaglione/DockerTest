CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_getopenevents()
 RETURNS TABLE(_event_id character varying, _source_event_id character varying, _agency_id bigint, _last_update timestamp without time zone, _event_status bigint)
 LANGUAGE sql
AS $function$ 

   SELECT event_id, source_event_id, owner_org_id, last_update, event_status FROM dynamicds.tblevent WHERE close_time IS NULL;

$function$
;
