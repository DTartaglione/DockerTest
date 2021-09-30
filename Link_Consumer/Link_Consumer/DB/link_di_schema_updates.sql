--001 - remove existing view (no change, just drop dependency) and tables we will correct
drop VIEW db_objects.vwgetlinks;
DROP TABLE dynamicds.tbllinktrafficdata;
DROP TABLE dynamicds.tbllinktrafficdata_temp;

--002 - create new tbllinktrafficdata table
CREATE TABLE dynamicds.tbllinktrafficdata (
	id int8 NOT NULL,
	agency_id int4 NOT NULL,
	speed_in_kph int8 NULL,
	travel_time_seconds int8 NULL,
	freeflow_in_kph int8 NULL,
	volume int8 NULL,
	occupancy int8 NULL,
	data_type varchar(32) NULL,
	last_update timestamp NOT NULL,
	--file_id character varying(255),
	status int not null,
	CONSTRAINT tbllinktrafficdata_pkey PRIMARY KEY (id, status),
	CONSTRAINT tbllink_tbllinktrafficdata FOREIGN KEY (id) REFERENCES staticds.tbllink(id)
);

--003 - create new tbllinktrafficdata_temp table
CREATE TABLE dynamicds.tbllinktrafficdata_temp (
	id int8 NOT NULL,
	agency_id int4 NOT NULL,
	speed_in_kph int8 NULL,
	travel_time_seconds int8 NULL,
	freeflow_in_kph int8 NULL,
	volume int8 NULL,
	occupancy int8 NULL,
	data_type varchar(32) NULL,
	last_update timestamp NOT NULL,
	file_id character varying(255),
	status int not null,
	CONSTRAINT tbllinktrafficdata_temp_pkey PRIMARY KEY (id)
);

--004 - create new historic link traffic table for XCM link data
DROP TABLE IF EXISTS historicds.tbllinktrafficdata_XCM;
CREATE TABLE historicds.tbllinktrafficdata_XCM (
    id SERIAL,
	link_id int8 NOT NULL,
	agency_id int4 NOT NULL,
	speed_in_kph int8 NULL,
	travel_time_seconds int8 NULL,
	volume int8 NULL,
	occupancy int8 NULL,
	data_type varchar(32) NULL,
	last_update timestamp NOT NULL,
	freeflow_in_kph int8 NULL
);
CREATE INDEX idx_historic_xcm_tbl_link_traffic_data_last_update ON historicds.tbllinktrafficdata_XCM USING btree (id, last_update DESC);

-- 005 - replace bulkupdatelinks SP
CREATE OR REPLACE FUNCTION db_objects.bulkupdatelinks(_file_id character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

BEGIN
	--first update the links we will be updating in the production table (set status = 0)
	update dynamicds.tbllinktrafficdata set status = 0 where  
    id in (select id from dynamicds.tbllinktrafficdata_temp where file_id = _file_id);
    
    --now copy the newer links into the prod table
    insert into dynamicds.tbllinktrafficdata 
    select 
    id, agency_id, speed_in_kph, travel_time_seconds, freeflow_in_kph, volume, occupancy, data_type, last_update, status
    from dynamicds.tbllinktrafficdata_temp where file_id = _file_id;
   
    --remove older link records with status 0
    delete from dynamicds.tbllinktrafficdata where status = 0;
   
    --copy the HERE data into the historic data set 
    INSERT INTO historicds.tbllinktrafficdata 
    SELECT id, agency_id, speed_in_kph, travel_time_seconds, volume, occupancy, data_type, last_update, freeflow_in_kph 
    from
    (select 
    id,
    agency_id,
    speed_in_kph,
    travel_time_seconds, 
    volume,
    occupancy,
    data_type,
    last_update,
    freeflow_in_kph
    FROM dynamicds.tbllinktrafficdata_temp WHERE
    agency_id = 3 and 
    id in
    (SELECT id from public.tblboundingboxlinks)
    and file_id = _file_id) as data_table;
   
    --copy the XCM data into the historic data set 
    INSERT INTO historicds.tbllinktrafficdata_XCM 
    (link_id, agency_id, speed_in_kph, travel_time_seconds, volume, occupancy, data_type, last_update, freeflow_in_kph)
    SELECT id, agency_id, speed_in_kph, travel_time_seconds, volume, occupancy, data_type, last_update, freeflow_in_kph 
    from
    (select 
    id,
    agency_id,
    speed_in_kph,
    travel_time_seconds, 
    volume,
    occupancy,
    data_type,
    last_update,
    freeflow_in_kph
    FROM dynamicds.tbllinktrafficdata_temp WHERE
    agency_id = 2 and 
    id in
    (SELECT id from public.tblboundingboxlinks)
    and file_id = _file_id) as data_table;

    --clear out the data from the temp table no error
    delete from dynamicds.tbllinktrafficdata_temp where file_id = _file_id;
   
    EXCEPTION WHEN OTHERS then
    --todo - raise error here for alerting that we failed somewhere
     --clear out the data from the temp table if error
    delete from dynamicds.tbllinktrafficdata_temp where file_id = _file_id;
    
end;

$function$
;

--006 - Minor change to view to use the new "status" field we added to tbllinktrafficdata and select where status = 1
CREATE OR REPLACE VIEW db_objects.vwgetlinks
AS SELECT l.id,
    l.speed_limit_kph,
    ld.speed_in_kph,
    ld.speed_in_kph::numeric / l.speed_limit_kph::numeric * 100::numeric AS threshold,
    ld.travel_time_seconds,
    l.geom,
    l.owner_org_id,
    l.link_name,
    l.link_direction,
    l.facility_name,
    ld.last_update
   FROM tbllink l
     JOIN dynamicds.tbllinktrafficdata ld ON l.id = ld.id AND l.owner_org_id = ld.agency_id
  WHERE 
  (ld.status = 1) 
  and 
  ((l.id IN ( SELECT tbllink.id
           FROM tbllink
          WHERE st_intersects(st_geomfromtext(('Polygon (('::text || ((( SELECT tblparams.param_value
                   FROM tblparams
                  WHERE tblparams.param_name::text = 'link_bounding_box'::text))::text)) || '))'::text, 4326), tbllink.geom) = true)) AND NOT (l.id IN ( SELECT tbllink.id
           FROM tbllink
          WHERE tbllink.owner_org_id = 3 AND tbllink.id::text ~~ '2%'::text)));

  
--007 - Repopulate the link bounding box table in publicds.
DELETE FROM public.tblboundingboxlinks;
INSERT INTO public.tblboundingboxlinks
SELECT id FROM staticds.tbllink
WHERE owner_org_id in ('2','3') AND
ST_Intersects(geom,ST_GeomFromText('POLYGON(('||(SELECT param_value FROM staticds.tblparams WHERE param_name = 'link_bounding_box')||'))',4326))=true;

         