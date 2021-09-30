--001 - First drop db_objects.vwgetcctv;
drop view db_objects.vwgetcctv;

--002 - Drop existing cctv table
DROP TABLE dynamicds.tblcctv;

--003 - clear existing 511NY CCTV data
delete from staticds.tbldevice where device_subtype_id = 40 and device_type_id = 2 and agency_id = 6;

--004 - modify CCTV schema to match what we did for VMS. Create new cctv table
CREATE TABLE dynamicds.tblcctv (
	id bigserial NOT NULL,
	agency_id int4 NOT NULL,
	native_cctv_id varchar(255) NOT NULL,
	cctv_name varchar(255) NOT NULL,
	roadway varchar(128) NULL,
	direction varchar(32) NULL,
	snapshot_url varchar(2048) NULL,
	video_url varchar(2048) null,
	blocked boolean not null default false,
	status int not null default 1,
	last_update timestamptz NULL,
	CONSTRAINT "tblcctv_pkey" PRIMARY KEY (id)
);

--005 - new SP to retrieve the CCTV currently in the database
--drop function db_objects.sp_getcctv();
CREATE OR REPLACE FUNCTION db_objects.sp_getcctv()
 RETURNS TABLE(_id integer, _native_cctv_id character varying, _agency_id integer, _cctv_name character varying, 
 _roadway character varying, _direction character varying, _snapshot_url character varying, 
 _video_url character varying, _blocked boolean, _status int, _latitude float, _longitude float, 
 _last_update timestamp with time zone)
 LANGUAGE sql
AS $function$ 

   SELECT s.id, s.native_device_id as native_cctv_id, s.agency_id, s.name, 
   d.roadway, d.direction, d.snapshot_url, d.video_url, d.blocked, d.status, 
   ST_Y(s.geom) as latitude, ST_X(s.geom) as longitude,
   d.last_update
   from staticds.tbldevice s 
   left join dynamicds.tblcctv d on 
   s.native_device_id = d.native_cctv_id
   and s.agency_id = d.agency_id 
   and s.id = d.id
   where s.device_type_id = 2 and s.device_subtype_id = 40;

$function$
;

--006 - new SP to insert/update new/existing CCTV
CREATE OR REPLACE FUNCTION db_objects.sp_updatecctv(_agency_id integer, _native_cctv_id character varying, 
_cctv_name character varying, _roadway character varying, _direction character varying,
_snapshot_url character varying, _video_url character varying,
_blocked boolean, _status int, _latitude float, _longitude float,
_last_update timestamp with time zone)
 RETURNS integer
 LANGUAGE plpgsql
AS $function$
       DECLARE _local_device_id integer := null;
	   DECLARE _device_id integer := null;
	   DECLARE _devicegeom geometry := null;
	   declare _device_type_id integer := null;
	   declare _device_subtype_id integer := null;
	
	   begin
	
		select id from staticds.tbldevicetype into _device_type_id where 
		lower(device_type) = 'cctv';
	
	    select id from staticds.tbldevicesubtype into _device_subtype_id where
	    device_type_id = _device_type_id;
		
	    _devicegeom = ST_GeomFromText('point (' || _longitude || ' ' || _latitude || ')', 4326);
		
	   IF EXISTS (SELECT 1 FROM staticds.tbldevice where 
		  (native_device_id = _native_cctv_id) and 
		  agency_id = _agency_id and 
		  device_subtype_id = _device_subtype_id and 
		  device_type_id = _device_type_id)
		   THEN
		     update staticds.tbldevice
		     set name = _cctv_name,
		     description = _cctv_name,
		     last_update = _last_update,
		     geom = _devicegeom 
		     where native_device_id = _native_cctv_id and 
		     agency_id = _agency_id and 
		     device_subtype_id = _device_subtype_id and 
		     device_type_id = _device_type_id;
		   else
		    insert into staticds.tbldevice 
		    (device_type_id, device_subtype_id, name, description, last_update, geom,
		    agency_id, native_device_id)
		    values 
		    (_device_type_id, _device_subtype_id, _cctv_name, _cctv_name, _last_update, _devicegeom, 
		    _agency_id, _native_cctv_id);
        END IF;
          
        SELECT id from staticds.tbldevice into _local_device_id 
             where native_device_id = _native_cctv_id and 
		     agency_id = _agency_id and 
		     device_subtype_id = _device_subtype_id and 
		     device_type_id = _device_type_id;
        
		IF EXISTS (SELECT 1 FROM dynamicds.tblcctv WHERE native_cctv_id = _native_cctv_id and agency_id = _agency_id)
		
		   THEN
		    UPDATE dynamicds.tblcctv
		    SET cctv_name = _cctv_name,
		    	roadway = _roadway,
		    	direction = _direction,
		    	snapshot_url = _snapshot_url,
		    	video_url = _video_url,
		    	blocked = _blocked,
		    	status = _status,
		    	last_update = _last_update
		     WHERE native_cctv_id = _native_cctv_id and agency_id = _agency_id;
		  
		   ELSE
		    INSERT INTO dynamicds.tblcctv
			(id, agency_id, native_cctv_id, cctv_name, roadway, direction, snapshot_url, video_url, 
			blocked, status, last_update)
			VALUES
			(_local_device_id, _agency_id, _native_cctv_id, _cctv_name, _roadway, _direction, 
		    _snapshot_url, _video_url, _blocked, _status, _last_update);
		   END IF;
		    
           return _local_device_id;
        
end;
$function$
;

-- 007 put vwgetcctv back
--This is a backup of vwgetcctv because we need to first drop it before we can recreate tblcctv in dynamicds.
CREATE OR REPLACE VIEW db_objects.vwgetcctv
AS SELECT d.id,
    d.device_type_id,
    dt.device_type,
    d.device_subtype_id,
    dst.device_subtype,
    d.name,
    d.last_update,
    d.geom,
    c.snapshot_url,
    c.video_url
   FROM staticds.tbldevice d
     JOIN staticds.tbldevicetype dt ON dt.id = d.device_type_id
     LEFT JOIN staticds.tbldevicesubtype dst ON dst.id = d.device_subtype_id
     JOIN dynamicds.tblcctv c ON c.id = d.id
  ORDER BY d.device_type_id, d.device_subtype_id;

 --008 - update existing tbldevice sequence with latest ID 
-- make sure to run all together.
 do $$
declare _max_id integer := null;
begin
select max(id)+1 from staticds.tbldevice into _max_id;
execute 'alter SEQUENCE staticds.tbldevice_id_seq RESTART with '|| _max_id;    
end;
$$ language plpgsql
 
--009 - verify tbldevice is still good
select * from staticds.tbldevice where device_type_id = 2 order by id desc;
