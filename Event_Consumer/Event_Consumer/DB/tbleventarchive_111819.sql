-- Step 1: Create the new table
-- Drop table

-- DROP TABLE historicds.tbleventarchive;

CREATE TABLE historicds.tbleventarchive (
	id serial not NULL,
	event_id varchar(64) NOT NULL,
	source_event_id varchar(128) NULL,
	owner_org_id int8 NULL,
	event_status int8 NULL,
	event_category_id int4 NULL,
	event_category varchar(128) NULL,
	event_type varchar(128) NULL,
	create_time timestamp NULL,
	last_update timestamp NULL,
	close_time timestamp NULL,
	facility_id int8 NULL,
	facility_name varchar(128) NULL,
	event_direction int8 NULL,
	article varchar(50) NULL,
	from_node_id int8 NULL,
	from_node_name varchar(128) NULL,
	to_node_id int8 NULL,
	to_node_name varchar(128) NULL,
	event_description varchar(1028) NULL,
	event_severity int8 NULL,
	from_latitude float8 NULL,
	from_longitude float8 NULL,
	to_latitude float8 NULL,
	to_longitude float8 NULL,
	estimated_duration int8 NULL,
	extra_info varchar(1028) NULL,
	city varchar(128) NULL,
	county varchar(128) NULL,
	state varchar(2) NULL,
	num_affected_lanes int8 NULL,
	num_total_lanes int8 NULL,
	lane_type varchar(128) NULL,
	lane_status varchar(50) NULL,
	start_date_time timestamp NULL,
	end_date_time timestamp NULL,
	update_count int8 NULL,
	geom geometry null,
	CONSTRAINT tbleventarchive_id_pkey PRIMARY KEY (event_id)
);

-- Step 2: Three events with null ID, but very old. just remove them for now.
delete from dynamicds.tblevent where id is null;

-- Step 3: Move the closed data from dynamicds.tblevent (note, may need to run this a few times)
insert into historicds.tbleventarchive 
select * from dynamicds.tblevent where event_status = 11 and close_time is not null;

-- Step 4: Ensure the data has been moved before removing from dynamicds
select count(*) from dynamicds.tblevent where event_status = 11 and close_time is not null;
select count(*) from historicds.tbleventarchive;

-- Step 5: Delete the closed events from dynamicds.tblevent where they exist in tbleventarchive to clear space
delete from dynamicds.tblevent where source_event_id in
(select source_event_id from historicds.tbleventarchive);

-- Step 6: Ensure data still in dynamicds.tblevent
select * from dynamicds.tblevent where event_status = 11;

