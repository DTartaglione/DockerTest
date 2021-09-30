--001 drop and create new tblatrdata table
drop table if exists dynamicds.tblatrdata;
CREATE TABLE dynamicds.tblatrdata (
	id bigserial NOT NULL,
	device_id int4 not null,
	volume int4 NULL,
	speed_in_mph int4 null,
	occupancy float4 null,
	interval_end timestamp without time zone not null,
	last_update timestamp NULL,
	CONSTRAINT pkey_tblatrdata PRIMARY KEY (id)
);
CREATE INDEX idx_dynamic_atr_data_last_update_device_id ON dynamicds.tblatrdata (date(interval_end) DESC,device_id);


--002 - new Stored procedure to insert the ATR data
CREATE OR REPLACE FUNCTION db_objects.sp_insertatrdata(_device_id integer, _device_type_id integer, _device_subtype_id integer, 
_name character varying, _description character varying,
_agency_id integer, _parent_device_id integer, _reporting_interval_in_minutes integer, _volume integer, _speed_in_mph integer, _occupancy float,
_interval_end timestamp with time zone, _last_update timestamp with time zone)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$ 

   
	declare _lcl_device_id integer := _device_id; 

	begin
		if not exists (select 1 from staticds.tbldevice where name = _name and device_subtype_id = _device_subtype_id
		and device_type_id = _device_type_id and agency_id = _agency_id) then
		   _lcl_device_id = nextval('staticds.tbldevice_id_seq');
		 
		  insert into staticds.tbldevice (id, device_type_id, device_subtype_id, name, description, last_update, agency_id) 
		  values
		  (_lcl_device_id, _device_type_id, _device_subtype_id, _name, _description, _last_update, _agency_id);
		
		  insert into staticds.tblsensors (id, parent_device_id, reporting_interval_in_minutes) values
		  (_lcl_device_id, _parent_device_id, _reporting_interval_in_minutes);
	 else	 
	   _lcl_device_id = id from staticds.tbldevice where name = _name and device_subtype_id = _device_subtype_id
	    and device_type_id = _device_type_id and agency_id = _agency_id;
	 end if;
		  
	 --if not exists (select 1 from dynamicds.tblatrdata where device_id = _lcl_device_id and interval_end = _interval_end) then
		
	        insert into dynamicds.tblatrdata (device_id ,volume, speed_in_mph, occupancy, interval_end, last_update) values 
		    (_lcl_device_id, _volume, _speed_in_mph, _occupancy, _interval_end, _last_update);
    -- end if;
	    
	  

     return _lcl_device_id;
   
   end;
$function$
;