--001 - Change to sp_airlinepaxtypelookup to return the airline code and name

drop function if exists alertsproducerds.sp_getairlinepaxtypeinfo();
CREATE OR REPLACE FUNCTION alertsproducerds.sp_getairlinepaxtypeinfo()
 RETURNS TABLE(_pax_type_id integer, _airline_id integer, _airline_code character varying(8), _airline_name character varying)
 LANGUAGE sql
AS $function$ 

   SELECT pax.pax_type_id, pax.airline_id, al.code, al.name FROM alertsproducerds.tblairlinepaxtypelookup pax 
   left join staticds.tblairline al 
   on al.id = pax.airline_id;

$function$
;
