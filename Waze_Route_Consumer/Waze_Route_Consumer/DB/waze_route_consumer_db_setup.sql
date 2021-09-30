--Postgres Database Design for Waze Data

--DROP TABLE staticds.tblwazeroute;
CREATE TABLE staticds.tblwazeroute(
	id integer NOT NULL,
	source_route_id varchar(32),
	agency_id integer,
	name varchar(255),
	length_in_meters integer,
	geom geometry,
	status integer,
	last_update timestamp without time zone,
	CONSTRAINT wazeroute_pkey PRIMARY KEY (id)
);
--DROP SEQUENCE staticds.tblwazeroute_id_seq;
CREATE SEQUENCE staticds.tblwazeroute_id_seq;

--SELECT nextval('staticds.tblwazeroute_id_seq')

--tblwazesubroute.name = FromName||' - '||ToName from Waze Consumer
--DROP TABLE staticds.tblwazesubroute;
CREATE TABLE staticds.tblwazesubroute(
	id integer NOT NULL,
	waze_route_id varchar(32),
	name varchar(255),
	length_in_meters integer,
	geom geometry,
	status integer,
	last_update timestamp without time zone,
	CONSTRAINT wazesubroute_pkey PRIMARY KEY (id)
);
--DROP SEQUENCE staticds.tblwazesubroute_id_seq;
CREATE SEQUENCE staticds.tblwazesubroute_id_seq;

--DROP TABLE dynamicds.tblwazeroutedata;
CREATE TABLE dynamicds.tblwazeroutedata(
	id integer NOT NULL,
	waze_route_id integer,
	travel_time_seconds integer,
	historic_travel_time_seconds integer,
	deviation_seconds integer,
	jam_level integer,
	last_update timestamp without time zone,
	CONSTRAINT wazeroutedata_pkey PRIMARY KEY (id),
	FOREIGN KEY (waze_route_id) REFERENCES staticds.tblwazeroute(id)
);
--DROP SEQUENCE dynamicds.tblwazeroutedata_id_seq;
CREATE SEQUENCE dynamicds.tblwazeroutedata_id_seq;

--DROP TABLE dynamicds.tblwazesubroutedata;
CREATE TABLE dynamicds.tblwazesubroutedata(
	id integer NOT NULL,
	waze_subroute_id integer,
	travel_time_seconds integer,
	historic_travel_time_seconds integer,
	deviation_seconds integer,
	jam_level integer,
	last_update timestamp without time zone,
	CONSTRAINT wazesubroutedata_pkey PRIMARY KEY (id),
	FOREIGN KEY (waze_subroute_id) REFERENCES staticds.tblwazesubroute(id)
);
--DROP SEQUENCE dynamicds.tblwazesubroutedata_id_seq;
CREATE SEQUENCE dynamicds.tblwazesubroutedata_id_seq;

--DROP TABLE xref_tables.tblwazesubroutelookup;
CREATE TABLE xref_tables.tblwazesubroutelookup(
	waze_subroute_id integer,
	source_route_id varchar(255),
	name varchar(255)
);

CREATE SCHEMA historicds;
--DROP TABLE historicds.tblwazeroutedata;
CREATE TABLE historicds.tblwazeroutedata(
	waze_route_id integer,
	travel_time_seconds integer,
	historic_travel_time_seconds integer,
	deviation_seconds integer,
	jam_level integer,
	update_timestamp timestamp without time zone
);
CREATE INDEX idx_historic_waze_data_last_update ON historicds.tblwazeroutedata (waze_route_id,update_timestamp DESC);

ALTER TABLE historicds.tblwazeroutedata DROP COLUMN IF EXISTS id;

--DELETE FROM staticds.tblagency WHERE agency_name = 'Waze';
INSERT INTO staticds.tblagency VALUES ('5','Waze');

CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_insertwazeroutedata(_agency_id int, _source_route_id character varying, _name character varying, _length_in_meters int, _last_update timestamp without time zone, _travel_time_seconds int, _historic_travel_time_seconds int, _deviation_seconds int, _jam_level int)
 RETURNS int
 LANGUAGE plpgsql
AS $function$

    declare _tblwazeroute_id  int := nextval('staticds.tblwazeroute_id_seq');
    declare _tblwazeroutedata_id int := nextval('dynamicds.tblwazeroutedata_id_seq');

	begin
		    
		  --insert into static table
		  INSERT INTO staticds.tblwazeroute
		  VALUES (_tblwazeroute_id,_source_route_id,_agency_id,_name,_length_in_meters,null,1,_last_update);
		   
		  INSERT INTO dynamicds.tblwazeroutedata
		  VALUES (_tblwazeroutedata_id,_tblwazeroute_id,_travel_time_seconds,_historic_travel_time_seconds,_deviation_seconds,_jam_level,_last_update);
		 
		  INSERT INTO historicds.tblwazeroutedata
		  VALUES (_tblwazeroute_id,_travel_time_seconds,_historic_travel_time_seconds,_deviation_seconds,_jam_level,_last_update);
		  
		  RETURN _tblwazeroute_id;

end;
$function$
;

CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_insertwazesubroutedata(_waze_route_id int,_source_route_id character varying, _name character varying, _length_in_meters int, _last_update timestamp without time zone, _travel_time_seconds int, _historic_travel_time_seconds int, _deviation_seconds int, _jam_level int)
 RETURNS int
 LANGUAGE plpgsql
AS $function$

    declare _tblwazesubroute_id int := nextval('staticds.tblwazesubroute_id_seq');
    declare _tblwazesubroutedata_id int := nextval('dynamicds.tblwazesubroutedata_id_seq');

	begin
		    
		  --insert into static table
		  INSERT INTO staticds.tblwazesubroute
		  VALUES (_tblwazesubroute_id,_waze_route_id,_name,_length_in_meters,null,1,_last_update);
		   
		  INSERT INTO dynamicds.tblwazesubroutedata
		  VALUES (_tblwazesubroutedata_id,_tblwazesubroute_id,_travel_time_seconds,_historic_travel_time_seconds,_deviation_seconds,_jam_level,_last_update);
		 
		  INSERT INTO xref_tables.tblwazesubroutelookup
		  VALUES (_tblwazesubroute_id,_source_route_id,_name);	  
		  
		  RETURN _tblwazesubroute_id;

end;
$function$
;

CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_updatewazeroutedata(_waze_route_id int, _last_update timestamp without time zone, _travel_time_seconds int, _historic_travel_time_seconds int, _deviation_seconds int, _jam_level int)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

      
	begin
		    
		    UPDATE dynamicds.tblwazeroutedata
		    SET travel_time_seconds = _travel_time_seconds,
				historic_travel_time_seconds = _historic_travel_time_seconds,
				deviation_seconds = _deviation_seconds,
				jam_level = _jam_level,
				last_update = _last_update
			WHERE waze_route_id = _waze_route_id;
		
			INSERT INTO historicds.tblwazeroutedata
		  	VALUES (_waze_route_id,_travel_time_seconds,_historic_travel_time_seconds,_deviation_seconds,_jam_level,_last_update);


end;
$function$
;

CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_updatewazesubroutedata(_waze_subroute_id int, _last_update timestamp without time zone, _travel_time_seconds int, _historic_travel_time_seconds int, _deviation_seconds int, _jam_level int)
 RETURNS void
 LANGUAGE plpgsql
AS $function$

      
	begin
		    
		    UPDATE dynamicds.tblwazesubroutedata
		    SET travel_time_seconds = _travel_time_seconds,
				historic_travel_time_seconds = _historic_travel_time_seconds,
				deviation_seconds = _deviation_seconds,
				jam_level = _jam_level,
				last_update = _last_update
			WHERE waze_subroute_id = _waze_subroute_id;


end;
$function$
;

CREATE OR REPLACE FUNCTION db_objects.sp_mod_di_updatewazelinestring(_route_type character varying, _linestring character varying, _source_route_id character varying)
 RETURNS void
 LANGUAGE plpgsql
AS $function$
      
	begin
		
		--For routes, we use the source route id
		if (_route_type = 'Route')
		then
			update staticds.tblwazeroute set geom = ST_GeomFromText('LINESTRING ('||_linestring||')',4326)
			where source_route_id = _source_route_id;
		end if;
		
		--For subroutes, the tblwazesubroute.id is retrieved in code and passed as the _source_route_id
		if (_route_type = 'SubRoute')
		then
			update staticds.tblwazesubroute set geom = ST_GeomFromText('LINESTRING ('||_linestring||')',4326)
			where id = _source_route_id::int;
		end if;
end;
$function$
;
