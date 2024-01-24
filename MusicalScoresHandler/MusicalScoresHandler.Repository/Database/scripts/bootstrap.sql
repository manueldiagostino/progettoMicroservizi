-- Adminer 4.8.1 PostgreSQL 16.1 (Debian 16.1-1.pgdg120+1) dump

\connect "postgres";

DROP TABLE IF EXISTS "genre";
DROP SEQUENCE IF EXISTS genre_id_seq;
CREATE SEQUENCE genre_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."genre" (
    "id" integer DEFAULT nextval('genre_id_seq') NOT NULL,
    "name" character varying(1024) NOT NULL,
    CONSTRAINT "genre_name" UNIQUE ("name"),
    CONSTRAINT "genre_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "genre";

DROP TABLE IF EXISTS "musical_score";
DROP SEQUENCE IF EXISTS musical_score_id_seq;
CREATE SEQUENCE musical_score_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."musical_score" (
    "id" integer DEFAULT nextval('musical_score_id_seq') NOT NULL,
    "title" character varying(1024) NOT NULL,
    "alias" character varying(1024),
    "year_of_composition" character varying(64),
    "description" character varying(2048),
    "opus" character varying(64),
    "author_id" integer NOT NULL,
    CONSTRAINT "musical_score_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "musical_score";

DROP TABLE IF EXISTS "pdf_file";
DROP SEQUENCE IF EXISTS pdf_file_id_seq;
CREATE SEQUENCE pdf_file_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."pdf_file" (
    "id" integer DEFAULT nextval('pdf_file_id_seq') NOT NULL,
    "musical_score_id" integer NOT NULL,
    "path" character varying(2048) NOT NULL,
    "upload_date" date NOT NULL,
    "publisher" character varying(512),
    "copyright_id" integer,
    "is_urtext" boolean DEFAULT false NOT NULL,
    "user_id" integer NOT NULL,
    "comments" character varying(2048) DEFAULT '',
    CONSTRAINT "pdf_file_path" UNIQUE ("path"),
    CONSTRAINT "pdf_file_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "pdf_file";

DROP TABLE IF EXISTS "score_genre_relationship";
DROP SEQUENCE IF EXISTS score_genre_relationship_id_seq;
CREATE SEQUENCE score_genre_relationship_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."score_genre_relationship" (
    "id" integer DEFAULT nextval('score_genre_relationship_id_seq') NOT NULL,
    "score_id" integer NOT NULL,
    "genre_id" integer NOT NULL,
    CONSTRAINT "score_genre_relationship_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "score_genre_relationship_score_id_genre_id" UNIQUE ("score_id", "genre_id")
) WITH (oids = false);

TRUNCATE "score_genre_relationship";

ALTER TABLE ONLY "public"."score_genre_relationship" ADD CONSTRAINT "score_genre_relationship_genre_id_fkey" FOREIGN KEY (genre_id) REFERENCES genre(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;
ALTER TABLE ONLY "public"."score_genre_relationship" ADD CONSTRAINT "score_genre_relationship_score_id_fkey" FOREIGN KEY (score_id) REFERENCES musical_score(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;

-- 2024-01-24 17:02:49.109602+00