-- Adminer 4.8.1 PostgreSQL 16.1 (Debian 16.1-1.pgdg120+1) dump

\connect "postgres";

DROP TABLE IF EXISTS "author";
DROP SEQUENCE IF EXISTS author_id_seq;
CREATE SEQUENCE author_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 7 CACHE 1;

CREATE TABLE "public"."author" (
    "id" integer DEFAULT nextval('author_id_seq') NOT NULL,
    "name" character varying(50) NOT NULL,
    "surname" character varying(50) NOT NULL,
    CONSTRAINT "Author_name_surname" UNIQUE ("name", "surname"),
    CONSTRAINT "Author_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "author";
INSERT INTO "author" ("id", "name", "surname") VALUES
(1,	'Fryderyk Franciszek',	'Chopin'),
(2,	'Ludwig',	'van Beethoven'),
(3,	'Wolfgang Amadeus',	'Mozart'),
(4,	'Franz Peter',	'Schubert'),
(5,	'Johann Sebastian',	'Bach'),
(6,	'Gustav',	'Mahler');

DROP TABLE IF EXISTS "external_link";
DROP SEQUENCE IF EXISTS external_links_id_seq;
CREATE SEQUENCE external_links_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 5 CACHE 1;

CREATE TABLE "public"."external_link" (
    "id" integer DEFAULT nextval('external_links_id_seq') NOT NULL,
    "authorId" integer NOT NULL,
    "url" character varying(2048) NOT NULL,
    CONSTRAINT "external_links_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "external_links_url" UNIQUE ("url")
) WITH (oids = false);

TRUNCATE "external_link";
INSERT INTO "external_link" ("id", "authorId", "url") VALUES
(1,	3,	'https://it.wikipedia.org/wiki/Wolfgang_Amadeus_Mozart'),
(2,	3,	'https://en.wikipedia.org/wiki/Wolfgang_Amadeus_Mozart'),
(3,	2,	'https://it.wikipedia.org/wiki/Ludwig_van_Beethoven'),
(4,	1,	'https://it.wikipedia.org/wiki/Fryderyk_Chopin');

DROP TABLE IF EXISTS "transactional_outbox";
DROP SEQUENCE IF EXISTS transactional_outbox_id_seq;
CREATE SEQUENCE transactional_outbox_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."transactional_outbox" (
    "id" integer DEFAULT nextval('transactional_outbox_id_seq') NOT NULL,
    "table" character varying(50) NOT NULL,
    "message" character varying(2048) NOT NULL,
    CONSTRAINT "transactional_outbox_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "transactional_outbox";

ALTER TABLE ONLY "public"."external_link" ADD CONSTRAINT "external_links_author_id_fkey" FOREIGN KEY ("authorId") REFERENCES author(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;

-- 2024-02-02 15:44:04.535759+00