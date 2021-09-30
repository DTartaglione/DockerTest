CREATE OR REPLACE FUNCTION db_objects.sp_updatevms(_agency_id integer, _native_vms_id character varying, 
_vms_name character varying, _roadway character varying, _direction character varying, _latitude double precision, 
_longitude double precision, _message character varying, _last_update timestamp with time zone)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
       DECLARE _local_id integer := null;
	   DECLARE _device_id integer := null;
	   DECLARE _devicegeom geometry := null;
	  -- declare _max_id integer := null; 
	
	   begin
		   
		--SELECT max(id) + 1 from staticds.tbldevice into _max_id;
	
	    _devicegeom = ST_GeomFromText('point (' || _longitude || ' ' || _latitude || ')', 4326);
		
	   IF EXISTS (SELECT 1 FROM staticds.tbldevice where 
		  native_device_id = _native_vms_id and 
		  agency_id = _agency_id and 
		  device_subtype_id = 41 and 
		  device_type_id = 1)
		   THEN
		     update staticds.tbldevice
		     set name = _vms_name,
		     description = _vms_name,
		     last_update = _last_update,
		     geom = _devicegeom 
		     where native_device_id = _native_vms_id and 
		     agency_id = _agency_id and 
		     device_subtype_id = 41 and 
		     device_type_id = 1;
		    
		   else
		    insert into staticds.tbldevice 
		    (device_type_id, device_subtype_id, name, description, last_update, geom,
		    image_rotation, agency_id, native_device_id)
		    values 
		    (1, 41, _vms_name, _vms_name, _last_update, _devicegeom, 
		     null, _agency_id, _native_vms_id);
		    
         END IF;
          
        SELECT id from staticds.tbldevice into _device_id where native_device_id = _native_vms_id and 
		     agency_id = _agency_id and 
		     device_subtype_id = 41 and 
		     device_type_id = 1;
	   
		IF EXISTS (SELECT 1 FROM dynamicds.tblvms WHERE native_vms_id = _native_vms_id and agency_id = _agency_id)
		
		   THEN
		    UPDATE dynamicds.tblvms
		    SET vms_name = _vms_name,
		    	roadway = _roadway,
		    	direction = _direction,
		    	message = _message,
		    	last_update = _last_update,
		    	id = _device_id
		     WHERE native_vms_id = _native_vms_id and agency_id = _agency_id;
		  
		   ELSE
		    INSERT INTO dynamicds.tblvms
			(id, agency_id, native_vms_id, vms_name, roadway, direction, message, last_update)
			VALUES
			(_device_id, _agency_id, _native_vms_id, _vms_name, _roadway, _direction, _message, _last_update);
		   END IF;

          
end;
$function$
;
