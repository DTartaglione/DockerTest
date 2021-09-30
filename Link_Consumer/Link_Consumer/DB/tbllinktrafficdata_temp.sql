--001 - drop existing function for bulk update links that uses incorrect type
drop function if exists db_objects.bulkupdatelinks(int4);

--002 - Drop and readd the temp link table to remove the primary key, but make id and file name unique
DROP TABLE if exists dynamicds.tbllinktrafficdata_temp;
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
	file_id varchar(255) NULL,
	status int4 NOT NULL,
	CONSTRAINT tbllinktrafficdata_temp_pkey PRIMARY KEY (id, file_id)
);