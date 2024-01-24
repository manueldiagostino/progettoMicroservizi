-- Adminer 4.8.1 PostgreSQL 16.1 (Debian 16.1-1.pgdg120+1) dump

\connect "postgres";

DROP TABLE IF EXISTS "bio";
DROP SEQUENCE IF EXISTS bio_id_seq;
CREATE SEQUENCE bio_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."bio" (
    "id" integer DEFAULT nextval('bio_id_seq') NOT NULL,
    "text" text,
    "user_id" integer NOT NULL,
    CONSTRAINT "bio_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "bio";

DROP TABLE IF EXISTS "transactional_outbox";
CREATE TABLE "public"."transactional_outbox" (
    "id" integer NOT NULL,
    "table" character varying(50) NOT NULL,
    "message" character varying(2048) NOT NULL
) WITH (oids = false);

TRUNCATE "transactional_outbox";

DROP TABLE IF EXISTS "user";
DROP SEQUENCE IF EXISTS user_id_seq;
CREATE SEQUENCE user_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 3 CACHE 1;

CREATE TABLE "public"."user" (
    "id" integer DEFAULT nextval('user_id_seq') NOT NULL,
    "username" character varying(50) NOT NULL,
    "name" character varying(50),
    "surname" character varying(50),
    "propic_path" character varying(2048),
    "upload_time" bigint NOT NULL,
    "bio_id" integer,
    "salt" character varying(2048) NOT NULL,
    "hash" character varying(2048) NOT NULL,
    CONSTRAINT "user_id_bio" UNIQUE ("bio_id"),
    CONSTRAINT "user_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "user_username" UNIQUE ("username")
) WITH (oids = false);

TRUNCATE "user";
INSERT INTO "user" ("id", "username", "name", "surname", "propic_path", "upload_time", "bio_id", "salt", "hash") VALUES
(2,	'_linux_00_',	NULL,	NULL,	'ProfilePictures/221211_11h38m46s.jpeg',	1706094311,	NULL,	'8/9DAURNdAvJ8OnywvngOw==',	'xjh7LEFoAZFfVJpw88QhfNg0xAFhfTr8EkQckMUMGmg='),
(1,	'manuel2001',	'Manuel',	'Di Agostino',	'ProfilePictures/1706095149.jpeg',	1706041700,	NULL,	'GE4i+JL/vfQlFINnO89QOw==',	'qe+hGBDro/rBJ+QdzYgZA1GTLt0R1g6PHbsdSYTwnYc=');

ALTER TABLE ONLY "public"."bio" ADD CONSTRAINT "bio_id_user_fkey" FOREIGN KEY (user_id) REFERENCES "user"(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."user" ADD CONSTRAINT "user_id_bio_fkey" FOREIGN KEY (bio_id) REFERENCES bio(id) ON UPDATE CASCADE ON DELETE SET NULL NOT DEFERRABLE;

-- 2024-01-24 11:20:00.378792+00